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
using System.Linq;
using System.Text;
using DotNetIO.FileSystems.InMemory;
using NUnit.Framework;
using SharpTestsEx;

namespace DotNetIO.Tests
{
	public class in_mem_specification 
		: in_memory_file_system
	{
		[Test]
		public void folders_and_paths_are_case_insensitive_by_default()
		{
			given_directory(@"C:\test");
			given_directory(@"C:\TEST\test2");

			FileSystem.GetDirectory(@"C:\TEST")
				.Should().Be.SameInstanceAs(FileSystem.GetDirectory(@"C:\test"));
		}

		[Test]
		public void tostring_on_file_returns_input_path()
		{
			var path = @"C:\tmp\README.md";
			var file = FileSystem.GetFile(path);
			file.ToString().Should().Be(path);
		}

		[Test]
		public void can_add_folders_to_fs()
		{
			var fs = new InMemoryFileSystem().CreateChildDir(@"C:\added folder");
			fs.Directories.ShouldHaveCountOf(1);
		}

		[Test]
		public void can_add_sub_folders()
		{
			var fs = new InMemoryFileSystem().CreateChildDir(@"C:\rootdir\subdir");
			var rootdir = fs.GetDirectory(@"C:\rootdir");
			rootdir.Exists().Should().Be.True();

			var subdir = rootdir.GetDirectory("subdir");
			subdir.Path.FullPath.ShouldBe(@"C:\rootdir\subdir");
			subdir.Exists().Should().Be.True();
		}

		[Test]
		public void can_move_folder()
		{
			var fs = new InMemoryFileSystem();
			var source = fs.GetDirectory("C:\\source");
			source.GetFile("file-in-folder.txt").MustExist();
			var destination = fs.GetDirectory("C:\\destination");
			source.MoveTo(destination);

			source.GetFile("file-in-folder.txt").Exists().Should().Be.False();
			source.Exists().Should().Be.False();

			destination.Exists().Should().Be.True();
			destination.GetFile("file-in-folder.txt").Exists().Should().Be.True();
		}

		[Test]
		public void content_is_written_correctly()
		{
			var fs = new InMemoryFileSystem();

			using (var file = fs.CreateTempFile())
			{
				var content = string.Join(" ", Enumerable.Repeat("Test value", 5000).ToArray());
				using (var str = file.OpenWrite())
				using (var sw = new StreamWriter(str, Encoding.UTF8))
				{
					sw.Write(content);
				}
				using (var str = file.OpenRead())
				using (var sr = new StreamReader(str, Encoding.UTF8))
				{
					sr.ReadToEnd().ShouldBe(content);
				}
			}
		}
	}
}