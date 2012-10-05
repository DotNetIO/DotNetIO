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

using System.IO;
using DotNetIO.Tests.TestClasses;
using NUnit.Framework;

namespace DotNetIO.Tests.writing_content
{
	[TestFixture(typeof (TestInMemoryFileSystem))]
	[TestFixture(typeof (TestLocalFileSystem))]
	public class using_opencreate<T> 
		: file_system_ctxt<T> 
		where T : FileSystem, new()
	{
		public using_opencreate()
		{
			file = given_temp_file();
			given_content(1);
			given_content(42);
		}

		void given_content(byte data)
		{
			file.Write(data, FileMode.OpenOrCreate);
		}

		[Test]
		public void file_length_is_updated()
		{
			file.GetSize().ShouldBe(1);
		}

		[Test]
		public void file_content_is_written()
		{
			file.ShouldBe(42);
		}

		readonly TemporaryFile file;
	}
}