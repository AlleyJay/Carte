USE [Carte]
GO
/****** Object:  StoredProcedure [dbo].[Files_Select_ByFileTypeId]    Script Date: 9/27/2022 14:58:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: <Alexandra Johnston>
-- Create date: <08/24/2022>
-- Description: <A proc to select from dbo.Files by FileTypeId (Paginated).>
-- Code Reviewer:DEVONTAE ARCHIBALD

-- MODIFIED BY: author
-- MODIFIED DATE:12/1/2020
-- Code Reviewer:
-- Note:
-- =============================================
ALTER proc [dbo].[Files_Select_ByFileTypeId]

			
		    @PageIndex int 
		   ,@PageSize int
		   ,@FileTypeId int

as
 /* ----------- Test Code --------

			EXECUTE [dbo].[Files_Select_ByFileTypeId]
			
			@PageIndex = 0
	        ,@PageSize = 3
			,@FileTypeId = 1

 */
BEGIN

	DECLARE @offset int = @PageIndex * @PageSize

			SELECT f.[Id]
				  ,f.[Name]
				  ,f.[Url]
				  ,ft.Id as FileTypeId
				  ,ft.Name as FileTypeName
				  ,f.[CreatedBy]
				  ,f.[DateCreated]
				  ,f.[IsDeleted]
				  ,TotalCount = COUNT(1) OVER()

			FROM [dbo].[Files] as f inner join dbo.FileTypes as ft
			on f.FileTypeId = ft.Id
			ORDER BY Id
			
			OFFSET @offSet Rows
			Fetch Next @PageSize Rows ONLY

END


