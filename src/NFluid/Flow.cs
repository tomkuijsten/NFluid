using System;
using System.Collections.Generic;
using System.Linq;

namespace NFluid
{
    internal class Flow<T> : IFlow<T>
    {
        private Delegate InputFunction { get; }
        private Delegate[] InputParameters { get; }

        private IList<Type> _exceptionsToCatch;

        private IDictionary<string, object> _parameters;

        internal Flow(T input, IDictionary<string, object> parameters = null)
        {
            InputFunction = new Func<T, T>((i) => i);
            InputParameters = new Delegate[] { new Func<T>(() => input) };
            _parameters = parameters ?? new Dictionary<string, object>();
            _exceptionsToCatch = new List<Type>();
        }

        internal Flow(
            Delegate inputFunction,
            Delegate[] inputParameters,
            IDictionary<string, object> parameters = null)
        {
            InputFunction = inputFunction;
            InputParameters = inputParameters;
            _parameters = parameters ?? new Dictionary<string, object>();
            _exceptionsToCatch = new List<Type>();
        }

        public IFlow<T> Catch<TException>() where TException : Exception
        {
            _exceptionsToCatch.Add(typeof(TException));

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

        private IFlow<TResult> CallInternal<TResult>(Delegate d)
        {
            List<Delegate> parameterDelegates = new List<Delegate>();

            var parameters = d.Method.GetParameters().Skip(1);

            // assume the first one is the input
            parameterDelegates.Add(
                    new Func<object>(() =>
                    {
                        var materializedParameters = new List<object>();
                        foreach (var inputParam in InputParameters)
                        {
                            materializedParameters.Add(inputParam.DynamicInvoke());
                        }

                        return InputFunction.DynamicInvoke(materializedParameters.ToArray());
                    }));

            foreach (var p in parameters)
            {
                parameterDelegates.Add(new Func<object>(() =>
                {
                    if (_parameters.ContainsKey(p.Name))
                    {
                        return _parameters[p.Name];
                    }

                    return _parameters[p.ParameterType.ToString()];
                }));
            }

            return FlowFactory.Create<TResult>(d, parameterDelegates.ToArray(), _parameters);
        }

        public IFlow<T> Register<TInject>(TInject value)
        {
            _parameters.Add(typeof(TInject).ToString(), value);

            return this;
        }

        public IFlow<T> Register<TInject>(TInject value, string parameterName)
        {
            _parameters.Add(parameterName, value);

            return this;
        }

        public IFlow<TResult> Chain<TResult>(Func<T, TResult> method) => CallInternal<TResult>(method);
        public IFlow<TResult> Chain<TResult, T2>(Func<T, T2, TResult> method) => CallInternal<TResult>(method);
        public IFlow<TResult> Chain<TResult, T2, T3>(Func<T, T2, T3, TResult> method) => CallInternal<TResult>(method);
    }
}
