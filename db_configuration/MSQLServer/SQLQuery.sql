USE master
GO
IF NOT EXISTS (
   SELECT name
   FROM sys.databases
   WHERE name = N'DocumentAnalyzerDB'
)
CREATE DATABASE [DocumentAnalyzerDB]
GO

USE [DocumentAnalyzerDB]
-- Create a new table called 'Employees' in schema 'dbo'
-- Drop the table if it already exists
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL
DROP TABLE dbo.Employees
GO
-- Create the table in the specified schema
CREATE TABLE dbo.Employees
(
   Id				INT		NOT NULL   PRIMARY KEY, -- primary key column
   Name      [NVARCHAR](50)  NOT NULL,
   Password  [NVARCHAR](100)  NOT NULL,
   Email     [NVARCHAR](50)  NOT NULL
);
GO

-- Insert rows into table 'Employees'
INSERT INTO dbo.Employees
   ([Id],[Name],[Password],[Email])
VALUES
   ( 1, N'Orlando', N'df79s88sdf', N'orlando@document.com'),
   ( 2, N'Keith', N'dasdf3234asd', N'keith0@document.com'),
   ( 3, N'Donna', N'adfe998g7', N'donna0@document.com'),
   ( 4, N'Janet', N'djkj34h5h393', N'janet1@document.com')
GO

