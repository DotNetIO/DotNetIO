// Copyright 2004-2012 Henrik Feldt - https://github.com/DotNetIO
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DotNetIO.FileSystems.InMemory
{
	public class InMemoryFileSystem : FileSystem
	{
		readonly InMemoryFileSystemNotifier _notifier;
		Directory _systemTempDirectory;

		readonly object _syncRoot = new object();

		public InMemoryFileSystem()
		{
			_notifier = new InMemoryFileSystemNotifier();

			Directories = new Dictionary<string, InMemoryDirectory>(StringComparer.OrdinalIgnoreCase);
			CurrentDirectory = new Path(@"c:\");
		}

		FileSystemNotifier FileSystem.Notifier
		{
			get { return _notifier; }
		}

		internal InMemoryFileSystemNotifier Notifier
		{
			get { return _notifier; }
		}

		public Dictionary<string, InMemoryDirectory> Directories { get; private set; }

		InMemoryDirectory GetRoot(string path)
		{
			InMemoryDirectory directory;

			lock (Directories)
			{
				if (!Directories.TryGetValue(path, out directory))
				{
					Directories.Add(path, directory = new InMemoryDirectory(this, path, false));
				}
			}

			return directory;
		}

		public Directory GetDirectory(string directoryPath)
		{
			return GetDirectory(new Path(directoryPath));
		}

		public Directory GetDirectory(Path directoryPath)
		{ 
			var resolvedDirectoryPath = CurrentDirectory.Combine(directoryPath);
			var segments = resolvedDirectoryPath.Segments;
			var baseCase = GetFirstSegmentAsRootIfRooted(resolvedDirectoryPath, segments);
			return segments.Skip(1).Aggregate(baseCase, (current, segment) => current.GetDirectory(segment));
		}

		Directory GetFirstSegmentAsRootIfRooted(Path directoryPath, IEnumerable<string> segments)
		{
			return directoryPath.IsRooted
				       ? GetRoot(segments.First() + "\\")
				       : GetRoot(segments.First());
		}

		public File GetFile(string filePath)
		{
			return GetFile(filePath.ToPath());
		}

		public File GetFile(Path filePath)
		{
			var resolvedFilePath = CurrentDirectory.Combine(filePath);
			var pathSegments = resolvedFilePath.Segments;
			var baseCase = GetFirstSegmentAsRootIfRooted(filePath, pathSegments);
			var ownerFolder = pathSegments
				.Skip(1).Take(pathSegments.Count() - 2)
				.Aggregate(baseCase, (current, segment) => current.GetDirectory(segment));

			return ownerFolder.GetFile(pathSegments.Last());
		}

		public Path GetPath(string path)
		{
			return new Path(path);
		}

		public TemporaryDirectory CreateTempDirectory()
		{
			var sysTemp = (InMemoryDirectory) GetTempDirectory();

			var tempDirectory = new InMemoryTemporaryDirectory(this, sysTemp.Path.Combine(Path.GetRandomFileName()).FullPath)
				{
					Parent = sysTemp
				};

			lock (sysTemp.ChildDirectories)
				sysTemp.ChildDirectories.Add(tempDirectory);

			return tempDirectory;
		}

		public Directory CreateDirectory(string path)
		{
			return GetDirectory(path).MustExist();
		}

		public Directory CreateDirectory(Path path)
		{
			return GetDirectory(path).MustExist();
		}

		public TemporaryFile CreateTempFile()
		{
			var tempDirectory = (InMemoryDirectory) GetTempDirectory();
			var tempFile = new InMemoryTemporaryFile(tempDirectory.Path.Combine(Path.GetRandomFileName()).ToString())
				{
					FileSystem = this,
					Parent = tempDirectory
				};
			tempDirectory.Create();
			tempDirectory.ChildFiles.Add(tempFile);

			return tempFile;
		}

		public Directory GetTempDirectory()
		{
			if (_systemTempDirectory == null)
			{
				lock (_syncRoot)
				{
					Thread.MemoryBarrier();
					if (_systemTempDirectory == null)
						_systemTempDirectory = GetDirectory(Path.GetTempPath());
				}
			}
			return _systemTempDirectory;
		}

		public Directory GetCurrentDirectory()
		{
			return GetDirectory(CurrentDirectory);
		}

		public Path CurrentDirectory { get; set; }
	}
}