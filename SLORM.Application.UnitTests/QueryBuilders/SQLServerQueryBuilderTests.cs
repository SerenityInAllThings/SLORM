using SLORM.Application.QueryBuilders;
using SLORM.Application.QueryBuilders.QueryBuilders;
using SLORM.Application.UnitTests.TestHelpers;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Xunit;

namespace SLORM.Application.UnitTests.QueryBuilders
{
    public class SQLServerQueryBuilderTests
    {
        //[Fact]
        //public void Constructor_WhenNoArguments_ShouldReturnInstance()
        //{
        //    // Arrange/Act
        //    var instance = new SQLServerQueryBuilder();
        //    // Assert
        //    Assert.NotNull(instance);
        //}

        //[Fact]
        //public void GetTableDescriptionQuery_WhenTableNameIsValid_ShouldReturnQueryString()
        //{
        //    // Arrange
        //    var instance = new SQLServerQueryBuilder();
        //    // Act
        //    var query = instance.GetTableDescriptionQuery("my_table");
        //    // Assert
        //    Assert.True(!string.IsNullOrWhiteSpace(query.CommandText));
        //}

        //[Theory]
        //[InlineData("my_table")]
        //[InlineData("another_table")]
        //[InlineData("even_another_table")]
        //public void GetTableDescriptionQuery_WhenTableNameIsValid_ShouldIncludeTableNameInQueryParameters(string tableName)
        //{
        //    // Arrange
        //    var instance = new SQLServerQueryBuilder();
        //    // Act
        //    var query = instance.GetTableDescriptionQuery(tableName);
        //    // Assert
        //    var tableNamePresentInParameters = false;
        //    foreach (DbParameter currentParameter in query.Parameters)
        //        tableNamePresentInParameters = currentParameter.Value == tableName
        //            ? true
        //            : tableNamePresentInParameters;
        //    Assert.True(tableNamePresentInParameters);
        //}

        //[Theory]
        //[InlineData("my_table")]
        //[InlineData("another_table")]
        //[InlineData("even_another_table")]
        //public void GetTableDescriptionQuery_WhenTableNameIsValid_ShouldReturnParsableQuery(string tableName)
        //{
        //    // Arrange
        //    var instance = new SQLServerQueryBuilder();
        //    // Act
        //    var query = instance.GetTableDescriptionQuery(tableName);
        //    // Assert
        //    Assert.True(query.CommandText.IsSqlValid());
        //}

        //[Theory]
        //[InlineData("")]
        //[InlineData(" ")]
        //[InlineData(null)]
        //public void GetTableDescriptionQuery_WhenTableNameIsNull_ShouldThrowException(string tableName)
        //{
        //    // Arrange
        //    var instance = new SQLServerQueryBuilder();
        //    // Act/Assert
        //    Assert.Throws<ArgumentNullException>(() => instance.GetTableDescriptionQuery(tableName)); 
        //}
    }
}
