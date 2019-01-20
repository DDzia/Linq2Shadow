using System;
using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using Linq2Shadow.Utils;
using NUnit.Framework;

namespace Linq2ShadowTests.OperatorTests
{
    internal class FirstOperatorTests: TestBase
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

        [TestCase(true)]
        [TestCase(false)]
        public void Should_ReturnUserWithIdAreEqual2_When_IdAreEqual2PredicateUsedForFirstAggregation(bool useFirstOrDefault)
        {
            // Arrange
            const string member = "Id";
            const int idExpected = 2;
            var predicate = ExpressionUtils.MakeEquals(member, idExpected);

            // Act
            var query = _sut.FromTable(DbConfig.DbObjectNames.UsersTable);
            var rowFound = useFirstOrDefault
                ? query.FirstOrDefault(predicate)
                : query.First(predicate);
            

            // Assert
            Assert.AreEqual((int)rowFound[member], idExpected);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_ReturnNullForFirstOrDefaultOrThrownForFirst_When_IdAreEqual3PredicateUsedForFirstAggregation(bool useFirstOrDefault)
        {
            // Arrange
            const string member = "Id";
            const int notExistsId = 3; // User[Id] == 3 is not exists
            var filterExpr = ExpressionUtils.MakeEquals(member, notExistsId);

            // Act
            var query = _sut.FromTable(DbConfig.DbObjectNames.UsersTable);

            // Assert
            if (useFirstOrDefault)
            {
                var rowFound = query.FirstOrDefault(filterExpr);
                Assert.IsNull(rowFound);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => query.First(filterExpr));
            }
        }
    }
}
