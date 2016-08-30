--DROP TABLE [DEMI].[dbo].[Transformations];

CREATE TABLE [DEMI].[dbo].[Transformations](

	Id INT IDENTITY(1,1) NOT NULL,

	Create_Date_Time DATETIME NOT NULL DEFAULT (GETDATE()),
	--CONSTRAINT Transformation_Set_Id FOREIGN KEY (Id) REFERENCES Transformation_Sets(Id),
	Transformation_Set_Id int,
	Transformation_Set_Name VARCHAR(MAX) NULL,
	Target_Column VARCHAR(MAX) NULL,
	Transformation_Rule VARCHAR(MAX) NULL
);

--select * from [DEMI].[dbo].[Transformations]

--DELETE FROM [DEMI].[dbo].[Transformations]