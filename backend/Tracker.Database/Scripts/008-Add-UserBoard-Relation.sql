CREATE TABLE [dbo].[UserBoards] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [Role] INT NOT NULL,
    
    CONSTRAINT PK_UserBoards PRIMARY KEY ([UserId], [BoardId]),

    CONSTRAINT FK_UserBoards_Users
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_UserBoards_Boards
        FOREIGN KEY (BoardId)
        REFERENCES [dbo].[Boards](Id)
        ON DELETE CASCADE
)