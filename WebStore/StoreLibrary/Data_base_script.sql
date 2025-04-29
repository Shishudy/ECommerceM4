IF DB_ID('StoreDB') IS NOT NULL
BEGIN
    ALTER DATABASE [StoreDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; -- Forcefully close connections
    DROP DATABASE [StoreDB]; -- Drops the existing database
END
GO

CREATE DATABASE [StoreDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'web_store', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\web_store.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'web_store_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\web_store_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [StoreDB] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [StoreDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [StoreDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [StoreDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [StoreDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [StoreDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [StoreDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [StoreDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [StoreDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [StoreDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [StoreDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [StoreDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [StoreDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [StoreDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [StoreDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [StoreDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [StoreDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [StoreDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [StoreDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [StoreDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [StoreDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [StoreDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [StoreDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [StoreDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [StoreDB] SET RECOVERY FULL 
GO
ALTER DATABASE [StoreDB] SET  MULTI_USER 
GO
ALTER DATABASE [StoreDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [StoreDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [StoreDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [StoreDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [StoreDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [StoreDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'StoreDB', N'ON'
GO
ALTER DATABASE [StoreDB] SET QUERY_STORE = OFF
GO
USE [StoreDB]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[pk_address] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[fk_user] [nvarchar](500) NULL,
	[country] [nvarchar](50) NOT NULL,
	[phone] [nvarchar](50) NOT NULL,
	[email] [nvarchar](50) NOT NULL,
	[full_address] [nvarchar](50) NOT NULL,
	[name] [nvarchar](50) NULL,
	[toggle] [bit] NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[pk_address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Campaign]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Campaign](
	[pk_campaign] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[date_start] [date] NOT NULL,
	[date_end] [date] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Campaign] PRIMARY KEY CLUSTERED 
(
	[pk_campaign] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CampaignImage]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CampaignImage](
	[fk_campaign] [int] NOT NULL,
	[fk_image] [int] NOT NULL,
 CONSTRAINT [PK_CampaignImage] PRIMARY KEY CLUSTERED 
(
	[fk_campaign] ASC,
	[fk_image] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CampaignProduct]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CampaignProduct](
	[fk_campaign] [int] NOT NULL,
	[fk_product] [int] NOT NULL,
	[discount] [float] NOT NULL,
 CONSTRAINT [PK_CampaignProduct] PRIMARY KEY CLUSTERED 
(
	[fk_campaign] ASC,
	[fk_product] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[card]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[card](
	[pk_card] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[fk_user] [nvarchar](500) NOT NULL,
	[number] [bigint] NULL,
	[expiration] [date] NULL,
	[code] [int] NULL,
	[name] [nvarchar](50) NULL,
	[toogle] [bit] NULL,
 CONSTRAINT [PK_card] PRIMARY KEY CLUSTERED 
(
	[pk_card] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[pk_category] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[pk_category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Favourite]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Favourite](
	[fk_product] [int] NOT NULL,
	[fk_user] [nvarchar](500) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Image]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image](
	[pk_image] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[path_img] [nvarchar](50) NOT NULL,
	[name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[pk_image] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoice]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoice](
	[pk_invoice] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[fk_address_invoice] [int] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[nif] [int] NOT NULL,
	[date_invoice] [date] NOT NULL,
	[paypall_confirmation] [nvarchar](50) NULL,
	[amount] [float] NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[pk_invoice] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[pk_product] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[fk_image] [int] NOT NULL,
	[ean] [nvarchar](50) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[price] [float] NOT NULL,
	[stock] [int] NOT NULL,
	[toggle] [bit] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[pk_product] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[fk_product] [int] NOT NULL,
	[fk_category] [int] NOT NULL,
 CONSTRAINT [PK_ProductCategory_1] PRIMARY KEY CLUSTERED 
(
	[fk_product] ASC,
	[fk_category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductImage]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductImage](
	[fk_product] [int] NOT NULL,
	[fk_image] [int] NOT NULL,
 CONSTRAINT [PK_ProductImage] PRIMARY KEY CLUSTERED 
(
	[fk_product] ASC,
	[fk_image] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Purchase]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Purchase](
	[pk_purchase] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[fk_user] [nvarchar](500) NOT NULL,
	[fk_invoice] [int] NULL,
	[fk_review] [int] NULL,
	[date_purchase] [date] NULL,
	[status] [nvarchar](50) NOT NULL,
	[fk_address_shipment] [int] NULL,
	[fk_card] [int] NULL,
 CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED 
(
	[pk_purchase] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseProduct]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseProduct](
	[fk_purchase] [int] NOT NULL,
	[fk_product] [int] NOT NULL,
	[status] [nvarchar](50) NULL,
	[qtt] [int] NOT NULL,
	[fk_review] [int] NULL,
	[price] [float] NOT NULL,
 CONSTRAINT [PK_PurchaseProduct] PRIMARY KEY CLUSTERED 
(
	[fk_purchase] ASC,
	[fk_product] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Review]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Review](
	[pk_review] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[data_review] [date] NOT NULL,
	[stars] [int] NOT NULL,
	[comment] [nvarchar](50) NOT NULL,
	[toggle] [bit] NOT NULL,
 CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED 
(
	[pk_review] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewImages]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewImages](
	[fk_review] [int] NOT NULL,
	[fk_image] [int] NOT NULL,
 CONSTRAINT [PK_ReviewImages] PRIMARY KEY CLUSTERED 
(
	[fk_review] ASC,
	[fk_image] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[pk_user] [int] IDENTITY(1,1) NOT NULL, -- Added IDENTITY
	[fk_user] [nvarchar](500) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[pk_user] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserImage]    Script Date: 27/04/2025 18:53:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserImage](
	[fk_image] [int] NOT NULL,
	[fk_user] [nvarchar](500) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CampaignImage]  WITH CHECK ADD  CONSTRAINT [FK_CampaignImage_Campaign] FOREIGN KEY([fk_campaign])
REFERENCES [dbo].[Campaign] ([pk_campaign])
GO
ALTER TABLE [dbo].[CampaignImage] CHECK CONSTRAINT [FK_CampaignImage_Campaign]
GO
ALTER TABLE [dbo].[CampaignImage]  WITH CHECK ADD  CONSTRAINT [FK_CampaignImage_Image] FOREIGN KEY([fk_image])
REFERENCES [dbo].[Image] ([pk_image])
GO
ALTER TABLE [dbo].[CampaignImage] CHECK CONSTRAINT [FK_CampaignImage_Image]
GO
ALTER TABLE [dbo].[CampaignProduct]  WITH CHECK ADD  CONSTRAINT [FK_CampaignProduct_Campaign] FOREIGN KEY([fk_campaign])
REFERENCES [dbo].[Campaign] ([pk_campaign])
GO
ALTER TABLE [dbo].[CampaignProduct] CHECK CONSTRAINT [FK_CampaignProduct_Campaign]
GO
ALTER TABLE [dbo].[CampaignProduct]  WITH CHECK ADD  CONSTRAINT [FK_CampaignProduct_Product] FOREIGN KEY([fk_product])
REFERENCES [dbo].[Product] ([pk_product])
GO
ALTER TABLE [dbo].[CampaignProduct] CHECK CONSTRAINT [FK_CampaignProduct_Product]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_Address] FOREIGN KEY([fk_address_invoice])
REFERENCES [dbo].[Address] ([pk_address])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_Address]
GO
ALTER TABLE [dbo].[ProductCategory]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategory_Category] FOREIGN KEY([fk_category])
REFERENCES [dbo].[Category] ([pk_category])
GO
ALTER TABLE [dbo].[ProductCategory] CHECK CONSTRAINT [FK_ProductCategory_Category]
GO
ALTER TABLE [dbo].[ProductCategory]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategory_Product] FOREIGN KEY([fk_product])
REFERENCES [dbo].[Product] ([pk_product])
GO
ALTER TABLE [dbo].[ProductCategory] CHECK CONSTRAINT [FK_ProductCategory_Product]
GO
ALTER TABLE [dbo].[ProductImage]  WITH CHECK ADD  CONSTRAINT [FK_ProductImage_Image] FOREIGN KEY([fk_image])
REFERENCES [dbo].[Image] ([pk_image])
GO
ALTER TABLE [dbo].[ProductImage] CHECK CONSTRAINT [FK_ProductImage_Image]
GO
ALTER TABLE [dbo].[ProductImage]  WITH CHECK ADD  CONSTRAINT [FK_ProductImage_Product] FOREIGN KEY([fk_product])
REFERENCES [dbo].[Product] ([pk_product])
GO
ALTER TABLE [dbo].[ProductImage] CHECK CONSTRAINT [FK_ProductImage_Product]
GO
ALTER TABLE [dbo].[Purchase]  WITH CHECK ADD  CONSTRAINT [FK_Purchase_Address] FOREIGN KEY([fk_address_shipment])
REFERENCES [dbo].[Address] ([pk_address])
GO
ALTER TABLE [dbo].[Purchase] CHECK CONSTRAINT [FK_Purchase_Address]
GO
ALTER TABLE [dbo].[Purchase]  WITH CHECK ADD  CONSTRAINT [FK_Purchase_card] FOREIGN KEY([fk_card])
REFERENCES [dbo].[card] ([pk_card])
GO
ALTER TABLE [dbo].[Purchase] CHECK CONSTRAINT [FK_Purchase_card]
GO
ALTER TABLE [dbo].[Purchase]  WITH CHECK ADD  CONSTRAINT [FK_Purchase_Invoice] FOREIGN KEY([fk_invoice])
REFERENCES [dbo].[Invoice] ([pk_invoice])
GO
ALTER TABLE [dbo].[Purchase] CHECK CONSTRAINT [FK_Purchase_Invoice]
GO
ALTER TABLE [dbo].[Purchase]  WITH CHECK ADD  CONSTRAINT [FK_Purchase_Review] FOREIGN KEY([fk_review])
REFERENCES [dbo].[Review] ([pk_review])
GO
ALTER TABLE [dbo].[Purchase] CHECK CONSTRAINT [FK_Purchase_Review]
GO
ALTER TABLE [dbo].[PurchaseProduct]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseProduct_Product] FOREIGN KEY([fk_product])
REFERENCES [dbo].[Product] ([pk_product])
GO
ALTER TABLE [dbo].[PurchaseProduct] CHECK CONSTRAINT [FK_PurchaseProduct_Product]
GO
ALTER TABLE [dbo].[PurchaseProduct]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseProduct_Purchase] FOREIGN KEY([fk_purchase])
REFERENCES [dbo].[Purchase] ([pk_purchase])
GO
ALTER TABLE [dbo].[PurchaseProduct] CHECK CONSTRAINT [FK_PurchaseProduct_Purchase]
GO
ALTER TABLE [dbo].[PurchaseProduct]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseProduct_Review] FOREIGN KEY([fk_review])
REFERENCES [dbo].[Review] ([pk_review])
GO
ALTER TABLE [dbo].[PurchaseProduct] CHECK CONSTRAINT [FK_PurchaseProduct_Review]
GO
ALTER TABLE [dbo].[ReviewImages]  WITH CHECK ADD  CONSTRAINT [FK_ReviewImages_Image] FOREIGN KEY([fk_image])
REFERENCES [dbo].[Image] ([pk_image])
GO
ALTER TABLE [dbo].[ReviewImages] CHECK CONSTRAINT [FK_ReviewImages_Image]
GO
ALTER TABLE [dbo].[ReviewImages]  WITH CHECK ADD  CONSTRAINT [FK_ReviewImages_Review] FOREIGN KEY([fk_review])
REFERENCES [dbo].[Review] ([pk_review])
GO
ALTER TABLE [dbo].[ReviewImages] CHECK CONSTRAINT [FK_ReviewImages_Review]
GO
USE [master]
GO
ALTER DATABASE [StoreDB] SET  READ_WRITE 
GO
