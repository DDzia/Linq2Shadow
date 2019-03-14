using System;
using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using Linq2Shadow.Extensions;
using NUnit.Framework;

namespace Linq2ShadowTests.OperatorTests
{
    class SelectOnlyOperatorTests: TestBase
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
        public void Should_FetchUserNameOnly_When_SelectOnlyWithUserNameIsCalled()
        {
            // Arrange
            var expectedMembers = new[] {"UserName"};

            // Act
            var userFound = _db.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .SelectOnly(expectedMembers)
                .First();

            // Assert
            Assert.NotNull(userFound);
            Assert.IsEmpty(userFound.Keys.Except(expectedMembers));
        }

        [Test]
        public void Should_FetchUserNameAndIdOnly_When_SelectOnlyWithUserNameAndIdIsCalled()
        {
            // Arrange
            var expectedMembers = new[] { "UserName", "Id" };

            // Act
            var userFound = _db.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .SelectOnly(expectedMembers)
                .First();

            // Assert
            Assert.NotNull(userFound);
            Assert.IsEmpty(userFound.Keys.Except(expectedMembers));
        }

        [Test]
        public void Should_ThrowArgumentNullException_When_QueryIsNullArgument()
        {
            // Act, Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                QueryableExtensions.SelectOnly<ShadowRow>(null, new[] {"0"});
            });
        }

        [Test]
        public void Should_ThrowArgumentNullException_When_FieldNamesIsNullArgument()
        {
            // Act, Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _db.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .SelectOnly(null);
            });
        }

        [Test]
        public void Should_ThrowArgumentException_When_FieldNamesIsEmpty()
        {
            // Act, Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _db.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .SelectOnly(new string[0]);
            });
        }

        [Test]
        public void Should_ThrowArgumentException_When_FieldNamesContainsNull()
        {
            // Act, Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _db.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .SelectOnly(new [] { "UserName", null });
            });
        }

        [Test]
        public void Should_UseLastOperatorCall_When_WasCalledTwice()
        {
            // Arrange
            var expectedFields = new[] {"Id"};

            // Act
            var userFound = _db.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .SelectOnly(new[] {"UserName"})
                .SelectOnly(expectedFields)
                .First();

            // Assert
            Assert.IsNotNull(userFound);
            Assert.IsEmpty(userFound.Keys.Except(expectedFields));
        }
    }
}
