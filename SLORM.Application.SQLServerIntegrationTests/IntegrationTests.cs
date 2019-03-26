using SLORM.Application.Contexts;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace SLORM.Application.SQLServerIntegrationTests
{
    public class IntegrationTests : IDisposable
    {
        private readonly IDbConnection connection = new SqlConnection(Configuration.ConnectionString);

        public IntegrationTests()
        {
            Lifecycle.Initialize();
            TestTableManager.CreateTestTable();
            TestTableManager.InsertTestData();
        }

        public void Dispose()
        {
            TestTableManager.DeleteTestTable();
        }

        [Fact]
        public async Task Query_OnlyValidGroupByApplied_ShouldReturnSameResultsAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name), nameof(SampleData.Valid));

            var expectedResult = TestTableManager.TestData
                .GroupBy(p => new { p.Name, p.Valid });
            // Act
            var results = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), results.Rows.Count());
            for (var i = 0; i < results.Rows.Count(); i++)
            {
                var currentResultRow = results.Rows.ElementAt(i);
                var currentExpectedRowExists = expectedResult
                    .Any(group => group.Key.Name == currentResultRow.Values[0] && group.Key.Valid == bool.Parse(currentResultRow.Values[1]));
                Assert.True(currentExpectedRowExists);
            }
        }

        [Fact]
        public async Task Query_OnlyValidGroupByApplied2_ShouldReturnSameResultsAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name), nameof(SampleData.DateTime));

            var expectedResult = TestTableManager.TestData
                .GroupBy(p => new { p.Name, p.DateTime });
            // Act
            var results = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), results.Rows.Count());
            for (var i = 0; i < results.Rows.Count(); i++)
            {
                var currentResultRow = results.Rows.ElementAt(i);
                var currentExpectedRowExists = expectedResult
                    .Any(group => group.Key.Name == currentResultRow.Values[0] && group.Key.DateTime == DateTime.Parse(currentResultRow.Values[1]));
                Assert.True(currentExpectedRowExists);
            }
        }

        [Fact]
        public async Task Query_SomeInvalidGroupByApplied_ShouldReturnSameResultsAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name), nameof(SampleData.Valid), "someString", "ThatDoesNotExist", "a", "whatever");

            var expectedResult = TestTableManager.TestData
                .GroupBy(p => new { p.Name, p.Valid });
            // Act
            var results = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), results.Rows.Count());
            for (var i = 0; i < results.Rows.Count(); i++)
            {
                var currentResultRow = results.Rows.ElementAt(i);
                var currentExpectedRowExists = expectedResult
                    .Any(group => group.Key.Name == currentResultRow.Values[0] && group.Key.Valid == bool.Parse(currentResultRow.Values[1]));
                Assert.True(currentExpectedRowExists);
            }
        }

        [Fact]
        public async Task Query_CountExistentColumn_ShouldReturnSameAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.Count(nameof(SampleData.Name));

            var expectedResult =
                TestTableManager.TestData.GroupBy(t => t.Name)
                .Select(group =>
                {
                    return new
                    {
                        group.Key,
                        Name = group.Count(),
                    };
                });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.First(e => e.Key == currentRow.Values[0]);
                Assert.Equal(Convert.ToInt32(currentRow.Values[1]), equivalentExpectedRow.Name);
            }
        }

        [Fact]
        public async Task Query_CountMultipleExistentColumn_ShouldReturnSameAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Count(nameof(SampleData.Name));

            var expectedResult =
                TestTableManager.TestData.GroupBy(t => new { t.Name, t.Valid })
                .Select(group =>
                {
                    return new
                    {
                        group.Key,
                        Name = group.Count(),
                    };
                });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.First(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.Equal(Convert.ToInt32(currentRow.Values[2]), equivalentExpectedRow.Name);
            }
        }

        [Fact]
        public async Task Query_CountMultipleExistentColumnWithSomeInexistent_ShouldReturnSameAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Count(nameof(SampleData.Name));
            context.Count("randomColumn", "anotherInvalidColumn");

            var expectedResult =
                TestTableManager.TestData.GroupBy(t => new { t.Name, t.Valid })
                .Select(group =>
                {
                    return new
                    {
                        group.Key,
                        Name = group.Count(),
                    };
                });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.First(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.Equal(Convert.ToInt32(currentRow.Values[2]), equivalentExpectedRow.Name);
            }
        }

        [Fact]
        async Task Query_SumNumbers_ShouldReturnSameAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Sum(nameof(SampleData.Value));
            context.Sum(nameof(SampleData.Value2));
            context.Sum(nameof(SampleData.Value3));
            context.Sum("ColumnThatDoesNotExist");
            context.Sum(nameof(SampleData.DateTime)); // Not an number column
            context.Sum(nameof(SampleData.Value3)); // Column Already added

            var expectedResult =
                TestTableManager.TestData.GroupBy(t => new { t.Name, t.Valid })
                .Select(group =>
                {
                    return new
                    {
                        group.Key,
                        Name = group.Count(),
                        Value = group.Sum(t => t.Value),
                        Value2 = group.Sum(t => t.Value2),
                        Value3 = group.Sum(t => t.Value3),
                    };
                });
            // Act
            var result = await context.Query();
            // Assert
            // Checking if db returned same number of rows as expected.
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            // Checking columnCount
            Assert.Equal(5, result.Rows.First().Values.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.First(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.Equal(float.Parse(currentRow.Values[2]), equivalentExpectedRow.Value, 2);
                Assert.Equal(double.Parse(currentRow.Values[3]), equivalentExpectedRow.Value2, 2);
                Assert.Equal(int.Parse(currentRow.Values[4]), equivalentExpectedRow.Value3);
            }
        }

        [Theory]
        [InlineData("a")]
        [InlineData("a1")]
        [InlineData("3")]
        public async Task Query_WhenFilterTextContainsExcluding_ShouldReturnSameAsLinq(string filterContent)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.Name), filterContent, Enums.FilterRigor.Contains, Enums.FilterMethod.Excluding);

            var expectedResult = TestTableManager.TestData
                .Where(t => !t.Name.Contains(filterContent))
                .GroupBy(t => new { t.Name, t.Valid });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData("a")]
        [InlineData("1")]
        [InlineData("3")]
        public async Task Query_WhenFilterTextContainsIncluding_ShouldReturnSameAsLinq(string filterContent)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.Name), filterContent, Enums.FilterRigor.Contains, Enums.FilterMethod.Including);

            var expectedResult = TestTableManager.TestData
                .Where(t => t.Name.Contains(filterContent))
                .GroupBy(t => new { t.Name, t.Valid });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData("a")]
        [InlineData("1")]
        [InlineData("3")]
        public async Task Query_WhenFilterTextEqualsIncluding_ShouldReturnSameAsLinq(string filterContent)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.Name), filterContent, Enums.FilterRigor.Equals, Enums.FilterMethod.Including);

            var expectedResult = TestTableManager.TestData
                .Where(t => t.Name == filterContent)
                .GroupBy(t => new { t.Name, t.Valid });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData("a")]
        [InlineData("1")]
        [InlineData("3")]
        public async Task Query_WhenFilterTextEqualsExcluding_ShouldReturnSameAsLinq(string filterContent)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.Name), filterContent, Enums.FilterRigor.Equals, Enums.FilterMethod.Excluding);

            var expectedResult = TestTableManager.TestData
                .Where(t => t.Name != filterContent)
                .GroupBy(t => new { t.Name, t.Valid });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1.5)]
        [InlineData(10.5)]
        public async Task Query_WhenFilterNumberContainsIncluding_ShouldReturnSameAsLinq(float filterContent)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.Value), filterContent.ToString(), Enums.FilterRigor.Contains, Enums.FilterMethod.Including);

            var expectedResult = TestTableManager.TestData
                .Where(t => t.Value.ToString().Contains(filterContent.ToString()))
                .GroupBy(t => new { t.Name, t.Valid });

            var aCount2 = TestTableManager.TestData.Where(t => t.Value.ToString().Contains(filterContent.ToString()));

            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1.5)]
        [InlineData(10.5)]
        [InlineData(98)]
        [InlineData(101)]
        public async Task Query_WhenFilterNumberContainsExcluding_ShouldReturnSameAsLinq(double filterContent)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.Value2), filterContent.ToString(), Enums.FilterRigor.Contains, Enums.FilterMethod.Excluding);

            var expectedResult = TestTableManager.TestData
                .Where(t => !t.Value2.ToString().Contains(filterContent.ToString()))
                .GroupBy(t => new { t.Name, t.Valid });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(26)]
        public async Task Query_WhenFilterNumberEqualsIncluding_ShouldReturnSameAsLinq(int filterContent)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.Value3), filterContent.ToString(), Enums.FilterRigor.Equals, Enums.FilterMethod.Including);

            var expectedResult = TestTableManager.TestData
                .Where(t => t.Value3 == filterContent)
                .GroupBy(t => new { t.Name, t.Valid });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(26)]
        public async Task Query_WhenFilterNumberEqualsExcluding_ShouldReturnSameAsLinq(int filterContent)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.Value3), filterContent.ToString(), Enums.FilterRigor.Equals, Enums.FilterMethod.Excluding);

            var expectedResult = TestTableManager.TestData
                .Where(t => t.Value3 != filterContent)
                .GroupBy(t => new { t.Name, t.Valid });
            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData("11")]
        [InlineData("2019")]
        [InlineData("30")]
        public async Task Query_WhenFilterDateContainsIncluding_ShouldReturnSameAsLinq(string searchTerm)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.DateTime), searchTerm, Enums.FilterRigor.Contains, Enums.FilterMethod.Including);

            var expectedResult = TestTableManager.TestData
                .Where(t => t.DateTime.ToString("o").Contains(searchTerm))
                .GroupBy(t => new { t.Name, t.Valid });

            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData("11")]
        [InlineData("2019")]
        [InlineData("30")]
        public async Task Query_WhenFilterDateContainsExcluding_ShouldReturnSameAsLinq(string searchTerm)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.DateTime), searchTerm, Enums.FilterRigor.Contains, Enums.FilterMethod.Excluding);

            var expectedResult = TestTableManager.TestData
                .Where(t => !t.DateTime.ToString("o").Contains(searchTerm))
                .GroupBy(t => new { t.Name, t.Valid });

            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData("2019-04-02T00:00:00.0000000+00:00")]
        [InlineData("2019-03-23T00:00:00.0000000+00:00")]
        public async Task Query_WhenFilterDateEqualsIncluding_ShouldReturnSameAsLinq(string searchTerm)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.DateTime), searchTerm, Enums.FilterRigor.Equals, Enums.FilterMethod.Including);

            var expectedResult = TestTableManager.TestData
                .Where(t => searchTerm.Contains(t.DateTime.ToString("o")))
                .GroupBy(t => new { t.Name, t.Valid });

            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Theory]
        [InlineData("2019-04-02T00:00:00.0000000+00:00")]
        [InlineData("2019-03-23T00:00:00.0000000+00:00")]
        public async Task Query_WhenFilterDateEqualsExcluding_ShouldReturnSameAsLinq(string searchTerm)
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.Filter(nameof(SampleData.DateTime), searchTerm, Enums.FilterRigor.Equals, Enums.FilterMethod.Excluding);

            var expectedResult = TestTableManager.TestData
                .Where(t => !searchTerm.Contains(t.DateTime.ToString("o")))
                .GroupBy(t => new { t.Name, t.Valid });

            // Act
            var result = await context.Query();
            // Assert
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            foreach (var currentRow in result.Rows)
            {
                var equivalentExpectedRow = expectedResult.FirstOrDefault(e => e.Key.Name == currentRow.Values[0] && e.Key.Valid == bool.Parse(currentRow.Values[1]));
                Assert.NotNull(equivalentExpectedRow);
            }
        }

        [Fact]
        async Task Query_OrderByAsc_ShouldReturnSameAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.OrderBy(nameof(SampleData.Name), Enums.OrderType.ASC);

            var expectedResult =
                TestTableManager.TestData
                    .GroupBy(t => new { t.Name, t.Valid })
                    .OrderBy(g => g.Key.Name);
            // Act
            var result = await context.Query();
            // Assert
            // Checking if db returned same number of rows as expected.
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            for (var i = 0; i < result.Rows.Count(); i++)
            {
                var currentRow = result.Rows[i];
                var equivalentRow = expectedResult.ElementAt(i);
                Assert.Equal(equivalentRow.Key.Name, currentRow.Values[0]);
            }
        }

        [Fact]
        async Task Query_OrderByDesc_ShouldReturnSameAsLinq()
        {
            // Arrange
            var context = new SLORMContext(connection, TestTableManager.TableName);
            context.GroupBy(nameof(SampleData.Name));
            context.GroupBy(nameof(SampleData.Valid));
            context.OrderBy(nameof(SampleData.Name), Enums.OrderType.DESC);

            var expectedResult =
                TestTableManager.TestData
                    .GroupBy(t => new { t.Name, t.Valid })
                    .OrderByDescending(g => g.Key.Name);
            // Act
            var result = await context.Query();
            // Assert
            // Checking if db returned same number of rows as expected.
            Assert.Equal(expectedResult.Count(), result.Rows.Count());
            for (var i = 0; i < result.Rows.Count(); i++)
            {
                var currentRow = result.Rows[i];
                var equivalentRow = expectedResult.ElementAt(i);
                Assert.Equal(equivalentRow.Key.Name, currentRow.Values[0]);
            }
        }
    }
}
