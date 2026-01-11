using System.Net.Mail;
using System.Security.Cryptography;

namespace Tracker.WebApp.Shared;

public static class UiHelper
{
    private static readonly Random Random = new();

    public static int RandomItemCount()
    {
        return 1 + Random.Next(0, 3);
    }

    public static string RandomPercentTitleWidth()
    {
        var width = 30 + Random.Next(0, 40);
        return $"{width}%";
    }

    public static string RandomPixelTitleWidth()
    {
        var width = 100 + Random.Next(0, 100);
        return $"{width}px";
    }

    public static string RandomDescriptionWidth()
    {
        var width = 10 + Random.Next(0, 20);
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
            var addr = new MailAddress(email);
            return addr.Address != email;
        }
        catch
        {
            return true;
        }
    }

    public static string ShortenText(string text, int length, string ellipsis = "..")
    {
        var ellipsisLength = ellipsis.Length;
        if (text.Length < length)
        {
            return text;
        }

        return string.Concat(text.AsSpan(0, length - ellipsisLength), ellipsis);
    }

    public static string GetColorById(Guid id)
    {
        var bytes = SHA256.HashData(id.ToByteArray());
        var hue = bytes[0] % 360;
        return $"background:hsl({hue}, 60%, 55%);";
    }
}