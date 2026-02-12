CREATE TABLE [dbo].[CommentAttachments] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [ItemCommentId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    
    [OriginalFileName] NVARCHAR(255) NOT NULL,
    [ContentType] NVARCHAR(100) NOT NULL,
    [SizeBytes] BIGINT NOT NULL,

    [StorageFileName] NVARCHAR(63) NOT NULL,
    [StorageFolder] NVARCHAR(512) NOT NULL,

    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [UploadedAt] DATETIMEOFFSET NOT NULL,

    CONSTRAINT FK_CommentAttachments_ItemComments
        FOREIGN KEY (ItemCommentId)
        REFERENCES [dbo].[ItemComments](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_CommentAttachments_User
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE NO ACTION
)