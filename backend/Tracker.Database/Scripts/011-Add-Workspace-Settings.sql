ALTER TABLE [dbo].[Workspaces]
ADD 
   [Visibility] INT NOT NULL CONSTRAINT Df_Workspaces_Visibility DEFAULT 20,
   [MinCreateItemRole] INT NOT NULL CONSTRAINT Df_Workspaces_CreateBoard DEFAULT 30;

ALTER TABLE [dbo].[Workspaces]
DROP CONSTRAINT Df_Workspaces_Visibility, Df_Workspaces_CreateBoard;
