using System;
using System.Collections.Generic;

#nullable enable

namespace KDTree
{

    /// <summary>
    /// Delegate for building a KD-Tree node.
    /// </summary>
    /// <typeparam name="T">The type of values in the points, must implement IComparable.</typeparam>
    /// <param name="node">The current KD-Tree node being built.</param>
    /// <param name="depth">The current depth of the node in the tree.</param>
    /// <param name="maxDepth">The maximum depth allowed for the tree.</param>
    /// <param name="minNodeItemCount">The minimum number of items required in a node.</param>
    public delegate void BuildKDTreeDelegate<T>(KDTreeNode<T> node, int depth, int maxDepth, int minNodeItemCount) where T : IComparable;

    /// <summary>
    /// A K-dimensional tree implementation for efficient nearest neighbor searches in multi-dimensional space.
    /// </summary>
    /// <typeparam name="T">The type of values in the points, must implement IComparable.</typeparam>
    public class KDTree<T> where T : IComparable
    {
        const int MAX_DEPTH = 10;
        const int MIN_NODE_ITEM_COUNT = 3;

        /// <summary>
        /// Gets the maximum depth allowed for the KD-Tree.
        /// </summary>
        public int MaxDepth { get; private set; }
        /// <summary>
        /// Gets the minimum number of items required in a node.
        /// </summary>
        public int MinNodeItemCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the KD-Tree with the specified points.
        /// </summary>
        /// <param name="points">A list of points where each point is represented as a list of coordinates.</param>
        /// <param name="maxDepth">The maximum depth allowed for the tree.</param>
        /// <param name="minNodeItemCount">The minimum number of items required in a node.</param>
        public KDTree(IList<IList<T>> points, int maxDepth = MAX_DEPTH, int minNodeItemCount = MIN_NODE_ITEM_COUNT)
        {
            this.MaxDepth = maxDepth;
            this.MinNodeItemCount = minNodeItemCount;
            var root = new KDTreeNode<T>
            {
                Data = points
            };
            BuildTree(root, 1);
        }

        private void BuildTree(KDTreeNode<T> node, int depth)
        {
            if (depth >= MaxDepth || node.Data.Count <= MinNodeItemCount)
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

