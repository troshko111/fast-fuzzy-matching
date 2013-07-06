using System;
using System.Collections.Generic;

namespace FFM
{
    public class BKTreeNode<T>
    {
        public readonly T Data;

        private int _distance;
        public int DistanceToParentNode
        {
            get
            {
                return _distance;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Distance cannot be negative");
                _distance = value;
            }
        }

        private readonly Dictionary<int, BKTreeNode<T>> _children;
        public BKTreeNode(T data)
        {
            Data = data;
            _children = new Dictionary<int, BKTreeNode<T>>();
        }

        public void AddChild(BKTreeNode<T> child)
        {
            _children.Add(child.DistanceToParentNode, child);
        }

        public bool TryGetChildWith(int distance, out BKTreeNode<T> child)
        {
            return _children.TryGetValue(distance, out child);
        }

        public override string ToString()
        {
            return string.Format("Data: {0}, DistanceToParentNode: {1}", Data, DistanceToParentNode);
        }
    }
}