CREATE TABLE [dbo].[User]
(
	[UserId] INT NOT NULL PRIMARY KEY IDENTITY, 
	[UserName] VARCHAR(50) NOT NULL, 
	[HashedPassword] CHAR(10) NOT NULL, 
	[LastLoggedOnDatetime] DATETIME NOT NULL
)
