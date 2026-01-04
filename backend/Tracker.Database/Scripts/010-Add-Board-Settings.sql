ALTER TABLE [dbo].[Boards]
ADD 
   [Visibility] INT NOT NULL CONSTRAINT DF_Boards_Visibility DEFAULT 30,
   [MinCreateItemRole] INT NOT NULL CONSTRAINT Df_Boards_CreateItem DEFAULT 30,
   [MinMoveItemRole] INT NOT NULL CONSTRAINT Df_Boards_MoveItem DEFAULT 30,
   [MinCreateListRole] INT NOT NULL CONSTRAINT Df_Boards_CreateList DEFAULT 40,
   [MinMoveListRole] INT NOT NULL CONSTRAINT Df_Boards_MoveList DEFAULT 40;

ALTER TABLE [dbo].[Boards]
DROP CONSTRAINT Df_Boards_CreateItem,
				Df_Boards_MoveItem,
				Df_Boards_CreateList,
				Df_Boards_MoveList;
