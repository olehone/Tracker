UPDATE [dbo].[BoardItems]
SET IsDone = 0
WHERE IsDone IS NULL;

ALTER TABLE [dbo].[BoardItems]
ALTER COLUMN IsDone BIT NOT NULL;