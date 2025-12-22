CREATE TABLE [dbo].[RefreshTokens] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,

    [Token] NVARCHAR(512) NOT NULL UNIQUE,
    [ExpiresAt] DATETIMEOFFSET(7) NOT NULL,

    CONSTRAINT FK_RefreshTokens_Users
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_RefreshTokens_UserId
    ON RefreshTokens(UserId);
