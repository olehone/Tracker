CREATE TABLE [dbo].[BoardItemAssignee] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserBoardId] UNIQUEIDENTIFIER NOT NULL,
    [BoardItemId] UNIQUEIDENTIFIER NOT NULL,
    
    CONSTRAINT UQ_BoardItemAssignees UNIQUE ([UserBoardId], [BoardItemId]),

    CONSTRAINT FK_BoardItemAssignee_UserBoards
        FOREIGN KEY (UserBoardId)
        REFERENCES [dbo].[UserBoards](Id),

    CONSTRAINT FK_BoardItemAssignee_BoardItems
        FOREIGN KEY (BoardItemId)
        REFERENCES [dbo].[BoardItems](Id)
        ON DELETE CASCADE
)