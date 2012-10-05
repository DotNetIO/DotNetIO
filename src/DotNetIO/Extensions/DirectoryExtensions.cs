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
using System.Collections.Generic;
using System.Linq;

namespace DotNetIO
{
	public static class DirectoryExtensions
	{
		const string Subfolder = "**";

		/// <summary>
		/// </summary>
		/// <param name="directory"> The directory to watch for changes </param>
		/// <param name="filter"> </param>
		/// <param name="includeSubdirectories"> </param>
		/// <param name="created"> </param>
		/// <param name="modified"> </param>
		/// <param name="deleted"> </param>
		/// <param name="renamed"> </param>
		/// <returns> </returns>
		public static IDisposable FileChanges(
			this Directory directory,
			string filter = "*", bool includeSubdirectories = false, Action<File> created = null,
			Action<File> modified = null, Action<File> deleted = null, Action<File> renamed = null)
		{
			return directory
				.FileSystem
				.Notifier
				.RegisterNotification(directory.Path,
				                      filter, includeSubdirectories, created, modified,
				                      deleted, renamed);
		}

		public static IEnumerable<Directory> Directories(this FileSystem fileSystem, string filter)
		{
			return fileSystem.GetCurrentDirectory().Directories(filter);
		}

		public static Directory FindDirectory(this Directory directory, string path)
		{
			var child = directory.GetDirectory(path);
			return child.Exists() ? child : null;
		}

		public static File FindFile(this Directory directory, string filename)
		{
			var child = directory.GetFile(filename);
			return child.Exists() ? child : null;
		}

		public static Directory GetOrCreateDirectory(this Directory directory, params string[] childDirectories)
		{
			return childDirectories.Aggregate(
				directory, 
				(current, childDirectoryName) => current.GetDirectory(childDirectoryName).MustExist());
		}

		public static IEnumerable<File> Files(this FileSystem fileSystem, string filter)
		{
			return fileSystem.GetCurrentDirectory().Files(filter);
		}

		public static IEnumerable<File> Files(this Directory directory, string filter)
		{
			var pathSegments = GetFilterPaths(filter).ToList();

			if (pathSegments.Count() == 1)
				return directory.Files(filter, SearchScope.CurrentOnly);

			return GetFileSpecCore(directory, pathSegments, 0).ToList();
		}

		static IEnumerable<File> GetFileSpecCore(Directory directory, IList<string> segments, int position)
		{
			var segment = segments[position];

			if (position == segments.Count - 1)
			{
				return directory.Files(segment);
			}

			if (segment == Subfolder)
			{
				var isNextToLastSegment = position + 1 == segments.Count - 1;

				return isNextToLastSegment
					       ? directory.Files(segments[position + 1], SearchScope.SubFolders)
					       : directory.Directories(segments[position + 1], SearchScope.SubFolders)
						         .SelectMany(x => GetFileSpecCore(x, segments, position + 2));
			}

			return directory.Directories(segment, SearchScope.CurrentOnly)
				.SelectMany(x => GetFileSpecCore(x, segments, position + 1));
		}

		static IEnumerable<Directory> GetDirectorySpecCore(Directory directory, IList<string> segments, int position)
		{
			var segment = segments[position];
			if (position == segments.Count - 1)
				return directory.Directories(segment);

			if (segment == Subfolder)
			{
				var isNextToLastSegment = position + 1 == segments.Count - 1;
				return isNextToLastSegment
					       ? directory.Directories(segments[position + 1], SearchScope.SubFolders)
					       : directory.Directories(segments[position + 1], SearchScope.SubFolders)
						         .SelectMany(x => GetDirectorySpecCore(x, segments, position + 2));
			}
			return directory.Directories(segment, SearchScope.CurrentOnly)
				.SelectMany(x => GetDirectorySpecCore(x, segments, position + 1));
		}

		static IEnumerable<string> GetFilterPaths(string filter)
		{
			var lastWasSubFolder = false;

			var path = new Path(filter);

			foreach (var segment in path.Segments)
			{
				if (segment == Subfolder)
					if (!lastWasSubFolder)
						lastWasSubFolder = true;
					else
						continue;
				else
					lastWasSubFolder = false;

				yield return segment;
			}
		}

		public static IEnumerable<Directory> Directories(this Directory directory, string filter)
		{
			var pathSegments = GetFilterPaths(filter).ToList();

			if (pathSegments.Count == 1)
				return directory.Directories(filter, SearchScope.CurrentOnly);

			return GetDirectorySpecCore(directory, pathSegments, 0);
		}

		public static IEnumerable<Directory> AncestorsAndSelf(this Directory directory)
		{
			do
			{
				yield return directory;
				directory = directory.Parent;
			} while (directory != null);
		}
	}
}