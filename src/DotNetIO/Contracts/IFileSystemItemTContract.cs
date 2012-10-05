namespace DotNetIO.Contracts
{
	using System.Diagnostics.Contracts;

	[ContractClassFor(typeof(FileSystemItem<>))]
	internal abstract class FileSystemItemTContract<T> : FileSystemItem<T>
		where T : FileSystemItem
	{
		public T Create()
		{
			Contract.Ensures(!object.ReferenceEquals(Contract.Result<T>(), null));
			throw new System.NotImplementedException();
		}

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