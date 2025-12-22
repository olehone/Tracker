CREATE TABLE [dbo].[Workspaces] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Title] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(MAX) NULL
);


CREATE TABLE [dbo].[Boards] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [WorkspaceId] UNIQUEIDENTIFIER NOT NULL,
    [Title] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,

    CONSTRAINT FK_Boards_Workspaces
        FOREIGN KEY (WorkspaceId)
        REFERENCES [dbo].[Workspaces](Id)
        ON DELETE CASCADE
);

CREATE INDEX IN_Boards_WorkspaceId
    ON Boards(WorkspaceId);


CREATE TABLE [dbo].[BoardLists] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [Position] INT NOT NULL,
    [Title] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,

    CONSTRAINT FK_BoardLists_Boards
        FOREIGN KEY (BoardId)
        REFERENCES [dbo].[Boards](Id)
        ON DELETE CASCADE
);

CREATE INDEX IN_BoardLists_BoardId_Position
    ON Boards(BoardId, Position);


CREATE TABLE [dbo].[BoardItems] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [BoardListId] UNIQUEIDENTIFIER NOT NULL,
    [Position] INT NOT NULL,
    [Title] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,

    CONSTRAINT FK_BoardItems_BoardLists
        FOREIGN KEY (BoardListId)
        REFERENCES [dbo].[Boardlists](Id)
        ON DELETE CASCADE
);

CREATE INDEX IN_BoardItems_BoardListId_Position
    ON Boards(BoardListId, Position);