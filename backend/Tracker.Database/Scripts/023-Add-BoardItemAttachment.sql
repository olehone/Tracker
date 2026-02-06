CREATE TABLE [dbo].[BoardItemAttachments] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [BoardItemId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    
    [OriginalFileName] NVARCHAR(255) NOT NULL,
    [ContentType] NVARCHAR(100) NOT NULL,
    [SizeBytes] BIGINT NOT NULL,

    [StorageFileName] NVARCHAR(63) NOT NULL,
    [StorageFolder] NVARCHAR(512) NOT NULL,

    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [UploadedAt] DATETIMEOFFSET NOT NULL,

    CONSTRAINT FK_BoardItemAttachment_BoardItems
        FOREIGN KEY (BoardItemId)
        REFERENCES [dbo].[BoardItems](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_BoardItemAttachment_User
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE NO ACTION
)