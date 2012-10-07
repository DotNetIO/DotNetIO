﻿using System.IO;
using SharpTestsEx;

namespace DotNetIO.Tests
{
	public abstract class files<T> : file_system_ctxt<T> where T : FileSystem, new()
	{
		protected File write_to_file(byte[] value = null, FileMode mode = FileMode.Create, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None)
		{
			value = value ?? new[] { (byte)0 };

			var TempFile = TempDir.GetFile(Path.GetRandomFileName());

			TempFile.Exists().Should().Be.False();

			using (var stream = TempFile.Open(mode, access, share))
				stream.Write(value);

			return TempFile;
		}
	}
}
