﻿// Copyright 2004-2012 Henrik Feldt - https://github.com/DotNetIO
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace DotNetIO
{
	using System.IO;

	public static class StreamExtensions
	{
		public static void Write(this Stream stream, byte[] bytes)
		{
			stream.Write(bytes, 0, bytes.Length);
		}

		public static long CopyTo(this Stream stream, Stream destinationStream, uint bufSize = 4096)
		{
			var buffer = new byte[bufSize];
			var readCount = 0;
			long totalWritten = 0;
			while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				totalWritten += readCount;
				destinationStream.Write(buffer, 0, readCount);
			}

			return totalWritten;
		}
	}
}