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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetIO.Internal;

namespace DotNetIO.FileSystems.Local
{
	public abstract class LocalDirectory
		: Directory, IEquatable<Directory>
	{
		readonly LocalFileSystem _localFileSystem;

		protected LocalDirectory(Path directoryPath)
		{
			Contract.Requires(directoryPath != null);

			_localFileSystem = LocalFileSystem.Instance;
			Path = directoryPath;
		}

		protected abstract LocalDirectory CreateDirectory(Path path);

		public virtual bool Exists()
		{
			return System.IO.Directory.Exists(Path.FullPath);
		}

		public override string ToString()
		{
			return Path.FullPath;
		}

		public FileSystem FileSystem
		{
			get { return _localFileSystem; }
		}

		public string Name
		{
			get { return Path.GetFileName(Path.FullPath); }
		}

		public virtual Directory Parent
		{
			get
			{
				return _localFileSystem.GetDirectory(Path.Combine(".."));
			}
		}

		public Path Path { get; private set; }

		public virtual bool IsHardLink
		{
			get { return false; }
		}

		public abstract Directory LinkTo(Path path);

		public virtual Directory Target
		{
			get { return this; }
		}

		public virtual IEnumerable<Directory> Directories(string filter, SearchScope scope)
		{
			filter = filter.Trim();

			if (filter.EndsWith(Path.DirectorySeparatorChar + "") || filter.EndsWith(Path.AltDirectorySeparatorChar + ""))
				filter = filter.Substring(0, filter.Length - 1);

			if (Path.IsPathRooted(filter) || Regex.IsMatch(filter, "[A-Z]{1,3}:", RegexOptions.IgnoreCase))
			{
				var root = Path.GetPathRoot(filter);
				filter = filter.Substring(root.Length);
				return _localFileSystem.GetDirectory(root).Directories(filter, scope);
			}

			if (scope == SearchScope.CurrentOnly)
				return DirectoriesTopOnly(filter);
			
			return DirectoriesRecurse(_localFileSystem, this, filter);
		}

		static IEnumerable<Directory> DirectoriesRecurse(FileSystem fs, Directory directory, string filter)
		{
			// TODO: needs to be replaced with a cross-platform enumeration
			foreach (var topDir in LongPathDirectory.EnumerateDirectories(directory.Path, filter))
			{
				foreach (var subDir in DirectoriesRecurse(fs, fs.GetDirectory(topDir), filter))
					yield return subDir;

				yield return fs.CreateDirectory(topDir);
			}
		}

		IEnumerable<Directory> DirectoriesTopOnly(string filter)
		{
			return LongPathDirectory.EnumerateDirectories(Path, filter).Select(CreateDirectory);
		}

		public abstract File GetFile(string fileName);

		public IEnumerable<File> Files()
		{
			return LongPathDirectory.EnumerateFiles(Path).Select(_localFileSystem.GetFile);
		}

		public IEnumerable<File> Files(string filter, SearchScope scope)
		{
			filter = filter.Trim();

			if (filter.EndsWith(Path.DirectorySeparatorChar + "") ||
			    filter.EndsWith(Path.AltDirectorySeparatorChar + ""))
				filter += Path.DirectorySeparatorChar + "*";

			var di = new DirectoryInfo(Path.FullPath);

			return
				di.GetFiles(filter, scope == SearchScope.CurrentOnly
					    ? SearchOption.TopDirectoryOnly
					    : SearchOption.AllDirectories)
					.Select(x => x.FullName)
					.Select(GetFile)
					.Cast<File>();
		}

		public abstract Directory GetDirectory(string directoryName);

		public virtual IEnumerable<Directory> Directories()
		{
			return LongPathDirectory.EnumerateDirectories(Path).Select(CreateDirectory).Cast<Directory>();
		}

		public virtual void Delete()
		{
			if (System.IO.Directory.Exists(Path.FullPath))
				System.IO.Directory.Delete(Path.FullPath, true);
		}

		public virtual Directory Create()
		{
			LongPathDirectory.Create(Path);
			return this;
		}

		public FileSystemItem MoveTo(FileSystemItem newFileName)
		{
			LongPathDirectory.Move(Path, newFileName.Path);
			return newFileName;
		}

		public void CopyTo(FileSystemItem newItem)
		{
			var destDir = (Directory) newItem;

			destDir.MustExist();

			foreach (var file in Files())
				file.CopyTo(newItem);

			foreach (var directory in Directories())
				directory.CopyTo(destDir.GetDirectory(directory.Name).MustExist());
		}

		public bool Equals(Directory other)
		{
			if (ReferenceEquals(null, other)) return false;
			return other.Path.Equals(Path);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj as Directory == null) return false;
			return Equals((Directory) obj);
		}

		public override int GetHashCode()
		{
			return Path.GetHashCode();
		}
	}
}