UPDATE [dbo].[Users]
SET RoleEnum = CASE Role
				   WHEN 'User' THEN 10
				   WHEN 'Admin' THEN 20
			   END
WHERE RoleEnum IS NULL AND Role IS NOT NULL;

ALTER TABLE [dbo].[Users]
ALTER COLUMN RoleEnum INT NOT NULL;

ALTER TABLE [dbo].[Users]
ADD CONSTRAINT DF_Users_RoleEnum
DEFAULT 10 FOR RoleEnum;

ALTER TABLE [dbo].[Users]
DROP CONSTRAINT CK_Users_Role;

ALTER TABLE [dbo].[Users]
DROP CONSTRAINT DF_Users_Role;

ALTER TABLE [dbo].[Users]
DROP COLUMN Role;

EXEC sp_rename 'Users.RoleEnum', 'Role', 'COLUMN';