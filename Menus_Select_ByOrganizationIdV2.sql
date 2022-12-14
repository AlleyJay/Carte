USE [Carte]
GO
/****** Object:  StoredProcedure [dbo].[Menus_Select_ByOrganizationIdV2]    Script Date: 9/27/2022 15:01:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: Alexandra Johnston
-- Create date: 09/14/2022
-- Description: A proc to select from dbo.Menus by CreatedBy (Paginated).
-- I was asked to create another version of this proc to bring in tags for menu items for filtering.
-- Code Reviewer: Ashley Castro

-- MODIFIED BY: author
-- MODIFIED DATE:12/1/2020
-- Code Reviewer:
-- Note:
-- =============================================
ALTER proc [dbo].[Menus_Select_ByOrganizationIdV2]

			@OrganizationId int
		   ,@PageIndex int 
		   ,@PageSize int

as
 /* ----------- Test Code --------

			DECLARE @OrganizationId int = 8;

			EXECUTE [dbo].[Menus_Select_ByOrganizationIdV2] 
			 @OrganizationId
			,@PageIndex = 0
	        ,@PageSize = 3

 */
BEGIN

			DECLARE @offset int = @PageIndex * @PageSize

			SELECT m.[Id]
				  ,m.[Name]
				  ,m.[OrganizationId]
				  ,m.[Description]
				  ,Image = (
							SELECT f.Url
							FROM [dbo].[Files] as f 
							WHERE m.FileId = f.Id
							)
				  ,m.[CreatedBy]
				  ,m.[ModifiedBy]
				  ,m.[DateCreated]
				  ,m.[DateModified]
				  ,m.[IsDeleted]
				  ,m.[IsPublished]
				  ,m.[StartDate]
				  ,m.[EndDate]
				  ,m.[StartTime]
				  ,m.[EndTime]
				  ,m.[TimeZoneId]
				  ,MenuItems = ( 
								SELECT mi.Name as Name
									  ,mi.Description as Description
									  ,mi.ImageUrl as ImageUrl
									  ,mi.UnitCost as Price
									  ,Tags = (
												SELECT t.Id
													  ,t.Name as Name
												FROM [dbo].[Tags] as t inner join [dbo].[MenuItemTags] as mit
												ON mit.TagId = t.Id
												WHERE mi.Id = mit.MenuItemId
												FOR JSON AUTO)
								FROM [dbo].[MenuItems] as mi inner join [dbo].[MenuElements] as me
								ON me.MenuItemId = mi.Id
								WHERE me.MenuId = m.Id
								FOR JSON AUTO)
				  ,MenuDays = ( 
								SELECT dw.Id
									  ,dw.Name as Name
								FROM [dbo].[MenuDays] as md inner join [dbo].DaysOfWeek as dw
								on md.DaysOfWeekId = dw.Id
								WHERE md.MenuId = m.Id
								FOR JSON AUTO)
				  
				  ,TotalCount = COUNT(1) OVER()

			FROM [dbo].[Menus] as m
			WHERE OrganizationId = @OrganizationId
			ORDER BY Id
			
			OFFSET @offSet Rows
			Fetch Next @PageSize Rows ONLY

END