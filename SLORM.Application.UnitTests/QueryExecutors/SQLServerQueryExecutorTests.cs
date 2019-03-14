using Moq;
using SLORM.Application.Enums;
using SLORM.Application.QueryBuilders;
using SLORM.Application.QueryExecutors;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SLORM.Application.UnitTests.QueryExecutors
{
    public class SQLServerQueryExecutorTests
    {
        [Fact]
        public void Constructor_WhenGivenValidQueryBuilderResolver_ShouldReturnInstance()
        {
            // Arrange
            var queryBuilderResolverMock = GetSimpleQueryBuilderResolverMock();
            var sqlDeterminator = GetSimpleSQLServerDataTypeDeterminatorMock();
            // Act
            var instance = new SQLServerQueryExecutor(queryBuilderResolverMock, sqlDeterminator);
            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void Constructor_WhenQueryBuilderResolverIsNull_ShouldThrowException()
        {
            // Arrange
            var sqlDeterminator = GetSimpleSQLServerDataTypeDeterminatorMock();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SQLServerQueryExecutor(null, sqlDeterminator));
        }

        [Fact]
        public void Constructor_WhenSqlDeteminatorIsNull_ShouldThrowException()
        {
            // Arrange
            var queryBuilderResolverMock = GetSimpleQueryBuilderResolverMock();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SQLServerQueryExecutor(queryBuilderResolverMock, null));
        }

        [Fact]
        public async Task GetTableColumns_WhenConnectionIsNull_ShouldThrowException()
        {
            // Arrange
            var queryBuilderResolverMock = GetSimpleQueryBuilderResolverMock();
            var sqlDeterminator = GetSimpleSQLServerDataTypeDeterminatorMock();
            var instance = new SQLServerQueryExecutor(queryBuilderResolverMock, sqlDeterminator);
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GetTableColumns(null, "my_table"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task GetTableColumns_WhenTableNameIsNull_ShouldThrowExceptionAsync(string tableName)
        {
            // Arrange
            var queryBuilderResolverMock = GetSimpleQueryBuilderResolverMock();
            var sqlDeterminator = GetSimpleSQLServerDataTypeDeterminatorMock();
            var instance = new SQLServerQueryExecutor(queryBuilderResolverMock, sqlDeterminator);
            var dbConnection = GetSimpleDbConnectionMock();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GetTableColumns(dbConnection, tableName));
        }

        private DbCommand GetSimpleDbCommandMock()
        {
            return new Mock<DbCommand>().Object;
        }

        private IQueryBuilder GetSimpleQueryBuilderMock()
        {
            var dbCommand = GetSimpleDbCommandMock();
            var queryBuilderMock = new Mock<IQueryBuilder>();
            queryBuilderMock.Setup(qb => qb.GetTableDescriptionQuery(It.IsAny<string>()))
                .Returns(dbCommand);
            return queryBuilderMock.Object;
        }

        private IQueryBuilderResolver GetSimpleQueryBuilderResolverMock()
        {
            var queryBuilderMock = GetSimpleQueryBuilderMock();
            var queryBuilderResolverMock = new Mock<IQueryBuilderResolver>();
            queryBuilderResolverMock.Setup(qbr => qbr.ResolveFromSQLProvider(It.IsAny<SQLProvider>()))
                .Returns(queryBuilderMock);
            return queryBuilderResolverMock.Object;
        }

        private DbConnection GetSimpleDbConnectionMock()
        {
            return new Mock<DbConnection>().Object;
        }

        private ISQLServerDataTypeDeterminator GetSimpleSQLServerDataTypeDeterminatorMock()
        {
            var mock = new Mock<ISQLServerDataTypeDeterminator>();
            mock
                .Setup(s => s.FromDataTypeField(It.IsAny<string>()))
                .Returns(ColumnDataType.String);
            return mock.Object;
        }
    }
}
