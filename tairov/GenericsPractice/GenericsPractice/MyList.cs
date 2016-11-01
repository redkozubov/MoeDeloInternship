using System.Collections.Generic;
using System.Linq;

namespace GenericsPractice
{
    class MyList<T> where T : struct
    {
        private class Node
        {
            public T Value { get; set; }
            public Node NextNode { get; set; }
            public Node PreviousNode { get; set; }

            public Node()
                : this(default(T), null , null)
            {
            }

            public Node(T val) : this (val, null, null)
            {
            }

            public Node(T val, Node pNode, Node nNode)
            {
                Value = val;
                NextNode = nNode;
                PreviousNode = pNode;
            }
        }

        private Node firstNode;
        private Node lastNode;
        private Node currentNode;

        public T CurrentElement
        {
            get
            {
                if (currentNode != null)
                    return currentNode.Value;
                else
                    throw new NoMoreElementsException("В спике больше нет элементов");
            }
            set
            {
                if (currentNode != null)
                    currentNode.Value = value;
                else
                    throw new CurrentElementNotExistExcpetion("Попытка присвоить значение несущещствующему элементу");
            }
        }

        public void AddToHead(T val)
        {
            Node newNode = new Node(val, null, firstNode);
            if (firstNode != null)
                firstNode.PreviousNode = newNode;
            firstNode = newNode;
        }

        public void Add(T val)
        {
            Node newNode = new Node(val, lastNode, null);
            if (lastNode != null)
                lastNode.NextNode = newNode;
            lastNode = newNode;
        }

        public T GetNext()
        {
            if (currentNode == null)
                throw new CurrentElementNotExistExcpetion();
            currentNode = currentNode.NextNode;
            return CurrentElement;
        }

        public bool IsEmpty()
        {
            return firstNode == null;
        }

        public void GoToHead()
        {
            currentNode = firstNode;
        }

        public MyList()
        {
            firstNode = null;
            lastNode = null;
            currentNode = null;
        }

        public MyList(T val)
        {
            firstNode = new Node(val);
            lastNode = firstNode;
            currentNode = firstNode;
        }

        public MyList(IEnumerable<T> values)
        {
            var enumerable = values as T[] ?? values.ToArray();
            if (enumerable.Count() == 0)
            {
                firstNode = null;
                lastNode = null;
                currentNode = null;
                return;
            }
            firstNode = new Node(enumerable.First());
            lastNode = firstNode;
            currentNode = firstNode;
            foreach (T val in enumerable.Skip(1))
            {
                Node nextNode = new Node(val, lastNode, null);
                lastNode.NextNode = nextNode;
                lastNode = nextNode;
            }
        }
    }
}