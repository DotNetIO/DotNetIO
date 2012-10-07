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

namespace DotNetIO.FileSystems
{
	public class FileSystemNotificationIdentifier : IEquatable<FileSystemNotificationIdentifier>
	{
		public FileSystemNotificationIdentifier(Path path, WatcherChangeTypes changeTypes, string filter,
		                                        bool includeSubDirectories)
		{
			IncludeSubDirectories = includeSubDirectories;
			Path = path;
			Filter = filter;
			ChangeTypes = changeTypes;
		}

		public bool IncludeSubDirectories { get; private set; }

		public bool Equals(FileSystemNotificationIdentifier other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.IncludeSubDirectories.Equals(IncludeSubDirectories) && Equals(other.Path, Path) &&
			       Equals(other.Filter, Filter) && Equals(other.ChangeTypes, ChangeTypes);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (FileSystemNotificationIdentifier)) return false;
			return Equals((FileSystemNotificationIdentifier) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = IncludeSubDirectories.GetHashCode();
				result = (result*397) ^ Path.GetHashCode();
				result = (result*397) ^ Filter.GetHashCode();
				result = (result*397) ^ ChangeTypes.GetHashCode();
				return result;
			}
		}

		public static bool operator ==(FileSystemNotificationIdentifier left, FileSystemNotificationIdentifier right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(FileSystemNotificationIdentifier left, FileSystemNotificationIdentifier right)
		{
			return !Equals(left, right);
		}

		public Path Path { get; private set; }
		public string Filter { get; private set; }
		public WatcherChangeTypes ChangeTypes { get; private set; }
	}
}