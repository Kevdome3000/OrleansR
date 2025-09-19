namespace OrleansR.Tests.Core.Internal;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using OrleansR.Core.Provider;
using Xunit;

public record MyMethodRequest(string Message);


public interface ITestClient
{
    Task MyMethod(string arg1, int arg2, MyMethodRequest arg3);
    Task<int> MyMethodWithAReturnValue();
}


public class TypedClientBuilderTests
{
    [Fact]
    public async Task GetsAStronglyTypedClient()
    {
        Mock<IClientProxy> clientProxy = new();
        ITestClient client = TypedClientBuilder<ITestClient>.Build(clientProxy.Object);

        string arg1 = "MyArg1";
        int arg2 = 30;
        MyMethodRequest arg3 = new("Message");
        await client.MyMethod(arg1, arg2, arg3);

        clientProxy.Verify(
            x =>
                x.SendCoreAsync(
                    nameof(ITestClient.MyMethod),
                    new object[] { arg1, arg2, arg3 },
                    CancellationToken.None
                )
        );
    }


    [Fact]
    public async Task ThrowsInvalidOperationExceptionForMethodsWithReturnValues()
    {
        Mock<IClientProxy> clientProxy = new();
        ITestClient client = TypedClientBuilder<ITestClient>.Build(clientProxy.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(client.MyMethodWithAReturnValue);
    }
}

