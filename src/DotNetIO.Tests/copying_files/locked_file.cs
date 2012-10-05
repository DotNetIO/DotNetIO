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
using System.IO;
using DotNetIO.Tests.TestClasses;
using NUnit.Framework;
using SharpTestsEx;

namespace DotNetIO.Tests.copying_files
{
	[TestFixture(typeof (TestInMemoryFileSystem))]
	[TestFixture(typeof (TestLocalFileSystem))]
	public class locked_file<T> : files<T> where T : FileSystem, new()
	{
		[TestCase(FileShare.None)]
		[TestCase(FileShare.Write)]
		[TestCase(FileShare.Delete)]
		public void cannot_be_copied(FileShare fileShare)
		{
			var tempFile = FileSystem.CreateTempFile();

			using (tempFile.Open(FileMode.Append, FileAccess.Write, fileShare))
			{
				((Action) (
					          () => tempFile.CopyTo(tempFile.Parent.GetFile(Path.GetRandomFileName()))))
					.Should().Throw<IOException>();
			}
		}
	}
}