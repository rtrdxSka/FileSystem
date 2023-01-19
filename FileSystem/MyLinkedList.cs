using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    
        public class Node<Т>
        {
            public Т Value { get; set; }
            public Node<Т> NextNode { get; set; }
            public Node<Т> PrevNode { get; set; }


        }
        public class MyLinkedList<Т>
        {
            private int count = 0;
            private Node<Т> head, tail;
            public MyLinkedList()
            {
                head = tail = null;
            }

            public Node<Т> AddFirst(Т value)
            {
                var newNode = new Node<Т>
                {
                    Value = value,
                    PrevNode = null,
                    NextNode = head
                };

                //If there already is a first element in the list,
                //change its prevNode to the newly made newNode,
                //otherwise it becomes the both the tail and the head as the list is empty
                if (head != null)
                    head.PrevNode = newNode;
                else
                    tail = newNode;

                head = newNode;

                count++;
                return newNode;
            }

            public Node<Т> AddLast(Т value)
            {
                var newNode = new Node<Т>
                {
                    Value = value,
                    PrevNode = tail,
                    NextNode = null
                };

                if (tail != null)
                    tail.NextNode = newNode;
                else
                    head = newNode;

                tail = newNode;

                count++;
                return newNode;
            }

            public Node<Т> First() { return head; }
            public Node<Т> Last() { return tail; }
            public Node<Т> AddBefore(Node<Т> next, Т value)
            {
                if (next == null && head != null)
                    throw new ArgumentNullException("next is null");

                var newNode = new Node<Т>
                {
                    Value = value,
                    PrevNode = next != null ? next.PrevNode : null,
                    NextNode = next
                };
                if (head == null)
                {
                    head = tail = newNode;
                    return newNode;
                }
                if (next.PrevNode != null)
                    next.PrevNode.NextNode = newNode;
                else
                    head = newNode;

                next.PrevNode = newNode;
                count++;
                return newNode;

            }

            public Node<Т> AddAfter(Node<Т> prev, Т value)
            {
                if (prev == null && tail != null)
                    throw new ArgumentNullException("before is null");

                var newNode = new Node<Т>
                {
                    Value = value,
                    PrevNode = prev,
                    NextNode = prev?.NextNode
                };

                if (tail == null)
                {
                    head = tail = null;
                    return newNode;
                }
                if (prev.NextNode != null)
                    prev.NextNode.PrevNode = newNode;
                else
                    tail = newNode;

                prev.NextNode = newNode;

                count++;
                return newNode;

            }

            public void Remove(Node<Т> node)
            {
                if (node.NextNode != null)
                    node.NextNode.PrevNode = node.PrevNode;

                else
                    tail = node.PrevNode;

                if (node.PrevNode != null)
                    node.PrevNode.NextNode = node.NextNode;

                else
                    head = node.NextNode;

                count--;
            }

            public int Count()
            {
                return count;
            }

            public bool Contains(Т value)
            {
                var currentNode = head;
                while (currentNode != null)
                {
                    if (currentNode.Value.Equals(value))
                        return true;
                    currentNode = currentNode.NextNode;
                }
                return false;
            }
        }
    }

