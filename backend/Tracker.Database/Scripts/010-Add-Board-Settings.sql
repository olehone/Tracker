ALTER TABLE [dbo].[Boards]
ADD 
   [Visibility] INT NOT NULL CONSTRAINT DF_Boards_Visibility DEFAULT 30,
   [MinCreateItemRole] INT NOT NULL CONSTRAINT Df_Boards_CreateItem DEFAULT 30,
   [MinChangeItemRole] INT NOT NULL CONSTRAINT Df_Boards_ChangeItem DEFAULT 30,

   [MinCreateListRole] INT NOT NULL CONSTRAINT Df_Boards_CreateList DEFAULT 40,
   [MinChangeListRole] INT NOT NULL CONSTRAINT Df_Boards_ChangeList DEFAULT 40;

ALTER TABLE [dbo].[Boards]
DROP CONSTRAINT Df_Boards_CreateItem,
				Df_Boards_ChangeItem,
				Df_Boards_CreateList,
				Df_Boards_ChangeList;
