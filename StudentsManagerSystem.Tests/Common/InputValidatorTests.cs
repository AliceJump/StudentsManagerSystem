using StudentsManagerSystem.Common;
using Xunit;

namespace StudentsManagerSystem.Tests.Common;

public sealed class InputValidatorTests
{
    [Theory]
    [InlineData("20240001", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void ValidateStudentNo_ReturnsExpectedResult(string value, bool expected)
    {
        Assert.Equal(expected, InputValidator.ValidateStudentNo(value));
    }
}
