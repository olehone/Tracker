namespace Tracker.WebApp.Pages.Workspaces;
public partial class BoardSummarySkeleton
{
    private readonly Random random = new Random();
    private string RandomWidth()
    {
        int width = 30 + random.Next(0, 40);
        return $"{width}%";
    }
}