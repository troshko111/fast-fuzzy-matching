using System;
using System.Collections.Generic;

namespace FFM.BKTree
{
    public class BKTree<T>
    { 
        public BKTreeNode<T> Root { get; private set; }

        private readonly IDistanceMeasurer<T> _distanceMeasurer;

        public BKTree(IDistanceMeasurer<T> distanceMeasurer)
        {
            if (distanceMeasurer == null)
                throw new ArgumentNullException("distanceMeasurer");
            _distanceMeasurer = distanceMeasurer;
        }

        public void Add(BKTreeNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (Root == null)
            {
                Root = node;
                return;
            }

            AddTo(Root, node);
        }

        private void AddTo(BKTreeNode<T> subTree, BKTreeNode<T> node)
        {
            var distance = _distanceMeasurer.Measure(subTree.Data, node.Data);

            BKTreeNode<T> child;
            if (!subTree.TryGetChildWith(distance, out child))
            {
                node.DistanceToParentNode = distance;
                subTree.AddChild(node);
                return;
            }

            AddTo(child, node);
        }

        public List<Match<T>> Matches(T query, int maxDistance)
        {
            var results = new List<Match<T>>();
            Matches(Root, query, maxDistance, results);
            return results;
        }

        private void Matches(BKTreeNode<T> subTree, T query, int maxDistance, ICollection<Match<T>> matches)
        {
            var distance = _distanceMeasurer.Measure(subTree.Data, query);

            if (distance <= maxDistance)
                matches.Add(new Match<T>(subTree.Data, distance));

            var lowerDistance = distance - maxDistance > 0 ? distance - maxDistance : 0;
            var upperDistance = distance + maxDistance;

            for (var i = lowerDistance; i <= upperDistance; i++)
            {
                BKTreeNode<T> child;
                if (subTree.TryGetChildWith(i, out child))
                    Matches(child, query, maxDistance, matches);
            }
        }
    }
}