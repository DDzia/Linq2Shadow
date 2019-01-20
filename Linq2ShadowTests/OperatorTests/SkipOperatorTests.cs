using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using NUnit.Framework;

namespace Linq2ShadowTests.OperatorTests
{
    internal class SkipOperatorTests: TestBase
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
        public void Should_ReturnUsersExceptFirstFromUsersTable_When_Skip1IsApplied()
        {
            // Arrange
            var skipCount = 1;

            // Act
            var usersFound = _sut.FromTable(DbConfig.DbObjectNames.UsersTable)
                .Skip(skipCount)
                .ToList();

            // Assert
            Assert.AreEqual(usersFound.Count, 2);
            Assert.AreEqual(usersFound[0]["UserName"], "Alex");
            Assert.AreEqual(usersFound[1]["UserName"], "Katua");
        }

        [Test]
        public void Should_ReturnLastUsersFromUsersTable_When_Skip1UsedDouble()
        {
            // Arrange
            var skipCount = 1;

            // Act
            var usersFound = _sut.FromTable(DbConfig.DbObjectNames.UsersTable)
                .Skip(skipCount)
                .Skip(skipCount)
                .ToList();

            // Assert
            Assert.AreEqual(usersFound.Count, 1);
            Assert.AreEqual(usersFound[0]["UserName"], "Katua");
        }

        [Test]
        public void Should_ReturnUsersExceptKatuaOrderedByUserNameDescFromUsersTable_When_Skip1IsApplied()
        {
            // Arrange
            var skipCount = 1;

            // Act
            var usersFound = _sut.FromTable(DbConfig.DbObjectNames.UsersTable)
                .OrderByDescending(x => x["UserName"])
                .Skip(skipCount)
                .ToList();

            // Assert
            Assert.AreEqual(usersFound.Count, 2);
            Assert.AreEqual(usersFound[0]["UserName"], "Denis");
            Assert.AreEqual(usersFound[1]["UserName"], "Alex");
        }

        [Test]
        public void Should_ReturnEmptyCollection_When_SkipedAllItems()
        {
            // Arrange
            var skipCount = 3;

            // Act
            var usersFound = _sut.FromTable(DbConfig.DbObjectNames.UsersTable)
                .Skip(skipCount)
                .ToList();

            // Assert
            Assert.IsEmpty(usersFound);
        }

        [Test]
        public void Should_Return2ItemsCount_When_Skiped1ToUsersIsApplied()
        {
            // Arrange
            var skipCount = 1;

            // Act
            var usersFoundCount = _sut.FromTable(DbConfig.DbObjectNames.UsersTable)
                .Skip(skipCount)
                .Count();

            // Assert
            Assert.AreEqual(usersFoundCount, 2);
        }

        [Test]
        public void Should_ReturnOnlySecondItem_When_Skiped1WithFirstOperatorIsUsed()
        {
            // Arrange
            var skipCount = 1;

            // Act
            var userFound = _sut.FromTable(DbConfig.DbObjectNames.UsersTable)
                .Skip(skipCount)
                .First();

            // Assert
            Assert.AreEqual(userFound["UserName"], "Alex");
        }

        [Test]
        public void Should_Return0Count_When_SkipedMoreThanTotalCount()
        {
            // Arrange
            var skipCount = 4;

            // Act
            var usersFoundCount = _sut.FromTable(DbConfig.DbObjectNames.UsersTable)
                .Skip(skipCount)
                .Count();

            // Assert
            Assert.AreEqual(usersFoundCount, 0);
        }
    }
}
