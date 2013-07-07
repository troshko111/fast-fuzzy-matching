using System.Collections.Generic;
using FFM.BKTree;

namespace FFM.SampleApp.Index
{
    public class BKIndex : IIndex<string>
    {
        private readonly BKTree<string> _tree = new BKTree<string>(new DamerauLevenshteinStringDistanceMeasurer());

        public void Add(string data)
        {
            _tree.Add(new BKTreeNode<string>(data));
        }

        public List<Match<string>> Matches(string query, int maxDistance)
        {
            return _tree.Matches(query, maxDistance);
        }
    }
}