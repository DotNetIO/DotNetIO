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

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DotNetIO.Contracts;

namespace DotNetIO
{
	/// <summary>
	/// 	A directory pointer. It might point to an existing directory or 
	/// 	be merely a handle that points to a directory that could be.
	/// </summary>
	[ContractClass(typeof (IDirectoryContract))]
	public interface Directory : FileSystemItem<Directory>
	{
		/// <summary>
		/// </summary>
		/// <param name="directoryName"> </param>
		/// <returns> </returns>
		Directory GetDirectory(string directoryName);

		/// <summary>
		/// </summary>
		/// <param name="fileName"> </param>
		/// <returns> </returns>
		File GetFile(string fileName);

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		IEnumerable<File> Files();

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		IEnumerable<Directory> Directories();

		/// <summary>
		/// </summary>
		/// <param name="filter"> </param>
		/// <param name="scope"> </param>
		/// <returns> </returns>
		IEnumerable<File> Files(string filter, SearchScope scope);

		/// <summary>
		/// </summary>
		/// <param name="filter"> </param>
		/// <param name="scope"> </param>
		/// <returns> </returns>
		IEnumerable<Directory> Directories(string filter, SearchScope scope);

		/// <summary>
		/// 	Gets whether this directory pointer is a hard link.
		/// </summary>
		bool IsHardLink { get; }

		/// <summary>
		/// 	Creates a junction point (Windows) or symlink (Unix) to the target path
		/// </summary>
		/// <param name="target"> Where to link </param>
		/// <returns> The directory that is the target directory. </returns>
		Directory LinkTo(Path target);

		/// <summary>
		/// 	Gets the target of the directory;
		/// 	the target of a junction point if a junction point;
		/// 	the identital directory otherwise.
		/// </summary>
		Directory Target { get; }
	}
}