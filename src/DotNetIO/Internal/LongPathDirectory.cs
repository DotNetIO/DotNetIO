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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DotNetIO.FileSystems.Local.Win32.Interop;

//     Copyright (c) Microsoft Corporation.  All rights reserved.

namespace DotNetIO.Internal
{
	/// <summary>
	/// 	Provides methods for creating, deleting, moving and enumerating directories and 
	/// 	subdirectories with long paths, that is, paths that exceed 259 characters.
	/// </summary>
	public static class LongPathDirectory
	{
		/// <summary>
		/// 	Creates the specified directory.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "string" /> containing the path of the directory to create.
		/// </param>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> contains one or more directories that could not be
		/// 	found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		/// <remarks>
		/// 	Note: Unlike <see cref = "System.IO.Directory.CreateDirectory(System.String)" />, this method only creates 
		/// 	the last directory in <paramref name = "path" />.
		/// </remarks>
		public static void Create(Path path)
		{
			if (!NativeMethods.CreateDirectory(path.LongFullPath, IntPtr.Zero))
			{
				// To mimic Directory.CreateDirectory, we don't throw if the directory (not a file) already exists
				var errorCode = Marshal.GetLastWin32Error();
				if (errorCode != NativeMethods.ERROR_ALREADY_EXISTS || !Exists(path))
					throw LongPathCommon.GetExceptionFromWin32Error(errorCode);
			}
		}

		/// <summary>
		/// 	Deletes the specified empty directory.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "String" /> containing the path of the directory to delete.
		/// </param>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> could not be found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> refers to a directory that is read-only.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> refers to a directory that is not empty.
		/// 	<para>
		/// 		-or-    
		/// 	</para>
		/// 	<paramref name = "path" /> refers to a directory that is in use.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		public static void Delete(Path path)
		{
			if (!NativeMethods.RemoveDirectory(path.LongFullPath))
				throw LongPathCommon.GetExceptionFromLastWin32Error();
		}

		/// <summary>
		/// 	Deletes files from the given path.
		/// </summary>
		/// <param name = "path">The path to delete files from.</param>
		/// <param name = "recursively">Whether to recurse through the directory and find all child directories and files
		/// 	and delete those</param>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> could not be found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> refers to a directory that is read-only.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> refers to a directory that is not empty.
		/// 	<para>
		/// 		-or-    
		/// 	</para>
		/// 	<paramref name = "path" /> refers to a directory that is in use.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		public static void Delete(Path path, bool recursively)
		{
			if (recursively)
			{
				foreach (var dir in EnumerateDirectories(path))
					Delete(dir, true);

				foreach (var file in EnumerateFiles(path))
					LongPathFile.Delete(file);

				Delete(path);
			}
			else Delete(path);
		}

		/// <summary>
		/// 	Returns a value indicating whether the specified path refers to an existing directory.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "String" /> containing the path to check.
		/// </param>
		/// <returns>
		/// 	<see langword = "true" /> if <paramref name = "path" /> refers to an existing directory; 
		/// 	otherwise, <see langword = "false" />.
		/// </returns>
		/// <remarks>
		/// 	Note that this method will return false if any error occurs while trying to determine 
		/// 	if the specified directory exists. This includes situations that would normally result in 
		/// 	thrown exceptions including (but not limited to); passing in a directory name with invalid 
		/// 	or too many characters, an I/O error such as a failing or missing disk, or if the caller
		/// 	does not have Windows or Code Access Security (CAS) permissions to to read the directory.
		/// </remarks>
		public static bool Exists(Path path)
		{
			bool isDirectory;

			if (LongPathCommon.Exists(path, out isDirectory))
				return isDirectory;

			return false;
		}

		/// <summary>
		/// 	Returns a enumerable containing the directory names of the specified directory.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "String" /> containing the path of the directory to search.
		/// </param>
		/// <returns>
		/// 	A <see cref = "IEnumerable{T}" /> containing the directory names within <paramref name = "path" />.
		/// </returns>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> contains one or more directories that could not be
		/// 	found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		public static IEnumerable<Path> EnumerateDirectories(Path path)
		{
			return EnumerateDirectories(path, null);
		}

		/// <summary>
		/// 	Returns a enumerable containing the directory names of the specified directory that 
		/// 	match the specified search pattern.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "String" /> containing the path of the directory to search.
		/// </param>
		/// <param name = "searchPattern">
		/// 	A <see cref = "String" /> containing search pattern to match against the names of the 
		/// 	directories in <paramref name = "path" />, otherwise, <see langword = "null" /> or an empty 
		/// 	string ("") to use the default search pattern, "*".
		/// </param>
		/// <returns>
		/// 	A <see cref = "IEnumerable{T}" /> containing the directory names within <paramref name = "path" />
		/// 	that match <paramref name = "searchPattern" />.
		/// </returns>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> contains one or more directories that could not be
		/// 	found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		public static IEnumerable<Path> EnumerateDirectories(Path path, string searchPattern)
		{
			return EnumerateFileSystemEntries(path, searchPattern, includeDirectories: true, includeFiles: false);
		}

		/// <summary>
		/// 	Returns a enumerable containing the file names of the specified directory.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "String" /> containing the path of the directory to search.
		/// </param>
		/// <returns>
		/// 	A <see cref = "IEnumerable{T}" /> containing the file names within <paramref name = "path" />.
		/// </returns>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> contains one or more directories that could not be
		/// 	found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		public static IEnumerable<Path> EnumerateFiles(Path path)
		{
			return EnumerateFiles(path, null);
		}

		/// <summary>
		/// 	Returns a enumerable containing the file names of the specified directory that 
		/// 	match the specified search pattern.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "String" /> containing the path of the directory to search.
		/// </param>
		/// <param name = "searchPattern">
		/// 	A <see cref = "String" /> containing search pattern to match against the names of the 
		/// 	files in <paramref name = "path" />, otherwise, <see langword = "null" /> or an empty 
		/// 	string ("") to use the default search pattern, "*".
		/// </param>
		/// <returns>
		/// 	A <see cref = "IEnumerable{T}" /> containing the file names within <paramref name = "path" />
		/// 	that match <paramref name = "searchPattern" />.
		/// </returns>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> contains one or more directories that could not be
		/// 	found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		public static IEnumerable<Path> EnumerateFiles(Path path, string searchPattern)
		{
			return EnumerateFileSystemEntries(path, searchPattern, includeDirectories: false, includeFiles: true);
		}

		/// <summary>
		/// 	Returns a enumerable containing the file and directory names of the specified directory.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "String" /> containing the path of the directory to search.
		/// </param>
		/// <returns>
		/// 	A <see cref = "IEnumerable{T}" /> containing the file and directory names within 
		/// 	<paramref name = "path" />.
		/// </returns>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> contains one or more directories that could not be
		/// 	found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		public static IEnumerable<Path> EnumerateFileSystemEntries(Path path)
		{
			return EnumerateFileSystemEntries(path, null);
		}

		/// <summary>
		/// 	Returns a enumerable containing the file and directory names of the specified directory 
		/// 	that match the specified search pattern.
		/// </summary>
		/// <param name = "path">
		/// 	A <see cref = "String" /> containing the path of the directory to search.
		/// </param>
		/// <param name = "searchPattern">
		/// 	A <see cref = "String" /> containing search pattern to match against the names of the 
		/// 	files and directories in <paramref name = "path" />, otherwise, <see langword = "null" /> 
		/// 	or an empty string ("") to use the default search pattern, "*".
		/// </param>
		/// <returns>
		/// 	A <see cref = "IEnumerable{T}" /> containing the file and directory names within 
		/// 	<paramref name = "path" />that match <paramref name = "searchPattern" />.
		/// </returns>
		/// <exception cref = "ArgumentNullException">
		/// 	<paramref name = "path" /> is <see langword = "null" />.
		/// </exception>
		/// <exception cref = "ArgumentException">
		/// 	<paramref name = "path" /> is an empty string (""), contains only white 
		/// 	space, or contains one or more invalid characters as defined in 
		/// 	<see cref = "System.IO.Path.GetInvalidPathChars()" />.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> contains one or more components that exceed
		/// 	the drive-defined maximum length. For example, on Windows-based 
		/// 	platforms, components must not exceed 255 characters.
		/// </exception>
		/// <exception cref = "PathTooLongException">
		/// 	<paramref name = "path" /> exceeds the system-defined maximum length. 
		/// 	For example, on Windows-based platforms, paths must not exceed 
		/// 	32,000 characters.
		/// </exception>
		/// <exception cref = "DirectoryNotFoundException">
		/// 	<paramref name = "path" /> contains one or more directories that could not be
		/// 	found.
		/// </exception>
		/// <exception cref = "UnauthorizedAccessException">
		/// 	The caller does not have the required access permissions.
		/// </exception>
		/// <exception cref = "IOException">
		/// 	<paramref name = "path" /> is a file.
		/// 	<para>
		/// 		-or-
		/// 	</para>
		/// 	<paramref name = "path" /> specifies a device that is not ready.
		/// </exception>
		public static IEnumerable<Path> EnumerateFileSystemEntries(Path path, string searchPattern)
		{
			return EnumerateFileSystemEntries(path, searchPattern, includeDirectories: true, includeFiles: true);
		}

		private static IEnumerable<Path> EnumerateFileSystemEntries(Path path, string searchPattern,
		                                                              bool includeDirectories, bool includeFiles)
		{
			var normalizedSearchPattern = LongPathCommon.NormalizeSearchPattern(searchPattern);
			var normalizedPath = LongPathCommon.NormalizeLongPath(path.FullPath);

			// First check whether the specified path refers to a directory and exists
			FileAttributes attributes;
			var errorCode = LongPathCommon.TryGetDirectoryAttributes(normalizedPath, out attributes);
			if (errorCode != 0)
				throw LongPathCommon.GetExceptionFromWin32Error(errorCode);

			return EnumerateFileSystemIterator(normalizedPath, normalizedSearchPattern, 
				includeDirectories,
				includeFiles);
		}

		private static IEnumerable<Path> EnumerateFileSystemIterator(string normalizedPath, string normalizedSearchPattern,
		                                                               bool includeDirectories, bool includeFiles)
		{
			// NOTE: Any exceptions thrown from this method are thrown on a call to IEnumerator<string>.MoveNext()

			var path = LongPathCommon.RemoveLongPathPrefix(normalizedPath);

			WIN32_FIND_DATA findData;
			using (var handle = BeginFind(System.IO.Path.Combine(normalizedPath, normalizedSearchPattern), out findData))
			{
				if (handle == null)
					yield break;

				do
				{
					var currentFileName = findData.cFileName;

					if (IsDirectory(findData.dwFileAttributes))
					{
						if (includeDirectories && !IsCurrentOrParentDirectory(currentFileName))
							yield return path.ToPath().Combine(currentFileName);
					}
					else
					{
						if (includeFiles)
							yield return path.ToPath().Combine(currentFileName);
					}
				} while (NativeMethods.FindNextFile(handle, out findData));

				var errorCode = Marshal.GetLastWin32Error();
				if (errorCode != NativeMethods.ERROR_NO_MORE_FILES)
					throw LongPathCommon.GetExceptionFromWin32Error(errorCode);
			}
		}

		private static SafeFindHandle BeginFind(string normalizedPathWithSearchPattern,
		                                        out WIN32_FIND_DATA findData)
		{
			var handle = NativeMethods.FindFirstFile(normalizedPathWithSearchPattern, out findData);
			if (handle.IsInvalid)
			{
				var errorCode = Marshal.GetLastWin32Error();
				if (errorCode != NativeMethods.ERROR_FILE_NOT_FOUND)
					throw LongPathCommon.GetExceptionFromWin32Error(errorCode);

				return null;
			}

			return handle;
		}

		public static void Move(Path sourcePath, Path targetPath)
		{
			if (!NativeMethods.MoveFile(sourcePath.FullPath, targetPath.FullPath))
				throw LongPathCommon.GetExceptionFromLastWin32Error();
		}

		internal static bool IsDirectory(FileAttributes attributes)
		{
			return (attributes & FileAttributes.Directory) == FileAttributes.Directory;
		}

		private static bool IsCurrentOrParentDirectory(string directoryName)
		{
			return directoryName.Equals(".", StringComparison.OrdinalIgnoreCase) ||
			       directoryName.Equals("..", StringComparison.OrdinalIgnoreCase);
		}
	}
}