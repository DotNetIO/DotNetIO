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

namespace DotNetIO.FileSystems.Local.Win32
{
	public class Win32FileSystem : LocalFileSystem
	{
		public override Directory GetDirectory(string directoryPath)
		{
			return GetDirectory(directoryPath.ToPath());
		}

		public override Directory GetDirectory(Path directoryPath)
		{
			return directoryPath.IsRooted
				? new Win32Directory(directoryPath)
				: new Win32Directory(new Path(Environment.CurrentDirectory).Combine(directoryPath));
		}

		public override File GetFile(string filePath)
		{
			return GetFile(filePath.ToPath());
		}

		public override File GetFile(Path filePath)
		{
			return filePath.IsRooted
				? new Win32File(filePath, CreateDirectory)
				: new Win32File(new Path(Environment.CurrentDirectory).Combine(filePath), CreateDirectory);
		}

		public override Directory GetTempDirectory()
		{
			return new Win32Directory(Path.GetTempPath().FullPath);
		}
	}
}