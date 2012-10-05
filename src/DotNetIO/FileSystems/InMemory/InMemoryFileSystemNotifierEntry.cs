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

using System;
using System.Collections.Generic;
using System.Linq;
using DotNetIO.Internal;

namespace DotNetIO.FileSystems.InMemory
{
	public class InMemoryFileSystemNotifierEntry : FileSytemNotifierEntry
	{
		public FileSystemNotificationIdentifier Identifier { get; private set; }
		private readonly List<Action<File>> _handlers = new List<Action<File>>();

		public InMemoryFileSystemNotifierEntry(FileSystemNotificationIdentifier notificationIdentifier)
		{
			Identifier = notificationIdentifier;
		}

		public IDisposable AddNotifiers(params Action<File>[] entries)
		{
			lock (_handlers)
			{
				_handlers.AddRange(entries.Where(x => x != null));
			}
			return new ExecuteOnDispose(() =>
				{
					lock (_handlers)
					{
						foreach (var value in entries)
							_handlers.Remove(value);
					}
				});
		}

		public void ExecuteHandlers(File file)
		{
			IEnumerable<Action<File>> handlers;
			lock (_handlers)
			{
				handlers = _handlers.Copy();
			}
			foreach (var handler in handlers)
				handler(file);
		}
	}
}