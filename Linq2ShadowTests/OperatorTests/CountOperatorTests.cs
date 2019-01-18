using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using NUnit.Framework;

namespace Linq2ShadowTests.OperatorTests
{
    internal class CountOperatorTests: TestBase
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
        public void Should_Return3_When_CountCalledWithoutWhere()
        {
            // Act
            var count = _sut.FromTableFunction(DbConfig.DbObjectNames.GetAllUsersFunction)
                            .Count();

            // Assert
            Assert.AreEqual(count, 3);
        }

        [Test]
        public void Should_Return2_When_CountCalledWithWhere()
        {
            // Arrange
            var ids = new[] { 0, 2 };

            // Act
            var count = _sut.FromTableFunction(DbConfig.DbObjectNames.GetAllUsersFunction)
                            .Count(x => Enumerable.Contains(ids, (int)x["Id"]));

            // Assert
            Assert.AreEqual(count, 2);
        }
    }
}
