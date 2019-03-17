using System;
using System.Linq;
using System.Threading.Tasks;
using Linq2Shadow;
using Linq2Shadow.Utils;
using NUnit.Framework;

namespace Linq2ShadowTests.QueryToTableTests
{
    [TestFixture]
    public class QueryToTable_ToCollectionTests : TestBase
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
        public void Shoud_ReadAllPropertiesCorrectlly_When_DataIsFilled()
        {
            // Act
            var row = _sut.QueryToTable(DbConfig.DbObjectNames.TypesTable)
                .First<dynamic>();

            // Assert
            Assert.AreEqual(row.Bool.GetType(), typeof(bool));
            Assert.AreEqual(row.Byte.GetType(), typeof(byte));
            Assert.AreEqual(row.DTime.GetType(), typeof(DateTime));
            Assert.AreEqual(row.DDecimal.GetType(), typeof(decimal));
            Assert.AreEqual(row.DDouble.GetType(), typeof(double));
            Assert.AreEqual(row.GGuid.GetType(), typeof(Guid));
            Assert.AreEqual(row.String.GetType(), typeof(string));
            Assert.AreEqual(row.Int16.GetType(), typeof(Int16));
            Assert.AreEqual(row.Int32.GetType(), typeof(Int32));
            Assert.AreEqual(row.Int64.GetType(), typeof(Int64));
        }

        [Test]
        public void Should_ReturnAllUsers_When_QueryHasNoOperators()
        {
            // Arrange
            const int expectedItemCount = 3;

            // Act
            var usersFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .ToList();

            // Assert
            Assert.AreEqual(usersFound.Count, expectedItemCount);
        }

        [Test]
        public void Should_FoundUsers_When_PredicateAreEqualPredicateWithAllPrimitiveTypesIsApplied()
        {
            // Act
            var userDenisFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis"))
                .ToList<dynamic>();

            var userAlexFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreEquals("LastName", (string)null))
                .ToList<dynamic>();

            var userKatuaFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreEquals("Id", 2))
                .ToList<dynamic>();

            var alexBirthday = new DateTime(1986, 9, 10);
            var userAlexByBirthDayFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreEquals("BirthDate", alexBirthday))
                .ToList<dynamic>();

            var marriedKatuaFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreEquals("Married", true))
                .ToList<dynamic>();

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.AreEquals(null, "Dzianis"))
                    .ToList<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.AreEquals("", "Dzianis"))
                    .ToList<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.AreEquals("    ", "Dzianis"))
                    .ToList<dynamic>();
            });

            Assert.AreEqual(userDenisFound.Count, 1);
            Assert.AreEqual(userDenisFound.First().UserName, "Dzianis");

            Assert.AreEqual(userAlexFound.Count, 1);
            Assert.AreEqual(userAlexFound.First().UserName, "Alex");
            Assert.AreEqual(userAlexFound.First().LastName, null);

            Assert.AreEqual(userKatuaFound.Count, 1);
            Assert.AreEqual(userKatuaFound.First().UserName, "Katrin");
            Assert.AreEqual(userKatuaFound.First().Id, 2);

            Assert.AreEqual(userAlexByBirthDayFound.Count, 1);
            Assert.AreEqual(userAlexByBirthDayFound.First().UserName, "Alex");
            Assert.AreEqual(userAlexByBirthDayFound.First().BirthDate, alexBirthday);

            Assert.AreEqual(marriedKatuaFound.Count, 1);
            Assert.AreEqual(marriedKatuaFound.First().UserName, "Katrin");
            Assert.AreEqual(marriedKatuaFound.First().Married, true);
        }

        [Test]
        public void Should_FoundUsers_When_PredicateAreNotEqualPredicateWithAllPrimitiveTypesIsApplied()
        {
            // Act
            var usersAlexKatrinFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreNotEquals("UserName", "Dzianis"))
                .ToList<dynamic>();

            var usersDzianisKatrinFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreNotEquals("LastName", (string)null))
                .ToList<dynamic>();

            var usersDzianisAlexFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreNotEquals("Id", 2))
                .ToList<dynamic>();

            var dzianisBirthday = new DateTime(1992, 9, 12);
            var usersWithNoDzianisBithdayFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreNotEquals("BirthDate", dzianisBirthday))
                .ToList<dynamic>();

            var usersNoMarriedFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.AreNotEquals("Married", true))
                .ToList<dynamic>();

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.AreNotEquals(null, "Dzianis"))
                    .ToList<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.AreNotEquals("", "Dzianis"))
                    .ToList<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.AreNotEquals("    ", "Dzianis"))
                    .ToList<dynamic>();
            });

            Assert.AreEqual(usersAlexKatrinFound.Count, 2);
            Assert.IsTrue(usersAlexKatrinFound.All(x => x.UserName != "Dzianis"));

            Assert.AreEqual(usersDzianisKatrinFound.Count, 2);
            Assert.IsTrue(usersDzianisKatrinFound.All(x => x.LastName != null));

            Assert.AreEqual(usersDzianisAlexFound.Count, 2);
            Assert.IsTrue(usersDzianisAlexFound.All(x => x.Id != 2));

            Assert.AreEqual(usersWithNoDzianisBithdayFound.Count, 2);
            Assert.IsTrue(usersWithNoDzianisBithdayFound.All(x => x.BirthDate != dzianisBirthday));

            Assert.AreEqual(usersNoMarriedFound.Count, 2);
            Assert.IsTrue(usersNoMarriedFound.All(x => x.Married != true));
        }

        [Test]
        public void Should_FoundAllUsersWhichUserNameContainsSymbolI_When_PredicateStringContainsIsApplied()
        {
            // Arrange
            const string searchSegment = "i";

            // Act
            var usersFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.StringContains("UserName", searchSegment))
                .ToArray<dynamic>();

            // Assert
            Assert.AreEqual(usersFound.Length, 2);
            Assert.IsTrue(usersFound.Select(x => x.UserName).All(x => x.Contains(searchSegment)));
        }

        [Test]
        public void Should_FoundAllUsersWhichUserNameStartsWithSymbolS_When_PredicateStringStartsWithIsApplied()
        {
            // Arrange
            const string searchSegment = "D";

            // Act
            var usersFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.StringStartsWith("UserName", searchSegment))
                .ToArray<dynamic>();

            // Assert
            Assert.AreEqual(usersFound.Length, 1);
            Assert.AreEqual(usersFound[0].UserName, "Dzianis");
        }

        [Test]
        public void Should_FoundAllUsersWhichUserNameEndsWithSymbolX_When_PredicateStringEndsWithIsApplied()
        {
            // Arrange
            const string searchSegment = "x";

            // Act
            var usersFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.StringEndsWith("UserName", searchSegment))
                .ToArray<dynamic>();

            // Assert
            Assert.AreEqual(usersFound.Length, 1);
            Assert.AreEqual(usersFound[0].UserName, "Alex");
        }

        [Test]
        public void Should_FoundUsers_When_PredicateGreaterThanPredicateWithAllPrimitiveTypesIsApplied()
        {
            // Arrange
            var dateEtalone = new DateTime(1985, 9, 10);

            // Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan(null, 1))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan("", 1))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan("    ", 1))
                    .ToArray<dynamic>();
            });

            var userKatrinFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.GreaterThan("Id", 1))
                .ToList<dynamic>();

            var usersAlexAndDenisFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.GreaterThan("BirthDate", dateEtalone))
                .ToList<dynamic>();

            var usersWhichHasChildrenFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.GreaterThan("ChildCount", 0))
                .ToArray<dynamic>();

            // Assert
            Assert.AreEqual(userKatrinFound.Count, 1);
            Assert.AreEqual(userKatrinFound.First().UserName, "Katrin");
            Assert.Greater(userKatrinFound.First().Id, 1);

            Assert.AreEqual(usersAlexAndDenisFound.Count, 2);
            Assert.AreEqual(usersAlexAndDenisFound.Select(x => x.UserName).Intersect(new dynamic[] { "Dzianis", "Alex" }).Count(), 2);
            Assert.IsTrue(usersAlexAndDenisFound.Select(x => x.BirthDate).All(x => x > dateEtalone));

            Assert.AreEqual(usersWhichHasChildrenFound.Length, 1);
            Assert.AreEqual(usersWhichHasChildrenFound[0].UserName, "Katrin");
            Assert.Greater(usersWhichHasChildrenFound[0].ChildCount, 0);
        }

        [Test]
        public void Should_FoundUsers_When_PredicateGreaterThanOrEqualPredicateWithAllPrimitiveTypesIsApplied()
        {
            // Arrange
            var dateEtalone = new DateTime(1985, 9, 11);

            // Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan(null, 1))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan("", 1))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan("    ", 1))
                    .ToArray<dynamic>();
            });

            var usersAlexKatrinFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.GreaterThanOrEqual("Id", 1))
                .ToArray<dynamic>();

            var usersAlexAndDzianisFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.GreaterThanOrEqual("BirthDate", dateEtalone))
                .ToList<dynamic>();

            var usersWhichHasChildrenFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.GreaterThanOrEqual("ChildCount", 1))
                .ToArray<dynamic>();

            // Assert
            Assert.AreEqual(usersAlexKatrinFound.Length, 2);
            Assert.AreEqual(usersAlexKatrinFound[0].UserName, "Alex");
            Assert.AreEqual(usersAlexKatrinFound[1].UserName, "Katrin");
            Assert.GreaterOrEqual(usersAlexKatrinFound[0].Id, 1);
            Assert.GreaterOrEqual(usersAlexKatrinFound[1].Id, 1);

            Assert.AreEqual(usersAlexAndDzianisFound.Count, 2);
            Assert.AreEqual(usersAlexAndDzianisFound.Select(x => x.UserName).Intersect(new dynamic[] { "Dzianis", "Alex" }).Count(), 2);
            Assert.IsTrue(usersAlexAndDzianisFound.Select(x => x.BirthDate).All(x => x > dateEtalone));

            Assert.AreEqual(usersWhichHasChildrenFound.Length, 1);
            Assert.AreEqual(usersWhichHasChildrenFound[0].UserName, "Katrin");
            Assert.GreaterOrEqual(usersWhichHasChildrenFound[0].ChildCount, 0);
        }

        [Test]
        public void Should_FoundUsers_When_PredicateLessThanPredicateWithAllPrimitiveTypesIsApplied()
        {
            // Arrange
            var dateEtalone = new DateTime(1992, 9, 12);

            // Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan(null, 1))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan("", 1))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan("    ", 1))
                    .ToArray<dynamic>();
            });

            var userDziaisFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.LessThan("Id", 1))
                .ToList<dynamic>();

            var usersAlexAndKatrinFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.LessThan("BirthDate", dateEtalone))
                .ToList<dynamic>();

            var usersWhichHasChildrenFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.LessThan("ChildCount", 2))
                .ToArray<dynamic>();

            // Assert
            Assert.AreEqual(userDziaisFound.Count, 1);
            Assert.AreEqual(userDziaisFound.First().UserName, "Dzianis");
            Assert.Less(userDziaisFound.First().Id, 1);

            Assert.AreEqual(usersAlexAndKatrinFound.Count, 2);
            Assert.AreEqual(usersAlexAndKatrinFound.Select(x => x.UserName).Intersect(new dynamic[] { "Katrin", "Alex" }).Count(), 2);
            Assert.IsTrue(usersAlexAndKatrinFound.Select(x => x.BirthDate).All(x => x < dateEtalone));

            Assert.AreEqual(usersWhichHasChildrenFound.Length, 1);
            Assert.AreEqual(usersWhichHasChildrenFound[0].UserName, "Katrin");
            Assert.Less(usersWhichHasChildrenFound[0].ChildCount, 2);
        }

        [Test]
        public void Should_FoundUsers_When_PredicateLessThanOrEqualPredicateWithAllPrimitiveTypesIsApplied()
        {
            // Arrange
            var dateEtalone = new DateTime(1992, 9, 11);

            // Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan(null, 1))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan("", 1))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.GreaterThan("    ", 1))
                    .ToArray<dynamic>();
            });

            var usersDzianisAlexFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.LessThanOrEqual("Id", 1))
                .ToArray<dynamic>();

            var usersAlexAndKatrinFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.LessThanOrEqual("BirthDate", dateEtalone))
                .ToList<dynamic>();

            var usersWhichHasChildrenFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.LessThanOrEqual("ChildCount", 1))
                .ToArray<dynamic>();

            // Assert
            Assert.AreEqual(usersDzianisAlexFound.Length, 2);
            Assert.AreEqual(usersDzianisAlexFound[0].UserName, "Dzianis");
            Assert.LessOrEqual(usersDzianisAlexFound[0].Id, 1);
            Assert.AreEqual(usersDzianisAlexFound[1].UserName, "Alex");
            Assert.LessOrEqual(usersDzianisAlexFound[1].Id, 1);


            Assert.AreEqual(usersAlexAndKatrinFound.Count, 2);
            Assert.AreEqual(usersAlexAndKatrinFound.Select(x => x.UserName).Intersect(new dynamic[] { "Katrin", "Alex" }).Count(), 2);
            Assert.IsTrue(usersAlexAndKatrinFound.Select(x => x.BirthDate).All(x => x < dateEtalone));

            Assert.AreEqual(usersWhichHasChildrenFound.Length, 1);
            Assert.AreEqual(usersWhichHasChildrenFound[0].UserName, "Katrin");
            Assert.LessOrEqual(usersWhichHasChildrenFound[0].ChildCount, 2);
        }

        [Test]
        public void Should_GroupPredicates_When_LogicalAndOperatorIsApplied()
        {
            // Act
            var userDzianisFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(
                    ExpressionBuilders.Predicates.LogicalAnd(
                        ExpressionBuilders.Predicates.AreEquals("Id", 0),
                        ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis")
                    )
                )
                .FirstOrDefault<dynamic>();

            var noUserFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(
                    ExpressionBuilders.Predicates.LogicalAnd(
                        ExpressionBuilders.Predicates.AreEquals("Id", 1),
                        ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis")
                    )
                )
                .FirstOrDefault<dynamic>();

            // Assert
            Assert.AreEqual(userDzianisFound.UserName, "Dzianis");
            Assert.AreEqual(userDzianisFound.Id, 0);

            Assert.IsNull(noUserFound);

            Assert.Throws<ArgumentNullException>(() =>
            {
                ExpressionBuilders.Predicates.LogicalAnd(
                    null,
                    ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis")
                );
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                ExpressionBuilders.Predicates.LogicalAnd(
                    ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis"),
                    null
                );
            });

            Assert.Throws<ArgumentNullException>(() => ExpressionBuilders.Predicates.LogicalAnd(null));

            Assert.Throws<ArgumentException>(() =>
            {
                ExpressionBuilders.Predicates.LogicalAnd(
                    new[]
                    {
                        ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis"),
                        null
                    }
                );
            });
        }

        [Test]
        public void Should_GroupPredicates_When_LogicalOrOperatorIsApplied()
        {
            // Act
            var usersDzianisAndAlexFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(
                    ExpressionBuilders.Predicates.LogicalOr(
                        ExpressionBuilders.Predicates.AreEquals("Id", 0),
                        ExpressionBuilders.Predicates.AreEquals("UserName", "Alex")
                    )
                )
                .ToArray<dynamic>();

            var noUserFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(
                    ExpressionBuilders.Predicates.LogicalOr(
                        ExpressionBuilders.Predicates.AreEquals("Id", -1),
                        ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis0")
                    )
                )
                .FirstOrDefault<dynamic>();

            // Assert
            Assert.AreEqual(usersDzianisAndAlexFound.Length, 2);
            Assert.IsTrue(usersDzianisAndAlexFound.All(x => x.Id == 0 || x.UserName == "Alex"));

            Assert.IsNull(noUserFound);

            Assert.Throws<ArgumentNullException>(() =>
            {
                ExpressionBuilders.Predicates.LogicalOr(
                    null,
                    ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis")
                );
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                ExpressionBuilders.Predicates.LogicalOr(
                    ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis"),
                    null
                );
            });

            Assert.Throws<ArgumentNullException>(() => ExpressionBuilders.Predicates.LogicalOr(null));

            Assert.Throws<ArgumentException>(() =>
            {
                ExpressionBuilders.Predicates.LogicalOr(
                    new[]
                    {
                        ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis"),
                        null
                    }
                );
            });
        }

        [Test]
        public void Should_UseInCondition_When_CollectionContainsExpressionIsUsed()
        {
            // Arrange
            var ids = new[] {0, 1};

            // Act
            var usersDzianisAndAlexFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.CollectionContains(ids, "Id"))
                .ToArray<dynamic>();

            var usersNoFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.CollectionContains(new []{-1,-2}, "Id"))
                .ToArray<dynamic>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.CollectionContains<object>(null, "Id"))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.CollectionContains(ids, null))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.CollectionContains(ids, string.Empty))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.CollectionContains(ids, "  "))
                    .ToArray<dynamic>();
            });

            // Assert
            Assert.AreEqual(usersDzianisAndAlexFound.Length, 2);
            Assert.IsTrue(usersDzianisAndAlexFound.All(x => ids.Contains((int)x.Id)));

            Assert.IsEmpty(usersNoFound);
        }

        [Test]
        public void Should_UseInCondition_When_CollectionNotContainsExpressionIsUsed()
        {
            // Arrange
            var ids = new[] { 0, 1 };
            var negativeIds = new[] {-1, -2};

            // Act
            var userKartrinFound = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.CollectionNotContains(ids, "Id"))
                .FirstOrDefault<dynamic>();

            var usersAll = _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                .Where(ExpressionBuilders.Predicates.CollectionNotContains(negativeIds, "Id"))
                .ToArray<dynamic>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.CollectionNotContains<object>(null, "Id"))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.CollectionNotContains(ids, null))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.CollectionNotContains(ids, string.Empty))
                    .ToArray<dynamic>();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                _sut.QueryToTable(DbConfig.DbObjectNames.UsersTable)
                    .Where(ExpressionBuilders.Predicates.CollectionNotContains(ids, "  "))
                    .ToArray<dynamic>();
            });

            // Assert
            Assert.IsNotNull(userKartrinFound);
            Assert.IsFalse(ids.Contains((int)userKartrinFound.Id));

            Assert.AreEqual(usersAll.Length, 3);
            Assert.IsTrue(usersAll.All(x => !negativeIds.Contains((int)x.Id)));
        }
    }
}
