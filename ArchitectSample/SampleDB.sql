USE [master]
GO
/****** Object:  Database [Club]    Script Date: 2020/01/08 11:01:07 ******/
CREATE DATABASE [Club]
GO
ALTER DATABASE [Club] SET COMPATIBILITY_LEVEL = 130
GO
ALTER DATABASE [Club] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Club] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Club] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Club] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Club] SET ARITHABORT OFF 
GO
ALTER DATABASE [Club] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Club] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Club] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Club] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Club] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Club] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Club] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Club] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Club] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Club] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Club] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Club] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Club] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Club] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Club] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Club] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Club] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Club] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Club] SET  MULTI_USER 
GO
ALTER DATABASE [Club] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Club] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Club] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Club] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Club] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Club] SET QUERY_STORE = OFF
GO

USE [Club]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

USE [Club]
GO
/****** Object:  Table [dbo].[Club]    Script Date: 2020/01/08 11:01:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Club](
	[ClubID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL
) ON [PRIMARY]
GO

USE [Club]
GO

/****** Object:  UserDefinedTableType [dbo].[ClubType]    Script Date: 2020/01/08 11:18:23 ******/
CREATE TYPE [dbo].[ClubType] AS TABLE(
	[ClubID] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[IsActive] [bit] NULL,
	PRIMARY KEY CLUSTERED 
(
	[ClubID] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
ALTER TABLE [dbo].[Club] ADD  CONSTRAINT [DF_Club_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

USE [master]
GO
ALTER DATABASE [Club] SET  READ_WRITE 
GO

USE [Club]
GO
INSERT INTO [Club] (ClubID, [Name], IsActive)
VALUES
(9, N'�d���f', CONVERT(bit, 'True')),
(10, N'�f�οP', CONVERT(bit, 'True')),
(11, N'�^�e�k', CONVERT(bit, 'False')),
(12, N'���G��', CONVERT(bit, 'False')),
(15, N'�ڶ���޳', CONVERT(bit, 'False')),
(16, N'ù�ɧg', CONVERT(bit, 'False')),
(17, N'�d�Q�S', CONVERT(bit, 'True')),
(18, N'���R��', CONVERT(bit, 'False')),
(19, N'�����Q', CONVERT(bit, 'False')),
(20, N'�i�Φm', CONVERT(bit, 'False')),
(21, N'���{�g', CONVERT(bit, 'True')),
(23, N'���Ͷv', CONVERT(bit, 'False')),
(24, N'�c����', CONVERT(bit, 'False')),
(25, N'�H����', CONVERT(bit, 'True')),
(26, N'����ʹ', CONVERT(bit, 'True')),
(27, N'�x����', CONVERT(bit, 'False')),
(28, N'�G�K��', CONVERT(bit, 'True')),
(29, N'�P�Y�u', CONVERT(bit, 'False')),
(30, N'�L�ɱd', CONVERT(bit, 'False')),
(31, N'���T��', CONVERT(bit, 'True')),
(32, N'�P����', CONVERT(bit, 'False')),
(33, N'�\�a�q', CONVERT(bit, 'True')),
(34, N'���h��', CONVERT(bit, 'True')),
(35, N'���R�y', CONVERT(bit, 'True')),
(36, N'�s����', CONVERT(bit, 'True')),
(37, N'�B�a��', CONVERT(bit, 'True')),
(38, N'�¬��', CONVERT(bit, 'True')),
(39, N'���u��', CONVERT(bit, 'True'))
GO
