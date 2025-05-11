using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace KDTree.Tests
{
    public class KDTreeEdgeCasesTests(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;

        /// <summary>
        /// Calculates the Euclidean distance between two points.
        /// </summary>
        private static double CalculateDistance<T>(IList<T> point1, IList<T> point2) where T : IConvertible
        {
            double distance = 0;
            for (int i = 0; i < point1.Count; i++)
            {
                var diff = point1[i].ToDouble(null) - point2[i].ToDouble(null);
                distance += diff * diff;
            }
            return Math.Sqrt(distance);
        }

        /// <summary>
        /// Finds the nearest neighbor to a query point using brute force approach.
        /// </summary>
        private static IList<T> FindNearestNeighborBruteForce<T>(IList<IList<T>> points, IList<T> queryPoint) where T : IConvertible
        {
            var minDistance = double.MaxValue;
            IList<T> nearest = null;

            foreach (var point in points)
            {
                double distance = CalculateDistance(point, queryPoint);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = point;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Generates a list of random points with the specified dimensions.
        /// </summary>
        private static List<IList<T>> GenerateRandomPoints<T>(int numPoints, int dimensions, Random random, Func<Random, T> valueGenerator)
        {
            var points = new List<IList<T>>();

            for (int i = 0; i < numPoints; i++)
            {
                var point = new List<T>();
                for (int j = 0; j < dimensions; j++)
                {
                    point.Add(valueGenerator(random));
                }
                points.Add(point);
            }

            return points;
        }

        [Fact]
        public void Constructor_WithEmptyPointsList_ThrowsArgumentException()
        {
            // Arrange
            var points = new List<IList<double>>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new KDTree<double>(points));
            Assert.Equal("Points list cannot be empty. (Parameter 'points')", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullPointsList_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new KDTree<double>(null));
            Assert.Equal("Value cannot be null. (Parameter 'points')", exception.Message);
        }

        [Fact]
        public void Constructor_WithSinglePoint_CreatesValidTree()
        {
            // Arrange
            var points = new List<IList<double>>
            {
                new List<double> { 1, 2 }
            };

            // Act
            var kdTree = new KDTree<double>(points);

            // Assert
            Assert.NotNull(kdTree);

            // Test FindNearestNeighbor with the same point
            var nearestPoint = kdTree.FindNearestNeighbor(new List<double> { 1, 2 });
            Assert.Equal(1, nearestPoint[0]);
            Assert.Equal(2, nearestPoint[1]);

            // Test FindNearestNeighbor with a different point
            nearestPoint = kdTree.FindNearestNeighbor(new List<double> { 3, 4 });
            Assert.Equal(1, nearestPoint[0]);
            Assert.Equal(2, nearestPoint[1]);
        }

        [Fact]
        public void FindNearestNeighbor_WithIdenticalPoints_ReturnsAnyOfThosePoints()
        {
            // Arrange
            var duplicatePoint = new List<double> { 1, 2 };
            var points = new List<IList<double>>
            {
                duplicatePoint,
                new List<double> { 1, 2 }, // Duplicate point
                new List<double> { 1, 2 }  // Duplicate point
            };
            var kdTree = new KDTree<double>(points);

            // Act
            var nearestPoint = kdTree.FindNearestNeighbor(new List<double> { 1, 2 });

            // Assert
            Assert.Equal(1, nearestPoint[0]);
            Assert.Equal(2, nearestPoint[1]);

            // Verify it's one of the points from our list
            Assert.Contains(points, p =>
                p.Count == nearestPoint.Count &&
                p[0].Equals(nearestPoint[0]) &&
                p[1].Equals(nearestPoint[1]));
        }

        [Fact]
        public void FindNearestNeighbor_WithNullQueryPoint_ThrowsArgumentNullException()
        {
            // Arrange
            var points = new List<IList<double>> { new List<double> { 1, 2 } };
            var kdTree = new KDTree<double>(points);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => kdTree.FindNearestNeighbor(null));
            Assert.Contains("Value cannot be null", exception.Message);
        }

        [Fact]
        public void FindNearestNeighbor_WithDimensionMismatch_ThrowsArgumentException()
        {
            // Arrange
            var points = new List<IList<double>> { new List<double> { 1, 2 } }; // 2D points
            var kdTree = new KDTree<double>(points);
            var queryPoint = new List<double> { 1, 2, 3 }; // 3D point

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => kdTree.FindNearestNeighbor(queryPoint));
            Assert.Contains("Query point has 3 dimensions, but tree points have 2 dimensions", exception.Message);
        }

        [Fact]
        public void FindNearestNeighbor_WithLargeNumberOfPoints_CompletesInReasonableTime()
        {
            // Arrange
            var random = new Random(1337); // Fixed seed for reproducibility
            const int numPoints = 10000;
            const int dimensions = 5;

            var points = GenerateRandomPoints<double>(
                numPoints,
                dimensions,
                random,
                r => r.NextDouble() * 100);

            var stopwatch = Stopwatch.StartNew();
            var kdTree = new KDTree<double>(points);
            stopwatch.Stop();
            _output.WriteLine($"Tree construction time for {numPoints} points: {stopwatch.ElapsedMilliseconds}ms");

            // Create a random query point
            var queryPoint = new List<double>();
            for (int j = 0; j < dimensions; j++)
            {
                queryPoint.Add(random.NextDouble() * 100);
            }

            // Act
            stopwatch.Restart();
            var nearestPoint = kdTree.FindNearestNeighbor(queryPoint);
            stopwatch.Stop();

            // Assert
            Assert.NotNull(nearestPoint);
            _output.WriteLine($"Nearest neighbor search time: {stopwatch.ElapsedMilliseconds}ms");

            // Verify the result by comparing with a brute force approach
            stopwatch.Restart();
            IList<double> bruteForceNearest = FindNearestNeighborBruteForce(points, queryPoint);
            stopwatch.Stop();
            _output.WriteLine($"Brute force search time: {stopwatch.ElapsedMilliseconds}ms");

            // Calculate the distance between the KD-Tree result and the brute force result
            double resultDistance = CalculateDistance(nearestPoint, bruteForceNearest);

            // The KD-Tree result should be the same as the brute force result or very close
            Assert.True(resultDistance < 1e-10, $"KD-Tree result differs from brute force result by {resultDistance}");
        }

        [Fact]
        public void FindNearestNeighbor_WithIntegerPoints_ReturnsCorrectNeighbor()
        {
            // Arrange
            var points = new List<IList<int>>
            {
                new List<int> { 2, 3 },
                new List<int> { 5, 4 },
                new List<int> { 9, 6 },
                new List<int> { 4, 7 },
                new List<int> { 8, 1 },
                new List<int> { 7, 2 }
            };
            var kdTree = new KDTree<int>(points);
            var queryPoint = new List<int> { 6, 5 };

            // Act
            var nearestPoint = kdTree.FindNearestNeighbor(queryPoint);

            // Assert
            Assert.NotNull(nearestPoint);
            Assert.Equal(2, nearestPoint.Count);

            // Find expected nearest point using brute force
            var expectedNearest = FindNearestNeighborBruteForce(points, queryPoint);
            Assert.Equal(expectedNearest[0], nearestPoint[0]);
            Assert.Equal(expectedNearest[1], nearestPoint[1]);

            // Verify it's the point (5, 4) which should be nearest to (6, 5)
            Assert.Equal(5, nearestPoint[0]);
            Assert.Equal(4, nearestPoint[1]);

            // Calculate and verify the distance
            double distance = CalculateDistance(nearestPoint, queryPoint);
            Assert.Equal(Math.Sqrt(2), distance, precision: 6);
        }

        [Fact]
        public void FindNearestNeighbor_WithHighDimensionalPoints_ReturnsCorrectNeighbor()
        {
            // Arrange
            const int numPoints = 100;
            const int dimensions = 20;
            var random = new Random(42);

            var points = GenerateRandomPoints<double>(
                numPoints,
                dimensions,
                random,
                r => r.NextDouble() * 100);

            var kdTree = new KDTree<double>(points);

            // Create a random query point
            var queryPoint = new List<double>();
            for (int j = 0; j < dimensions; j++)
            {
                queryPoint.Add(random.NextDouble() * 100);
            }

            // Act
            var nearestPoint = kdTree.FindNearestNeighbor(queryPoint);

            // Assert
            Assert.NotNull(nearestPoint);
            Assert.Equal(dimensions, nearestPoint.Count);

            // Verify the result by comparing with a brute force approach
            IList<double> bruteForceNearest = FindNearestNeighborBruteForce(points, queryPoint);

            // Calculate the distance between the KD-Tree result and the brute force result
            double resultDistance = CalculateDistance(nearestPoint, bruteForceNearest);

            // The KD-Tree result should be the same as the brute force result or very close
            Assert.True(resultDistance < 1e-10, $"KD-Tree result differs from brute force result by {resultDistance}");
        }
    }
}