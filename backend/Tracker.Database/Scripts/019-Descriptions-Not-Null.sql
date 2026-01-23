UPDATE [dbo].[BoardItems]
SET Description = ''
WHERE Description IS NULL;

ALTER TABLE [dbo].[BoardItems]
ALTER COLUMN Description NVARCHAR(MAX) NOT NULL;


UPDATE [dbo].[Boards]
SET Description = ''
WHERE Description IS NULL;

ALTER TABLE [dbo].[Boards]
ALTER COLUMN Description NVARCHAR(MAX) NOT NULL;


UPDATE [dbo].[BoardLists]
SET Description = ''
WHERE Description IS NULL;

ALTER TABLE [dbo].[BoardLists]
ALTER COLUMN Description NVARCHAR(MAX) NOT NULL;