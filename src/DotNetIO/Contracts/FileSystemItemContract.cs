namespace DotNetIO.Contracts
{
	using System.Diagnostics.Contracts;

	[ContractClassFor(typeof(FileSystemItem))]
	public class FileSystemItemContract : FileSystemItem
	{
		public Path Path
		{
			get
			{
				Contract.Ensures(Contract.Result<Path>() != null);
				throw new System.NotImplementedException();
			}
		}

		public Directory Parent
		{
			get
			{
				Contract.Ensures(
					Contract.Result<Directory>() == null 
					|| Contract.Result<Directory>() != null);

				throw new System.NotImplementedException();
			}
		}

		public FileSystem FileSystem
		{
			get
			{
				Contract.Ensures(Contract.Result<FileSystemItem>() != null);
				throw new System.NotImplementedException();
			}
		}

		public bool Exists()
		{
			throw new System.NotImplementedException();
		}

		public string Name
		{
			get
			{
				Contract.Ensures(Contract.Result<string>() != null);
				throw new System.NotImplementedException();
			}
		}

		public void Delete()
		{
			throw new System.NotImplementedException();
		}

		public void CopyTo(FileSystemItem item)
		{
			Contract.Requires(item != null);
			throw new System.NotImplementedException();
		}

		public FileSystemItem MoveTo(FileSystemItem item)
		{
			Contract.Requires(item != null);
			throw new System.NotImplementedException();
		}
	}
}