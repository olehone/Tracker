using System.Text;

namespace Tracker.Infrastructure.AzureAI;

public class FaqDocument
{
    public string Id { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
    public string Content
    {
        get
        {
            return "Q: " + Question + "\n"
                + "A: " + Answer;
        }
    }

    public FaqDocument(string id, string question, string answer)
    {
        Id = id;
        Question = question;
        Answer = answer;
    }
}
