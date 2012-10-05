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

using System.Diagnostics.Contracts;
using DotNetIO.Contracts;

namespace DotNetIO
{
	[ContractClass(typeof (FileSystemItemTContract<>))]
	public interface FileSystemItem<out T> : FileSystemItem
		where T : FileSystemItem
	{
		T Create();
	}

	[ContractClass(typeof (FileSystemItemContract))]
	public interface FileSystemItem
	{
		Path Path { get; }

		/// <summary>
		/// 	Gets the parent of this item; null if there is no parent.
		/// </summary>
		Directory Parent { get; }

		FileSystem FileSystem { get; }

		bool Exists();

		string Name { get; }

		/// <summary>
		/// 	Deletes the item from the file system.
		/// </summary>
		void Delete();

		/// <summary>
		/// 	Copies the callee to the file system item passed as parameter,
		/// 	and overwrites it if it already exists.
		/// </summary>
		/// <param name = "item">The target of the copy. Targets that work:
		/// 	<list>
		/// 		<item>Directory -> Directory, OK</item>
		/// 		<item>Directory -> File, Exception</item>
		/// 		<item>File -> File, OK</item>
		/// 		<item>File -> Directory, OK</item>
		/// 	</list>
		/// </param>
		void CopyTo(FileSystemItem item);

		/// <summary>
		/// Moves the current directory or file to the target. The
		/// instance still points to the old file or directory
		/// that does not exist anymore.
		/// </summary>
		/// <param name="target">The target file system item</param>
		/// <returns>The newly overwritten or created target item.</returns>
		FileSystemItem MoveTo(FileSystemItem target);
	}
}