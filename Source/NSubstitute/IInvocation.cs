using System;

namespace NSubstitute
{
    public interface IInvocation : IEquatable<IInvocation>
    {
        void SetReturn(object valueToReturn);
        Type GetReturnType();
    }
}