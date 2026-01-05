ALTER TABLE [dbo].[Workspaces]
ADD 
   [Visibility] INT NOT NULL CONSTRAINT Df_Workspaces_Visibility DEFAULT 20,
   [MinCreateBoardRole] INT NOT NULL CONSTRAINT Df_Workspaces_CreateBoard DEFAULT 30,
   [MinChangeBoardRole] INT NOT NULL CONSTRAINT Df_Workspaces_ChangeBoard DEFAULT 30;

ALTER TABLE [dbo].[Workspaces]
DROP CONSTRAINT Df_Workspaces_Visibility, Df_Workspaces_CreateBoard, Df_Workspaces_ChangeBoard;
