using System.Collections.Generic;

namespace Phone.Common.Collections
{
    public class Group<T> : IEnumerable<T>
    {
        public Group(string title, IEnumerable<T> items) : this()
        {
            this.Title = title;
            this.Items = new List<T>(items);
        }

        public Group()
        {
            return;
        }

        public override bool Equals(object obj)
        {
            var that = obj as Group<T>;
            var same = (that != null) && (this.Title.Equals(that.Title));
            return same;
        }

        public override int GetHashCode()
        {
            return this.Title.GetHashCode() ^ this.Items.GetHashCode();
        }

        public string Title { get; set; }
        public IList<T> Items { get; set; }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion
    }
}
