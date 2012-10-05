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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace DotNetIO
{
	///<summary>
	///	An implementation of the MapPath which seems to be working well with
	///	both testfixtures and online.
	///</summary>
	public class MapPathImpl : MapPath
	{
		readonly Func<string, string> _function;

		///<summary>
		///	Default c'tor.
		///</summary>
		public MapPathImpl()
		{
		}

		/// <summary>
		/// 	Function may be null. Use when this class delegates.
		/// </summary>
		/// <param name="function"> </param>
		public MapPathImpl(Func<string, string> function)
		{
			_function = function;
		}

		///<summary>
		///	Gets the absolute path given a string formatted
		///	as a map path, for example:
		///	"~/plugins" or "plugins/integrated" or "C:\a\b\c.txt" or "\\?\C:\a\b"
		///	would all be valid map paths.
		///</summary>
		///<param name="path"> </param>
		///<returns> </returns>
		[SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength",
			Justification = "I don't care that much about performance")]
		public string MapPath(string path)
		{
			if (Path.IsPathRooted(path))
				return Path.GetFullPath(path);

			if (_function != null)
			{
				var mapPath = _function(path);
				Contract.Assume(!string.IsNullOrEmpty(mapPath),
				                "This is a user-provided function, and code-contracts don't allow me to reason with the typed for Func in a Requires in c'tor.");
				return mapPath;
			}

			path = Path.NormDirSepChars(path);

			// TODO: consider Environment.CurrentDirectory

			if (string.IsNullOrEmpty(path))
				return AppDomain.CurrentDomain.BaseDirectory;

			if (path[0] == '~')
				path = path.Substring(1);

			if (path.Length == 0)
				return AppDomain.CurrentDomain.BaseDirectory;

			if (Path.DirectorySeparatorChar == path[0])
				path = path.Substring(1);

			return path == string.Empty
				       ? AppDomain.CurrentDomain.BaseDirectory
				       : Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory.Combine(path));
		}
	}
}