CREATE TABLE [dbo].[UserSubscriptions] (
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[UserId] UNIQUEIDENTIFIER NOT NULL,

	[Plan] INT NOT NULL,
	[CurrentPeriodEnd] DATETIMEOFFSET NULL,
	[StripeCustomerId] NVARCHAR NULL,
	[StripeSubscriptionId] NVARCHAR NULL,

	CONSTRAINT UQ_UserSubscriptions_UserId UNIQUE (UserId),

	CONSTRAINT FK_UserSubscriptions_User
		FOREIGN KEY (UserId)
		REFERENCES [dbo].[Users](Id)
		ON DELETE CASCADE
)