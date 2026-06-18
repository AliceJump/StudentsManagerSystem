using StudentsManagerSystem.Common;
using Xunit;

namespace StudentsManagerSystem.Tests.Common;

public sealed class AcademicYearHelperTests
{
    [Theory]
    [InlineData("2024", "2024")]
    [InlineData("2024-2025", "2024")]
    [InlineData("", "")]
    public void NormalizeStartYear_ReturnsStartYear(string input, string expected)
    {
        Assert.Equal(expected, AcademicYearHelper.NormalizeStartYear(input));
    }
}
