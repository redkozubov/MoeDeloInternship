using System.Collections.Generic;

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
            {
                Value = default(T);
                NextNode = null;
                PreviousNode = null;
            }

            public Node(T val)
            {
                Value = val;
                NextNode = null;
                PreviousNode = null;
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
                    throw new CurrentElementNonExistent("Попытка присвоить значение несущещствующему элементу");
            }
        }

        public void AddToHead(T val)
        {
            Node newNode = new Node(val, null, firstNode);
            firstNode.PreviousNode = newNode;
            firstNode = newNode;
        }

        public void Add(T val)
        {
            Node newNode = new Node(val, lastNode, null);
            lastNode.NextNode = newNode;
            lastNode = newNode;
        }

        public T GetNext()
        {
            currentNode = currentNode.NextNode;
            return CurrentElement;
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
            foreach (T val in values)
            {
                if (firstNode == null)
                {
                    firstNode = new Node(val);
                    lastNode = firstNode;
                }
                else
                {
                    Node nextNode = new Node(val, lastNode, null);
                    lastNode.NextNode = nextNode;
                    lastNode = nextNode;
                }
            }
            GoToHead();
        }
    }
}