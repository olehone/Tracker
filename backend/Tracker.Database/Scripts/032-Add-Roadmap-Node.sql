CREATE TABLE [dbo].[RoadmapNodes] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [BoardItemId] UNIQUEIDENTIFIER NOT NULL,
    [X] FLOAT NOT NULL DEFAULT 0,
    [Y] FLOAT NOT NULL DEFAULT 0,
 
    CONSTRAINT UQ_RoadmapNodes_Board_Item
        UNIQUE ([BoardId], [BoardItemId]),
 
    CONSTRAINT FK_RoadmapNodes_Board
        FOREIGN KEY ([BoardId])
        REFERENCES [dbo].[Boards]([Id])
        ON DELETE CASCADE,
 
    CONSTRAINT FK_RoadmapNodes_BoardItem
        FOREIGN KEY ([BoardItemId])
        REFERENCES [dbo].[BoardItems]([Id])
        ON DELETE NO ACTION
);