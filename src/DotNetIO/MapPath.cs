#region license

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

using System.Diagnostics.Contracts;
using DotNetIO.Contracts;

namespace DotNetIO
{
	///<summary>
	///	Small interface for the map path functionality.
	///</summary>
	[ContractClass(typeof (MapPathContract))]
	public interface MapPath
	{
		///<summary>
		///	Gets the absolute path given a string formatted
		///	as a map path, for example:
		///	"~/plugins" or "plugins/integrated" or "C:\a\b\c.txt" or "\\?\C:\a\b"
		///	would all be valid map paths.
		///</summary>
		///<param name = "path"></param>
		///<returns></returns>
		string MapPath(string path);
	}
}