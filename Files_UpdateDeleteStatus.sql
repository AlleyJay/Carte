USE [Carte]
GO
/****** Object:  StoredProcedure [dbo].[Files_UpdateDeleteStatus]    Script Date: 9/27/2022 14:59:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: <Alexandra Johnston>
-- Create date: <08/24/2022>
-- Description: <A proc to update the IsDeleted column to true in dbo.Files.>
-- Code Reviewer:DEVONTAE ARCHIBALD

-- MODIFIED BY: author
-- MODIFIED DATE:12/1/2020
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Files_UpdateDeleteStatus]
			
			@Id int
as
/*------- Test Code------
		
			Declare @Id int = 2

			Execute [dbo].[Files_UpdateDeleteStatus]

						@Id 

			SELECT * 
			FROM dbo.Files
			WHERE Id = @Id

*/
BEGIN

			UPDATE [dbo].[Files]
			   SET [IsDeleted] = 1

			   WHERE Id = @Id

END

