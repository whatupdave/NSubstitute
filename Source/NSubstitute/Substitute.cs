using System;
using LinFu.AOP.Interfaces;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;

namespace NSubstitute
{
    public class Substitute : ISubstitute
    {
        readonly ICallStack _callStack;
        readonly ICallResults _callResults;
        readonly ISubstitutionContext _context;

        public Substitute(ICallStack callStack, ICallResults callResults, ISubstitutionContext context)
        {
            _callStack = callStack;
            _callResults = callResults;
            _context = context;
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            var lastCall = _callStack.Pop();
            _callResults.SetResult(lastCall, valueToReturn);
        }

        public void MemberInvoked(IInvocation invocation)
        {
            _callStack.Push(invocation);
            _context.LastSubstituteCalled(this);
            var valueToReturn = _callResults.GetResult(invocation);
            invocation.SetReturn(valueToReturn);
        }

        public static T For<T>()
        {
            var proxyFactory  = new ProxyFactory();
            return proxyFactory.CreateProxy<T>(new SubstituteInterceptor(new Substitute(new CallStack(), new CallResults(), SubstitutionContext.Current)));
        }

    }

    public class SubstituteInterceptor : IInterceptor
    {
        readonly Substitute _substitute;

        public SubstituteInterceptor(Substitute substitute)
        {
            _substitute = substitute;
        }

        public object Intercept(IInvocationInfo info)
        {
            var invocation = new Invocation(info.TargetMethod.Name, info.ReturnType);
            _substitute.MemberInvoked(invocation);
            return invocation.GetReturn();
        }

        class Invocation : IInvocation
        {
            readonly string _name;
            readonly Type _returnType;
            object _valueToReturn;

            public Invocation(string name, Type returnType)
            {
                _name = name;
                _returnType = returnType;
            }

            public void SetReturn(object valueToReturn)
            {
                _valueToReturn = valueToReturn;
            }

            public object GetReturn()
            {
                return _valueToReturn;
            }

            public Type GetReturnType()
            {
                return _returnType;
            }

            public bool Equals(IInvocation other)
            {
                return _name == ((Invocation) other)._name;
            }

            public override bool Equals(object obj)
            {
                if (obj is Invocation) return Equals((Invocation) obj);
                return false;
            }

            public override int GetHashCode()
            {
                return _name.GetHashCode();
            }
        }
    }


}