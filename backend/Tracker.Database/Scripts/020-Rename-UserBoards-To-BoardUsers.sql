ALTER TABLE [dbo].[UserBoards]
DROP CONSTRAINT UQ_UserBoards;

ALTER TABLE [dbo].[UserBoards]
ADD CONSTRAINT UQ_BoardUsers UNIQUE ([UserId], [BoardId]);


ALTER TABLE [dbo].[UserBoards]
DROP CONSTRAINT FK_UserBoards_Users

ALTER TABLE [dbo].[UserBoards]
ADD CONSTRAINT FK_BoardUsers_Users
        FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users](Id)
        ON DELETE CASCADE;


ALTER TABLE [dbo].[UserBoards]
DROP CONSTRAINT FK_UserBoards_Boards

ALTER TABLE [dbo].[UserBoards]
ADD CONSTRAINT FK_BoardUsers_Boards
        FOREIGN KEY (BoardId)
        REFERENCES [dbo].[Boards](Id)
        ON DELETE CASCADE;


EXEC sp_rename 'dbo.UserBoards', 'BoardUsers';