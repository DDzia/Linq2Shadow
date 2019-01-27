CREATE TABLE dbo.tUsers (
	Id INTEGER,
	UserName NVARCHAR(128),
	BirthDate DATE,
    LastName NVARCHAR(128) NULL,
    Married BIT,
    ChildCount INTEGER
)