# Get started

> [!IMPORTANT]
> In all samples we will use database client from the **System.Data.SqlClient** package. For more about engines support, see [compatibility page](/compatibility).

## Project preparing
### Creating project via dotnet CLI
```cmd
dotnet new console -n SampleApp
cd SampleApp
```

### Install Linq2Shadow package
```cmd
dotnet add SampleApp package Linq2Shadow
```

### Install database client package
```cmd
dotnet add . package System.Data.SqlClient
```

## DatabaseContext
The DatabaseContext object present the context over pure connection to make query operations(for example: query to table, view, stored procedure, function and etc.). For more info about DatabaseContext, see [API](/api/Linq2Shadow.DatabaseContext.html).

> [!NOTE]
> SqlServer connection string is not defined in here code. We will use  connection string from [attachments](attachments.html#connection-string).

Instantiating the DatabaseContext object:

```csharp
Func<SqlConnection> connectionFactory = () => new SqlConnection(connectionString);  
var db = new DatabaseContext(connectionFactory);
```

## ShadowRow

Try execute this code:
```csharp
ShadowRow userFound = db.QueryToTable("tUsers").First();
```

As you can see, that the result of query is the ShadowRow object. The The ShadowRow is needed to present the entry retrieved from query. We can don't know table name(and entity structure also) at compilation time, because data source can be passed from the client or 3rd-party service, for example.

Example to get property of entry:
```csharp
string uname = userFound["UserName"]; // "Dzianis"
dynamic userDynamic = userFound;
// dynamic access to row properties is possible
Console.WriteLine(uname == userDynamic.UserName); // True

IEnumerable<object> values = userFound.Values; // ["Dzianis", 0]
int countOfProperties = userFound.Count; // 1
IEnumerable<string> propertyNames = userFound.Keys; // ["UserName", "Married"]
```

##  Query examples
>[!NOTE]
> In those samples will be used DatabaseContext object created at above.

# [Query to table](#tab/quey-to-table)
```csharp
var usersFound = db.QueryToTable("tUsers")  
    .ToList();
```

# [Query to view](#tab/quey-to-view)
Queries to view has the same approach as to table.

# [Query to store procedure](#tab/quey-to-stored-procedure)
```csharp
var spArgs = new Dictionary<string, object>(){ {"UserName", "Dzianis"} };  
var usersFound = db.QueryToStoredProcedure("spFindUsers", spArgs)  
    .ToList();

// TODO: typed params approach
var spArgsTyped = new { UserName = "Dzianis" };  
var usersFoundSame = db.QueryToStoredProcedure("spFindUsers", spArgsTyped)  
    .ToList();
```

# [Query to function](#tab/quey-to-function)
```csharp
var functionArguments = new object[] { "Dzianis" };  
var usersFound = db.QueryToStoredProcedure("fFindUsers", functionArguments)  
    .ToList();
```
***

### Paging sample
```csharp
var userFound = db.QueryToTable("tUsers")  
    .Skip(1)  
    .Take(1)  
    .ToList();
```

### Filtering example

> [!NOTE]
> More about ExpressionBuilders.Predicates class see [below](#predicates).

```csharp
var userFound = db.QueryToTable("tUsers")  
    .Where(ExpressionBuilders.Predicates.AreEqual("UserName", "Dzianis"))  
    .ToList();
```

### Count example
> [!NOTE]
> More about ExpressionBuilders.Predicates class see [below](#predicates).

```csharp
var countAl = db.QueryToTable("tUsers").Count();

var countDzianises = db.QueryToTable("tUsers")  
    .Count(ExpressionBuilders.Predicates.AreEqual("UserName", "Dzianis"));
```

### Ordering example

> [!NOTE]
> Today, assembly has no contains helpers to create the MemberAccess expressions with ShadowRow like `x => x["UserName"]`. You can create him manually, but know that it is planned to future.

```csharp
var usersOrdered = db.QueryToTable("tUsers")  
    .OrderBy(x => x["UserName"])  
    .ThenByDescending(x => x["Marrried"])  
    .ToList();
```

### First example
```csharp
var firstFound = db.QueryToTable("tUsers")
    .First(); // or FirstOrDefault()
```

### Reading via loop
```csharp
// All data are not loaded to memory, here.  
// Data reads like `from row to row`: EnumeratorObj.Move() initiate DbDataReader.Read()  
foreach(var row in db.QueryToTable("tUsers"))  
{  
    Console.WriteLine(row["UserName"]);
}
```

### Select example
```csharp
var usersFound = db.QueryToTable("tUsers")  
    .SelectOnly(new [] { "Married" })  
    .First();
// Output: [0]
// !Attention!: UserName is skipped
```

### Async example
```csharp
await db.QueryToTable("tUsers").ToListAsync();
await db.QueryToTable("tUsers").FirstAsync();
await db.QueryToTable("tUsers").CountAsync();
```

### Predicates

> [!TIP]
> Usage the methods of ExpressionBuilders.Predicates class is recomended way to building predicates. Today all predicate building are similar `x => (int)x == 1`, where `x` is ShadowRow, but in future approach for it can be changed, but interface of ExpressionBuilders.Predicates class will be stable.

For building predicates let's use methods of static ExpressionBuilders.Predicates class. This class has lot of methods like those:

```csharp
// SQL equivalent:  `Age=1`
ExpressionBuilders.Predicates.AreEquals("Age", 1);

// SQL equivalent:  `Age!=1`
ExpressionBuilders.Predicates.AreNotEquals("Age", 1);

// SQL equivalent:  `Age IN [1]`
ExpressionBuilders.Predicates.CollectionContains(new [] { 1 }, "Age");

// SQL equivalent:  `Age NOT IN [1]`
ExpressionBuilders.Predicates.CollectionNotContains(new [] { 1 }, "Age");

// SQL equivalent:  `Age > 1`
ExpressionBuilders.Predicates.GreaterThan("Age", 1);

// SQL equivalent:  `Age >= 1`
ExpressionBuilders.Predicates.GreaterThanOrEqual("Age", 1);

// SQL equivalent:  `Age < 1`
ExpressionBuilders.Predicates.LessThan("Age", 1);

// SQL equivalent:  `Age <= 1`
ExpressionBuilders.Predicates.LessThanOrEqual("Age", 1);

// SQL equivalent:  `Age > 1 AND Age < 2`
ExpressionBuilders.Predicates.LessThanOrEqual(
    ExpressionBuilders.Predicates.GreaterThan("Age", 1),
    ExpressionBuilders.Predicates.LessThan("Age", 2)
);

// SQL equivalent:  `Age = 1 OR Age = 2`
ExpressionBuilders.Predicates.LessThanOrEqual(
    ExpressionBuilders.Predicates.AreEquals("Age", 1),
    ExpressionBuilders.Predicates.AreEquals("Age", 2)
);

// SQL equivalent:  `UserName LIKE '%Dzi%'`
ExpressionBuilders.Predicates.StringContains("UserName", "Dzi");

// SQL equivalent:  `UserName LIKE '%Dzi'`
ExpressionBuilders.Predicates.StringEndsWith("UserName", "Dzi");

// SQL equivalent:  `UserName LIKE 'Dzi%'`
ExpressionBuilders.Predicates.StringStartsWith("UserName", "Dzi");
```

##  Update source

Approaches to update data source:

```csharp
var updateMap = new Dictionary<string, object>(){{"UserName", "SuperDzianis"}};  
var updatePredicate = ExpressionBuiders.Predicates.AreEquals("UserName", "Dzianis");  

// update all rows  
var udpdatedUsersCount = db.Update("tUsers", updateMap);  

// update with predicate(part of data)
var udpdatedUsersCount = db.Update("tUsers", updateMap, updatePredicate);  

// same with typed model
var udpdatedUsersCount = db.Update("tUsers", new { UserName="Dzianis" }, updatePredicate);  

// update asynchronously
var udpdatedUsersCount = db.UpdateAsync("tUsers", new { UserName="Dzianis" }, CancellationToken.None);  
```

## Remove from source

Approaches to remove data from sources like table or view:

```csharp
// Update all records from tUsets table
db.Remove("tUsers");

// Update users with 'Dzianis' UserName
db.Remove("tUsers", ExpressionBuilders.Predicates.AreEquals("UserName", "Dzianis"));
```
> [!NOTE]
> Database Context object has same an asynchronous Remove overloads with **Async** postfix. Asynchronous overloads supports cancellation also.
