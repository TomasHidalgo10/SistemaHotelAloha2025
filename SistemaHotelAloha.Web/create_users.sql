
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SistemaHotelAloha')
BEGIN
    CREATE DATABASE SistemaHotelAloha;
END
GO

USE SistemaHotelAloha;
GO

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserName NVARCHAR(100) NOT NULL UNIQUE,
        Email NVARCHAR(200) NULL,
        PasswordHash VARBINARY(64) NOT NULL,
        Salt VARBINARY(16) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO
