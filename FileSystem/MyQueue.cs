using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    public class MyQueue<T>
    {
        public MyLinkedList<T> queue = new MyLinkedList<T>();

        public void EnQueue(T element)
        {
            queue.AddFirst(element);
        }
        public T DeQueue()
        {
            var treeNode = queue.Last().Value;
            queue.Remove(queue.Last());
            return treeNode;
        }
        public bool IsEmpty()
        {
            if (queue.First() == null)
                return true;
            else
                return false;
        }
    }
}
