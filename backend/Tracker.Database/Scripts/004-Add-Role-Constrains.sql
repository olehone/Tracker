UPDATE [dbo].[Users]
SET [Role] = 'User'
WHERE [Role] IS NULL;

ALTER TABLE [dbo].[Users]
ALTER COLUMN [Role] VARCHAR(20) NOT NULL;

ALTER TABLE [dbo].[Users]
ADD CONSTRAINT DF_Users_Role
DEFAULT 'User' FOR [Role];

ALTER TABLE [dbo].[Users]
ADD CONSTRAINT CK_Users_Role
CHECK ([Role] IN ('User', 'Admin'));