using Moq;
using SLORM.Application.Contexts;
using SLORM.Application.Enums;
using SLORM.Application.Exceptions;
using SLORM.Application.QueryBuilders;
using SLORM.Application.QueryExecutors;
using SLORM.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SLORM.Application.UnitTests.Contexts
{
    public class SLORMContextTests
    {
        public SLORMContextTests()
        {
            Lifecycle.Initialize();
        }

        [Theory]
        [InlineData("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password = myPassword;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;")]
        [InlineData("Server=myServerName\\myInstanceName;Database=myDataBase;User Id=myUsername;Password = myPassword;")]
        [InlineData("Data Source=myServerAddress;Initial Catalog=myDataBase;Integrated Security=SSPI;User ID = myDomain\\myUsername;Password = myPassword;")]
        [InlineData("Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;")]
        public void Constructor_WhenSqlServerProvider_ShouldReturnInstance(string connectionString)
        {
            // Arrange
            var queryExecutorResolverMock = GetSimpleQueryExecutorResolverMock();
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);
            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            // Act
            var ctx = new SLORMContext(dbConnectionMock.Object, "my_table");
            // Assert
            Assert.NotNull(ctx);
        }

        [Fact]
        public void Constructor_WhenMissingTableName_ShouldThrowException()
        {
            // Arrange
            var connectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password = myPassword;";
            var queryExecutorResolverMock = GetSimpleQueryExecutorResolverMock();
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);
            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            // Act
            var ctx = new SLORMContext(dbConnectionMock.Object, "my_table");
            // Assert
            Assert.NotNull(ctx);
        }

        [Theory]
        [InlineData("Data Source=MyOracleDB;Integrated Security=yes;")]
        [InlineData("Data Source=MyOracleDB;User Id=myUsername;Password=myPassword;Integrated Security = no;")]
        [InlineData("SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=MyHost)(PORT=MyPort))(CONNECT_DATA=(SERVICE_NAME=MyOracleSID)));uid = myUsername;pwd = myPassword;")]
        [InlineData("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=MyHost)(PORT=MyPort))(CONNECT_DATA=(SERVICE_NAME=MyOracleSID)));User Id = myUsername;Password = myPassword;")]
        [InlineData("Data Source=myOracleDB;User Id=/;")]
        [InlineData("Data Source=myOracleDB;User Id=myUsername;Password=myPassword;")]
        [InlineData("Data Source=TORCL;User Id=myUsername;Password=myPassword;")]
        [InlineData("Data Source=username/password@//myserver:1521/my.service.com;")]
        [InlineData("Data Source=username/password@myserver//instancename;")]
        [InlineData("Data Source=username/password@myserver/myservice:dedicated/instancename;")]
        public void Constructor_WhenOracleProvider_ShouldThrowException(string connectionString)
        {
            // Arrange
            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            // Act/Assert
            Assert.Throws<UnknownSQLProviderException>(() => new SLORMContext(dbConnectionMock.Object, "my_table"));
        }

        [Theory]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;")]
        [InlineData("Server=myServerAddress;Port=1234;Database=myDataBase;Uid=myUsername;Pwd = myPassword;")]
        [InlineData("Server=serverAddress1, serverAddress2, serverAddress3;Database=myDataBase;Uid = myUsername;Pwd = myPassword;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;SslMode = Preferred;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;SslMode = Required;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;SSL Mode = Required;CertificateFile = C:\\folder\\client.pfx;CertificatePassword = pass;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;SSL Mode = Required;Certificate Store Location = CurrentUser;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;AllowBatch = False;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;AllowUserVariables = True;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;AllowZeroDateTime = True;")]
        [InlineData("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;ConvertZeroDateTime = True;")]
        public void Constructor_WhenMysqlProvider_ShouldThrowException(string connectionString)
        {
            // Arrange
            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            // Act/Assert
            Assert.Throws<UnknownSQLProviderException>(() => new SLORMContext(dbConnectionMock.Object, "my_table"));
        }

        [Theory]
        [InlineData("my_table")]
        [InlineData("my_")]
        [InlineData("my_table2")]
        public void GetTableName_WhenGet_ShouldReturnTableNameSetInConstructor(string tableName)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var queryExecutorResolverMock = GetSimpleQueryExecutorResolverMock();
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);
            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            // Act
            var ctx = new SLORMContext(dbConnectionMock.Object, tableName);
            // Assert
            Assert.Equal(tableName, ctx.TableName);
        }

        [Theory]
        [InlineData("my_column")]
        [InlineData("my_column2")]
        [InlineData("my_")]
        public void GroupBy_WhenColumnInTable_ShouldAddToColumnsToGroupBy(string columnName)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnName);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);
            // Act
            ctx.GroupBy(columnName);
            // Assert
            var columnWasAddedToGroupByList = ctx.ColumnsToGroupBy.Any(c => c.Name == columnName);
            Assert.True(columnWasAddedToGroupByList);
        }

        [Fact]
        public void GroupBy_WhenNullArgument_ShouldThrowError()
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var queryExecutorResolverMock = GetSimpleQueryExecutorResolverMock();
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ctx.GroupBy(null));
        }

        [Theory]
        [InlineData("my_column")]
        [InlineData("my_column2")]
        [InlineData("my_")]
        public void GroupBy_WhenColumnNotInTable_ShouldNotAddToColumnsToGroupBy(string columnName)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var columnsInTable = new string[] { "column1", "column2", "column3" };
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnsInTable);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);
            // Act
            ctx.GroupBy(columnName);
            // Assert
            var columnWasAddedToGroupByList = ctx.ColumnsToGroupBy.Any(c => c.Name == columnName);
            Assert.False(columnWasAddedToGroupByList);
        }

        [Theory]
        [InlineData("my_column")]
        [InlineData("my_column2")]
        [InlineData("my_column3")]
        public void GroupBy_WhenColumnAlreadyAdded_ShouldNotDuplicate(string columnName)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnName);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);
            ctx.GroupBy(columnName);
            // Act
            ctx.GroupBy(columnName);
            // Assert
            var columnOcurrencesInGroupByList = ctx.ColumnsToGroupBy.Where(c => c.Name == columnName).Count();
            Assert.Equal(1, columnOcurrencesInGroupByList);
        }

        [Theory]
        [InlineData("my_column")]
        [InlineData("my_column2")]
        [InlineData("my_")]
        public void Count_WhenProvidedColumnIsInTable_ShouldAddColumnToCountList(string columnName)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnName);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);
            // Act
            ctx.Count(columnName);
            // Assert
            var columnWasAddedToGroupByList = ctx.ColumnsToCount.Any(c => c.Name == columnName);
            Assert.True(columnWasAddedToGroupByList);
        }

        [Fact]
        public void Count_WhenColumnNameIsNull_ShouldThrowException()
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var queryExecutorResolverMock = GetSimpleQueryExecutorResolverMock();
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ctx.Count(null));
        }

        [Theory]
        [InlineData("my_column")]
        [InlineData("my_column2")]
        [InlineData("my_column3")]
        public void Count_WhenColumnAlreadyCounted_ShouldNotDuplicate(string columnName)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnName);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);
            ctx.Count(columnName);
            // Act
            ctx.Count(columnName);
            // Assert
            var columnOcurrencesInCountList = ctx.ColumnsToCount.Where(c => c.Name == columnName).Count();
            Assert.Equal(1, columnOcurrencesInCountList);
        }

        [Theory]
        [InlineData("Column_1111")]
        [InlineData("Column_222")]
        [InlineData("C333")]
        public void Count_WhenProvidedColumnIsNotInTable_ShouldNotAddToColumnsToCount(string columnName)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var columnsInTable = new string[] { "column1", "column2", "column3" };
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnsInTable);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);
            // Act
            ctx.GroupBy(columnName);
            // Assert
            var columnWasAddedToGroupByList = ctx.ColumnsToGroupBy.Any(c => c.Name == columnName);
            Assert.False(columnWasAddedToGroupByList);
        }

        [Theory]
        [InlineData("column1", "value", FilterRigor.Contains, FilterMethod.Excluding)]
        [InlineData("column2", "value1", FilterRigor.Equals, FilterMethod.Including)]
        [InlineData("column2", "value2", FilterRigor.Contains, FilterMethod.Excluding)]
        [InlineData("column3", "value3", FilterRigor.Equals, FilterMethod.Including)]
        [InlineData("column3", "value4", FilterRigor.Contains, FilterMethod.Excluding)]
        public void Filter_WhenValidFilter_ShouldAddFilterToFilterList(string columnName, string value, FilterRigor rigor, FilterMethod method)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var columnsInTable = new string[] { "column1", "column2", "column3" };
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnsInTable);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);

            var columnFilter = new ColumnFilterRequest(columnName, value, rigor, method);
            // Act
            ctx.Filter(columnFilter);
            // Assert
            var filterRequest = ctx.ColumnsToFilter.First(c => c.Column.Name == columnFilter.ColumnName && c.FilterMethod == columnFilter.FilterMethod && c.FilterRigor == columnFilter.FilterRigor && c.Value == columnFilter.Value);
            Assert.Equal(columnFilter.ColumnName, filterRequest.Column.Name);
            Assert.Equal(columnFilter.Value, filterRequest.Value);
            Assert.Equal(columnFilter.FilterMethod, filterRequest.FilterMethod);
            Assert.Equal(columnFilter.FilterRigor, filterRequest.FilterRigor);
        }

        [Theory]
        [InlineData("column10", "value", FilterRigor.Contains, FilterMethod.Excluding)]
        [InlineData("column20", "value1", FilterRigor.Equals, FilterMethod.Including)]
        [InlineData("column20", "value2", FilterRigor.Contains, FilterMethod.Excluding)]
        [InlineData("column30", "value3", FilterRigor.Equals, FilterMethod.Including)]
        [InlineData("column30", "value4", FilterRigor.Contains, FilterMethod.Excluding)]
        public void Filter_WhenColumnToFilterNotInTable_ShouldNotAddFilter(string columnName, string value, FilterRigor rigor, FilterMethod method)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var columnsInTable = new string[] { "column1", "column2", "column3" };
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnsInTable);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);

            var columnFilter = new ColumnFilterRequest(columnName, value, rigor, method);
            // Act
            ctx.Filter(columnFilter);
            // Assert
            var filterRequest = ctx.ColumnsToFilter.FirstOrDefault(c => c.Column.Name == columnFilter.ColumnName && c.FilterMethod == columnFilter.FilterMethod && c.FilterRigor == columnFilter.FilterRigor && c.Value == columnFilter.Value);
            Assert.Equal(default(ColumnFilter), filterRequest);
        }

        [Theory]
        [InlineData("column1", "value", FilterRigor.Contains, FilterMethod.Excluding)]
        [InlineData("column2", "value1", FilterRigor.Equals, FilterMethod.Including)]
        [InlineData("column2", "value2", FilterRigor.Contains, FilterMethod.Excluding)]
        [InlineData("column3", "value3", FilterRigor.Equals, FilterMethod.Including)]
        [InlineData("column3", "value4", FilterRigor.Contains, FilterMethod.Excluding)]
        public void Filter_WhenColumnToFilterAlreadyInFilterList_ShouldDuplicateFilter(string columnName, string value, FilterRigor rigor, FilterMethod method)
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var columnsInTable = new string[] { "column1", "column2", "column3" };
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnsInTable);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);

            var columnFilter = new ColumnFilterRequest(columnName, value, rigor, method);
            ctx.Filter(columnFilter);
            // Act
            ctx.Filter(columnFilter);
            // Assert
            var filterRequestCount = ctx.ColumnsToFilter.Where(c => c.Column.Name == columnFilter.ColumnName && c.FilterMethod == columnFilter.FilterMethod && c.FilterRigor == columnFilter.FilterRigor && c.Value == columnFilter.Value).Count();
            Assert.Equal(2, filterRequestCount);
        }

        [Fact]
        public void Filter_WhenNullParameter_ShouldThrowException()
        {
            // Arrange
            var connectionString = "Data Source=190.190.200.100,1433;Initial Catalog = myDataBase;User ID = myUsername;Password = myPassword;";
            var columnsInTable = new string[] { "column1", "column2", "column3" };
            var queryExecutorResolverMock = GetQueryExecutorResolverMockWithColumns(columnsInTable);
            Lifecycle.Container.RegisterInstance(typeof(IQueryExecutorResolver), queryExecutorResolverMock);

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            var ctx = new SLORMContext(dbConnectionMock.Object, connectionString);

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ctx.Filter(null));
        }

        private IQueryExecutor GetSimpleQueryExecutorMock()
        {
            var queryExecutorMock = new Mock<IQueryExecutor>();
            ICollection<TableColumn> tableColumns = new List<TableColumn>();
            var response = Task.FromResult(tableColumns);
            queryExecutorMock
                .Setup(qe => qe.GetTableColumns(It.IsAny<DbConnection>(), It.IsAny<string>()))
                .Returns(response);

            return queryExecutorMock.Object;
        }

        private IQueryExecutorResolver GetSimpleQueryExecutorResolverMock()
        {
            var queryExecutorMock = GetSimpleQueryExecutorMock();
            var queryExecutorResolverMock = new Mock<IQueryExecutorResolver>();
            queryExecutorResolverMock
                .Setup(qer => qer.GetQueryExecutorFromProviderType(It.IsAny<SQLProvider>()))
                .Returns(queryExecutorMock);
            return queryExecutorResolverMock.Object;
        }

        private IQueryExecutorResolver GetQueryExecutorResolverMockWithColumns(params string[] columnNames)
        {
            var queryExecutorMock = new Mock<IQueryExecutor>();
            ICollection<TableColumn> tableColumns = new List<TableColumn>();
            foreach (var currentColumnName in columnNames)
                tableColumns.Add(new TableColumn(currentColumnName, ColumnDataType.String));
            var response = Task.FromResult(tableColumns);
            queryExecutorMock
                .Setup(qe => qe.GetTableColumns(It.IsAny<DbConnection>(), It.IsAny<string>()))
                .Returns(response);

            var queryExecutorResolverMock = new Mock<IQueryExecutorResolver>();
            queryExecutorResolverMock
                .Setup(qer => qer.GetQueryExecutorFromProviderType(It.IsAny<SQLProvider>()))
                .Returns(queryExecutorMock.Object);
            return queryExecutorResolverMock.Object;
        }
    }
}