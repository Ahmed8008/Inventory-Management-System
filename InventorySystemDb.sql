USE [InventorySystem]
GO
/****** Object:  Table [dbo].[AddCart]    Script Date: 5/13/2024 10:06:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddCart](
	[Cart_id] [int] IDENTITY(1,1) NOT NULL,
	[Stock_id] [int] NULL,
	[SellingPrice] [int] NULL,
	[User_Id] [int] NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_AddCart] PRIMARY KEY CLUSTERED 
(
	[Cart_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AddQuantity]    Script Date: 5/13/2024 10:06:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddQuantity](
	[Q_id] [int] IDENTITY(1,1) NOT NULL,
	[AddQuantity] [int] NULL,
	[User_Id] [int] NULL,
	[Stock_id] [int] NULL,
 CONSTRAINT [PK_AddQuantity] PRIMARY KEY CLUSTERED 
(
	[Q_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Inquiry]    Script Date: 5/13/2024 10:06:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Inquiry](
	[Inquiry_id] [int] IDENTITY(1,1) NOT NULL,
	[EntryDate] [date] NULL,
	[Name] [varchar](50) NULL,
	[Quantity] [int] NULL,
	[RetailPrice] [int] NULL,
	[PurchasingPrice] [int] NULL,
	[SellingPrice] [int] NULL,
	[ExpiryDate] [date] NULL,
	[Investment] [int] NULL,
	[Revenue] [int] NULL,
	[Profit] [int] NULL,
	[User_Id] [int] NULL,
	[Stock_id] [int] NULL,
 CONSTRAINT [PK_Inquiry] PRIMARY KEY CLUSTERED 
(
	[Inquiry_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SoldStock]    Script Date: 5/13/2024 10:06:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SoldStock](
	[Sold_id] [int] IDENTITY(1,1) NOT NULL,
	[SoldTo] [varchar](50) NULL,
	[SoldDate] [date] NULL,
	[Cart_id] [int] NULL,
	[Stock_id] [int] NULL,
	[User_Id] [int] NULL,
	[SellingPrice] [int] NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_SoldStock] PRIMARY KEY CLUSTERED 
(
	[Sold_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StockIn]    Script Date: 5/13/2024 10:06:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StockIn](
	[Stock_id] [int] IDENTITY(1,1) NOT NULL,
	[Quantity] [int] NULL,
	[Name] [varchar](50) NULL,
	[Weight] [int] NULL,
	[RetailPrice] [int] NULL,
	[PurchasingPrice] [int] NULL,
	[ExpiryDate] [varchar](max) NULL,
	[EntryDate] [datetime] NULL,
	[User_Id] [int] NULL,
	[AddQuantity] [int] NULL,
 CONSTRAINT [PK_StockIn] PRIMARY KEY CLUSTERED 
(
	[Stock_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 5/13/2024 10:06:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[User_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Password] [varchar](max) NULL,
	[roles] [varchar](50) NULL,
	[ResetPasswordToken] [nvarchar](max) NULL,
	[ResetPasswordTokenExpiry] [datetime] NULL,
	[UserStatus] [varchar](50) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[User_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
