USE [Carte]
GO
/****** Object:  StoredProcedure [dbo].[Files_Insert]    Script Date: 9/27/2022 14:57:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: <Alexandra Johnston>
-- Create date: <08/24/2022>
-- Description: <Proc for inserting into dbo.Files>
-- Code Reviewer: DEVONTAE ARCHIBALD

-- MODIFIED BY: author
-- MODIFIED DATE:12/1/2020
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Files_Insert]
			
			@Name nvarchar(255)
           ,@Url nvarchar(255)
           ,@FileType nvarchar(50)
           ,@CreatedBy int
		   ,@Id int OUTPUT
as
/*------- Test Code------

			DECLARE @Id int = 0
					,@IsDeleted int = 0

			DECLARE @Name nvarchar(255) = 'Test Again'
				   ,@Url nvarchar(255) = 'Another Test'
				   ,@FileType nvarchar(50) = '.jpeg'
				   ,@CreatedBy int = 6

			EXECUTE [dbo].[Files_Insert]

					@Name
				   ,@Url 
				   ,@FileType 
				   ,@CreatedBy
				   ,@IsDeleted OUTPUT
				   ,@Id OUTPUT

				   Select @Id

				   Select *
				   FROM dbo.Files as f inner join dbo.FileTypes as ft
				   ON ft.Id = f.FileTypeId
				  

*/
BEGIN

			DECLARE		@FileTypeId int

			SET			@FileTypeId = (SELECT Id from [dbo].[FileTypes]
			WHERE		[Name] = @FileType)

			INSERT INTO [dbo].[Files]
					   ([Name]
					   ,[Url]
					   ,[FileTypeId]
					   ,[CreatedBy])
				 VALUES
					   (@Name
					   ,@Url
					   ,@FileTypeId
					   ,@CreatedBy)

					   SET @Id =SCOPE_IDENTITY()

END