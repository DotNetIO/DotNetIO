﻿// Copyright 2004-2012 Henrik Feldt - https://github.com/DotNetIO
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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DotNetIO.FileSystems.Local.Win32.Interop;

namespace DotNetIO.Internal
{
	public static class LongPathCommon
	{
		internal static string NormalizeSearchPattern(string searchPattern)
		{
			if (String.IsNullOrEmpty(searchPattern) || searchPattern == ".")
				return "*";

			return searchPattern;
		}

		public static string NormalizeLongPath(string path)
		{
			return NormalizeLongPath(path, "path");
		}

		// Normalizes path (can be longer than MAX_PATH) and adds \\?\ long path prefix
		internal static string NormalizeLongPath(string path, string parameterName)
		{
			Contract.Requires(path != null, parameterName);
			Contract.Ensures(Contract.Result<string>() != null);

			if (path.Length == 0)
				throw new ArgumentException(
					String.Format(CultureInfo.CurrentCulture, "'{0}' cannot be an empty string.", parameterName), parameterName);

			var buffer = new StringBuilder(path.Length + 1); // Add 1 for NULL
			var length = NativeMethods.GetFullPathName(path, (uint) buffer.Capacity, buffer, IntPtr.Zero);
			if (length > buffer.Capacity)
			{
				// Resulting path longer than our buffer, so increase it

				buffer.Capacity = (int) length;
				length = NativeMethods.GetFullPathName(path, length, buffer, IntPtr.Zero);
			}

			if (length == 0)
			{
				throw GetExceptionFromLastWin32Error(parameterName);
			}

			if (length > NativeMethods.MAX_LONG_PATH)
			{
				throw GetExceptionFromWin32Error(NativeMethods.ERROR_FILENAME_EXCED_RANGE, parameterName);
			}

			return AddLongPathPrefix(buffer.ToString());
		}

		static bool TryNormalizeLongPath(string path, out string result)
		{
			try
			{
				result = NormalizeLongPath(path);
				return true;
			}
			catch (ArgumentException)
			{
			}
			catch (PathTooLongException)
			{
			}

			result = null;
			return false;
		}

		static string AddLongPathPrefix(string path)
		{
			return path.StartsWith(NativeMethods.LongPathPrefix) ? path : NativeMethods.LongPathPrefix + path;
		}

		internal static string RemoveLongPathPrefix(string normalizedPath)
		{
			return normalizedPath.Substring(NativeMethods.LongPathPrefix.Length);
		}

		internal static bool Exists(Path path, out bool isDirectory)
		{
			string normalizedPath;
			if (TryNormalizeLongPath(path.FullPath, out normalizedPath))
			{
				FileAttributes attributes;
				var errorCode = TryGetFileAttributes(normalizedPath, out attributes);
				if (errorCode == 0)
				{
					isDirectory = LongPathDirectory.IsDirectory(attributes);
					return true;
				}
			}

			isDirectory = false;
			return false;
		}

		internal static int TryGetDirectoryAttributes(string normalizedPath, out FileAttributes attributes)
		{
			var errorCode = TryGetFileAttributes(normalizedPath, out attributes);
			if (!LongPathDirectory.IsDirectory(attributes))
				errorCode = NativeMethods.ERROR_DIRECTORY;

			return errorCode;
		}

		internal static int TryGetFileAttributes(string normalizedPath, out FileAttributes attributes)
		{
			// NOTE: Don't be tempted to use FindFirstFile here, it does not work with root directories

			attributes = NativeMethods.GetFileAttributes(normalizedPath);
			if ((int) attributes == NativeMethods.INVALID_FILE_ATTRIBUTES)
				return Marshal.GetLastWin32Error();

			return 0;
		}

		internal static Exception GetExceptionFromLastWin32Error()
		{
			return GetExceptionFromLastWin32Error("path");
		}

		internal static Exception GetExceptionFromLastWin32Error(string parameterName)
		{
			return GetExceptionFromWin32Error(Marshal.GetLastWin32Error(), parameterName);
		}

		internal static Exception GetExceptionFromWin32Error(int errorCode)
		{
			return GetExceptionFromWin32Error(errorCode, "errorCode");
		}

		internal static Exception GetExceptionFromWin32Error(int errorCode, string parameterName)
		{
			var message = GetMessageFromErrorCode(errorCode);

			switch (errorCode)
			{
				case NativeMethods.ERROR_FILE_NOT_FOUND:
					return new FileNotFoundException(message);

				case NativeMethods.ERROR_PATH_NOT_FOUND:
					return new DirectoryNotFoundException(message);

				case NativeMethods.ERROR_ACCESS_DENIED:
					return new UnauthorizedAccessException(message);

				case NativeMethods.ERROR_FILENAME_EXCED_RANGE:
					return new PathTooLongException(message);

				case NativeMethods.ERROR_INVALID_DRIVE:
					return new DriveNotFoundException(message);

				case NativeMethods.ERROR_OPERATION_ABORTED:
					return new OperationCanceledException(message);

				case NativeMethods.ERROR_INVALID_NAME:
					return new ArgumentException(message, parameterName);

				default:
					return new IOException(message, NativeMethods.MakeHRFromErrorCode(errorCode));
			}
		}

		static string GetMessageFromErrorCode(int errorCode)
		{
			var buffer = new StringBuilder(512);

			var bufferLength =
				NativeMethods.FormatMessage(
					NativeMethods.FORMAT_MESSAGE_IGNORE_INSERTS | NativeMethods.FORMAT_MESSAGE_FROM_SYSTEM |
					NativeMethods.FORMAT_MESSAGE_ARGUMENT_ARRAY, IntPtr.Zero, errorCode, 0, buffer, buffer.Capacity, IntPtr.Zero);

			Contract.Assert(bufferLength != 0);

			return buffer.ToString();
		}
	}
}