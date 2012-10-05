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
using System.IO;

namespace DotNetIO.FileSystems.Local
{
	public class TemporaryLocalFile : TemporaryFile
	{
		readonly File _backingFile;
		
		public TemporaryLocalFile(File backingFile)
		{
			_backingFile = backingFile;

			if (!backingFile.Exists())
				backingFile.Create();
		}

		~TemporaryLocalFile()
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
			_backingFile.Delete();
		}

		#region delegating to file

		public Path Path
		{
			get { return _backingFile.Path; }
		}

		public Directory Parent
		{
			get { return _backingFile.Parent; }
		}

		public FileSystem FileSystem
		{
			get { return _backingFile.FileSystem; }
		}

		public bool Exists()
		{
			return _backingFile.Exists();
		}

		public string Name
		{
			get { return _backingFile.Name; }
		}

		public void Delete()
		{
			_backingFile.Delete();
		}

		public void CopyTo(FileSystemItem item)
		{
			_backingFile.CopyTo(item);
		}

		public FileSystemItem MoveTo(FileSystemItem target)
		{
			return _backingFile.MoveTo(target);
		}

		public File Create()
		{
			return _backingFile.Create();
		}

		public string NameWithoutExtension
		{
			get { return _backingFile.NameWithoutExtension; }
		}

		public string Extension
		{
			get { return _backingFile.Extension; }
		}

		public long GetSize()
		{
			return _backingFile.GetSize();
		}

		public DateTimeOffset? GetLastModifiedTimeUtc()
		{
			return _backingFile.GetLastModifiedTimeUtc();
		}

		public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int bufSize = 4096,
		                   FileOptions fileOptions = FileOptions.Asynchronous)
		{
			return _backingFile.Open(fileMode, fileAccess, fileShare, bufSize, fileOptions);
		}

		#endregion
	}
}