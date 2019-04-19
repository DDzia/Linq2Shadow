using System.Threading.Tasks;
using Linq2Shadow;
using NUnit.Framework;

namespace Linq2ShadowTests.DatabaseContextTests
{
    [TestFixture]
    public partial class DatabaseContextTests : TestBase
    {
        private DatabaseContext _sut;

        protected override Task BeforeEach()
        {
            DbConfig.ReInitDatabase();
            _sut = new DatabaseContext(DbConfig.CreateConnectionAndOpen);
            return base.BeforeEach();
        }

        protected override Task AfterEach()
        {
            _sut.Dispose();
            return base.AfterEach();
        }
    }
}
