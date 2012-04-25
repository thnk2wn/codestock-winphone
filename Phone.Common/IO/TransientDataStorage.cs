using System;
using System.Linq.Expressions;
using Microsoft.Phone.Shell;
//using System.Reflection;

namespace Phone.Common.IO
{
    public class TransientDataStorage : IDataStorage
    {
        public bool Backup(string token, object value)
        {
            if (null == value)
                return false;

            var store = PhoneApplicationService.Current.State;
            if (store.ContainsKey(token))
                store[token] = value;
            else
                store.Add(token, value);

            return true;
        }

        public bool Backup<T>(Expression<Func<T>> propertyExpression, T value)
        {
            var body = (MemberExpression)propertyExpression.Body;
            var key = body.Expression.Type.Name + "." + body.Member.Name;

            // not available on WP7
            // var value = propertyExpression.Compile();

            // property info works but GetValue will throw MethodAccessException on WP7 unless the property is public
            // var pi = body.Member.DeclaringType.GetProperty(body.Member.Name, BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);
            // var value = pi.GetValue(sender, BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);

            // a bit awkward to have T value parameter like we are passing it twice in 2 diff ways for value but limited
            return Backup(key, value);
        }

        public T Restore<T>(Expression<Func<T>> propertyExpression)
        {
            var body = (MemberExpression)propertyExpression.Body;
            var key = body.Expression.Type.Name + "." + body.Member.Name;
            return Restore<T>(key);
        }

        public T Restore<T>(Expression<Func<T>> propertyExpression, T defaultValue)
        {
            var body = (MemberExpression)propertyExpression.Body;
            var key = body.Expression.Type.Name + "." + body.Member.Name;
            return Restore(key, defaultValue);
        }

        public T Restore<T>(string token)
        {
            var store = PhoneApplicationService.Current.State;
            if (!store.ContainsKey(token))
                return default(T);

            return (T)store[token];
        }

        public T Restore<T>(string token, T defaultValue)
        {
            var store = PhoneApplicationService.Current.State;
            if (!store.ContainsKey(token))
                return defaultValue;

            return (T)store[token];
        }
    }
}
