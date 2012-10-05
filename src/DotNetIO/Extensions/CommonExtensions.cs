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

namespace DotNetIO
{
	/// <summary>
	/// 	Common extensions among <see cref="File" /> and <see cref="Directory" />.
	/// </summary>
	public static class CommonExtensions
	{
		public static T MustExist<T>(this T directory)
			where T : FileSystemItem<T>
		{
			if (!directory.Exists()) directory.Create();
			return directory;
		}
	}
}