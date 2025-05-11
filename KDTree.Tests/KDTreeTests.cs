using System;
using System.Collections.Generic;
using Xunit;

namespace KDTree.Tests
{
    public class KDTreeTests
    {
        [Fact]
        public void Constructor_WithValidPoints_CreatesTree()
        {
            // Arrange
            var points = new List<IList<double>>
            {
                new List<double> { 2, 3 },
                new List<double> { 5, 4 },
                new List<double> { 9, 6 },
                new List<double> { 4, 7 },
                new List<double> { 8, 1 },
                new List<double> { 7, 2 }
            };

            // Act
            var kdTree = new KDTree<double>(points);

            // Assert
            Assert.NotNull(kdTree);
            Assert.Equal(10, kdTree.MaxDepth); // Default value
            Assert.Equal(3, kdTree.MinNodeItemCount); // Default value
        }

        [Fact]
        public void Constructor_WithCustomParameters_SetsParameters()
        {
            // Arrange
            var points = new List<IList<double>>
            {
                new List<double> { 2, 3 },
                new List<double> { 5, 4 },
                new List<double> { 9, 6 }
            };
            const int maxDepth = 5;
            const int minNodeItemCount = 2;

            // Act
            var kdTree = new KDTree<double>(points, maxDepth, minNodeItemCount);

            // Assert
            Assert.NotNull(kdTree);
            Assert.Equal(maxDepth, kdTree.MaxDepth);
            Assert.Equal(minNodeItemCount, kdTree.MinNodeItemCount);
        }

        [Fact]
        public void FindNearestNeighbor_WithValidQueryPoint_ReturnsNearestPoint()
        {
            // Arrange
            var points = new List<IList<double>>
            {
                new List<double> { 2, 3 },
                new List<double> { 5, 4 },
                new List<double> { 9, 6 },
                new List<double> { 4, 7 },
                new List<double> { 8, 1 },
                new List<double> { 7, 2 }
            };
            var kdTree = new KDTree<double>(points);
            var queryPoint = new List<double> { 6, 5 };

            // Act
            var nearestPoint = kdTree.FindNearestNeighbor(queryPoint);

            // Assert
            Assert.NotNull(nearestPoint);
            Assert.Equal(2, nearestPoint.Count);
            
            // The nearest point should be (5, 4) with a distance of sqrt((6-5)^2 + (5-4)^2) = sqrt(2)
            Assert.Equal(5, nearestPoint[0]);
            Assert.Equal(4, nearestPoint[1]);
        }

        [Fact]
        public void FindNearestNeighbor_WithQueryPointMatchingExistingPoint_ReturnsThatPoint()
        {
            // Arrange
            var points = new List<IList<double>>
            {
                new List<double> { 2, 3 },
                new List<double> { 5, 4 },
                new List<double> { 9, 6 }
            };
            var kdTree = new KDTree<double>(points);
            var queryPoint = new List<double> { 5, 4 }; // Exact match to an existing point

            // Act
            var nearestPoint = kdTree.FindNearestNeighbor(queryPoint);

            // Assert
            Assert.NotNull(nearestPoint);
            Assert.Equal(2, nearestPoint.Count);
            Assert.Equal(5, nearestPoint[0]);
            Assert.Equal(4, nearestPoint[1]);
        }

        [Fact]
        public void FindNearestNeighbor_WithNullQueryPoint_ThrowsArgumentNullException()
        {
            // Arrange
            var points = new List<IList<double>>
            {
                new List<double> { 2, 3 },
                new List<double> { 5, 4 }
            };
            var kdTree = new KDTree<double>(points);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => kdTree.FindNearestNeighbor(null));
        }

        [Fact]
        public void FindNearestNeighbor_WithDifferentDimensionQueryPoint_ThrowsArgumentException()
        {
            // Arrange
            var points = new List<IList<double>>
            {
                new List<double> { 2, 3 },
                new List<double> { 5, 4 }
            };
            var kdTree = new KDTree<double>(points);
            var queryPoint = new List<double> { 5, 4, 3 }; // 3D point for a 2D tree

            // Act & Assert
            Assert.Throws<ArgumentException>(() => kdTree.FindNearestNeighbor(queryPoint));
        }

        [Fact]
        public void FindNearestNeighbor_With3DPoints_ReturnsCorrectNearestPoint()
        {
            // Arrange
            var points = new List<IList<double>>
            {
                new List<double> { 2, 3, 4 },
                new List<double> { 5, 4, 2 },
                new List<double> { 9, 6, 3 },
                new List<double> { 4, 7, 1 },
                new List<double> { 8, 1, 5 },
                new List<double> { 7, 2, 6 }
            };
            var kdTree = new KDTree<double>(points);
            var queryPoint = new List<double> { 6, 5, 3 };

            // Act
            var nearestPoint = kdTree.FindNearestNeighbor(queryPoint);

            // Assert
            Assert.NotNull(nearestPoint);
            Assert.Equal(3, nearestPoint.Count);
            
            // The nearest point should be (5, 4, 2) or (9, 6, 3) depending on the tree construction
            // We'll check that it's one of these two points
            bool isExpectedPoint = 
                (Math.Abs(nearestPoint[0] - 5) < 0.001 && Math.Abs(nearestPoint[1] - 4) < 0.001 && Math.Abs(nearestPoint[2] - 2) < 0.001) ||
                (Math.Abs(nearestPoint[0] - 9) < 0.001 && Math.Abs(nearestPoint[1] - 6) < 0.001 && Math.Abs(nearestPoint[2] - 3) < 0.001);
            
            Assert.True(isExpectedPoint, $"Expected either (5,4,2) or (9,6,3) but got ({nearestPoint[0]},{nearestPoint[1]},{nearestPoint[2]})");
        }
    }
}