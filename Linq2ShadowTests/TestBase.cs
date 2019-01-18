using System.Threading.Tasks;
using NUnit.Framework;

namespace Linq2ShadowTests
{
    [Parallelizable(ParallelScope.None)]
    internal abstract class TestBase
    {
        [OneTimeSetUp]
        public Task SetUpOnce() => BeforeAll();
        protected virtual Task BeforeAll() => Task.CompletedTask;

        [OneTimeTearDown]
        public void TearDownOnce() => AfterAll();
        protected virtual Task AfterAll() => Task.CompletedTask;

        [SetUp]
        public Task SetUp() => BeforeEach();
        protected virtual Task BeforeEach() => Task.CompletedTask;

        [TearDown]
        public void TearDown() => AfterEach();

        protected virtual Task AfterEach() => Task.CompletedTask;
    }
}
