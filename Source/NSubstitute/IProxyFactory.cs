namespace NSubstitute
{
    public interface IProxyFactory
    {
        T GenerateProxy<T>(IInvocationHandler invocationHandler);
    }
}