CREATE TABLE [dbo].[BoardItemAttachments] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [BoardItemId] UNIQUEIDENTIFIER NOT NULL,
    
    [OriginalFileName] NVARCHAR(255) NOT NULL,
    [ContentType] NVARCHAR(100) NOT NULL,
    [SizeBytes] BIGINT NOT NULL,

    [StorageFileName] NVARCHAR(63) NOT NULL,
    [StorageFolder] NVARCHAR(512) NOT NULL,

    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIMEOFFSET NOT NULL,

    CONSTRAINT FK_BoardItemAttachment_BoardItems
        FOREIGN KEY (BoardItemId)
        REFERENCES [dbo].[BoardItems](Id)
        ON DELETE NO ACTION
)