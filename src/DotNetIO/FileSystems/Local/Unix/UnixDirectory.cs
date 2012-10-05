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

using Mono.Unix;

namespace DotNetIO.FileSystems.Local.Unix
{
	public class UnixDirectory : LocalDirectory
	{
		public UnixDirectory(Path directoryPath)
			: base(directoryPath)
		{
			GetUnixInfo(directoryPath.FullPath);
		}

		public override Directory GetDirectory(string directoryName)
		{
			return new UnixDirectory(Path.Combine(directoryName));
		}

		public override File GetFile(string fileName)
		{
			return new UnixFile(Path, CreateDirectory);
		}

		public override bool IsHardLink
		{
			get { return UnixDirectoryInfo.IsSymbolicLink; }
		}

		public override Directory Target
		{
			get
			{
				return IsHardLink
					       ? new UnixDirectory(((UnixSymbolicLinkInfo) UnixDirectoryInfo)
						                           .GetContents()
						                           .FullName
						                           .ToPath())
					       : this;
			}
		}

		protected UnixFileSystemInfo UnixDirectoryInfo { get; set; }

		public override Directory LinkTo(Path path)
		{
			UnixDirectoryInfo.CreateLink(path.FullPath);
			return CreateDirectory(path);
		}

		protected override LocalDirectory CreateDirectory(Path path)
		{
			return new UnixDirectory(path);
		}

		void GetUnixInfo(string fullName)
		{
			UnixDirectoryInfo = UnixFileSystemInfo.GetFileSystemEntry(fullName);
		}
	}
}