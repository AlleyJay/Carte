USE [Carte]
GO
/****** Object:  StoredProcedure [dbo].[Menus_UpdateDeleteStatus]    Script Date: 9/27/2022 15:02:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: <Alexandra Johnston>
-- Create date: <09/08/2022>
-- Description: <A proc to update the IsDeleted column to true in dbo.Menus.>
-- Code Reviewer: Ashley Castro

-- MODIFIED BY: author
-- MODIFIED DATE:12/1/2020
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Menus_UpdateDeleteStatus]
			
			@Id int
as
/*------- Test Code------
		
			Declare @Id int = 5

			Execute [dbo].[Menus_UpdateDeleteStatus]

						@Id 

			SELECT * 
			FROM dbo.Menus
			WHERE Id = @Id

*/
BEGIN

			UPDATE [dbo].[Menus]
			   SET [IsDeleted] = 1
				  ,[IsPublished] = 1

			   WHERE Id = @Id

END