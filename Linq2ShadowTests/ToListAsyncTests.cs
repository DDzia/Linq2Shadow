using System.Threading.Tasks;
using Linq2Shadow;
using NUnit.Framework;
using Linq2Shadow.Extensions;

namespace Linq2ShadowTests
{
    [TestFixture]
    public class ToListAsyncTests: TestBase
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
        public async Task Shoud_ReturnAllUsers_When_ToListAsyncIsUsed()
        {
            // Act
            var row = await _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .ToListAsync();

            // Assert
            Assert.AreEqual(row.Count, 3);
        }
    }
}
