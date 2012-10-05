#region license

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

#endregion

namespace DotNetIO.FileSystems
{
	public abstract class AbstractFileSystem : FileSystem
	{
		public abstract TemporaryDirectory CreateTempDirectory();
		
		public abstract Directory CreateDirectory(string path);
		
		public abstract Directory CreateDirectory(Path path);
		
		public abstract TemporaryFile CreateTempFile();

		public abstract FileSystemNotifier Notifier { get; }

		public abstract Directory GetDirectory(string directoryPath);

		public abstract Directory GetDirectory(Path directoryPath);
		
		public abstract File GetFile(string filePath);

		public abstract File GetFile(Path filePath);

		public abstract Path GetPath(string path);
		
		public abstract Directory GetTempDirectory();
		
		public abstract Directory GetCurrentDirectory();
	}
}