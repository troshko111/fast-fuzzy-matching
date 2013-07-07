using System.Collections.Generic;

namespace FFM.SampleApp.Index
{
    public class TrieIndex : IIndex<string>
    {
        private readonly Trie.Trie _trie = new Trie.Trie();

        public void Add(string data)
        {
            _trie.Add(data);
        }

        public List<Match<string>> Matches(string query, int maxDistance)
        {
            return _trie.Matches(query, maxDistance);
        }
    }
}