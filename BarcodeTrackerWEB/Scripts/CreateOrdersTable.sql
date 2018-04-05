﻿/*
Deployment script for 97B67F8402D6019D1C4BB83821C031DD_DIO 2015\PROJECTS\ICT3714_PROJECTDEMO_45957193\ICT3714_PROJECTDEMO_45957193\APP_DATA\MAINDB.MDF

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "97B67F8402D6019D1C4BB83821C031DD_DIO 2015\PROJECTS\ICT3714_PROJECTDEMO_45957193\ICT3714_PROJECTDEMO_45957193\APP_DATA\MAINDB.MDF"
:setvar DefaultFilePrefix "97B67F8402D6019D1C4BB83821C031DD_DIO 2015\PROJECTS\ICT3714_PROJECTDEMO_45957193\ICT3714_PROJECTDEMO_45957193\APP_DATA\MAINDB.MDF_"
:setvar DefaultDataPath "C:\Users\Rogan\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"
:setvar DefaultLogPath "C:\Users\Rogan\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Rename refactoring operation with key  is skipped, element [dbo].[Orders].[Id] (SqlSimpleColumn) will not be renamed to OrderId';


GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
/*
The column [dbo].[Orders].[Id] is being dropped, data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [dbo].[Orders]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Orders] (
    [OrderId]             INT        IDENTITY (1, 1) NOT NULL,
    [CustomerId]          INT        NOT NULL,
    [SaleRepId]           INT        NOT NULL,
    [OrderDate]           DATE       NOT NULL,
    [PurchaseOrderNumber] NCHAR (10) NOT NULL,
    [TotalAmount]         MONEY      NOT NULL,
    [Status]              INT        NOT NULL,
    PRIMARY KEY CLUSTERED ([OrderId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Orders])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Orders] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Orders] ([OrderId], [CustomerId], [SaleRepId], [OrderDate], [PurchaseOrderNumber], [TotalAmount], [Status])
        SELECT   [OrderId],
                 [CustomerId],
                 [SaleRepId],
                 [OrderDate],
                 [PurchaseOrderNumber],
                 [TotalAmount],
                 [Status]
        FROM     [dbo].[Orders]
        ORDER BY [OrderId] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Orders] OFF;
    END

DROP TABLE [dbo].[Orders];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Orders]', N'Orders';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT N'The transacted portion of the database update succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT N'The transacted portion of the database update failed.'
GO
DROP TABLE #tmpErrors
GO
PRINT N'Update complete.';


GO
