using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using NUnit.Framework;

namespace Linq2ShadowTests.OperatorTests
{
    internal class OrderByOperatorTests : TestBase
    {
        private DatabaseContext _sut;

        protected override Task BeforeAll()
        {
            DbConfig.ReInitDatabase();
            _sut = new DatabaseContext(DbConfig.CreateConnectionAndOpen);
            return Task.CompletedTask;
        }

        protected override Task AfterAll()
        {
            _sut?.Dispose();
            _sut = null;
            return Task.CompletedTask;
        }

        [Test]
        public void Should_ReturnDataSortedByUserName_When_AllUsersIsReturned()
        {
            // Arrange
            var orderMember = "UserName";

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction)
                            .OrderBy(x => x[orderMember])
                            .ToArray();

            // Assert
            Assert.AreEqual(data.Count(), 3);
        }

        [Test]
        public void Should_ReturnItemsWithCorrectOrdering_When_CompositeOrderingIsApplied()
        {
            // Arrange
            var expectedUniqIdsOrder = new[] { 40, 10, 30, 20 };

            // Act
            var data = _sut.QueryToTable(DbConfig.DbObjectNames.ReportsTable)
                           .OrderByDescending(x => x["Id"])
                           .ThenBy(x => x["FileName"])
                           .ThenByDescending(x => x["CreatedBy"])
                           .ToArray();

            // Assert
            for (int i = 0; i < expectedUniqIdsOrder.Length; i++)
            {
                Assert.IsTrue((int)data[i]["UniqueId"] == expectedUniqIdsOrder[i]);
            }
        }
    }
}
