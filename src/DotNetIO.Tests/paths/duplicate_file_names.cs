using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace DotNetIO.Tests
{
	class duplicate_file_names
	{
		[Test]
		// pass-through cases
		[TestCase("abc",	new string[0],	"abc",		255u)]
		[TestCase("abc.ext",new string[0],	"abc.ext",	255u)]
		[TestCase("abc.ext",new[] { "abc" },"abc.ext",	255u)]
		[TestCase("abc ",	new[]{"abc"},	"abc ",		255u)]
		// rename cases
		[TestCase("abc",	new[]{"abc"},	"abc_1",	255u)]
		[TestCase("abc",	new[]{"abc", "abc_1"},	"abc_2",	255u)]
		[TestCase("a",		new[]{"a"},		"_1",		2u)]
		[TestCase("a",		new[] { "a", "_1", "_2", "_3", "_4", "_5", "_6", "_7", "_8", "_9", "_a" },
											"_b",		2u)]
		[TestCase("a.b",	new[]{"a.b"},	"a_1.b",	5u)]
		public void can_generate_next(string wanted, string[] existing, string renamed, uint nameLimit)
		{
			var wantedPath = wanted.ToPath();
			var existingNames = new HashSet<string>(existing);

			Assert.That(wantedPath.NextName(existingNames, nameLimit),
				Is.EqualTo(renamed.ToPath()));
		}

		[Test]
		public void count_over_36()
		{
			var wanted = "abc";
			var otherForSix = "123456789abcdefghijklmnopqrstuvwxyz".ToCharArray().Select(c => wanted + "_" + c);
			var otherForFive = "123456789abcdefghijklmnopqrstuvwxyz".ToCharArray().Select(c => "abc" + "_" + c);
			var otherForFour = "123456789abcdefghijklmnopqrstuvwxyz".ToCharArray().Select(c => "ab" + "_" + c);

			var path = wanted.ToPath();
			Assert.That(
				path.NextName(new HashSet<string>(otherForSix.Concat(new[]{"abc"})), 6u).FullPath,
				Is.EqualTo("abc_10"));
			var others = otherForFive.Concat(new[] {"abc"});
			Assert.That(
				path.NextName(new HashSet<string>(others), 5u).FullPath,
				Is.EqualTo("ab_10"));
			Assert.That(
				path.NextName(new HashSet<string>(otherForFour.Concat(new[]{"abc"})), 4u).FullPath,
				Is.EqualTo("a_10"));
		}

		[Test, TestCase("a", new[] { "a" }, 2u)]
		public void out_of_names_exception(string wanted, string[] existing, uint nameLimit)
		{
			var wantedPath = wanted.ToPath();
			var defaultRenames = existing.Concat("123456789abcdefghijklmnopqrstuvwxyz".ToCharArray().Select(c => "_" + c));
			var existingNames = new HashSet<string>(defaultRenames);

			Assert.Throws<OutOfNamesException>(() =>
				wantedPath.NextName(existingNames, nameLimit));
		}

		[Test, TestCase(1, "1")]
		[TestCase(35, "z")]
		[TestCase(42, "16")]
		[TestCase(71, "1z")]
		public void count_up_test(int targ, string expected)
		{
			int count = 0;
			foreach (var num in CommonExtensions.CountUp(1))
			{
				if (++count == targ)
				{
					Assert.That(num, Is.EqualTo(expected));
					break;
				}
			}
		}
	}
}