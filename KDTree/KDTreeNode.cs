using System;
using System.Collections.Generic;

namespace KDTree
{
    /// <summary>
    /// A K-dimensional tree node that contains a list of points and references to its left and right children.
    /// </summary>
    /// <typeparam name="T">The type of values in the points, must implement IComparable.</typeparam>
    public class KDTreeNode<T> where T : IComparable
    {
        /// <summary>
        /// Gets or sets the list of points at this node.
        /// </summary>
        public IList<IList<T>> Data { get; set; } = [];

        /// <summary>
        /// Gets or sets the left child of this node.
        /// </summary>
        public KDTreeNode<T> Left { get; set; }

        /// <summary>
        /// Gets or sets the right child of this node.
        /// </summary>
        public KDTreeNode<T> Right { get; set; }

        /// <summary>
        /// Gets a value indicating whether this node is a leaf (has no children).
        /// </summary>
        public bool IsLeaf => Left == null && Right == null;
    }
}