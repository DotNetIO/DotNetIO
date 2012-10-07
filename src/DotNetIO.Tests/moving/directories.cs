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

using DotNetIO.Tests.TestClasses;
using NUnit.Framework;
using SharpTestsEx;

namespace DotNetIO.Tests.moving
{
	[TestFixture(typeof (TestInMemoryFileSystem))]
	[TestFixture(typeof (TestLocalFileSystem))]
	internal class directories<T> 
		: file_system_ctxt<T> 
		where T : FileSystem, new()
	{
		private readonly Directory _sourceDir;
		private readonly Directory _destDir;
		private readonly string _sourcePath;
		private string _destPath;

		public directories()
		{
			var tempDir = given_temp_dir();
			_sourceDir = tempDir.GetDirectory("temp");
			_sourcePath = _sourceDir.Path.FullPath;
			_sourceDir.GetFile("test.txt").MustExist();

			_destDir = tempDir.GetDirectory("temp2");
			_destPath = _destDir.Path.FullPath;
			_sourceDir.MoveTo(_destDir);
		}

		[Test]
		public void original_doesnt_exist()
		{
			_sourceDir.Exists().Should().Be.False();
		}

		[Test]
		public void original_stays_at_original_path()
		{
			_sourceDir.Path.FullPath.ShouldBe(_sourcePath);
		}

		[Test]
		public void destination_exists()
		{
			_destDir.Exists().Should().Be.True();
		}

		[Test]
		public void destination_has_file()
		{
			_destDir.GetFile("test.txt").MustExist();
		}
	}
}