using NUnit.Framework;
using SharpTestsEx;

namespace DotNetIO.Tests.paths
{
	public class path_specification
	{
		[Test]
		public void path_has_segments()
		{
			var path = new Path(@"C:\this\that");
			path.Segments.ShouldHaveSameElementsAs(new[] { @"C:", "this", "that" });
		}

		[Test]
		public void path_with_unc_root_doesnt_include_unc_root_in_segments()
		{
			@"\\?\C:\a\b".ToPath()
				.Segments
				.Should().Not.Contain("?");
		}

		[Test]
		public void trailing_slash_is_always_normalized()
		{
			new Path(@"C:\a\b").Should().Be.EqualTo(new Path(@"C:\a\b"));
		}

		[TestCase(@"test\folder")]
		[TestCase(@"test/folder")]
		public void relative_path_is_not_rooted()
		{
			new Path(@"test\folder").IsRooted.Should().Be.False();
		}

		[TestCase(@"c:\test\folder", @"c:\test", "folder")]
		[TestCase(@"c:\test\folder", @"c:\test\another", @"..\folder")]
		[TestCase(@"c:\test\folder", @"c:\test\nested\folder", @"..\..\folder")]
		public void absolute_path_is_made_relative(string source, string basePath, string result)
		{
			new Path(source)
				.MakeRelative(new Path(basePath))
				.FullPath.ShouldBe(result);
		}

		[Test]
		public void relative_path_is_made_relative_by_returning_itself()
		{
			new Path("folder")
				.MakeRelative(new Path(@"c:\tmp"))
				.FullPath.Should().Be("folder");
		}

		[TestCase(@"C:\a", @"C:\")]
		[TestCase(@"C:\a\b", @"C:\a")]
		[TestCase(@"C:\a\b\c", @"C:\a\b")]
		[TestCase(@"C:\a", @"C:\")]
		[TestCase(@"C:\a", @"C:\")]
		[TestCase(@"C:\a\b\c.txt", @"C:\a\b")]
		[TestCase(@"\\?\C:\a\b\c d e.txt", @"C:\a\b")]
		[TestCase(@"\\?\C:\a\b\c d e.txt", @"C:\a\b")]
		[TestCase(@"\\?\C:\a\b\", @"C:\a")]
		public void gettting_without_last_bit_should_act_only_on_folder_and_file_part(
			string firstPart, string result)
		{
			Path.GetPathWithoutLastBit(firstPart)
				.Should().Be(new Path(result));
		}

		[TestCase(@"c:\test", @"c:\")]
		[TestCase(@"c:\test\", @"c:\test\")]
		public void drive_and_directory_depends_on_position_of_separator(string path, string directory)
		{
			new Path(path)
				.DriveAndDirectory
				.Should().Be(directory);
		}
	}
}