using Moq;
using SLORM.Application.Contexts;
using SLORM.Application.Enums;
using SLORM.Application.Exceptions;
using SLORM.Application.QueryExecutors;
using SLORM.Application.ValueObjects;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace SLORM.Application.UnitTests.Contexts
{
    public class SLORMContextTests
    {
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
            var ctx = new SLORMContext(dbConnectionMock.Object);
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
            Lifecycle.Initialize();
            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            // Act/Assert
            Assert.Throws<UnknownSQLProviderException>(() => new SLORMContext(dbConnectionMock.Object));
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
            Lifecycle.Initialize();
            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Setup(conn => conn.ConnectionString).Returns(connectionString);
            // Act/Assert
            Assert.Throws<UnknownSQLProviderException>(() => new SLORMContext(dbConnectionMock.Object));
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
    }
}