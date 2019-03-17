using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using Linq2Shadow.Utils;
using NUnit.Framework;

namespace Linq2ShadowTests.DatabaseContextTests
{
    [TestFixture]
    public class DatabaseContextUpdateTests: TestBase
    {
        private DatabaseContext _sut;

        protected override Task BeforeEach()
        {
            DbConfig.ReInitDatabase();
            _sut = new DatabaseContext(DbConfig.CreateConnectionAndOpen);
            return Task.CompletedTask;
        }

        protected override Task AfterEach()
        {
            _sut?.Dispose();
            _sut = null;
            return Task.CompletedTask;
        }

        [Test]
        public void Should_UpdateDzianisUserNameToSuperDzianis_When_PredicateWithUNameDzianisIsUsed()
        {
            // Arrange
            var newUName = "SuperDzianis";
            var oldUName = "Dzianis";
            const string updateMember = "UserName";
            var updateFields = new Dictionary<string, object>() { { updateMember, newUName } };

            // Act
            var updatedRows = _sut.Update(DbConfig.DbObjectNames.UsersTable,
                updateFields,
                ExpressionBuilders.Predicates.AreEquals(updateMember, oldUName));

            // Assert
            Assert.AreEqual(updatedRows, 1);

            var newDzianisFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .FirstOrDefault(ExpressionBuilders.Predicates.AreEquals(updateMember, newUName));
            Assert.IsNotNull(newDzianisFound);
            Assert.AreEqual(newDzianisFound["UserName"], newUName);
        }

        [Test]
        public void Should_UpdateAllUsers_When_PredicateIsNotUsed()
        {
            // Arrange
            var newUName = "SuperDzianis";
            const string updateMember = "UserName";
            var updateFields = new Dictionary<string, object>() { { updateMember, newUName } };

            // Act
            var updatedRows = _sut.Update(DbConfig.DbObjectNames.UsersTable, updateFields);

            // Assert
            Assert.AreEqual(updatedRows, 3);
        }

        [Test]
        public async Task Should_UpdateAllUsersAsynchronously_When_PredicateIsNotUsed()
        {
            // Arrange
            var newUName = "SuperDzianis";

            // Act
            var updatedRows = await _sut.UpdateAsync(DbConfig.DbObjectNames.UsersTable, new { UserName=newUName });

            // Assert
            Assert.AreEqual(updatedRows, 3);
        }

        [Test]
        public void Should_ThrowArgumentNullException_When_UpdateTargetIsNull()
        {
            // Arrange
            var updateMap = new Dictionary<string, object>(){{"UserName", "Dzianis"}};

            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => _sut.Update(null, updateMap));
        }

        [Test]
        public void Should_ThrowArgumentException_When_UpdateTargetIsEmpty()
        {
            // Arrange
            var updateMap = new Dictionary<string, object>() { { "UserName", "Dzianis" } };

            // Act, Assert
            Assert.Throws<ArgumentException>(() => _sut.Update(string.Empty, updateMap));
            Assert.Throws<ArgumentException>(() => _sut.Update("    ", updateMap));
        }

        [Test]
        public void Should_ThrowArgumentNullException_When_UpdateFieldsIsNull()
        {
            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => _sut.Update(DbConfig.DbObjectNames.UsersTable, null));
        }

        [Test]
        public void Should_ThrowArgumentException_When_UpdateFieldsIsEmpty()
        {
            // Arrange
            var updateMap = new Dictionary<string, object>();

            // Act, Assert
            Assert.Throws<ArgumentException>(() => _sut.Update(DbConfig.DbObjectNames.UsersTable, updateMap));
        }

        [Test]
        public void Should_ThrowArgumentException_When_UpdateFieldsHasInvalidFieldNames()
        {
            // Arrange
            var updateMap = new Dictionary<string, object>(){ { " ", "Oops"} };

            // Act, Assert
            Assert.Throws<ArgumentException>(() => _sut.Update(DbConfig.DbObjectNames.UsersTable, updateMap));
        }

        [Test]
        public void Should_UpdateAllUsers_When_PredicateIsNotUsedForTypedModel()
        {
            // Arrange
            var newUName = "SuperDzianis";
            var updateMap = new {UserName = newUName};

            // Act
            var updatedRows = _sut.Update(DbConfig.DbObjectNames.UsersTable, updateMap);

            // Assert
            Assert.AreEqual(updatedRows, 3);
        }
    }
}
