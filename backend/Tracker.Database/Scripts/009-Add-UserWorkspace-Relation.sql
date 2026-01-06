CREATE TABLE [dbo].[UserWorkspaces] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [WorkspaceId] UNIQUEIDENTIFIER NOT NULL,
    [Role] INT NOT NULL,
    
    CONSTRAINT UQ_UserWorkspacess UNIQUE ([UserId], [WorkspaceId]),

    CONSTRAINT FK_UserWorkspaces_Users
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_UserWorkspaces_Workspaces
        FOREIGN KEY (WorkspaceId)
        REFERENCES [dbo].[Workspaces](Id)
        ON DELETE CASCADE
)