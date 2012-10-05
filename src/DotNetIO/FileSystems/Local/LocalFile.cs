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
using System.Diagnostics.Contracts;
using System.IO;

namespace DotNetIO.FileSystems.Local
{
	public abstract class LocalFile : File, IEquatable<LocalFile>
	{
		protected readonly Func<Path, Directory> _directoryFactory;

		public LocalFile(Path filePath, Func<Path, Directory> directoryFactory)
		{
			Contract.Requires(filePath != null);
			Contract.Requires(directoryFactory != null);

			_directoryFactory = directoryFactory;

			Path = filePath;
		}

		public virtual bool Exists()
		{
			var i = Path.Info;
			var p = Path.FullPath;
			var file = p.StartsWith(i.UNCPrefix) ? p.Substring(i.UNCPrefix.Length).TrimStart('\\') : p;
			return System.IO.File.Exists(file);
		}

		public virtual FileSystem FileSystem
		{
			get { return LocalFileSystem.Instance; }
		}

		public virtual string Name
		{
			get { return Path.GetFileName(Path.FullPath); }
		}

		public virtual string NameWithoutExtension
		{
			get { return Path.GetFileNameWithoutExtension(Path.FullPath); }
		}

		public virtual string Extension
		{
			get { return Path.GetExtension(Path.FullPath); }
		}

		public virtual long GetSize()
		{
			return new FileInfo(Path.FullPath).Length;
		}

		public virtual DateTimeOffset? GetLastModifiedTimeUtc()
		{
			return Exists()
				? new DateTimeOffset(new FileInfo(Path.FullPath).LastWriteTimeUtc, TimeSpan.Zero) 
				: (DateTimeOffset?) null;
		}

		public override string ToString()
		{
			return Path.FullPath;
		}

		public virtual Directory Parent
		{
			get
			{
				try
				{
					var path = Path.GetPathWithoutLastBit(Path.FullPath);
					return path == null
						       ? null
						       : _directoryFactory(path);
				}
				catch (DirectoryNotFoundException)
				{
					return null;
				}
			}
		}

		public virtual Path Path { get; private set; }

		public virtual Stream Open(FileMode fileMode,
			FileAccess fileAccess, FileShare fileShare, int bufSize, FileOptions fileOptions)
		{
			PrepareOpen(fileMode, fileAccess);
			return System.IO.File.Open(Path.FullPath, fileMode, fileAccess, fileShare);
		}

		protected void PrepareOpen(FileMode fileMode, FileAccess fileAccess)
		{
			if (!Exists() && !Parent.Exists())
				if (fileMode == FileMode.Create || fileMode == FileMode.CreateNew || fileMode == FileMode.OpenOrCreate)
					Parent.Create();

			Validate.FileAccess(fileMode, fileAccess);
		}

		public virtual void Delete()
		{
			if (System.IO.File.Exists(Path.FullPath))
				System.IO.File.Delete(Path.FullPath);
		}

		public virtual void CopyTo(FileSystemItem item)
		{
			var destinationPath = PrepareCopyTo(item);
			System.IO.File.Copy(Path.FullPath, destinationPath, true);
		}

		// ensure the item exists
		protected string PrepareCopyTo(FileSystemItem item)
		{
			string destinationPath;

			if (item is Directory)
			{
				((Directory) item).MustExist();
				destinationPath = item.Path.Combine(Name).FullPath;
			}
			else
			{
				item.Parent.MustExist();
				destinationPath = (item).Path.FullPath;
			}

			return destinationPath;
		}

		public virtual FileSystemItem MoveTo(FileSystemItem item)
		{
			System.IO.File.Move(Path.FullPath, item.Path.FullPath);
			return item;
		}

		public virtual File Create()
		{
			// creates the parent if it doesnt exist
			if (!Parent.Exists())
				Parent.Create();

			System.IO.File.Create(Path.FullPath).Close();
			return this;
		}

		#region equality implementation

		public bool Equals(LocalFile other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Path, Path);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (LocalFile)) return false;
			return Equals((LocalFile) obj);
		}

		public override int GetHashCode()
		{
			return Path.GetHashCode();
		}

		public static bool operator ==(LocalFile left, LocalFile right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LocalFile left, LocalFile right)
		{
			return !Equals(left, right);
		}

		#endregion
	}
}