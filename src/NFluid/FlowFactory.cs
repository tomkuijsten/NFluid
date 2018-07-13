using System;
using System.Collections.Generic;

namespace NFluid
{
    internal class FlowFactory
    {
        internal static IFlow<T> Create<T>(T input, IDictionary<string, object> parameters = null)
        {
            return new Flow<T>(input, parameters);
        }

        internal static IFlow<T> Create<T>(
            Delegate inputFunction,
            Delegate[] inputParameters,
            IDictionary<string, object> parameters = null)
        {
            return new Flow<T>(inputFunction, inputParameters, parameters);
        }
    }
}
