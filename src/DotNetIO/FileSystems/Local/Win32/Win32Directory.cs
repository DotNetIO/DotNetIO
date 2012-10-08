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

using System.Diagnostics.Contracts;
using DotNetIO.FileSystems.Local.Win32.Interop;
using DotNetIO.Internal;

namespace DotNetIO.FileSystems.Local.Win32
{
	public class Win32Directory : LocalDirectory
	{
		public Win32Directory(Path directoryPath) : base(directoryPath)
		{
			Contract.Requires(directoryPath != null);
		}

		public override bool Exists()
		{
			WIN32_FIND_DATA findData;
			using (var handle = NativeMethods.FindFirstFile(Path.LongFullPath, out findData))
				return !handle.IsInvalid;
		}

		public override void Delete()
		{
			if (IsHardLink)
				JunctionPoint.Delete(Path);
			else
				LongPathDirectory.Delete(Path, true);
		}

		public override bool IsHardLink
		{
			get { return JunctionPoint.Exists(Path); }
		}

		public override Directory LinkTo(Path path)
		{
			if (!Exists())
				throw new System.IO.IOException("Source path does not exist or is not a directory.");

			JunctionPoint.Create(GetDirectory(path.FullPath), Path.FullPath, true);
			return new Win32Directory(path);
		}

		protected override LocalDirectory CreateDirectory(Path path)
		{
			return new Win32Directory(path);
		}

		public override Directory GetDirectory(string directoryName)
		{
			return new Win32Directory(Path.Combine(directoryName));
		}

		public override File GetFile(string fileName)
		{
			return new Win32File(Path.Combine(fileName), CreateDirectory);
		}

		public override Directory Target
		{
			get { return IsHardLink ? new Win32Directory(JunctionPoint.GetTarget(Path)) : this; }
		}
	}
}