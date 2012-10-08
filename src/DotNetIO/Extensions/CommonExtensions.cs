// Copyright 2004-2012 Henrik Feldt - https://github.com/DotNetIO
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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace DotNetIO
{
	/// <summary>
	/// 	Common extensions among <see cref="File" /> and <see cref="Directory" />.
	/// </summary>
	public static class CommonExtensions
	{
		public static T MustExist<T>(this T fileSystemItem)
			where T : FileSystemItem<T>
		{
			if (!fileSystemItem.Exists())
				fileSystemItem.Create();

			return fileSystemItem;
		}

		/// <summary>
		/// Given a path 'wanted' that is wanted (i.e. you'd prefer that name), and
		/// a hash set of existing file *names* (not paths!), and a limit on how
		/// long you allow file names to be, generate a new name, suffixed
		/// by '_NUM', where NUM is a base36 number [0-9a-z], and is generated
		/// *before* the extension. Consume characters from the end of the *name without
		/// extension* of the file name, towards the start of the file name, if the
		/// added suffix makes the file name too long. The suffix generator does not generate
		/// index-zero based, but one-based.
		/// </summary>
		/// <param name="wanted">The path that you want to write to.</param>
		/// <param name="existing">Existing files in the folder.</param>
		/// <param name="nameLimit">The file name limit, defaults to NTFS 255 characters</param>
		/// <exception cref="OutOfNamesException">
		/// Can't generate more names, because
		/// there aren't enough characters in the name space allowed by the limit.</exception>
		/// <returns></returns>
		[Pure]
		public static Path NextName(this Path wanted, HashSet<string> existing, uint nameLimit = 255u)
		{
			Contract.Requires(wanted != null);
			Contract.Requires(existing != null);
			Contract.Ensures(Contract.Result<Path>() != null);
			Contract.Ensures(
				Path.GetFileName(Contract.Result<Path>().FullPath).Length <= nameLimit);

			const string spacer = "_";

			var name = Path.GetFileName(wanted.FullPath);

			if (!existing.Contains(name))
				return wanted;

			foreach (var suffixValue in CountUp(1))
			{
				var suffixSize = spacer.Length + suffixValue.Length; // "_1zaf"
				var ext = Path.GetExtension(wanted.FullPath);
				var nameNoExt = Path.GetFileNameWithoutExtension(wanted.FullPath);

				var changeIndex = 0u;
				while (
					changeIndex < nameNoExt.Length
					/* total is within name size limits */
					&& changeIndex + suffixSize + ext.Length < nameLimit)
					changeIndex++;

				var newName = string.Format("{0}{1}{2}{3}", nameNoExt.Substring(0, (int) changeIndex), spacer, suffixValue, ext);
				if (newName.Length > nameLimit)
					break; // out of names

				if (existing.Contains(newName))
					continue;

				return newName.ToPath();
			}

			throw new OutOfNamesException("There are no more names available");
		}

		public static IEnumerable<string> CountUp(long start, string baseAlphabet = "0123456789abcdefghijklmnopqrstuvwxyz")
		{
			var suffixChars = baseAlphabet.ToCharArray();

			do
			{
				var number = new Stack<char>();

				long rem;
				var div = Math.DivRem(start, suffixChars.Length, out rem);
				number.Push(suffixChars[rem]);

				while (div > 0)
				{
					div = Math.DivRem(div, suffixChars.Length, out rem);
					number.Push(suffixChars[rem]);
				}

				yield return new string(number.ToArray());

			} while (++start < long.MaxValue);
		}
	}

	[Serializable]
	public class OutOfNamesException : Exception
	{
		public OutOfNamesException(string message) : base(message)
		{
		}
	}
}