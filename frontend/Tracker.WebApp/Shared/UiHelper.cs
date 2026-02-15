using System.Net.Mail;
using System.Security.Cryptography;
using MudBlazor;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Shared;

public static class UiHelper
{
    private static readonly Random Random = new();

    public static int RandomItemCount(int min = 1, int max = 4)
    {
        return Random.Next(min, max);
    }

    public static bool RandomBool(double truePart = 0.5)
    {
        return Random.NextDouble() < truePart;
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

    // Could be file flags or mime content type
    public static bool IsImage(FileDto file)
    {
        return IsImage(file.FileName);
    }

    public static bool IsImage(string fileName)
    {
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        return imageExtensions.Any(ext =>
            fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }

    public static string FileSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };

        double size = bytes;
        int unit = 0;

        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        if (unit == 0)
        {
            return $"{bytes} B";
        }

        return $"{size:0.##} {units[unit]}";
    }
}