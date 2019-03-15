# Attachments

## SQL Attachments

> [!NOTE]
> You can execute this SQL on any Sql Server engine, but in samples we will use the express instance. To simplify samples, we can use integrated security.

### Define schema
```sql
CREATE DATABASE SamplesDb
GO

USE SamplesDb
GO

CREATE TABLE tUsers(
    UserName NVARCHAR(50),
    Married BIT
)
GO

CREATE PROCEDURE spFindUsersByUserName
	@uname NVARCHAR(50)
AS
	SELECT * FROM tUsers WHERE UserName=@uname
GO
```

### Fill data
```sql
INSERT INTO tUsers VALUES  
('Dzianis', 0),  
('Alex', 0),  
('Katrin', 1)
```

After this step data of table will be as:

| UserName | Marrried |
|----------| -------- |
| Dzianis  | 0        |
| Alex     | 0        |
| Katrin   | 1        |

### Connection String
```
Server=(localdb)\mssqllocaldb;Database=SamplesDb;Trusted_Connection=True;Integrated Security=True;
```