namespace DotNetIO.Contracts
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// Contract for the IDirectory handle
	/// </summary>
	[ContractClassFor(typeof(Directory))]
	internal abstract class IDirectoryContract : Directory
	{
		public Directory GetDirectory(string directoryName)
		{
			Contract.Requires(directoryName != null);
			Contract.Ensures(Contract.Result<Directory>() != null);

			throw new NotImplementedException();
		}

		public File GetFile(string fileName)
		{
			Contract.Requires(fileName != null);
			Contract.Ensures(Contract.Result<File>() != null);

			throw new NotImplementedException();
		}

		public IEnumerable<File> Files()
		{
			Contract.Ensures(Contract.Result<IEnumerable<File>>() != null);

			throw new NotImplementedException();
		}

		public IEnumerable<Directory> Directories()
		{
			Contract.Ensures(Contract.Result<IEnumerable<Directory>>() != null);

			throw new NotImplementedException();
		}

		public IEnumerable<File> Files(string filter, SearchScope scope)
		{
			Contract.Requires(filter != null);
			Contract.Ensures(Contract.Result<IEnumerable<File>>() != null);

			throw new NotImplementedException();
		}

		public IEnumerable<Directory> Directories(string filter, SearchScope scope)
		{
			Contract.Requires(filter != null);
			Contract.Ensures(Contract.Result<IEnumerable<Directory>>() != null);

			throw new NotImplementedException();
		}

		public bool IsHardLink
		{
			get { throw new NotImplementedException(); }
		}

		public Directory LinkTo(Path path)
		{
			Contract.Requires(path != null);
			Contract.Ensures(Contract.Result<Directory>() != null);

			throw new NotImplementedException();
		}

		public Directory Target
		{
			get
			{
				Contract.Ensures(Contract.Result<Directory>() != null);
				throw new NotImplementedException();
			}
		}

		public abstract Directory Create();

		public abstract Path Path { get; }
		public abstract Directory Parent { get; }
		public abstract FileSystem FileSystem { get; }
		public abstract bool Exists();
		public abstract string Name { get; }

		public abstract void Delete();

		public abstract void CopyTo(FileSystemItem item);

		public abstract FileSystemItem MoveTo(FileSystemItem item);
	}
}