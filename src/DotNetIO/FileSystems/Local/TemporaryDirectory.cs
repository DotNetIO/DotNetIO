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

namespace DotNetIO.FileSystems.Local
{
	public class TemporaryDirectory : DotNetIO.TemporaryDirectory
	{
		public Directory UnderlyingDirectory { get; set; }

		public TemporaryDirectory(Directory unerlyingDirectory)
		{
			UnderlyingDirectory = unerlyingDirectory;
			if (!UnderlyingDirectory.Exists())
				UnderlyingDirectory.Create();
		}

		public Directory Create()
		{
			return UnderlyingDirectory.Create();
		}

		public Directory GetDirectory(string directoryName)
		{
			return UnderlyingDirectory.GetDirectory(directoryName);
		}

		public File GetFile(string fileName)
		{
			return UnderlyingDirectory.GetFile(fileName);
		}

		public IEnumerable<File> Files()
		{
			return UnderlyingDirectory.Files();
		}

		public IEnumerable<Directory> Directories()
		{
			return UnderlyingDirectory.Directories();
		}

		public IEnumerable<File> Files(string filter, SearchScope scope)
		{
			return UnderlyingDirectory.Files(filter, scope);
		}

		public IEnumerable<Directory> Directories(string filter, SearchScope scope)
		{
			return UnderlyingDirectory.Directories(filter, scope);
		}

		public bool IsHardLink
		{
			get { return UnderlyingDirectory.IsHardLink; }
		}

		public Directory LinkTo(Path path)
		{
			return UnderlyingDirectory.LinkTo(path);
		}

		public Directory Target
		{
			get { return UnderlyingDirectory.Target; }
		}

		public Path Path
		{
			get { return UnderlyingDirectory.Path; }
		}

		public Directory Parent
		{
			get { return UnderlyingDirectory.Parent; }
		}

		public FileSystem FileSystem
		{
			get { return UnderlyingDirectory.FileSystem; }
		}

		public bool Exists()
		{
			return UnderlyingDirectory.Exists();
		}

		public string Name
		{
			get { return UnderlyingDirectory.Name; }
		}

		public void Delete()
		{
			UnderlyingDirectory.Delete();
		}

		public void CopyTo(FileSystemItem item)
		{
			UnderlyingDirectory.CopyTo(item);
		}

		public FileSystemItem MoveTo(FileSystemItem item)
		{
			UnderlyingDirectory.MoveTo(item);
			return item;
		}


		~TemporaryDirectory()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Delete();
		}
	}
}