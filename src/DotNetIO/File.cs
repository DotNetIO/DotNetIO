﻿#region license

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

#endregion

using System;
using System.IO;

namespace DotNetIO
{
	/// <summary>
	/// 	Interface representation of a file.
	/// </summary>
	public interface File : FileSystemItem<File>
	{
		/// <summary>
		/// 	Gets the name of the file without an extension.
		/// </summary>
		string NameWithoutExtension { get; }

		/// <summary>
		/// 	Gets the extension, if any, of the file.
		/// </summary>
		string Extension { get; }

		/// <summary>
		/// 	Gets the size of the file.
		/// </summary>
		/// <exception cref = "IOException">A device or hard drive was not ready.</exception>
		/// <exception cref = "FileNotFoundException">The file was not found on the file system.</exception>
		long GetSize();

		/// <summary>
		/// 	Gets the last modified date of the file.
		/// </summary>
		/// <exception cref = "IOException">
		/// 	If the file was not found.
		/// </exception>
		DateTimeOffset? GetLastModifiedTimeUtc();

		/// <summary>
		/// 	Open a file handle.
		/// </summary>
		/// <param name = "fileMode"></param>
		/// <param name = "fileAccess"></param>
		/// <param name = "fileShare"></param>
		/// <param name="bufSize"> </param>
		/// <param name="fileOptions">
		/// Give hints to the operating system
		/// so that it can optimize for your use-case.
		/// </param>
		/// <returns>The stream of the file.</returns>
		Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare,
			int bufSize = 4096, FileOptions fileOptions = FileOptions.Asynchronous);
	}
}