# Quick start

> [!IMPORTANT]
> In all samples we can use database client of **System.Data.SqlClient** package. [More about compatibility.](/compatibility)

## Project preparing
### 1.1. Creating project via dotnet CLI
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
The DatabaseContext object present the context over query operations(for example: query to table, view, stored procedure, function and etc.). For more info about DatabaseContext, see [API](/api/Linq2Shadow.DatabaseContext.html).

> [!NOTE]
> SqlServer connection string is not defined in code here. We can use sample of the connection string from [attachments](attachments.html#connection-string).

```csharp
Func<SqlConnection> connectionFactory = () => new SqlConnection(connectionString);  
var db = new DatabaseContext(connectionFactory);
```

## ShadowRow

Try execute this code:
```csharp
ShadowRow userFound = db.QueryToTable("tUsers").First();
```

As you can see, that the result of query is the ShadowRow object. The ShadowRow is needed to present the entry retrieved from qeury. We can don't know table name(and entity structure) at compilation time, because it can be passed from client ot 3party service, for example.

Sample to get property of user entry :
```charp
string uname = userFound["UserName"]; // "Dzianis"
dynamic userDynamic = userFound;
// dynamic access to row properties is possible
Console.WriteLine(uname == userDynamic.UserName); // True

IEnumerable<object> values = userFound.Values; // ["Dzianis", 0]
int countOfProperties = userFound.Count; // 1
IEnumerable<string> propertyNames = userFound.Keys; // ["UserName", "Married"]
```


##  Query Samples
>[!NOTE]
> In those samples will be used Linq2Shadow.DatabaseContext object created at above.

# [Query to table](#tab/quey-to-table)
```csharp
var usersFound = db.QueryToTable("tUsers")  
    .toList();
```

# [Query to view](#tab/quey-to-view)
Queries to view has the same approach as to table.

# [Query to store procedure](#tab/quey-to-stored-procedure)
```csharp
var spArgs = new Dictionary<string, object>(){ {"UserName", "Dzianis"} };  
var usersFound = db.QueryToStoredProcedure("spFindUsers", spArgs)  
    .toList();

// TODO: types params approach
var spArgsTyped = new { UserName = "Dzianis" };  
var usersFoundSame = db.QueryToStoredProcedure("spFindUsers", spArgsTyped)  
    .toList();
```

# [Query to function](#tab/quey-to-function)
```csharp
var functionArguments = new object[] { "Dzianis" };  
List<Linq2Shadow.ShadowRow> rows = db.QueryToStoredProcedure("fFindUsers", functionArguments)  
    .toList();
```
***

## Paging sample
```csharp
var userFound = db.QueryToTable("tUsers")  
    .Skip(1)  
    .Take(1)  
    .ToList();
```

## Filtering sample
```csharp
var userFound = db.QueryToTable("tUsers")  
    .Where(ExpressionBuilders.Predicates.AreEqual("UserName", "Dzianis"))  
    .ToList();
```

## Count sample
```csharp
var countAl = db.QueryToTable("tUsers").Count();

var countDzianises = db.QueryToTable("tUsers")  
    .Count(ExpressionBuilders.Predicates.AreEqual("UserName", "Dzianis"));

var sameCountDzianises = db.QueryToTable("tUsers")  
    .Where(ExpressionBuilders.Predicates.AreEqual("UserName", "Dzianis"))
    .Count();
```

## Ordering sample
```csharp
var usersOrdered = db.QueryToTable("tUsers")  
    .OrderBy(x => x["UserName"])  
    .ThenByDescending(x => x["Married"])  
    .ToList();
```

## First sample
```csharp
var firstFound = db.QueryToTable("tUsers")
    .First(); // or FirstOrDefault()
```

## Reading via loop
```csharp
foreach(var row in db.QueryToTable("tUsers"))  
{  
    Console.WriteLine(row["UserName"]);
}
```

## Async samples
```csharp
await db.QueryToTable("tUsers").ToListAsync();
await db.QueryToTable("tUsers").FirstAsync();
await db.QueryToTable("tUsers").CountAsync();
```