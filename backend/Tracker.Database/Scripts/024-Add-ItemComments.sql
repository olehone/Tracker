CREATE TABLE [dbo].[ItemComments] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [BoardItemId] UNIQUEIDENTIFIER NOT NULL,
    
    [Content] NVARCHAR(MAX) NULL,
    [UploadedAt] DATETIMEOFFSET NOT NULL,
    [UpdatedAt] DATETIMEOFFSET NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_ItemComment_User
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_ItemComment_BoardItems
        FOREIGN KEY (BoardItemId)
        REFERENCES [dbo].[BoardItems](Id)
        ON DELETE CASCADE
)