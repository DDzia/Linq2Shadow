using System;
using System.Linq;
using Linq2Shadow.Utils;
using NUnit.Framework;

namespace Linq2ShadowTests.DatabaseContextTests
{
    public partial class DatabaseContextTests
    {
        [Test]
        public void Should_DoesNotRemoveAnythingThrowArgumentNullException_When_SourceParameterIsNull()
        {
            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => _sut.Remove(null));
        }

        [Test]
        public void Should_DoesNotRemoveAnythingAndThrowArgumentNullException_When_SourceParameterIsWhitespace()
        {
            // Act, Assert
            Assert.Throws<ArgumentException>(() => _sut.Remove(" "));
        }

        [Test]
        public void Should_RemoveAllUsers_When_PredicateWasNotAccepted()
        {
            // Act
            var affectedRecords = _sut.Remove(DbConfig.DbObjectNames.UsersTable);

            // Assert
            Assert.AreEqual(3, affectedRecords);
            var usersFoundCount = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable).Count();
            Assert.AreEqual(0, usersFoundCount);
        }

        [Test]
        public void Should_RemoveUserWithAlexUserName_When_UserNameEqAlexPredicateIsUsed()
        {
            // Arrange
            var predicate = ExpressionBuilders.Predicates.AreEquals("UserName", "Alex");

            // Act
            var affectedRecords = _sut.Remove(DbConfig.DbObjectNames.UsersTable, predicate);

            // Assert
            Assert.AreEqual(1, affectedRecords);
            var alexesCount = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Count(predicate);
            Assert.AreEqual(0, alexesCount);
        }
    }
}
