using System;
using DotNetIO.FileSystems.InMemory;

namespace DotNetIO.Tests.TestClasses
{
	public class TestInMemoryFileSystem : InMemoryFileSystem
	{
		public TestInMemoryFileSystem()
		{
			CurrentDirectory = new Path(Environment.CurrentDirectory);
		}
	}
}