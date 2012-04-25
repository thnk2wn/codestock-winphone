using System.Collections.Generic;

namespace CodeStock.Data.Model
{
    public class ModelBase<T>
    {
        public List<T> d { get; set; }
        public object meta { get; set; }
    }
}
