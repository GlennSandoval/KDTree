using Medians;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// My learning project for KD Trees

namespace KDTree
{

    public class Tree<T> where T : IComparable
    {

        private Node<T> _root;
        private const int MAXDEPTH = 10;
        private const int MIN_NODE_ITEM_COUNT = 3;

        public Tree(IList<IList<T>> points)
        {
            _root = new Node<T>();
            _root.Data = points;
            BuildTree(_root, 1);
        }

        private void BuildTree(Node<T> node, int depth)
        {
            if (depth >= MAXDEPTH || node.Data.Count <= MIN_NODE_ITEM_COUNT)
            {
                return;
            }

            var data = new List<IList<T>>(node.Data);
            var index = depth % data[0].Count; // data[0].Count is K

            var median = Median.FindMedian<IList<T>>(data, (left, right) =>
            {
                var leftValue = left[index];
                var rightValue = right[index];
                return leftValue.CompareTo(rightValue);
            });

            var splitPos = data.IndexOf(median);
            var leftNode = new Node<T>();
            node.Left = leftNode;

            var rightNode = new Node<T>();
            node.Right = rightNode;

            foreach (var point in data)
            {
                if (point[index].CompareTo(median[index]) > 0)
                {
                    leftNode.Data.Add(point);
                }
                else
                {
                    rightNode.Data.Add(point);
                }
            }

            BuildTree(leftNode, depth + 1);
            BuildTree(rightNode, depth + 1);

        }


    }

    class Node<T> where T : IComparable
    {
        public Node<T> Left;
        public Node<T> Right;
        public IList<IList<T>> Data = new List<IList<T>>();
    }

}
