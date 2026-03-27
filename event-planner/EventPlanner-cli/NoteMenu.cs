namespace EvenPlannerCLI;

using Spectre.Console;
using PlannerService;
using Persistence = PlannerService.Storage;
using System.Diagnostics;

class NoteMenu : INestedMenu {
    public string MenuName { get; } = "Notes";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {new EditNote(), new ShowAllNotes()},
        "What would you like to do?"
    );
    public void Run(Event Event) {
        Menu.Run(Event);
    }
}

class EditNote : INestedMenu
{
    public string MenuName { get; } = "Edit Note";

    public void Run(Event Event)
    {
        var notes = Event.Notes.Concat(Event.GetAllGuestNotes());

        if (! notes.Any())
        {
            AnsiConsole.Confirm("Must have at least one Note before it can be editted. (Enter to Continue)");
            return;
        }
        var note = AnsiConsole.Prompt(
            new SelectionPrompt<Note>()
            .Title("Select Note to Edit")
            .WrapAround()
            .AddChoices(notes)
            .UseConverter(option => Persistence.Notes.ReadNote(option))
        );

        Persistence.Notes.EditNote(note);

        Console.WriteLine("Done");
    }
}

class ShowAllNotes : INestedMenu
{
    public string MenuName { get; } = "Show All Notes";

    public void Run(Event Event)
    {
        var eventNotes = new Tree("Event Notes")
            .Guide(TreeGuide.BoldLine)
            .Style(Style.Parse("dim"));
        
        foreach (var note in Event.Notes)
        {
            var text = Persistence.Notes.ReadNote(note);
            if (text != null)
            {
                eventNotes.AddNode(text);
            }
        }
        
        AnsiConsole.Write(eventNotes);

        var allGuestNotes = new Tree("Guest Notes")
            .Guide(TreeGuide.BoldLine)
            .Style(Style.Parse("dim"));
        
        foreach (var invitation in Event.Guests)
        {
            var guestNotes = allGuestNotes.AddNode($"{invitation.Guest.Name} Notes");

            foreach (var note in invitation.Notes) {
                var text = Persistence.Notes.ReadNote(note);
                if (text != null)
                {
                    guestNotes.AddNode(text);
                }
            }
        }

        AnsiConsole.Write(allGuestNotes);
    }
}