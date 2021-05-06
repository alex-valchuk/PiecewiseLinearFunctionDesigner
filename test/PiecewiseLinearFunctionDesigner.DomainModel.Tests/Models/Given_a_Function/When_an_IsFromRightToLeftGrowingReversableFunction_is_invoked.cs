using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using Xunit;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Tests.Models.Given_a_Function
{
    public class When_an_IsFromRightToLeftGrowingReversableFunction_is_invoked
    {
        [Theory]
        [ClassData(typeof(FromRightToLeftGrowingReversableFunctionDataGenerator))]
        public void Should_return_true_if_function_stably_grows_from_right_to_left(params Point[] points)
        {
            // Arrange
            var sut = new Function
            {
                Points = points.ToList()
            };
            
            // Assert
            Assert.True(sut.IsFromRightToLeftGrowingReversableFunction);
        }
        
        [Theory]
        [ClassData(typeof(NotAReversableFunctionThatStablyGrowsFromRightToLeftDataGenerator))]
        public void Should_return_false_if_function_does_not_stably_grows_from_right_to_left(params Point[] points)
        {
            // Arrange
            var sut = new Function
            {
                Points = points.ToList()
            };
            
            // Assert
            Assert.False(sut.IsFromRightToLeftGrowingReversableFunction);
        }
    }
    
    #region TestData
    public class FromRightToLeftGrowingReversableFunctionDataGenerator : IEnumerable<object[]>
    {
        private readonly List<Point[]> _data = new List<Point[]>
        {
            new[]
            {
                new Point(0, 0),
                new Point(-1, 1),
                new Point(-2, 2),
                new Point(-3, 3)
            },
            new[]
            {
                new Point(0, 0),
                new Point(-1, 2),
                new Point(-3, 4),
                new Point(-6, 5)
            },
            new[]
            {
                new Point(0, 0),
                new Point(-2, 1),
                new Point(-4, 3),
                new Point(-5, 6)
            }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class NotAReversableFunctionThatStablyGrowsFromRightToLeftDataGenerator : IEnumerable<object[]>
    {
        private readonly List<Point[]> _data = new List<Point[]>
        {
            new[]
            {
                new Point(0, 0),
                new Point(-1, -1),
                new Point(-2, -2),
                new Point(-3, -2)
            },
            new[]
            {
                new Point(0, 0),
                new Point(1, -1),
                new Point(2, -2),
                new Point(3, -2)
            },
            new[]
            {
                new Point(0, 0),
                new Point(-1, 1),
                new Point(-2, 2),
                new Point(-3, 2)
            },
            new[]
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(2, 1),
                new Point(3, 1)
            },
            new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            },
            new[]
            {
                new Point(0, 0),
                new Point(1, 1),
                new Point(2, 2),
                new Point(3, 1)
            }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    #endregion
}