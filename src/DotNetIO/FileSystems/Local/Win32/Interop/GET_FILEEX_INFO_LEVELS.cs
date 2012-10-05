using System;

namespace DotNetIO.FileSystems.Local.Win32.Interop
{
	// http://msdn.microsoft.com/en-us/library/windows/desktop/bb736257%28v=vs.85%29.aspx
	[Serializable]
	public enum GET_FILEEX_INFO_LEVELS
	{
		GetFileExInfoStandard = 0,
		GetFileExMaxInfoLevel = 1
	}
}