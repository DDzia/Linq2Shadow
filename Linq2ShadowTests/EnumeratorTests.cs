using System.Threading.Tasks;
using Linq2Shadow;
using NUnit.Framework;

namespace Linq2ShadowTests
{
    [TestFixture]
    public class EnumeratorTests: TestBase
    {
        private DatabaseContext _db;

        protected override Task BeforeEach()
        {
            _db = new DatabaseContext(DbConfig.CreateConnectionAndOpen);
            return base.BeforeEach();
        }

        protected override Task AfterEach()
        {
            _db.Dispose();
            return base.AfterEach();
        }

        [Test]
        public void Should_Enumerate3UserWithoutFiltersFromTable_When_TableHas3UsersOnly()
        {
            // Arrange
            var expectedCount = 3;

            // Act
            var counter = 0;
            foreach (var r in _db.QueryToTable(DbConfig.DbObjectNames.UsersTable))
            {
                counter++;
            }

            // Assert
            Assert.AreEqual(expectedCount, counter);
        }
    }
}
