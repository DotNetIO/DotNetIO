using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DotNetIO.Tests
{
	#region Full license
	// Copyright Sebastien Lamla

	// Permission is hereby granted, free of charge, to any person obtaining
	// a copy of this software and associated documentation files (the
	// "Software"), to deal in the Software without restriction, including
	// without limitation the rights to use, copy, modify, merge, publish,
	// distribute, sublicense, and/or sell copies of the Software, and to
	// permit persons to whom the Software is furnished to do so, subject to
	// the following conditions:
	// The above copyright notice and this permission notice shall be
	// included in all copies or substantial portions of the Software.
	// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
	// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
	// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
	// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
	// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

	#endregion
	public static class SpecExtensions
	{
		public static T ShouldBe<T>(this T valueToAnalyse, T expectedValue)
		{
			Assert.That(valueToAnalyse, Is.EqualTo(expectedValue));
			return valueToAnalyse;
		}

		public static T Check<T>(this T obj, Action<T> assertion)
		{
			assertion(obj);
			return obj;
		}

		public static IEnumerable<T> ShouldHaveCountOf<T>(this IEnumerable<T> values, int count)
		{
			values.ShouldNotBeNull().Count().ShouldBe(count);
			return values;
		}

		public static IEnumerable<T> ShouldHaveSameElementsAs<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
		{
			var enumerator1 = actual.GetEnumerator();
			var enumerator2 = expected.GetEnumerator();
			bool moveNext1 = false, moveNext2 = false;
			while (((moveNext1 = enumerator1.MoveNext()) && (moveNext2 = enumerator2.MoveNext()))
			       && moveNext1 == moveNext2)
				Assert.AreEqual(enumerator1.Current, enumerator2.Current);
			if (moveNext1 != moveNext2)
				Assert.Fail("The two enumerables didn't have the same number of elements.");
			enumerator1.Dispose();
			enumerator2.Dispose();
			return actual;
		}

		public static T ShouldNotBe<T>(this T valueToAnalyse, T expectedValue)
		{
			Assert.That(valueToAnalyse, Is.Not.EqualTo(expectedValue));
			return valueToAnalyse;
		}

		public static T ShouldNotBeNull<T>(this T obj) where T : class
		{
			Assert.IsNotNull(obj);
			return obj;
		}
	}
}