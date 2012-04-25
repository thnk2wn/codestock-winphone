using System;
using System.Linq.Expressions;

namespace Phone.Common.IO
{
    public interface IDataStorage
    {
        bool Backup(string token, object value);
        bool Backup<T>(Expression<Func<T>> propertyExpression, T value);

        T Restore<T>(string token);
        T Restore<T>(string token, T defaultValue);
        T Restore<T>(Expression<Func<T>> propertyExpression, T defaultValue);
    }
}
