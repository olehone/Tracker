CREATE TABLE [dbo].[UserWorkspaces] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [WorkspaceId] UNIQUEIDENTIFIER NOT NULL,
    [Role] INT NOT NULL,
    
    CONSTRAINT PK_UserWorkspacess PRIMARY KEY ([UserId], [WorkspaceId]),

    CONSTRAINT FK_UserWorkspaces_Users
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_UserWorkspaces_Workspaces
        FOREIGN KEY (WorkspaceId)
        REFERENCES [dbo].[Workspaces](Id)
        ON DELETE CASCADE
)