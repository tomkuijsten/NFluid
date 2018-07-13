using System;

namespace NFluid
{
    public interface IFlow<T>
    {
        /// <summary>
        /// Registers a value to the injection framework which will be used to pass as
        /// parameter whenever the type is used in a delegate.
        /// </summary>
        /// <param name="value">The parameter for the injection framework</param>
        /// <returns>The current object</returns>
        IFlow<T> Register<TInject>(TInject value);
        IFlow<T> Register<TInject>(TInject value, string parameterName);


        /// <summary>
        /// Start or continue a chain where the output of the first is used as input
        /// for the next.
        /// </summary>
        /// <typeparam name="TResult">Return type of the function</typeparam>
        /// <param name="doSomething"></param>
        /// <returns></returns>
        IFlow<TResult> Chain<TResult>(Func<T, TResult> method);

        /// <see cref="Chain{TResult}(Func{T, TResult})"/>
        IFlow<TResult> Chain<TResult, T2>(Func<T, T2, TResult> method);
        IFlow<TResult> Chain<TResult, T2, T3>(Func<T, T2, T3, TResult> method);

        IFlow<T> Catch<TException>() where TException : Exception;

        T Return();
    }
}
