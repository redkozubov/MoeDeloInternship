using System;

namespace generics_task
{
    public class ValueLinkedList <T> where T: struct
    {
        private Node<T> headNode;
        private Node<T> currentNode;
        public void addToHead(T value)
        {
            if (headNode!=null)
            {
                headNode = new Node<T>(value,headNode);
                if (currentNode == headNode.nextNode)
                {
                    currentNode = headNode;
                }
                return;
            }
            headNode = new Node<T>(value);
            currentNode = headNode;
        }
        public void moveToNext()
        {
            if (currentNode.nextNode != null)
            {
                currentNode = currentNode.nextNode;
                return;
            }
            throw new ListRangeException("Достигнут конец списка");
        }
        public void moveToPrevious()
        {
            if (currentNode.previousNode != null)
            {
                currentNode = currentNode.previousNode;
                return;
            }
            throw new ListRangeException("Достигнуто начало списка");
        }
        public T peekCurrent()
        {
            if (currentNode != null)
            {
                return currentNode.value;
            }
            throw new EmptyListException("Список пустой");
        }
        public void moveToHead()
        {
            if (headNode != null)
            {
                currentNode = headNode;
                return;
            }
            throw new EmptyListException("Список пустой");
        }
        public ValueLinkedList()
        {
            headNode = null;
            currentNode = null;
        }
        private class Node <V> where V: struct
        {
            public V value;
            public Node<V> nextNode;
            public Node<V> previousNode;
            public Node(V val)
            {
                value = val;
                nextNode = null;
                previousNode = null;
            }
            public Node(V val, Node<V> nextnode)
            {
                value = val;
                nextNode = nextnode;
                nextnode.previousNode = this;
                previousNode = null;
            }
        }
    } 
}