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
using DotNetIO.FileSystems.Local;

namespace DotNetIO.Internal
{
	public static class DebuggerHelper
	{
		public static void Print(this Path path)
		{
			var directory = LocalFileSystem.Instance.GetDirectory(path);

			foreach (var dir in directory.Directories())
				Console.WriteLine("D: {0}", dir);

			foreach (var file in directory.Files())
				Console.WriteLine("F: {0}", file);
		}
	}
}