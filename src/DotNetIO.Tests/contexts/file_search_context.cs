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

using System.Collections.Generic;
using DotNetIO.FileSystems.InMemory;

namespace DotNetIO.Tests
{
	public abstract class file_search_context
	{
		protected InMemoryFileSystem FileSystem;
		protected IEnumerable<File> Files;
		protected IEnumerable<Directory> Directories;
		
		public file_search_context()
		{
			FileSystem = new InMemoryFileSystem();
		}

		protected void given_file(string filePath)
		{
			FileSystem.GetFile(filePath).MustExist();
		}

		protected void when_searching_for_files(string searchSpec)
		{
			Files = FileSystem.Files(searchSpec);
		}

		protected void when_searching_for_directories(string searchSpec)
		{
			Directories = FileSystem.Directories(searchSpec);
		}

		protected void given_directory(string directory)
		{
			FileSystem.GetDirectory(directory).MustExist();
		}

		protected void given_currentDirectory(string currentDirectory)
		{
			FileSystem.CurrentDirectory = new Path(currentDirectory);
		}

		protected void given_currentDirectory(Path currentDirectory)
		{
			FileSystem.CurrentDirectory = currentDirectory;
		}
	}
}