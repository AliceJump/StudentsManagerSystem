using StudentsManagerSystem.Common;
using Xunit;

namespace StudentsManagerSystem.Tests.Common;

public sealed class InputValidatorTests
{
    [Theory]
    [InlineData("20240001", true)]
    [InlineData("abc", false)]
    [InlineData("", false)]
    public void ValidateStudentNo_ReturnsExpectedResult(string value, bool expected)
    {
        Assert.Equal(expected, InputValidator.ValidateStudentNo(value));
    }

    [Theory]
    [InlineData("2024", true)]
    [InlineData("2024-2025", false)]
    [InlineData("24", false)]
    public void ValidateAcademicYear_ReturnsExpectedResult(string value, bool expected)
    {
        Assert.Equal(expected, InputValidator.ValidateAcademicYear(value));
    }
}
