using System;
using System.Collections.Generic;

// My learning project for KD Trees

namespace KDTree
{
    /**
     * https://en.wikipedia.org/wiki/K-d_tree
     */
    /// <summary>
    /// A K-dimensional tree implementation for efficient nearest neighbor searches in multi-dimensional space.
    /// </summary>
    /// <typeparam name="T">The type of values in the points, must implement IComparable.</typeparam>
    public class Tree<T> where T : IComparable
    {
        private const int MAX_DEPTH = 10;
        private const int MIN_NODE_ITEM_COUNT = 3;

        /// <summary>
        /// Initializes a new instance of the KD-Tree with the specified points.
        /// </summary>
        /// <param name="points">A list of points where each point is represented as a list of coordinates.</param>
        public Tree(IList<IList<T>> points)
        {
            var root = new Node<T>
            {
                Data = points
            };
            BuildTree(root, 1);
        }

        private void BuildTree(Node<T> node, int depth)
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

            var leftChild = new Node<T>();
            node.Left = leftChild;

            var rightChild = new Node<T>();
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

    /// <summary>
    /// Represents a node in the KD-Tree.
    /// </summary>
    /// <typeparam name="T">The type of values in the points, must implement IComparable.</typeparam>
    class Node<T> where T : IComparable
    {
        /// <summary>
        /// The left child node.
        /// </summary>
        public Node<T> Left;
        
        /// <summary>
        /// The right child node.
        /// </summary>
        public Node<T> Right;
        
        /// <summary>
        /// The data points contained in this node.
        /// </summary>
        public IList<IList<T>> Data = new List<IList<T>>();
    }
}