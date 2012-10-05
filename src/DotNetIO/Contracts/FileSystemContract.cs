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
using DotNetIO.FileSystems;

namespace DotNetIO.Contracts
{
	/// <summary>
	/// 	Contract class for <see cref="FileSystem" />.
	/// </summary>
	[ContractClassFor(typeof (FileSystem))]
	internal abstract class FileSystemContract : FileSystem
	{
		public FileSystemNotifier Notifier
		{
			get
			{
				Contract.Ensures(Contract.Result<FileSystemNotifier>() != null);
				throw new NotImplementedException();
			}
		}

		public Directory GetDirectory(string directoryPath)
		{
			Contract.Requires(directoryPath != null);
			Contract.Ensures(Contract.Result<Directory>() != null);

			throw new NotImplementedException();
		}

		public Directory GetDirectory(Path directoryPath)
		{
			Contract.Requires(directoryPath != null);
			Contract.Ensures(Contract.Result<Directory>() != null);
			throw new NotImplementedException();
		}

		public Path GetPath(string path)
		{
			Contract.Ensures(Contract.Result<Path>() != null);

			throw new NotImplementedException();
		}

		public TemporaryDirectory CreateTempDirectory()
		{
			Contract.Ensures(Contract.Result<TemporaryDirectory>() != null);

			throw new NotImplementedException();
		}

		public Directory CreateDirectory(string path)
		{
			Contract.Requires(path != null);
			Contract.Ensures(Contract.Result<Directory>() != null);

			throw new NotImplementedException();
		}

		public Directory CreateDirectory(Path path)
		{
			Contract.Requires(path != null);
			Contract.Ensures(Contract.Result<Directory>() != null);

			throw new NotImplementedException();
		}

		public File GetFile(string filePath)
		{
			Contract.Requires(filePath != null);
			Contract.Ensures(Contract.Result<File>() != null);

			throw new NotImplementedException();
		}

		public File GetFile(Path filePath)
		{
			Contract.Requires(filePath != null);
			Contract.Ensures(Contract.Result<File>() != null);

			throw new NotImplementedException();
		}

		public TemporaryFile CreateTempFile()
		{
			Contract.Ensures(Contract.Result<TemporaryFile>() != null);
			throw new NotImplementedException();
		}

		public Directory GetTempDirectory()
		{
			Contract.Ensures(Contract.Result<Directory>() != null);

			throw new NotImplementedException();
		}

		public Directory GetCurrentDirectory()
		{
			Contract.Ensures(Contract.Result<Directory>() != null);

			throw new NotImplementedException();
		}
	}
}