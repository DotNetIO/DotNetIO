using DotNetIO.FileSystems;
using DotNetIO.FileSystems.Local;

namespace DotNetIO.Tests.TestClasses
{
	public class TestLocalFileSystem : FileSystem
	{
		public FileSystemNotifier Notifier
		{
			get { return _notifier; }
		}

		public Directory GetDirectory(string directoryPath)
		{
			return _local.GetDirectory(directoryPath);
		}

		public Directory GetDirectory(Path path)
		{
			return _local.GetDirectory(path);
		}

		public Path GetPath(string path)
		{
			return _local.GetPath(path);
		}

		public TemporaryDirectory CreateTempDirectory()
		{
			return _local.CreateTempDirectory();
		}

		public Directory CreateDirectory(string path)
		{
			return _local.CreateDirectory(path);
		}

		public Directory CreateDirectory(Path path)
		{
			return _local.CreateDirectory(path);
		}

		public File GetFile(string itemSpec)
		{
			return _local.GetFile(itemSpec);
		}

		public File GetFile(Path filePath)
		{
			return _local.GetFile(filePath);
		}

		public TemporaryFile CreateTempFile()
		{
			return _local.CreateTempFile();
		}

		public Directory GetTempDirectory()
		{
			return _local.GetTempDirectory();
		}

		public Directory GetCurrentDirectory()
		{
			return _local.GetCurrentDirectory();
		}

		readonly FileSystem _local = LocalFileSystem.Instance;
		readonly FileSystemNotifier _notifier = LocalFileSystemNotifier.Instance;
	}
}