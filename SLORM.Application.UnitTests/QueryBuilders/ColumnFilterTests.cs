using SLORM.Application.Enums;
using SLORM.Application.QueryBuilders;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SLORM.Application.UnitTests.QueryBuilders
{
    public class ColumnFilterTests
    {
        [Fact]
        public void FullConstructor_WhenValidArguments_ShouldReturnInstanceWithCorrectData()
        {
            // Arrange
            var column = new TableColumn("columnName", Enums.ColumnDataType.Date);
            var valueToFilter = new List<string>() { "aa" };
            var filterRigor = FilterRigor.Contains;
            var filterMethod = FilterMethod.Excluding;
            // Act
            var columnFilter = new ColumnFilter(column, valueToFilter, filterRigor, filterMethod); ;
            // Assert
            Assert.Equal(column, columnFilter.Column);
            Assert.Equal(filterRigor, columnFilter.FilterRigor);
            Assert.Equal(filterMethod, columnFilter.FilterMethod);
        }

        [Fact]
        public void FullConstructor_WhenColumnIsNull_ShouldThrowException()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
