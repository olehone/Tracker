ALTER TABLE [dbo].[Boards]
ADD 
   [ArchiveStatus] INT NOT NULL CONSTRAINT Df_Boards_ArchiveStatus DEFAULT 10

ALTER TABLE [dbo].[Boards]
DROP CONSTRAINT Df_Boards_ArchiveStatus