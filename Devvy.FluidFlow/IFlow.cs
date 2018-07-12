using System;

namespace NFluid
{
    public interface IFlow<T>
    {
        IFlow<T> AddParameters(params object[] args);
        IFlow<TResult> Call<TResult>(Func<T, TResult> doSomething);
        IFlow<TResult> Call<TResult, T2>(Func<T, T2, TResult> doSomething);
        IFlow<T> Catch<TException>() where TException : Exception;
        T Return();
    }
}
