using System;
using System.Collections.Generic;

#nullable enable

namespace KDTree
{
    /// <summary>
    /// A K-dimensional tree implementation for efficient nearest neighbor searches in multi-dimensional space.
    /// </summary>
    /// <typeparam name="T">The type of values in the points, must implement IComparable.</typeparam>
    public class KDTree<T> where T : IComparable, IConvertible
    {
        const int MAX_DEPTH = 10;
        const int MIN_NODE_ITEM_COUNT = 3;

        /// <summary>
        /// Gets the root node of the KD-Tree.
        /// </summary>
        private KDTreeNode<T> Root { get; set; }

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
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            
            if (points.Count == 0)
                throw new ArgumentException("Points list cannot be empty.", nameof(points));
            
            this.MaxDepth = maxDepth;
            this.MinNodeItemCount = minNodeItemCount;
            Root = new KDTreeNode<T>
            {
                Data = points
            };
            BuildTree(Root, 1);
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

        /// <summary>
        /// Finds the nearest neighbor to the specified query point based on Euclidean distance.
        /// </summary>
        /// <param name="queryPoint">The point to find the nearest neighbor for.</param>
        /// <returns>The nearest neighbor point to the query point.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the query point is null.</exception>
        public IList<T> FindNearestNeighbor(IList<T> queryPoint)
        {
            ArgumentNullException.ThrowIfNull(queryPoint);

            if (Root.Data[0].Count != queryPoint.Count)
                throw new ArgumentException($"Query point has {queryPoint.Count} dimensions, but tree points have {Root.Data[0].Count} dimensions.");

            var bestDistance = double.MaxValue;
            IList<T> bestPoint = Root.Data[0];

            SearchNearestNeighbor(Root, queryPoint, 1, ref bestDistance, ref bestPoint);

            return bestPoint;
        }

        private void SearchNearestNeighbor(KDTreeNode<T> node, IList<T> queryPoint, int depth, ref double bestDistance, ref IList<T> bestPoint)
        {
            if (node == null || node.Data.Count == 0)
                return;

            // Check all points in this node
            foreach (var point in node.Data)
            {
                var distance = CalculateDistance(point, queryPoint);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = point;
                }
            }

            if (node.IsLeaf)
                return;

            // Select axis based on depth
            var axis = depth % queryPoint.Count;

            // Determine which child to search first
            KDTreeNode<T> firstChild;
            KDTreeNode<T> secondChild;

            // If the query point's value is less than the median, search left first
            if (node.Data.Count > 0)
            {
                var median = node.Data[(int)Math.Floor(node.Data.Count / 2.0)];
                
                if (queryPoint[axis].CompareTo(median[axis]) < 0)
                {
                    firstChild = node.Left;
                    secondChild = node.Right;
                }
                else
                {
                    firstChild = node.Right;
                    secondChild = node.Left;
                }

                // Search the first child
                SearchNearestNeighbor(firstChild, queryPoint, depth + 1, ref bestDistance, ref bestPoint);

                // Check if we need to search the second child
                // Calculate the distance from the query point to the splitting plane
                var axisDistance = Math.Abs(queryPoint[axis].ToDouble(null) - median[axis].ToDouble(null));

                // If the distance to the splitting plane is less than the best distance,
                // we need to check the other side as well
                if (axisDistance < bestDistance)
                {
                    SearchNearestNeighbor(secondChild, queryPoint, depth + 1, ref bestDistance, ref bestPoint);
                }
            }
        }

        private double CalculateDistance(IList<T> point1, IList<T> point2)
        {
            double sum = 0;
            for (int i = 0; i < point1.Count; i++)
            {
                var diff = point1[i].ToDouble(null) - point2[i].ToDouble(null);
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }

    }
}

