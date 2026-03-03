ALTER TABLE [dbo].[UserSubscriptions]
    ALTER COLUMN [StripeCustomerId] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[UserSubscriptions]
    ALTER COLUMN [StripeSubscriptionId] NVARCHAR(255) NULL;