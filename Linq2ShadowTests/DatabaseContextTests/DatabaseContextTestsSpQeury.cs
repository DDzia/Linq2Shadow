using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using NUnit.Framework;

namespace Linq2ShadowTests.DatabaseContextTests
{
    [TestFixture]
    public class DatabaseContextTestsSpQeury: TestBase
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
        public void Should_Return2RecordsFromSp_When_SpWasReturn2Records()
        {
            // Act
            var data = _sut.QueryToStoredProcedure(DbConfig.DbObjectNames.GetAllUsersSp, new
            {
                param0 = 1,
                param1 = "Denis"
            });

            // Assert
            Assert.AreEqual(data.Count(), 3);
        }
    }
}
