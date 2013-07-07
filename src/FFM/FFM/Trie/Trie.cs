using System;
using System.Collections.Generic;
using System.Linq;

namespace FFM.Trie
{
    //Based on http://stevehanov.ca/blog/index.php?id=114 
    //http://en.wikipedia.org/wiki/Trie
    public class Trie
    {
        internal TrieNode Root;

        public Trie()
        {
            Root = new TrieNode('0');
        }

        public void Add(string word)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word");

            var current = Root;
            foreach (var letter in word)
            {
                TrieNode child;
                if (!current.TryGetChildWith(letter, out child))
                    current.AddChild(child = new TrieNode(letter));

                current = child;
            }

            current.Word = word;
        }

        public List<Match<string>> Matches(string word, int maxDistance)
        {
            var currentRow = new int[word.Length + 1];
            for (var i = 0; i < currentRow.Length; i++)
                currentRow[i] = i;

            var results = new List<Match<string>>();

            foreach (var child in Root.Children)
                Matches(child, child.Letter, word, currentRow, results, maxDistance);

            return results;
        }

        private void Matches(TrieNode node,
                             char letter,
                             string word,
                             int[] previousRow,
                             List<Match<string>> results,
                             int maxDistance)
        {
            var columns = word.Length + 1;
            var currentRow = new List<int> { previousRow[0] + 1 };

            foreach (var column in Enumerable.Range(1, columns - 1))
            {
                var insertCost = currentRow[column - 1] + 1;
                var deleteCost = previousRow[column] + 1;
                int replaceCost;

                if (word[column - 1] != letter)
                    replaceCost = previousRow[column - 1] + 1;
                else
                    replaceCost = previousRow[column - 1];

                currentRow.Add(Math.Min(Math.Min(insertCost, deleteCost), replaceCost)); //min of 3
            }

            if (currentRow[currentRow.Count - 1] <= maxDistance && !string.IsNullOrEmpty(node.Word))
                results.Add(new Match<string>(node.Word, currentRow[currentRow.Count - 1]));

            if (currentRow.Min() <= maxDistance)
            {
                foreach (var child in node.Children)
                    Matches(child, child.Letter, word, currentRow.ToArray(), results, maxDistance);
            }
        }
    }
}