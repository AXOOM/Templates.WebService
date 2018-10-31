using System;
using Moq.AutoMock;

namespace MyVendor.MyService
{
    public abstract class AutoMockingFactsBase<TSubject> : AutoMocker, IDisposable
        where TSubject : class
    {
        protected TSubject Subject => CreateInstance<TSubject>();
        
        public virtual void Dispose() => VerifyAll();
    }
}
