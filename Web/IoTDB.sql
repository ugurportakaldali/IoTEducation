SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IOT_Device](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](5) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[LastAccessDate] [datetime] NULL,
 CONSTRAINT [PK_IOT_Device] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IOT_DeviceData](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DeviceID] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[DeviceDate] [datetime] NOT NULL,
	[Latitude] [decimal](12, 8) NOT NULL,
	[Longitude] [decimal](12, 8) NOT NULL,
	[Humidity] [decimal](5, 2) NOT NULL,
	[Temperature] [decimal](5, 2) NOT NULL,
	[DeviceHealth] [tinyint] NOT NULL,
	[RawData] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_IOT_DeviceData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[IOT_DeviceData]  WITH CHECK ADD  CONSTRAINT [FK_IOT_DeviceData_IOT_Device] FOREIGN KEY([DeviceID])
REFERENCES [dbo].[IOT_Device] ([ID])
GO
ALTER TABLE [dbo].[IOT_DeviceData] CHECK CONSTRAINT [FK_IOT_DeviceData_IOT_Device]
GO
