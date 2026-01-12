-- Database schema and stored procedures for Customer Management System
-- Run this script in your SQL Server instance to create tables and stored procedures

SET NOCOUNT ON;

-- 1. Create Tables
IF OBJECT_ID('dbo.Countries', 'U') IS NOT NULL DROP TABLE dbo.Countries;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;

CREATE TABLE Countries (
    CountryID INT PRIMARY KEY IDENTITY(1,1),
    CountryName NVARCHAR(100) NOT NULL
);

CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Phone NVARCHAR(20),
    CountryID INT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CountryID) REFERENCES Countries(CountryID)
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(20) DEFAULT 'User'
);

-- 2. Seed basic countries
INSERT INTO Countries (CountryName) VALUES ('United States'), ('Canada'), ('United Kingdom');

-- 3. Stored Procedures for Customers (use parameterized queries from app code)
IF OBJECT_ID('dbo.sp_CreateCustomer', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CreateCustomer;
GO
CREATE PROCEDURE dbo.sp_CreateCustomer
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20) = NULL,
    @CountryID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Customers (FirstName, LastName, Email, Phone, CountryID)
    VALUES (@FirstName, @LastName, @Email, @Phone, @CountryID);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

IF OBJECT_ID('dbo.sp_UpdateCustomer', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_UpdateCustomer;
GO
CREATE PROCEDURE dbo.sp_UpdateCustomer
    @CustomerID INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20) = NULL,
    @CountryID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Customers
    SET FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        Phone = @Phone,
        CountryID = @CountryID
    WHERE CustomerID = @CustomerID;
END
GO

IF OBJECT_ID('dbo.sp_SoftDeleteCustomer', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_SoftDeleteCustomer;
GO
CREATE PROCEDURE dbo.sp_SoftDeleteCustomer
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Customers SET IsActive = 0 WHERE CustomerID = @CustomerID;
END
GO

IF OBJECT_ID('dbo.sp_GetCustomerById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetCustomerById;
GO
CREATE PROCEDURE dbo.sp_GetCustomerById
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.*, co.CountryName
    FROM Customers c
    LEFT JOIN Countries co ON c.CountryID = co.CountryID
    WHERE c.CustomerID = @CustomerID;
END
GO

-- Server-side paging/searching for report
IF OBJECT_ID('dbo.sp_GetCustomersPaged', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetCustomersPaged;
GO
CREATE PROCEDURE dbo.sp_GetCustomersPaged
    @PageNumber INT,
    @PageSize INT,
    @Search NVARCHAR(200) = NULL,
    @SortColumn NVARCHAR(50) = 'CreatedAt',
    @SortDirection NVARCHAR(4) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    ;WITH Filtered AS (
        SELECT c.CustomerID, c.FirstName, c.LastName, c.Email, c.Phone, c.IsActive, c.CreatedAt, co.CountryName
        FROM Customers c
        LEFT JOIN Countries co ON c.CountryID = co.CountryID
        WHERE (@Search IS NULL OR (
            c.FirstName LIKE '%' + @Search + '%' OR
            c.LastName LIKE '%' + @Search + '%' OR
            c.Email LIKE '%' + @Search + '%' OR
            co.CountryName LIKE '%' + @Search + '%'
        ))
    )
    SELECT COUNT(1) OVER() AS TotalCount, *
    FROM (
        SELECT *, ROW_NUMBER() OVER (
            ORDER BY
                CASE WHEN @SortColumn = 'FirstName' AND @SortDirection = 'ASC' THEN FirstName END ASC,
                CASE WHEN @SortColumn = 'FirstName' AND @SortDirection = 'DESC' THEN FirstName END DESC,
                CASE WHEN @SortColumn = 'LastName' AND @SortDirection = 'ASC' THEN LastName END ASC,
                CASE WHEN @SortColumn = 'LastName' AND @SortDirection = 'DESC' THEN LastName END DESC,
                CASE WHEN @SortColumn = 'Email' AND @SortDirection = 'ASC' THEN Email END ASC,
                CASE WHEN @SortColumn = 'Email' AND @SortDirection = 'DESC' THEN Email END DESC,
                CASE WHEN @SortColumn = 'CountryName' AND @SortDirection = 'ASC' THEN CountryName END ASC,
                CASE WHEN @SortColumn = 'CountryName' AND @SortDirection = 'DESC' THEN CountryName END DESC,
                CASE WHEN @SortColumn = 'CreatedAt' AND @SortDirection = 'ASC' THEN CreatedAt END ASC,
                CASE WHEN @SortColumn = 'CreatedAt' AND @SortDirection = 'DESC' THEN CreatedAt END DESC
        ) AS RowNum
        FROM Filtered
    ) t
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END
GO

-- Stored procedures for Users
IF OBJECT_ID('dbo.sp_CreateUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CreateUser;
GO
CREATE PROCEDURE dbo.sp_CreateUser
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(MAX),
    @Role NVARCHAR(20) = 'User'
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Users (Username, PasswordHash, Role)
    VALUES (@Username, @PasswordHash, @Role);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

IF OBJECT_ID('dbo.sp_GetUserByUsername', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetUserByUsername;
GO
CREATE PROCEDURE dbo.sp_GetUserByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Users WHERE Username = @Username;
END
GO
