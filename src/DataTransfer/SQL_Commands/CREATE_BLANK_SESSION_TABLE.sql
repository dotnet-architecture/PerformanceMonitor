USE [PerformanceData]
GO
DROP TABLE IF EXISTS [dbo].[Session]
CREATE TABLE [dbo].[Session](
	[application] [varchar](max) NULL,
	[processs] [varchar](max) NULL,
	[OS] [varchar](max) NULL,
	[sampleRate] [int],
	[sendRate] [int],
	[processorCount] [int],
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED ([id] ASC) 
) 
GO


