using System;
using System.Collections.Generic;
using System.Linq;

namespace FFM
{
    public class BKTree
    {
        public BKTreeNode Root;

        private readonly IDistance _distance;

        public BKTree(IDistance distance)
        {
            _distance = distance;
        }

        public void Add(BKTreeNode node)
        {
            if (Root == null)
            {
                Root = node;
                return;
            }

            AddChild(Root, node);
        }

        private void AddChild(BKTreeNode node, BKTreeNode newNode)
        {
            var d = _distance.Calculate(node.Data, newNode.Data);
            if (node.Children.All(child => child.DistanceToParent != d))
            {
                newNode.DistanceToParent = d;
                node.Children.Add(newNode);
                return;
            }

            var corresponding = node.Children.Single(c => c.DistanceToParent == d);
            AddChild(corresponding, newNode);
        }

        public List<string> Matches(string query, int maxDistance)
        {
            var results = new List<string>();
            Matches(Root, query, maxDistance, results);
            return results;
        }

        private void Matches(BKTreeNode node, string query, int maxDistance, List<string> matches)
        {
            var d = _distance.Calculate(node.Data, query);
            if (d <= maxDistance)
                matches.Add(node.Data);

            var start = d - maxDistance > 0 ? d - maxDistance : 0;
            var end = d + maxDistance;
            for (var i = start; i <= end; i++)
            {
                var child = node.Children.SingleOrDefault(c => c.DistanceToParent == i);
                if (child != null)
                    Matches(child, query, maxDistance, matches);
            }
        }
    }

    public class BKTreeNode
    {
        public string Data;
        public int DistanceToParent;
        public List<BKTreeNode> Children;

        public BKTreeNode(string data)
        {
            Data = data;
            Children = new List<BKTreeNode>();
        }
    }
}