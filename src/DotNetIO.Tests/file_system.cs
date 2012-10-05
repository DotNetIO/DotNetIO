// Copyright 2012 Henrik Feldt - https://github.com/DotNetIO
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
using System.IO;
using System.Linq;
using System.Threading;
using DotNetIO.Tests.TestClasses;
using NUnit.Framework;
using SharpTestsEx;

namespace DotNetIO.Tests
{
	[TestFixture(typeof (TestInMemoryFileSystem))]
	[TestFixture(typeof (TestLocalFileSystem))]
	public class file_system<T> 
		: file_system_ctxt<T> where T : FileSystem, new()
	{
		[Test]
		public void receives_creation_notification()
		{
			var triggered = new ManualResetEventSlim(false);

			using (var tempDir = FileSystem.CreateTempDirectory())
			{
				string filePath = null;
				using (tempDir.FileChanges(created: f =>
					{
						filePath = f.Path.FullPath;
						triggered.Set();
					}))
				{
					tempDir.GetFile("receives_creation_notification.txt").MustExist();
					triggered.Wait();
					filePath.ToPath().ShouldBe(
						tempDir.Path.Combine("receives_creation_notification.txt"));
				}
			}
		}

		[Test]
		public void temp_file_exists_after_creation_and_is_deleted_when_used()
		{
			string fullPath;

			using (var tempFile = FileSystem.CreateTempFile())
			{
				tempFile.Exists().Should().Be.True();

				fullPath = tempFile.Path.FullPath;

				var tempFile2 = FileSystem.GetFile(fullPath);

				tempFile2.Exists().Should().Be.True();
				tempFile2.ShouldBe(tempFile);
			}

			FileSystem.GetFile(fullPath).Exists().Should().Be.False();
		}

		[Test]
		public void temp_directory_is_rooted_correctly()
		{
			using (var tempDirectory = FileSystem.CreateTempDirectory())
			{
				tempDirectory.Parent.ShouldBe(FileSystem.GetTempDirectory());
			}
		}

		[Test]
		public void directories_always_created()
		{
			using (var tmpDir = FileSystem.CreateTempDirectory())
			{
				var directory = FileSystem.GetDirectory(tmpDir.Path.Combine("test.html"));
				directory.Exists().Should().Be.False();
			}
		}

		[Test]
		public void created_directory_exists()
		{
			using (var tmpDir = FileSystem.CreateTempDirectory())
			{
				var directory = FileSystem.CreateDirectory(tmpDir.Path.Combine("temp.html"));
				directory.Exists().Should().Be.True();
			}
		}

		[Test]
		public void can_get_subdirectory_of_non_existant_directory()
		{
			var rootPath = new Path(Environment.GetEnvironmentVariable("TEMP"));

			var directory = FileSystem.GetDirectory(rootPath.Combine("should-not-be-created")).GetDirectory(@"sub1\sub2");
			try
			{
				directory.Path.FullPath.Should().Be.EqualTo(rootPath.FullPath + @"\should-not-be-created\sub1\sub2");
				directory.Exists().Should().Be.False();
			}
			finally
			{
				directory.Delete();
			}
		}

		[Test]
		public void can_get_file_with_directory_path()
		{
			FileSystem.GetDirectory(@"C:\can_get_file_with_directory_path")
				.GetFile(@"folder\file.txt")
				.Path.FullPath.ShouldBe(@"C:\can_get_file_with_directory_path\folder\file.txt");
		}

		[Test]
		public void directory_is_resolved_relative_to_current_directory()
		{
			var dir = FileSystem.GetDirectory("a child of the relative");

			dir.Path.FullPath
				.Should().Be.EqualTo(System.IO.Path.Combine(CurrentDirectory, "a child of the relative"));

			dir.Exists().Should().Be.False();
		}

		[Test]
		public void files_are_resolved_relative_to_current_directory()
		{
			FileSystem.GetFile("rohan.html").Path
				.ShouldBe(new Path(CurrentDirectory).Combine("rohan.html"));
		}

		[Test]
		public void recursive_search_for_directories_returns_correct_directory()
		{
			using (var tempDirectory = FileSystem.CreateTempDirectory())
			{
				var root = tempDirectory.GetDirectory("rootdir");
				root.GetDirectory("subdir").MustExist();
				root.GetDirectory("carl").MustExist();

				tempDirectory.Directories("s*", SearchScope.SubFolders).ShouldHaveCountOf(1)
					.First().Name.ShouldBe("subdir");
			}
		}

		[Test]
		public void recursive_search_for_files_returns_correct_files()
		{
			using (var tempDirectory = FileSystem.CreateTempDirectory())
			{
				var root = tempDirectory.GetDirectory("rootdir");
				root.GetFile("subfile").MustExist();
				root.GetFile("carl").MustExist();

				tempDirectory.Files("s*", SearchScope.SubFolders).ShouldHaveCountOf(1)
					.First().Name.ShouldBe("subfile");
			}
		}

		[Test]
		public void two_directories_are_equal()
		{
			FileSystem.GetDirectory("dir")
				.ShouldBe(FileSystem.GetDirectory("dir"));
		}

		[Test]
		public void two_files_are_equal()
		{
			FileSystem.GetDirectory("abc.html")
				.ShouldBe(FileSystem.GetDirectory("abc.html"));
		}

		[Test]
		public void non_existant_file_opened_for_write_is_created_automatically()
		{
			var file = FileSystem.GetFile(new Path(Environment.GetEnvironmentVariable("TEMP")).Combine("non-existent-text-file.txt"));
			try
			{
				file.Exists().Should().Be.False();
				file.OpenWrite().Close();
				file.Exists().Should().Be.True();
			}
			finally
			{
				file.Delete();
			}
		}

		[Test]
		public void file_paths_are_normalized()
		{
			var file = FileSystem.GetFile(@"C:\\folder\\file");
			file.Path.ShouldBe(new Path(@"C:\folder\file"));
		}

		[Test]
		public void trailing_slash_is_not_significant()
		{
			var first = FileSystem.GetDirectory(@"C:\abc");
			var second = FileSystem.GetDirectory(@"C:\abc\");

			first.ShouldBe(second);
		}

		[Test]
		public void standard_directory_is_not_a_link()
		{
			using (var dir = FileSystem.CreateTempDirectory())
				dir.IsHardLink.Should().Be.False();
		}

		[Test]
		public void can_create_link()
		{
			var tempLinkFolder = Guid.NewGuid().ToString();
			using (var concreteDir = FileSystem.CreateTempDirectory())
			{
				concreteDir.Exists().Should().Be.True();

				var file = concreteDir.GetFile("test.txt");
				file.OpenWrite().Close();
				file.Exists().Should().Be.True();

				var linkedPath = Path.GetTempPath().Combine(tempLinkFolder);
				
				var linkedDirectory = concreteDir.LinkTo(linkedPath);
				linkedDirectory.IsHardLink.Should().Be.True();

				linkedDirectory.Delete();
				linkedDirectory.Exists().Should().Be.False();
				concreteDir.Exists().Should().Be.True();
				concreteDir.GetFile("test.txt").Exists().Should().Be.True();
			}
		}

		[Test]
		public void different_directories_are_not_equal()
		{
			FileSystem.GetDirectory(@"C:\tmp\1\").ShouldNotBe(FileSystem.GetDirectory(@"C:\tmp\2\"));
		}

		[Test]
		public void link_has_reference_to_target()
		{
			using (var tempDir = FileSystem.CreateTempDirectory())
			{
				var concreteDir = tempDir.GetDirectory("temp").MustExist();
				var linkedDir = concreteDir.LinkTo(tempDir.Path.Combine("link"));
				linkedDir.Target.ShouldBe(concreteDir);
				linkedDir.Delete();
			}
		}

		[Test]
		public void delete_parent_deletes_child_folder()
		{
			var dir1 = FileSystem.GetTempDirectory().GetDirectory("test");
			var child = dir1.GetDirectory("test").MustExist();
			dir1.Delete();
			dir1.Exists().Should().Be.False();
			child.Exists().Should().Be.False();
		}

		[Test]
		public void deleted_child_directory_doesnt_show_up_in_child_directories()
		{
			var dir1 = FileSystem.GetTempDirectory().GetDirectory("test");
			var child = dir1.GetDirectory("test").MustExist();
			child.Delete();

			dir1.Directories().ShouldHaveCountOf(0);
		}

		[Test]
		public void deleted_hardlink_doesnt_delete_subfolder()
		{
			var dir1 = FileSystem.GetTempDirectory().GetDirectory("test").MustExist();
			var child = dir1.GetDirectory("test").MustExist();

			var link = dir1.LinkTo(FileSystem.GetTempDirectory().Path.Combine("testLink"));
			link.Delete();
			link.Exists().Should().Be.False();
			dir1.Exists().Should().Be.True();
			child.Exists().Should().Be.True();
		}

		[Test]
		public void moving_file_moves_a_file()
		{
			var temporaryFile = FileSystem.CreateTempFile();
			var newFileName = FileSystem.GetTempDirectory().GetFile(Guid.NewGuid().ToString());
			temporaryFile.MoveTo(newFileName);

			FileSystem.GetFile(temporaryFile.Path.FullPath).Exists().Should().Be.False();
			newFileName.Exists().Should().Be.True();
		}

		[Test]
		public void moving_directory_moves_directories()
		{
			var tempDirectory = FileSystem.CreateTempDirectory();
			var oldDirName = tempDirectory.Path.FullPath;

			var newDir = FileSystem.GetDirectory(FileSystem.GetTempDirectory().Path.Combine(Guid.NewGuid() + "/").FullPath);
			tempDirectory.MoveTo(newDir);
			newDir.Exists().Should().Be.True();
			FileSystem.GetDirectory(oldDirName).Exists().Should().Be.False();
		}

		[TestCase(FileAccess.Read, FileShare.None, FileAccess.Read)]
		[TestCase(FileAccess.Read, FileShare.Write, FileAccess.Read)]
		[TestCase(FileAccess.Write, FileShare.None, FileAccess.Write)]
		[TestCase(FileAccess.Write, FileShare.Read, FileAccess.Write)]
		[TestCase(FileAccess.Read, FileShare.Read, FileAccess.Write)]
		public void failing_locks(FileAccess firstAccess, FileShare @lock, FileAccess nextAccess)
		{
			using (var temporaryFile = FileSystem.CreateTempFile())
			using (var stream1 = temporaryFile.Open(FileMode.OpenOrCreate, firstAccess, @lock))
				this.Executing(() => temporaryFile.Open(FileMode.OpenOrCreate, nextAccess, FileShare.None))
					.Should().Throw<IOException>();
		}

		[Test]
		public void append_only_in_write_mode()
		{
			using (var tempFile = FileSystem.CreateTempFile())
				this.Executing(() => tempFile.Open(FileMode.Append, FileAccess.Read, FileShare.None))
					.Should().Throw<ArgumentException>();
		}

		[Test]
		public void open_write_with_truncate_creates_a_new_stream()
		{
			using (var temporaryFile = given_temp_file("Hello"))
			using (var writer = temporaryFile.Open(FileMode.Truncate, FileAccess.Write, FileShare.None))
				writer.Length.ShouldBe(0);
		}

		[Test]
		public void open_write_with_create_creates_a_new_stream()
		{
			using (var temporaryFile = given_temp_file("Hello"))
			using (var writer = temporaryFile.Open(FileMode.Create, FileAccess.Write, FileShare.None))
				writer.Length.ShouldBe(0);
		}

		[Test]
		public void copy_files_copies_content()
		{
			using (var tempDir = FileSystem.CreateTempDirectory())
			{
				var tempFile = tempDir.GetFile("test.txt");
				WriteString(tempFile, "test data");
				var copyFile = tempDir.GetFile("test2.txt");
				tempFile.CopyTo(copyFile);

				ReadString(tempFile).ShouldBe("test data");
				ReadString(copyFile).ShouldBe("test data");
			}
		}

		[Test]
		public void can_read_data_from_two_readers()
		{
			using (var file = given_temp_file("content"))
			{
				using (var first = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
				using (var second = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					first.ReadByte().ShouldBe('c');
					second.ReadByte().ShouldBe('c');
				}
			}
		}

		[Test]
		public void duplicates_directory_content_when_copyto_called()
		{
			using (var source = FileSystem.CreateTempDirectory())
			using (var destination = FileSystem.CreateTempDirectory())
			{
				WriteString(source.GetFile("test.txt"), "test data");
				WriteString(source.GetDirectory("testDir").GetFile("test2.txt"), "test data2");

				source.CopyTo(destination);

				destination.GetFile("test.txt")
					.Check(x => x.Exists().Should().Be.True())
					.Check(x => ReadString(x).ShouldBe("test data"));

				var dest = destination.GetDirectory("testDir");

				dest.Exists().Should().Be.True();
				var destFile = dest.GetFile("test2.txt");
				destFile.Exists().Should().Be.True();
				ReadString(destFile).ShouldBe("test data2");
			}
		}
	}
}