namespace NFluid
{
    public static class FlowExtensions
    {
        public static IFlow<T> StartFlow<T>(this T obj)
        {
            return FlowFactory.Create(obj);
        }
    }
}
