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

using DotNetIO.Tests.TestClasses;
using NUnit.Framework;
using SharpTestsEx;

namespace DotNetIO.Tests.writing_content
{
	[TestFixture(typeof (TestInMemoryFileSystem))]
	[TestFixture(typeof (TestLocalFileSystem))]
	public class to_long_paths<T>
		: file_system_ctxt<T>
		where T : FileSystem, new()
	{
		Directory _dir;
		TemporaryDirectory _temporaryDirectory;

		public to_long_paths()
		{
			// establish
			_temporaryDirectory = FileSystem.CreateTempDirectory();
			_temporaryDirectory.Exists().Should().Be.True();
			_temporaryDirectory.Should().Not.Be.Null();

			// given
			_dir = _temporaryDirectory.GetDirectory(new string('a', 255));
			_dir.Should().Not.Be.Null();
			_dir.MustExist();
		}

		[Test]
		public void the_dir_name_should_equal()
		{
			_temporaryDirectory.Path.Combine(new string('a', 255))
				.FullPath
				.ShouldBe(_dir.Path.FullPath);
		}

		[Test]
		public void it_should_exist()
		{
			_dir.Exists().Should().Be.True();
		}

		[Test]
		public void can_create_file()
		{
			var file = _dir.GetFile("abc");
			file.Write(42);

			using (var sr = _dir.GetFile("abc").OpenRead())
				sr.ReadByte().Should().Be.EqualTo(42);
		}

		[TestFixtureTearDown]
		public void teardown()
		{
			_temporaryDirectory.Dispose();
		}
	}
}