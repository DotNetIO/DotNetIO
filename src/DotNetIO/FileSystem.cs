// Copyright 2012 Henrik Feldt

using System.Diagnostics.Contracts;
using DotNetIO.Contracts;
using DotNetIO.FileSystems;

namespace DotNetIO
{
	/// <summary>
	/// 	Interface denoting a file system. This is the core abstraction of DotNetIO; providing 
	/// 	file systems that work across platforms.
	/// </summary>
	[ContractClass(typeof (FileSystemContract))]
	public interface FileSystem
	{
		FileSystemNotifier Notifier { get; }

		/// <summary>
		/// 	Creates a new dictory pointer given a directory path. This path may be relative, absolute or UNC.
		/// </summary>
		/// <param name="directoryPath"> </param>
		/// <returns> </returns>
		Directory GetDirectory(string directoryPath);

		/// <summary>
		/// 	Gets the directory for the passed <see cref="Path"/>.
		/// </summary>
		/// <param name="directoryPath"> </param>
		/// <returns></returns>
		Directory GetDirectory(Path directoryPath);

		Path GetPath(string path);

		TemporaryDirectory CreateTempDirectory();

		Directory CreateDirectory(string path);

		Directory CreateDirectory(Path path);

		File GetFile(string filePath);

		File GetFile(Path filePath);

		TemporaryFile CreateTempFile();

		Directory GetTempDirectory();

		Directory GetCurrentDirectory();
	}
}