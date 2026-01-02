namespace Tracker.WebApp.Shared;

public static class UiHelper
{
    private static readonly Random Random = new Random();

    public static int RandomItemCount()
    {
        return 1 + Random.Next(0, 3);
    }

    public static string RandomPercentTitleWidth()
    {
        int width = 30 + Random.Next(0, 40);
        return $"{width}%";
    }

    public static string RandomPixelTitleWidth()
    {
        int width = 100 + Random.Next(0, 100);
        return $"{width}px";
    }

    public static string RandomDescriptionWidth()
    {
        int width = 10 + Random.Next(0, 20);
        return $"{width}%";
    }

    public static bool IsEmailInvalid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return true;
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address != email;
        }
        catch
        {
            return true;
        }
    }
}