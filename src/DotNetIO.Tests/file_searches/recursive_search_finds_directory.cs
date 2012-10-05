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

using System.Linq;
using NUnit.Framework;

namespace DotNetIO.Tests.file_searches
{
	[TestFixture("c:\\path\\file.txt", "c:\\**\\file.txt")]
	[TestFixture("c:\\path\\file.txt", "c:\\path\\**\\file.txt")]
	[TestFixture("c:\\path\\file.txt", "c:\\path\\**\\**\\file.txt")]
	[TestFixture("c:\\path\\file.txt", "c:\\p*\\*.txt")]
	[TestFixture("c:\\path\\file.txt", "c:\\path\\*.txt")]
	[TestFixture("c:\\path\\file.txt", "**\\file.txt")]
	[TestFixture("c:\\path\\file.txt", "path\\**\\file.txt")]
	[TestFixture("c:\\path\\file.txt", "path\\**\\**\\file.txt")]
	[TestFixture("c:\\path\\file.txt", "p*\\*.txt")]
	[TestFixture("c:\\path\\file.txt", "path\\*.txt")]
	[TestFixture("c:\\path\\file.txt", "c:\\path\\file.txt")]
	[TestFixture("c:\\path\\file.txt", "c:\\path\\file.txt", "c:\\path\\")]
	public class recursive_file_search : file_search_context
	{
		readonly string existingFile;

		public recursive_file_search(string file, string searchSpec) : this(file, searchSpec, null)
		{
		}

		public recursive_file_search(string file, string searchSpec, string currentDirectory)
		{
			existingFile = file;

			if (currentDirectory != null)
				given_currentDirectory(currentDirectory);

			given_file(file);

			when_searching_for_files(searchSpec);
		}

		[Test]
		public void file_is_found()
		{
			Files.ShouldHaveCountOf(1).First().Path.FullPath.ShouldBe(existingFile);
		}
	}
}