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
using DotNetIO.FileSystems.Local.Win32.Interop;
using DotNetIO.Internal;

namespace DotNetIO.FileSystems.Local.Win32
{
	public class Win32File
		: LocalFile
	{
		public Win32File(Path filePath, Func<Path, Directory> directoryFactory) 
			: base(filePath, directoryFactory)
		{
		}

		public override bool Exists()
		{
			return LongPathFile.Exists(Path);
		}

		// http://stackoverflow.com/questions/8991192/check-filesize-without-opening-file-in-c
		public override long GetSize()
		{
			using (var fh = LongPathFile.GetFileHandle(Path,
				FileMode.Open, FileAccess.Read, FileShare.ReadWrite, FileOptions.None))
			{
				if (fh.IsInvalid)
					throw LongPathCommon.GetExceptionFromLastWin32Error();

				long size;
				if (NativeMethods.GetFileSizeEx(fh, out size))
					return size;

				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
		}

		public override DateTimeOffset? GetLastModifiedTimeUtc()
		{
			var normalizedPath = LongPathCommon.NormalizeLongPath(Path.FullPath);

			if (!Exists())
				return null;

			WIN32_FILE_ATTRIBUTE_DATA data;
			if (!NativeMethods.GetFileAttributesEx(
				normalizedPath,
				GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard,
				out data))
				throw LongPathCommon.GetExceptionFromLastWin32Error();

			var ft = data.ftLastWriteTime;
			return DateTime.FromFileTime((((long) ft.dwHighDateTime) << 32) + ft.dwLowDateTime);
		}

		public override Stream Open(FileMode fileMode,
			FileAccess fileAccess, FileShare fileShare, int bufSize, FileOptions fileOptions)
		{
			PrepareOpen(fileMode, fileAccess);
			return LongPathFile.Open(Path, fileMode, fileAccess, fileShare);
		}

		public override void Delete()
		{
			if (LongPathFile.Exists(Path))
				LongPathFile.Delete(Path);
		}

		public override void CopyTo(FileSystemItem item)
		{
			var destinationPath = PrepareCopyTo(item);
			LongPathFile.Copy(Path.FullPath, destinationPath, true);
		}

		public override FileSystemItem MoveTo(FileSystemItem item)
		{
			LongPathFile.Move(Path, item.Path);
			return item;
		}

		public override File Create()
		{
			// creates the parent if it doesnt exist
			if (!Parent.Exists())
				Parent.Create();

			var handle = LongPathFile.GetFileHandle(Path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite, FileOptions.None);
			handle.Dispose();
			
			return this;
		}
	}
}