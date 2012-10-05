using DotNetIO.FileSystems.InMemory;

namespace DotNetIO.Tests
{
	public abstract class in_memory_file_system
	{
		public in_memory_file_system()
		{
			FileSystem = new InMemoryFileSystem();
		}

		protected InMemoryFileSystem FileSystem { get; set; }

		protected void given_directory(string cTest)
		{
			FileSystem.GetDirectory(cTest).MustExist();
		}
	}
}