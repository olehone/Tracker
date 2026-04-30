CREATE TABLE [dbo].[BoardItemAttachmentLinks](
    [AttachmentId] UNIQUEIDENTIFIER NOT NULL,
    [BoardItemId] UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_BoardItemAttachmentLinks]
        PRIMARY KEY ([AttachmentId]),

    CONSTRAINT [FK_BoardItemAttachmentLinks_Attachment]
        FOREIGN KEY ([AttachmentId]) REFERENCES [dbo].[Attachments]([Id])
        ON DELETE CASCADE,

    CONSTRAINT [FK_BoardItemAttachmentLinks_BoardItem]
        FOREIGN KEY ([BoardItemId]) REFERENCES [dbo].[BoardItems]([Id])
        ON DELETE CASCADE
);

CREATE TABLE [dbo].[CommentAttachmentLinks](
    [AttachmentId] UNIQUEIDENTIFIER NOT NULL,
    [ItemCommentId] UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_CommentAttachmentLinks]
        PRIMARY KEY ([AttachmentId]),

    CONSTRAINT [FK_CommentAttachmentLinks_Attachment]
        FOREIGN KEY ([AttachmentId]) REFERENCES [dbo].[Attachments]([Id])
        ON DELETE CASCADE,

    CONSTRAINT [FK_CommentAttachmentLinks_Comment]
        FOREIGN KEY ([ItemCommentId]) REFERENCES [dbo].[ItemComments]([Id])
        ON DELETE CASCADE
);