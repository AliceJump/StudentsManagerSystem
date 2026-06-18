using StudentsManagerSystem.Services;
using Xunit;

namespace StudentsManagerSystem.Tests.Services;

public sealed class ServiceResultTests
{
    [Fact]
    public void Success_ReturnsSucceededTrue()
    {
        var result = ServiceResult.Success();
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Failure_ReturnsSucceededFalse()
    {
        var result = ServiceResult.Failure("错误");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public void GenericSuccess_ReturnsData()
    {
        var result = ServiceResult<int>.Success(42);
        Assert.Equal(42, result.Data);
    }
}
