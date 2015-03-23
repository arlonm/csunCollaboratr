USE [master]
GO
/****** Object:  Database [DB_80003_devonly]    Script Date: 2/25/2015 7:04:53 PM ******/
CREATE DATABASE [DB_80003_devonly]
 CONTAINMENT = NONE
 /*ON  PRIMARY 
( NAME = N'DB_80003_devonly_data', FILENAME = N'e:\sqldata\DB_80003_devonly_data.mdf' , SIZE = 2560KB , MAXSIZE = 512000KB , FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DB_80003_devonly_log', FILENAME = N'f:\sqllog\DB_80003_devonly_log.ldf' , SIZE = 1024KB , MAXSIZE = 1024000KB , FILEGROWTH = 10%)*/
GO
ALTER DATABASE [DB_80003_devonly] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DB_80003_devonly].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DB_80003_devonly] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET ARITHABORT OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [DB_80003_devonly] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DB_80003_devonly] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DB_80003_devonly] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DB_80003_devonly] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DB_80003_devonly] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DB_80003_devonly] SET  MULTI_USER 
GO
ALTER DATABASE [DB_80003_devonly] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DB_80003_devonly] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DB_80003_devonly] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DB_80003_devonly] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [DB_80003_devonly]
GO
/****** Object:  StoredProcedure [dbo].[AccountCreate]    Script Date: 2/25/2015 7:04:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AccountCreate]
(
	@Username varchar(64),
	@Email varchar(128),
	@Password varchar(128),
	@Salt varchar(128),
	@IPADDRESS VARCHAR(64)
)
AS
BEGIN
	DECLARE @SUCCESS BIT
	DECLARE @MESSAGE VARCHAR(256)
	DECLARE @EXISTING_EMAIL VARCHAR(128)
	DECLARE @EXISTING_USERNAME VARCHAR(64)
	SELECT
		@EXISTING_EMAIL = A.Email,
		@EXISTING_USERNAME = A.Username
	FROM
		CRAcct A 
	WHERE
		(   (A.USERNAME = @Username)
			OR A.EMAIL = @Email)

	
	IF @EXISTING_EMAIL IS NOT NULL						--Fail
	BEGIN
		SET @SUCCESS = 0
		SET @MESSAGE = 'Email already registered'
	END
	ELSE IF @EXISTING_USERNAME IS NOT NULL				--Fail
	BEGIN
		set @SUCCESS = 0 
		set @MESSAGE = 'Username already registered'
	END
	ELSE												--Succeed
	BEGIN
		INSERT INTO CRAcct
		(USERNAME, EMAIL, DATECREATED, ACTIVE, LOCKED, DateLastLoggedIn, DateLastAttempt, LoginFailures,
		LastIPAddress, [Password], [SALT])
		VALUES
		(@Username, @Email, GETDATE(), 1, 0, NULL, NULL, 0, @IPADDRESS, @Password, @Salt)

		SET @SUCCESS = 1
		SET @MESSAGE = 'Account successfully added'
	END

	select
		@SUCCESS AS [SUCCESS],
		@MESSAGE AS [MESSAGE]

	--Table [1]
	SELECT
		*
	FROM
	CRAcct A
	WHERE
		A.AccountId = @@IDENTITY


END
GO
/****** Object:  StoredProcedure [dbo].[AccountLogin]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AccountLogin]
(
	@LoginName varchar(128),
	@Password varchar(256)
)
AS
BEGIN

	DECLARE @SUCCESS BIT
	SET @SUCCESS = 1

	DECLARE @MESSAGE VARCHAR(256)

	DECLARE @INCREMENTFAILURES BIT
	SET @INCREMENTFAILURES = 0 

	DECLARE @CURRENT_USERNAME	VARCHAR(64)
	DECLARE @CURRENT_PASSWORD	VARCHAR(256)
	DECLARE @CURRENT_ACCTID		INT
	DECLARE @DATE_LAST_ATTEMPT	DATETIME
	DECLARE @ACTIVE				BIT
	DECLARE @LOCKED				BIT

	SELECT
		@CURRENT_USERNAME = A.Username,
		@CURRENT_PASSWORD = A.Password,
		@CURRENT_ACCTID = A.AccountId,
		@DATE_LAST_ATTEMPT = A.DateLastAttempt,
		@ACTIVE = A.Active,
		@LOCKED = A.Locked
	FROM
		CRAcct A 
	WHERE
		(
			A.USERNAME = @LOGINNAME
			OR
			A.Email = @LoginName
		) 
	IF @CURRENT_ACCTID IS NULL
	BEGIN
		SET @MESSAGE = 'Invalid credentials'
		SET @SUCCESS = 0
	END
	ELSE IF @ACTIVE = 0 
	BEGIN
		SET @MESSAGE = 'Account is currently inactive. Contact support for help'
		SET @SUCCESS = 0
	END
	ELSE IF @LOCKED = 1
	BEGIN
		SET @MESSAGE = 'Account has been locked'
		SET @SUCCESS = 0
	END
	ELSE IF DATEDIFF(SECOND, @DATE_LAST_ATTEMPT, GETDATE()) < 1
	BEGIN
		SET @MESSAGE = 'Attempting to log in too soon.'
		SET @SUCCESS = 0
		SET @INCREMENTFAILURES = 1
	END

	IF @SUCCESS = 1
	BEGIN
		IF @CURRENT_PASSWORD = @Password
		BEGIN
			SET @SUCCESS = 1
			SET @MESSAGE= 'Success'

			UPDATE CRAcct
			SET LoginFailures = 0,
				DateLastAttempt = GETDATE(),
				DateLastLoggedIn = GETDATE()
			WHERE
				AccountId = @CURRENT_ACCTID
		END
		ELSE
		BEGIN
			SET @SUCCESS = 0
			SET @MESSAGE = 'Invalid credentials'

		END
	END
	
	IF @SUCCESS = 0 AND @INCREMENTFAILURES = 1
	BEGIN
			UPDATE CRAcct
			SET LoginFailures = LoginFailures + 1,
				DateLastAttempt = GETDATE()
			WHERE
				AccountId = @CURRENT_ACCTID
	END
	ELSE IF @SUCCESS = 0 AND @INCREMENTFAILURES = 0
	BEGIN
			UPDATE CRAcct
			SET DateLastAttempt = GETDATE()
			WHERE
				AccountId = @CURRENT_ACCTID
	END

	SELECT --Table[0]
	@SUCCESS as [SUCCESS],
	@MESSAGE as [MESSAGE]

	SELECT --Table[1]
	*
	from
	CRAcct a
	where
	a.Accountid = @CURRENT_ACCTID

END
GO
/****** Object:  StoredProcedure [dbo].[AccountRead]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AccountRead] @ACCOUNTID INT
AS
BEGIN

	SELECT
	*
	FROM
	CRAcct A
	WHERE
		A.AccountId = @ACCOUNTID


END

GO
/****** Object:  StoredProcedure [dbo].[AccountReadAll]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AccountReadAll]
AS
BEGIN

	SELECT
	*
	FROM
	CRAcct

END

GO
/****** Object:  StoredProcedure [dbo].[AccountUpdate]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AccountUpdate]
(
	@AccountId int,
	@Username varchar(64),
	@Email varchar(128),
	@Active bit,
	@Locked bit,
	@LoginFailures tinyint,
	@password varchar(128),
	@salt varchar(128)

)
AS
BEGIN

	DECLARE @SUCCESS BIT
	DECLARE @MESSAGE VARCHAR(256)

	IF EXISTS(SELECT 1 FROM CRAcct A WHERE A.ACCOUNTID = @AccountId)
	BEGIN
		UPDATE CRAcct
		SET
			Username = @Username,
			Email = @Email,
			Active = @ACTIVE,
			LOCKED = @LOCKED,
			LoginFailures = @LoginFailures,
			[Password] = @password,
			Salt = @salt

		WHERE
			AccountId = @AccountId

		SET @SUCCESS = 1
		SET @MESSAGE = 'Account updated'
	END
	ELSE
	BEGIN
		SET @SUCCESS = 0
		SET @MESSAGE = 'Account does not exist'
	END
	SELECT
		@SUCCESS AS [SUCCESS],
		@MESSAGE AS [MESSAGE]

	EXEC AccountRead @ACCOUNTID = @ACCOUNTID
END
GO
/****** Object:  StoredProcedure [dbo].[DoesEmailExists]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DoesEmailExists](
	@Email varchar(256)
)
AS
BEGIN
	SELECT
	count(a.Email) as [RESULT]
	FROM
	CRAcct a
	where
	a.Email = LTRIM(RTRIM(@Email))


END

GO
/****** Object:  StoredProcedure [dbo].[GenerateEmailResetToken]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GenerateEmailResetToken] @email varchar(256)
AS
BEGIN
	
	IF EXISTS(SELECT 1 FROM CRAcct A WHERE A.Email = @Email)
	BEGIN
		UPDATE CRAcct SET PasswordResetToken = REPLACE(NEWID(), '-', '')
		WHERE
		EMail = @email
	END

	SELECT 
		PasswordResetToken 
	FROM
		CRAcct 
	WHERE
		Email = @email
END

GO
/****** Object:  StoredProcedure [dbo].[PasswordRecoveryAdd]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PasswordRecoveryAdd]
(
	@RecoveryString varchar(64),
	@Email varchar(256)
)
AS
BEGIN
	
	DECLARE @SUCCESS bit
	DECLARE @MESSAGE varchar(256)

	insert into CRPasswordRecovery
	(RecoveryString, Email, Pending)
	VALUES(@RecoveryString, @Email, 1)

	SET @SUCCESS = 1
	set @MESSAGE = ''

	SELECT @SUCCESS as [success],
	@MESSAGE as [message]

END

GO
/****** Object:  StoredProcedure [dbo].[SaltReadByLoginName]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaltReadByLoginName]
	@LoginName varchar(256)
AS
	
	select
		[salt]
	from
		CRAcct a
	where
		a.Username = @LoginName
		OR
		a.Email = @LoginName

RETURN 0
GO
/****** Object:  Table [dbo].[CRAcct]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CRAcct](
	[AccountId] [int] IDENTITY(1425,1) NOT NULL,
	[Username] [varchar](64) NOT NULL,
	[Email] [varchar](128) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[Active] [bit] NOT NULL,
	[Locked] [bit] NOT NULL,
	[DateLastLoggedIn] [datetime] NULL,
	[DateLastAttempt] [datetime] NULL,
	[LoginFailures] [tinyint] NOT NULL,
	[LastIPAddress] [varchar](32) NOT NULL,
	[Password] [varchar](128) NOT NULL,
	[Salt] [varchar](128) NOT NULL,
	[PasswordResetToken] [varchar](64) NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CRPasswordRecovery]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CRPasswordRecovery](
	[CRRecoveryID] [bigint] IDENTITY(1,1) NOT NULL,
	[RecoveryString] [varchar](64) NOT NULL,
	[Email] [varchar](256) NOT NULL,
	[Pending] [bit] NOT NULL,
 CONSTRAINT [PK_CRPasswordRecovery] PRIMARY KEY CLUSTERED 
(
	[CRRecoveryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CRRooms]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CRRooms](
	[RoomId] [int] IDENTITY(1,1) NOT NULL,
	[RoomName] [varchar](128) NOT NULL,
	[RoomDescription] [varchar](512) NULL,
	[DateCreated] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
 CONSTRAINT [PK_CRRooms] PRIMARY KEY CLUSTERED 
(
	[RoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RoomOwnerMap]    Script Date: 2/25/2015 7:04:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomOwnerMap](
	[RoomOwnerMapId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[RoomId] [int] NOT NULL,
 CONSTRAINT [PK_RoomOwnerMap] PRIMARY KEY CLUSTERED 
(
	[RoomOwnerMapId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_BtrxAcct_Email]    Script Date: 2/25/2015 7:04:54 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_BtrxAcct_Email] ON [dbo].[CRAcct]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_BtrxAcct_Username]    Script Date: 2/25/2015 7:04:54 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_BtrxAcct_Username] ON [dbo].[CRAcct]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CRAcct] ADD  CONSTRAINT [DF_CRAcct_PasswordResetToken]  DEFAULT ('') FOR [PasswordResetToken]
GO
USE [master]
GO
ALTER DATABASE [DB_80003_devonly] SET  READ_WRITE 
GO
