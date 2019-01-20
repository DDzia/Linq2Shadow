using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using NUnit.Framework;

namespace Linq2ShadowTests.OperatorTests
{
    // TODO: need check with count
    class TakeOperatorTests: TestBase
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
        public void Should_ReturnFirstUser_When_Take1IsUsed()
        {
            // Arrange
            var takeCount = 1;

            // Act
            var usersFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Take(takeCount)
                .ToList();

            // Assert
            Assert.AreEqual(usersFound.Count, 1);
            Assert.AreEqual(usersFound[0]["UserName"], "Denis");
        }

        [Test]
        public void Should_ReturnAlexUser_When_Take1AndOrderByUserNameIsUsed()
        {
            // Arrange
            var takeCount = 1;

            // Act
            var usersFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .OrderBy(x => x["UserName"])
                .Take(takeCount)
                .ToList();

            // Assert
            Assert.AreEqual(usersFound.Count, 1);
            Assert.AreEqual(usersFound[0]["UserName"], "Alex");
        }

        [Test]
        public void Should_ReturnTakeValue_When_CountCalled()
        {
            // Arrange
            var takeCount = 1;

            // Act
            var usersFoundCount = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Take(takeCount)
                .Count();

            // Assert
            Assert.AreEqual(usersFoundCount, 1);
        }

        [Test]
        public void Should_ReturnReallyCountValue_When_TakeIsVeryLong()
        {
            // Arrange
            var takeCount = 4;

            // Act
            var usersFoundCount = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Take(takeCount)
                .Count();

            // Assert
            Assert.AreEqual(usersFoundCount, 3);
        }

        [Test]
        public void Should_ReturnSecondRow_When_Skip1AndTake1IsUsed()
        {
            // Act
            var usersFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Skip(1)
                .Take(1)
                .ToList();

            // Assert
            Assert.AreEqual(usersFound.Count, 1);
            Assert.AreEqual(usersFound[0]["UserName"], "Alex");
        }
    }
}
