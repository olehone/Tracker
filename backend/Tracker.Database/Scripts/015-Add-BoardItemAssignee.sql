CREATE TABLE [dbo].[BoardItemAssignees] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [BoardUserId] UNIQUEIDENTIFIER NOT NULL,
    [BoardItemId] UNIQUEIDENTIFIER NOT NULL,
    
    CONSTRAINT UQ_BoardItemAssignees UNIQUE ([BoardUserId], [BoardItemId]),

    CONSTRAINT FK_BoardItemAssignee_BoardUsers
        FOREIGN KEY (BoardUserId)
        REFERENCES [dbo].[BoardUserId](Id),

    CONSTRAINT FK_BoardItemAssignee_BoardItems
        FOREIGN KEY (BoardItemId)
        REFERENCES [dbo].[BoardItems](Id)
        ON DELETE CASCADE
)