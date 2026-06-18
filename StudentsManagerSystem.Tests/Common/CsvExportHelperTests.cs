using StudentsManagerSystem.Common;
using Xunit;

namespace StudentsManagerSystem.Tests.Common;

public sealed class CsvExportHelperTests
{
    [Theory]
    [InlineData("hello world", "hello world")]
    [InlineData("a,b", "\"a,b\"")]
    [InlineData("\"hello\"", "\"\"\"hello\"\"\"")]
    public void Escape_ReturnsExpectedResult(string value, string expected)
    {
        Assert.Equal(expected, CsvExportHelper.Escape(value));
    }
}
