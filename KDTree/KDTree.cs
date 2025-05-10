using System;
using System.Collections.Generic;

namespace KDTree
{
    /// <summary>
    /// A K-dimensional tree implementation for efficient nearest neighbor searches in multi-dimensional space.
    /// </summary>
    /// <typeparam name="T">The type of values in the points, must implement IComparable.</typeparam>
    public class KDTree<T> where T : IComparable
    {
        private const int MAX_DEPTH = 10;
        private const int MIN_NODE_ITEM_COUNT = 3;

        /// <summary>
        /// Initializes a new instance of the KD-Tree with the specified points.
        /// </summary>
        /// <param name="points">A list of points where each point is represented as a list of coordinates.</param>
        public KDTree(IList<IList<T>> points)
        {
            var root = new KDTreeNode<T>
            {
                Data = points
            };
            BuildTree(root, 1);
        }

        private void BuildTree(KDTreeNode<T> node, int depth)
        {
            if (depth >= MAX_DEPTH || node.Data.Count <= MIN_NODE_ITEM_COUNT)
            {
                return;
            }

            var pointList = new List<IList<T>>(node.Data);
            // Select axis based on depth so that axis cycles through all valid values
            var axis = depth % pointList[0].Count;

            // Sort point list and choose median as pivot element
            pointList.Sort((left, right) =>
            {
                var leftValue = left[axis];
                var rightValue = right[axis];
                return leftValue.CompareTo(rightValue);
            });
            var median = pointList[(int)Math.Floor(pointList.Count / 2.0)];

            var leftChild = new KDTreeNode<T>();
            node.Left = leftChild;

            var rightChild = new KDTreeNode<T>();
            node.Right = rightChild;

            foreach (var point in pointList)
            {
                if (point[axis].CompareTo(median[axis]) > 0)
                {
                    leftChild.Data.Add(point);
                }
                else
                {
                    rightChild.Data.Add(point);
                }
            }

            BuildTree(leftChild, depth + 1);
            BuildTree(rightChild, depth + 1);
        }
    }
}
