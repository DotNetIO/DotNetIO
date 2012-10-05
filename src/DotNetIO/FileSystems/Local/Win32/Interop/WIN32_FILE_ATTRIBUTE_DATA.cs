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
using System.Runtime.InteropServices.ComTypes;

namespace DotNetIO.FileSystems.Local.Win32.Interop
{
	// http://msdn.microsoft.com/en-us/library/windows/desktop/aa365739%28v=vs.85%29.aspx
	[Serializable]
	public struct WIN32_FILE_ATTRIBUTE_DATA
	{
		public FileAttributes dwFileAttributes;
		public FILETIME ftCreationTime;
		public FILETIME ftLastAccessTime;
		public FILETIME ftLastWriteTime;
		public int nFileSizeHigh;
		public int nFileSizeLow;
	}
}