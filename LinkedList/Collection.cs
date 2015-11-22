using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collections
{
    // Linked list with nodes in one block of memory
    public class Collection<T>
    {
        public List<CollectionElement<T>> UnorderedElements = new List<CollectionElement<T>>();
        public CollectionElement<T> First;
        public CollectionElement<T> Last;

        public int Length { get { return UnorderedElements.Count; } }

        public void PushAfter(CollectionElement<T> node, T value)
        {
            if (node == Last)
            {
                Last.Next = new CollectionElement<T>() { Value = value, Prev = Last };
            }
            else
            {
                node.Next = new CollectionElement<T>() { Value = value, Prev = node, Next = node.Next };
            }

            Length++;
        }

        public void PushBefore(

        public void PushFront(T value)
        {
            First = new CollectionElement<T>() { Value = value, Next = First };
            First.Next.Prev = First;

            Length++;
        }

        public void PushBack(T value)
        {
            Last = new CollectionElement<T>() { Value = value, Prev = First };
            Last.Prev.Next = Last;

            Length++;
        }
    }
}
