namespace Tracker.Infrastructure.AzureAI;

public static class BasicFaqDocuments
{
    public static List<FaqDocument> GetFaqDocuments()
    {
        return [
            new("faq-sub-001",
                """
                Q: What subscription plans are available?
                A: We offer three plans: Free, Basic, and Pro.
                Free plan includes no premium features.
                Basic plan unlocks calendar view and AI chat.
                Pro plan includes everything in Basic plus Eisenhower matrix.
                """),

            new("faq-sub-002",
                """
                Q: How do I upgrade or cancel my subscription?
                A: Go to Profile and find button about your subscription.
                Payments are processed securely via Stripe.
                Cancellation takes effect at the end of the current billing period.
                You keep access to premium features until then.
                """),

            new("faq-sub-003",
                """
                Q: What happens to my data if I downgrade to Free?
                A: Your data is preserved but premium features become inaccessible.
                """),

            new("faq-workspace-001",
                """
                Q: What is a workspace?
                A: A workspace is a shared space for your team.
                It contains all your boards, members, and settings in one place.
                You can belong to multiple workspaces with different roles in each.
                """),

            new("faq-workspace-002",
                """
                Q: How do I invite someone to my workspace?
                A: Go to Workspace > Members > Invite.
                Enter their username and assign a role: Admin, Member, or Viewer.
                This will work only if you have permissions.
                """),

            new("faq-board-001",
                """
                Q: What is a board and how do I create one?
                A: A board is a visual project management tool with lists and items.
                To create one, open your workspace and click New Board.
                You can set a name. Later you can change description, settings, and choose visibility: public or private.
                """),

            new("faq-items-001",
                """
                Q: How do I add and manage items on a board?
                A: Click the Add item button inside any column to add a new item.
                Items can have a title, description, assignees, due date, priority, comments and attachments.
                Drag and drop items between columns to update their status.
                """),

            new("faq-items-002",
                """
                Q: Can I assign multiple people to one item?
                A: Yes, each item supports multiple assignees.
                """),

            new("faq-attach-001",
                """
                Q: What file types and sizes are supported for attachments?
                A: You can attach images, PDFs, and documents up to 100MB per file.
                Files are stored securely in Azure Blob Storage.
                """),

            new("faq-comments-001",
                """
                Q: How do comments work on board items?
                A: Open any board item and scroll to the Comments section.
                Type your message and press Send. All users with access can comment.
                You can attach up to 5 files to your comment.
                """),

            new("faq-calls-001",
                """
                Q: How do I start a call with my team?
                A: You will see start call button only if you have permissions.
                You still could join ongoing call.
                Click the Call button inside any board to start a WebRTC call.
                Screen sharing is supported during calls. No external software required.
                """),

            new("faq-calendar-001",
                """
                Q: What is the calendar view?
                A: Calendar view shows all your board items with due dates in a monthly calendar layout.
                It is available on Basic and Pro plans.
                Click any date to see items due that day.
                """),

            new("faq-eisenhower-001",
                """
                Q: What is the Eisenhower matrix view?
                A: The Eisenhower matrix organizes tasks into four quadrants based on urgency and importance:
                Do First, Schedule, Delegate, and Eliminate.
                It is available on Pro plan and helps you prioritize work effectively.
                """),

            new("faq-archive-001",
                """
                Q: How does board archiving work?
                A: You can archive any board from Board Settings > Archive.
                Archived boards is preserved on Azure Blob Storage.
                Archiving and unarchiving take time.
                """),

            new("faq-reenbit-001",
                """
                Q: What is Reenbit?
                A: Reenbit is a software development hub based in Eastern Europe with over 100 experts and 7+ years of experience. They help product and business teams turn ideas into scalable digital solutions by combining custom software development, UI/UX design, data and cloud engineering, and AI integrations. With agile processes, transparent communication, and a flat structure, we make collaboration simple and decision-making fast.
                """),
        ];
    }
}