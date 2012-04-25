using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Phone.Common.Extensions.System.Collections.Generic_
{
    public static class IEnumerableOfTExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            if (enumerableList != null)
            {
                //create an emtpy observable collection object
                var observableCollection = new ObservableCollection<T>();

                //loop through all the records and add to observable collection object
                foreach (var item in enumerableList)
                    observableCollection.Add(item);

                //return the populated observable collection
                return observableCollection;
            }
            return null;
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
