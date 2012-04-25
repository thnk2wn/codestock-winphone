namespace Phone.Common.Extensions.System_
{
    public static class ObjectExtensions
    {
        public static T As<T>(this object obj)
        {
            return (T)obj;
        }

        public static T TryAs<T>(this object obj)
            where T: class 
        {
            return obj as T;
        }
    }
}
