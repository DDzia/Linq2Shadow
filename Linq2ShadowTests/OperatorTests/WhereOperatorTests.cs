using System;
using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using Linq2Shadow.Utils;
using NUnit.Framework;

namespace Linq2ShadowTests.OperatorTests
{
    internal class WhereOperatorTests: TestBase
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
        public void Should_ReturnDenisRecord_When_DenisIsExists()
        {
            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction)
                .Where(ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis"))
                .ToArray<dynamic>();

            // Assert
            Assert.AreEqual(data.Length, 1);
        }

        [Test]
        public void Should_ReturnTwoRecord_When_ContainsIsUsed()
        {
            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction)
                .Where(ExpressionBuilders.Predicates.StringContains("UserName", "e"))
                .ToArray();

            // Assert
            Assert.AreEqual(data.Length, 1);
        }

        [Test]
        public void Should_ReturnDenisRecord_When_EndWithS()
        {
            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction)
                .Where(ExpressionBuilders.Predicates.StringEndsWith("UserName", "s"))
                .ToList<dynamic>();

            // Assert
            Assert.AreEqual(data[0].UserName, "Dzianis");
        }

        [Test]
        public void Should_ReturnAlexRecord_When_StartWithS()
        {
            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => ((string) x["UserName"]).StartsWith("A"))
                            .ToArray();

            // Assert
            Assert.AreEqual(data[0]["UserName"], "Alex");
        }


        [Test]
        public void Should_ReturnAlexRecord_When_IdIs1()
        {
            // Arrange
            var filter = ExpressionBuilders.Predicates.AreEquals("Id", 1);

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction)
                .Where(x => x["Id"].Equals(1))
                .Where(filter)
                .ToArray();

            // Assert
            Assert.AreEqual(data[0]["UserName"], "Alex");
        }

        [Test]
        public void Should_ReturnAlexAndDenisRecords_When_IdIs1And2()
        {
            // Arrange
            var filter = ExpressionBuilders.Predicates.AreEquals("Id", 1);

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => x["Id"].Equals(1)).Where(filter).ToArray();

            // Assert
            Assert.AreEqual(data[0]["UserName"], "Alex");
        }

        [Test]
        public void Should_ReturnAllItems_When_QueryisCorrect()
        {
            // Arrange
            var v = Guid.NewGuid().ToString();

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x =>
                ((int) x["Id"] == 0 || (int) x["Id"] == 1 || (int) x["Id"] == 2)
                && (string) x["UserName"] != v).ToArray();

            // Assert
            Assert.AreEqual(data.Length, 3);
        }

        [Test]
        public void Should_ReturnItems_When_IdGreaterThan1()
        {
            // Arrange
            var v = Guid.NewGuid().ToString();

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction)
                .Where(x => (double) x["Id"] > 1)
                .ToList<dynamic>();

            // Assert
            Assert.AreEqual(data[0].UserName, "Katrin");
        }

        [Test]
        public void Should_ReturnItems_When_IdGreaterOrEqual1()
        {
            // Arrange
            var v = Guid.NewGuid().ToString();

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => (double) x["Id"] >= 1).ToArray();

            // Assert
            Assert.AreEqual(data.Length, 2);
        }

        [Test]
        public void Should_ReturnItems_When_IdLessThan1()
        {
            // Arrange
            var v = Guid.NewGuid().ToString();

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => (double) x["Id"] < 1).ToArray();

            // Assert
            Assert.AreEqual(data.Length, 1);
        }

        [Test]
        public void Should_ReturnItems_When_IdLessOrEqual1()
        {
            // Arrange
            var v = Guid.NewGuid().ToString();

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => (double) x["Id"] <= 1).ToArray();

            // Assert
            Assert.AreEqual(data.Length, 2);
        }

        [Test]
        public void Should_ReturnItem_When_IdIsEqual1()
        {
            // Arrange
            var v = Guid.NewGuid().ToString();

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => (double) x["Id"] == 1).ToArray();

            // Assert
            Assert.AreEqual(data[0]["UserName"].ToString(), "Alex");
        }

        [Test]
        public void Should_ReturnItem2_When_IdIsNotEqual1()
        {
            var v = Guid.NewGuid().ToString();

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => (double) x["Id"] != 1).ToArray();

            // Assert
            Assert.AreEqual(data.Length, 2);
        }

        [Test]
        public void Should_ReturnUsers_When_BirthdayFilterWasApplied()
        {
            // Arrange
            var v = Guid.NewGuid().ToString();

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => (double) x["Id"] != 1).ToArray();

            // Assert
            Assert.AreEqual(data.Length, 2);
        }

        [Test]
        public void Should_ReturnUsersWithId1or2_When_WhereWith1and2listIsApplied()
        {
            // Arrange
            var ids = new[] { 1, 2 };

            // Act
            var data = _sut.QueryToTableValuedFunction(DbConfig.DbObjectNames.GetAllUsersFunction).Where(x => ids.Contains((int) x["Id"])).ToArray();

            // Assert
            Assert.AreEqual(data.Length, 2);
        }
    }
}
