using System.Collections.ObjectModel;

namespace Phone.Common.Collections
{
    public class GroupedObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// The Group Title
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor ensure that a Group Title is included
        /// </summary>
        /// <param name="name">string to be used as the Group Title</param>
        public GroupedObservableCollection(string name)
        {
            this.Title = name;
        }

        /// <summary>
        /// Returns true if the group has a count more than zero
        /// </summary>
        public bool HasItems
        {
            get
            {
                return (Count != 0);
            }
            private set
            {
            }
        }
    }
}
