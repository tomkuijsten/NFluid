using System;
using System.Collections.Generic;
using System.Linq;

namespace Devvy.FluidFlow
{
    public interface IFlow<T>
    {
        IFlow<T> AddParameters(params object[] args);
        IFlow<TResult> Call<TResult>(Func<T, TResult> doSomething);
        IFlow<TResult> Call<TResult, T2>(Func<T, T2, TResult> doSomething);
        IFlow<T> Catch<TException>() where TException : Exception;
        T Return();
    }

    internal class Flow
    {
        internal static IFlow<T> Create<T>(T input, IDictionary<Type, object> parameters = null)
        {
            return new Flow<T>(input, parameters);
        }

        internal static IFlow<T> Create<T>(
            Delegate inputFunction,
            Delegate[] inputParameters,
            IDictionary<Type, object> parameters = null)
        {
            return new Flow<T>(inputFunction, inputParameters, parameters);
        }
    }

    internal class Flow<T> : IFlow<T>
    {
        private Delegate InputFunction { get; }
        private Delegate[] InputParameters { get; }

        private IList<Type> _exceptionsToCatch;

        private IDictionary<Type, object> _parameters;

        internal Flow(T input, IDictionary<Type, object> parameters = null)
        {
            InputFunction = new Func<T, T>((i) => i);
            InputParameters = new Delegate[] { new Func<T>(() => input) };
            _parameters = parameters ?? new Dictionary<Type, object>();
            _exceptionsToCatch = new List<Type>();
        }

        internal Flow(
            Delegate inputFunction,
            Delegate[] inputParameters,
            IDictionary<Type, object> parameters = null)
        {
            InputFunction = inputFunction;
            InputParameters = inputParameters;
            _parameters = parameters ?? new Dictionary<Type, object>();
            _exceptionsToCatch = new List<Type>();
        }

        public IFlow<T> Catch<TException>() where TException : Exception
        {
            _exceptionsToCatch.Add(typeof(TException));

            return this;
        }

        public IFlow<T> AddParameters(params object[] args)
        {
            args.ToList().ForEach(a => _parameters.Add(a.GetType(), a));

            return this;
        }

        public T Return()
        {
            var materializedParameters = new List<object>();
            foreach (var inputParam in InputParameters)
            {
                materializedParameters.Add(inputParam.DynamicInvoke());
            }

            return (T)InputFunction.DynamicInvoke(materializedParameters.ToArray());
        }

        public IFlow<TResult> Call<TResult>(Func<T, TResult> doSomething)
        {
            return Flow.Create<TResult>(
                doSomething,
                new Delegate[]
                {
                    new Func<object>(() =>
                    {
                        var materializedParameters = new List<object>();
                        foreach (var inputParam in InputParameters)
                        {
                            materializedParameters.Add(inputParam.DynamicInvoke());
                        }

                        return InputFunction.DynamicInvoke(materializedParameters.ToArray());
                    })
                },
                _parameters);
        }

        public IFlow<TResult> Call<TResult, T2>(Func<T, T2, TResult> doSomething)
        {
            //return Flow.Create(doSomething(Input, (T2)_parameters[typeof(T2)]), _parameters);
            return null;
        }
    }

    public static class FlowExtensions
    {
        public static IFlow<T> StartFlow<T>(this T obj)
        {
            return Flow.Create(obj);
        }
    }
}
