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
using DotNetIO.FileSystems.Local.Unix;
using DotNetIO.FileSystems.Local.Win32;

namespace DotNetIO.FileSystems.Local
{
	public abstract class LocalFileSystem : AbstractFileSystem
	{
		static volatile LocalFileSystem _instance;
		static readonly object _syncRoot = new object();

		readonly LocalFileSystemNotifier _notifier;

		public LocalFileSystem()
		{
			_notifier = new LocalFileSystemNotifier();
		}

		/// <summary>
		/// 	Gets an instance of the local file system. This property
		/// 	is a singleton.
		/// </summary>
		/// <exception cref="PlatformNotSupportedException">If your platform isn't supported by this API.
		/// 	Currently Windows, OSX and Unix platforms are supported.</exception>
		public static LocalFileSystem Instance
		{
			get
			{
				Contract.Ensures(Contract.Result<LocalFileSystem>() != null);

				if (_instance == null)
					lock (_syncRoot)
						if (_instance == null)
							_instance = CreatePlatformSpecificInstance();

				return _instance;
			}
		}

		static LocalFileSystem CreatePlatformSpecificInstance()
		{
			Contract.Ensures(Contract.Result<LocalFileSystem>() != null);

			var platformId = (int) Environment.OSVersion.Platform;

			if (platformId == (int) PlatformID.Win32NT)
				return CreateWin32FileSystem();

			if (platformId == 4 || platformId == 128 || platformId == (int) PlatformID.MacOSX)
				return UnixFileSystem();

			throw new NotSupportedException("Platform not supported");
		}

		static LocalFileSystem CreateWin32FileSystem()
		{
			Contract.Ensures(Contract.Result<LocalFileSystem>() != null);

			return new Win32FileSystem();
		}

		static LocalFileSystem UnixFileSystem()
		{
			Contract.Ensures(Contract.Result<LocalFileSystem>() != null);

			return new UnixFileSystem();
		}

		public override Directory CreateDirectory(string path)
		{
			return GetDirectory(path).Create();
		}

		public override Directory CreateDirectory(Path path)
		{
			Contract.Ensures(Contract.Result<Directory>() != null);

			return GetDirectory(path).Create();
		}

		public override TemporaryDirectory CreateTempDirectory()
		{
			Contract.Ensures(Contract.Result<TemporaryDirectory>() != null);

			var tempPath = Path.GetTempPath();
			var dirName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
			var directory = CreateDirectory(tempPath.Combine(dirName)); // creates dir

			return new TemporaryLocalDirectory(directory);
		}

		public override TemporaryFile CreateTempFile()
		{
			Contract.Ensures(Contract.Result<TemporaryFile>() != null);

			var randomFilePath = Path.GetTempFileName(); // creates file
			var backingFile = GetFile(randomFilePath);

			return new TemporaryLocalFile(backingFile);
		}

		public override FileSystemNotifier Notifier
		{
			get { return _notifier; }
		}

		public override Path GetPath(string path)
		{
			Contract.Requires(path != null);

			return new Path(path);
		}

		public override Directory GetCurrentDirectory()
		{
			return GetDirectory(Environment.CurrentDirectory);
		}
	}
}