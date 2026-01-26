UPDATE [dbo].[BoardItems]
SET Importance = 10
WHERE Importance IS NULL;

ALTER TABLE [dbo].[BoardItems]
ALTER COLUMN Importance INT NOT NULL;