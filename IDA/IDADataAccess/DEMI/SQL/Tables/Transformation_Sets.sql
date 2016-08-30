--DROP TABLE [DEMI].[dbo].[Transformation_Sets];

CREATE TABLE [DEMI].[dbo].[Transformation_Sets](

	Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Create_Date_Time DATETIME NOT NULL DEFAULT (GETDATE()),
	Name VARCHAR(MAX) NULL,
	Owner_Id INT NULL DEFAULT (0),
);


--SELECT * FROM [DEMI].[dbo].[Transformation_Sets];

--DELETE  FROM [DEMI].[dbo].[Transformation_Sets];