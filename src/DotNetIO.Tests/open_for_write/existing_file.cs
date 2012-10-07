﻿#region license

// Copyright 2012 Henrik Feldt - https://github.com/DotNetIO
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.IO;
using DotNetIO.Tests.TestClasses;
using NUnit.Framework;
using SharpTestsEx;

namespace DotNetIO.Tests.open_for_write
{
	[TestFixture(typeof (TestInMemoryFileSystem))]
	[TestFixture(typeof (TestLocalFileSystem))]
	public class existing_file<T> : files<T> where T : FileSystem, new()
	{
		private File file;

		public existing_file()
		{
			given_temp_dir();
		}

		[Test]
		public void file_is_appended()
		{
			file = write_to_file();

			file.Write(1, FileMode.Append);

			file.ShouldBe(0, 1);
		}

		[TestCase(FileMode.Truncate)]
		[TestCase(FileMode.Create)]
		public void file_is_truncated_for(FileMode fileMode)
		{
			file = write_to_file();

			file.Write(1, fileMode);

			file.ShouldBe(1);
		}

		[Test]
		public void file_is_edited()
		{
			file = write_to_file(new byte[] {0, 1});
			file.Write(2, FileMode.Open);
			file.ShouldBe(2, 1);
		}

		[Test]
		public void file_cannot_be_created_new()
		{
			file = write_to_file();
			this.Executing(() => file.Write(1, FileMode.CreateNew)).Should().Throw<IOException>();
		}
	}
}