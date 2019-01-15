using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpToSql;
using NUnit.Framework;

namespace TestsExpToSql
{
    public class A
    {
        public bool g;
    }

    public class DatabaseContextTests
    {
        private DatabaseContext _sut;

        [SetUp]
        public void SetUp()
        {
            DataBaseConfig.ReCreateDatabase();
            _sut = new DatabaseContext(new Lazy<IDbConnection>(DataBaseConfig.CreateConnectionAndOpen));
        }

        [TearDown]
        public void TearDown()
        {
            _sut?.Dispose();
            _sut = null;
        }

        [Test]
        public void Should_ReturnAllData_WhenWhereIsNotCalled()
        {
            // Expression<Func<DynamicRow, bool>> a = x => (bool)x[""] == true;

            // Act
            var data = _sut.FromTableFunction("fGetAllUsers")
                .Where(x => x["asdf"].Equals(false))
                // .Where(x => true.Equals(x["asdf"]))
                .OrderBy(x => x["sad"])
                // .
                .ToArray();

            // Assert
            Assert.AreEqual(data.Length, 3);
        }


        [Test]
        public void Should_Return2RecordsFromSp_When_SpWasReturn2Records()
        {
            // Act
            var data = _sut.FromStoredProcedure("spReturnAllUsers", new
            {
                param0 = 1,
                param1 = "Denis"
            });

            // Assert
            Assert.AreEqual(data.Count(), 3);
        }
    }
}