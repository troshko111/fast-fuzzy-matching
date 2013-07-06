using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FFM.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void add()
        {
            var tree = new BKTree(new DamerauLevenshteinDistance());

            tree.Add(new BKTreeNode("book"));//root
            tree.Add(new BKTreeNode("rook"));//1
            tree.Add(new BKTreeNode("nooks"));//2
            tree.Add(new BKTreeNode("boon"));//1-2

            var matches = tree.Matches("boon", 3);
            foreach (var match in matches)
            {
                Debug.WriteLine(match);
            }
        }
    }
}
