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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetIO.Internal;

namespace DotNetIO.FileSystems.InMemory
{
	public class InMemoryDirectory : Directory, IEquatable<Directory>
	{
		InMemoryDirectory _source;
		readonly InMemoryFileSystem _fileSystem;
		Directory _target;

		public InMemoryDirectory(InMemoryFileSystem fileSystem, string directoryPath, bool exists = true)
		{
			_fileSystem = fileSystem;

			_source = this;
			_exists = exists;

			Path = new Path(directoryPath);

			ChildDirectories = new List<InMemoryDirectory>();
			ChildFiles = new List<InMemoryFile>();
		}

		public List<InMemoryDirectory> ChildDirectories { get; set; }
		public List<InMemoryFile> ChildFiles { get; set; }

		bool _exists;

		bool FileSystemItem.Exists()
		{
			return _exists;
		}

		public FileSystem FileSystem
		{
			get { return _fileSystem; }
		}

		public bool IsHardLink { get; private set; }

		public string Name
		{
			get { return Path.GetDirectoryName(Path.FullPath); }
		}

		public Directory Parent { get; set; }

		public Path Path { get; private set; }

		public Directory Target
		{
			get { return _target ?? this; }
			set { _target = value; }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (!(obj is Directory)) return false;
			return Equals((Directory) obj);
		}

		public override int GetHashCode()
		{
			return Path.GetHashCode();
		}

		public IEnumerable<Directory> Directories()
		{
			lock (ChildDirectories)
			{
				return ChildDirectories
					.Cast<Directory>()
					.Where(x => x.Exists()).ToList();
			}
		}

		public IEnumerable<Directory> Directories(string filter, SearchScope scope)
		{
			var path = new Path(filter);
			// if it's a rooted or it's a in the form of e.g. "C:"
			if (path.IsRooted || Regex.IsMatch(filter, "[A-Z]{1,3}:", RegexOptions.IgnoreCase))
			{
				var directory = FileSystem.GetDirectory(path.Segments.First());
				return path.Segments.Count() == 1
					       ? new[] {directory}
					       : directory.Directories(string.Join("\\", path.Segments.Skip(1)
						                                                 .DefaultIfEmpty("*")
						                                                 .ToArray()));
			}

			var filterRegex = filter.Wildcard();
			lock (ChildDirectories)
			{
				var immediateChildren = ChildDirectories
					.Cast<Directory>()
					.Where(x => x.Exists() && filterRegex.IsMatch(x.Name));

				return scope == SearchScope.CurrentOnly
					       ? immediateChildren.ToList()
					       : immediateChildren
						         .Concat(ChildDirectories.SelectMany(x => x.Directories(filter, scope))).ToList();
			}
		}

		public IEnumerable<File> Files()
		{
			lock (ChildFiles)
			{
				return ChildFiles.Where(x => x.Exists()).Cast<File>().ToList();
			}
		}

		public IEnumerable<File> Files(string filter, SearchScope searchScope)
		{
			lock (ChildFiles)
				lock (ChildDirectories)
				{
					var filterRegex = filter.Wildcard();
					var immediateChildren = ChildFiles.Where(x => x.Exists() && filterRegex.IsMatch(x.Name)).Cast<File>();
					return searchScope == SearchScope.CurrentOnly
						       ? immediateChildren.ToList()
						       : immediateChildren.Concat(ChildDirectories.SelectMany(x => x.Files(filter, searchScope))).ToList();
				}
		}

		public Directory GetDirectory(string directoryName)
		{
			if (Path.IsPathRooted(directoryName))
				return FileSystem.GetDirectory(directoryName);

			InMemoryDirectory inMemoryDirectory;

			lock (ChildDirectories)
			{
				inMemoryDirectory = ChildDirectories.FirstOrDefault(x => x.Name.Equals(directoryName, StringComparison.OrdinalIgnoreCase));

				if (inMemoryDirectory == null)
				{
					inMemoryDirectory = new InMemoryDirectory(_fileSystem, Path.Combine(directoryName).FullPath, false)
						{
							Parent = this
						};

					ChildDirectories.Add(inMemoryDirectory);
				}
			}

			return inMemoryDirectory;
		}

		public File GetFile(string fileName)
		{
			InMemoryFile file;
			lock (ChildFiles)
			{
				file = ChildFiles.FirstOrDefault(x => x.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));
				if (file == null)
				{
					file = new InMemoryFile(Path.Combine(fileName).FullPath) {Parent = this};
					ChildFiles.Add(file);
				}
				file.FileSystem = FileSystem;
			}
			return file;
		}

		public Directory LinkTo(Path path)
		{
			var directory = GetDirectory(path.FullPath);

			if (directory.Exists())
				throw new IOException(string.Format("Cannot create link at location '{0}', a directory already exists.", path));
			
			var linkDirectory = (InMemoryDirectory) directory;
			linkDirectory.ChildDirectories = ChildDirectories;
			linkDirectory.ChildFiles = ChildFiles;
			linkDirectory.IsHardLink = true;
			linkDirectory._exists = true;
			linkDirectory.Target = this;
			
			return linkDirectory;
		}

		public bool Equals(Directory other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Path, Path);
		}

		public void Delete()
		{
			if (!IsHardLink)
			{
				foreach (var childDirectory in ChildDirectories.Copy())
					childDirectory.Delete();
			}
			_exists = false;
		}

		public void CopyTo(FileSystemItem item)
		{
			if (item is File)
				throw new IOException("Cannot copy a directory to a file.");
			var target = (Directory) item;
			lock (ChildFiles)
				lock (ChildDirectories)
				{
					foreach (var file in ChildFiles)
						file.CopyTo(target.GetFile(file.Name));
					foreach (var dir in ChildDirectories)
						dir.CopyTo(target.GetDirectory(dir.Name).MustExist());
				}
		}

		public FileSystemItem MoveTo(FileSystemItem item)
		{
			if (!item.Path.IsRooted) throw new ArgumentException("Has to be a fully-qualified path for a move to succeed.");
			var newDirectory = (InMemoryDirectory) FileSystem.GetDirectory(item.Path.FullPath);
			newDirectory._exists = true;

			lock (ChildFiles)
				lock (ChildDirectories)
					lock (newDirectory.ChildFiles)
						lock (newDirectory.ChildDirectories)
						{
							newDirectory.ChildFiles = ChildFiles;
							newDirectory.ChildDirectories = ChildDirectories;

							foreach (var file in newDirectory.ChildFiles.OfType<InMemoryFile>())
								file.Parent = newDirectory;

							foreach (var file in newDirectory.ChildDirectories.OfType<InMemoryDirectory>())
								file.Parent = newDirectory;

							_exists = false;
							ChildFiles = new List<InMemoryFile>();
							ChildDirectories = new List<InMemoryDirectory>();
						}

			return newDirectory;
		}

		public Directory Create()
		{
			_exists = true;
			if (Parent != null && !Parent.Exists())
				Parent.Create();
			return this;
		}

		public override string ToString()
		{
			return Path.FullPath;
		}
	}
}