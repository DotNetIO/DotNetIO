using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace DotNetIO.Tests
{
	public abstract class file_system_ctxt<T> where T : FileSystem, new()
	{
		public file_system_ctxt()
		{
			CurrentDirectory = Environment.CurrentDirectory;
			FileSystem = new T();
		}

		protected FileSystem FileSystem { get; private set; }
		protected string CurrentDirectory { get; set; }

		protected TemporaryDirectory given_temp_dir()
		{
			return TempDir = FileSystem.CreateTempDirectory();
		}

		protected TemporaryDirectory TempDir { get; set; }

		protected TemporaryFile given_temp_file(string content = null)
		{
			var temporaryFile = FileSystem.CreateTempFile();

			if (content != null)
				WriteString(temporaryFile, content);

			return temporaryFile;
		}

		protected string ReadString(File file)
		{
			using (var reader = file.OpenRead())
			{
				var stream = new MemoryStream();
				reader.CopyTo(stream);

				return Encoding.UTF8.GetString(stream.ToArray());
			}
		}

		protected void WriteString(File temporaryFile, string content)
		{
			using (var writer = temporaryFile.OpenWrite())
				writer.Write(Encoding.UTF8.GetBytes(content));
		}

		[TestFixtureTearDown]
		public void delete_temp_dir()
		{
			if (TempDir != null)
				TempDir.Dispose();
		}
	}
}