ALTER TABLE [dbo].[UserWorkspaces]
DROP CONSTRAINT UQ_UserWorkspacess;

ALTER TABLE [dbo].[UserWorkspaces]
ADD CONSTRAINT UQ_WorkspaceUsers UNIQUE ([UserId], [WorkspaceId]);


ALTER TABLE [dbo].[UserWorkspaces]
DROP CONSTRAINT FK_UserWorkspaces_Users

ALTER TABLE [dbo].[UserWorkspaces]
ADD CONSTRAINT FK_WorkspaceUsers_Users
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE CASCADE;


ALTER TABLE [dbo].[UserWorkspaces]
DROP CONSTRAINT FK_UserWorkspaces_Workspaces

ALTER TABLE [dbo].[UserWorkspaces]
ADD CONSTRAINT FK_WorkspaceUsers_Workspaces
        FOREIGN KEY (WorkspaceId)
        REFERENCES [dbo].[Workspaces](Id)
        ON DELETE CASCADE;


EXEC sp_rename 'dbo.UserWorkspaces', 'WorkspaceUsers';