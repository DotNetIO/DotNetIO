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

namespace DotNetIO
{
	/// <summary>
	/// 	Specifies under what scope the search/lookup should happen.
	/// </summary>
	public enum SearchScope
	{
		/// <summary>
		/// 	Only search the current folder.
		/// </summary>
		CurrentOnly,

		/// <summary>
		/// 	Search both the folder and all other folders.
		/// </summary>
		SubFolders
	}
}