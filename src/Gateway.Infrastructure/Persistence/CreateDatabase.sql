IF DB_ID(N'AccountingGateway') IS NULL
    CREATE DATABASE [AccountingGateway];
GO

USE [AccountingGateway];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Connectors')
BEGIN
    CREATE TABLE [dbo].[Connectors]
    (
        [Id]                    UNIQUEIDENTIFIER NOT NULL,
        [BizDomainId]           UNIQUEIDENTIFIER NOT NULL,
        [ApiKey]                NVARCHAR(200)    NOT NULL,
        [LoginTokenEncrypted]   NVARCHAR(500)    NOT NULL,
        [AgentSecretEncrypted]  NVARCHAR(500)    NOT NULL,
        [IsActive]              BIT              NOT NULL,
        [CreatedAt]             DATETIMEOFFSET   NOT NULL,
        [UpdatedAt]             DATETIMEOFFSET   NOT NULL,
        [LastAgentConnectedAt]  DATETIMEOFFSET   NULL,
        [LastAgentSeenAt]       DATETIMEOFFSET   NULL,

        CONSTRAINT [PK_Connectors] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Connectors_ApiKey] ON [dbo].[Connectors] ([ApiKey]);
    CREATE UNIQUE INDEX [IX_Connectors_BizDomainId] ON [dbo].[Connectors] ([BizDomainId]);
END
GO