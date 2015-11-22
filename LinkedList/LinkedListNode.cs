using System;
using System.Linq;
using System.Threading.Tasks;

namespace Collections
{
    public class CollectionElement<T>
    {
        public int PrevPosition = -1;
        public int NextPosition = -1;
        public T Value;

        public CollectionElement<T> Prev(Collection<T> collection)
        {
            if (PrevPosition == -1)
            {
                return null;
            }

            return collection.UnorderedElements[PrevPosition];
        }

        public CollectionElement<T> Next(Collection<T> collection)
        {
            if (NextPosition == -1)
            {
                return null;
            }

            return collection.UnorderedElements[NextPosition];
        }
    }
}
