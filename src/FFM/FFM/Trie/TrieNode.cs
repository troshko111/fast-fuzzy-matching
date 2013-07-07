using System.Collections.Generic;

namespace FFM.Trie
{
    internal class TrieNode
    {
        public readonly char Letter;

        private readonly SortedList<char, TrieNode> _children;
        public IEnumerable<TrieNode> Children
        {
            get
            {
                return _children.Values;
            }
        }

        internal string Word;

        public TrieNode(char letter)
        {
            Letter = letter;
            _children = new SortedList<char, TrieNode>();
        }

        public void AddChild(TrieNode child)
        {
            _children.Add(child.Letter, child);
        }

        public bool TryGetChildWith(char character, out TrieNode child)
        {
            return _children.TryGetValue(character, out child);
        }

        public override string ToString()
        {
            return string.Format("Letter: {0}, Word: {1}", Letter, Word ?? "NULL");
        }
    }
}