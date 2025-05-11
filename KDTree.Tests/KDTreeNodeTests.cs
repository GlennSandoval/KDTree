using System;
using System.Collections.Generic;
using Xunit;

namespace KDTree.Tests
{
    public class KDTreeNodeTests
    {
        [Fact]
        public void Constructor_CreatesEmptyDataList()
        {
            // Act
            var node = new KDTreeNode<double>();

            // Assert
            Assert.NotNull(node.Data);
            Assert.Empty(node.Data);
        }

        [Fact]
        public void IsLeaf_WithNoChildren_ReturnsTrue()
        {
            // Arrange
            var node = new KDTreeNode<double>
            {
                Data =
                [
                    [1, 2]
                ]
            };

            // Act & Assert
            Assert.True(node.IsLeaf);
        }

        [Fact]
        public void IsLeaf_WithLeftChild_ReturnsFalse()
        {
            // Arrange
            var node = new KDTreeNode<double>
            {
                Data =
                [
                    [1, 2]
                ],
                Left = new KDTreeNode<double>()
            };

            // Act & Assert
            Assert.False(node.IsLeaf);
        }

        [Fact]
        public void IsLeaf_WithRightChild_ReturnsFalse()
        {
            // Arrange
            var node = new KDTreeNode<double>
            {
                Data =
                [
                    [1, 2]
                ],
                Right = new KDTreeNode<double>()
            };

            // Act & Assert
            Assert.False(node.IsLeaf);
        }

        [Fact]
        public void IsLeaf_WithBothChildren_ReturnsFalse()
        {
            // Arrange
            var node = new KDTreeNode<double>
            {
                Data =
                [
                    [1, 2]
                ],
                Left = new KDTreeNode<double>(),
                Right = new KDTreeNode<double>()
            };

            // Act & Assert
            Assert.False(node.IsLeaf);
        }

        [Fact]
        public void Data_CanAddPoints()
        {
            // Arrange
            var node = new KDTreeNode<double>();
            var point1 = new List<double> { 1, 2 };
            var point2 = new List<double> { 3, 4 };

            // Act
            node.Data.Add(point1);
            node.Data.Add(point2);

            // Assert
            Assert.Equal(2, node.Data.Count);
            Assert.Same(point1, node.Data[0]);
            Assert.Same(point2, node.Data[1]);
        }
    }
}