CREATE TABLE [dbo].[RoadmapArrows] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [SourceNodeId] UNIQUEIDENTIFIER NOT NULL,
    [TargetNodeId] UNIQUEIDENTIFIER NOT NULL,
 
    CONSTRAINT FK_RoadmapArrows_Source
        FOREIGN KEY ([SourceNodeId])
        REFERENCES [dbo].[RoadmapNodes]([Id])
        ON DELETE CASCADE,
 
    CONSTRAINT FK_RoadmapArrows_Target
        FOREIGN KEY ([TargetNodeId])
        REFERENCES [dbo].[RoadmapNodes]([Id])
        ON DELETE NO ACTION
);