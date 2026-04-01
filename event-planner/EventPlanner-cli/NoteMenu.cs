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

    protected class NoteSelect<T, V>
    {
        string Name;
        public T Data;
        public V Group;
        public NoteSelect(string name, T data, V group)
        {
            Name = name;
            Data = data;
            Group = group;
        }

        public override string ToString()
        {
            return Name;
        }
    }


    public void Run(Event Event)
    {

        // Sentinal object for selection screen.
        var EventSelect = new NoteSelect<Note, INoteable>("Event Notes", null, null);

        var eventNotes = Event.Notes.Select(n => new NoteSelect<Note, INoteable>(
            Persistence.Notes.ReadNote(n),
            n,
            Event));

        var notePrompt = new SelectionPrompt<NoteSelect<Note, INoteable>>()
            .Title("Select Note to Edit")
            .WrapAround()
            .Mode(SelectionMode.Leaf);

        if (eventNotes.Any())
            notePrompt.AddChoiceGroup(EventSelect, eventNotes);

        foreach (var guest in Event.Guests)
        {
            var guestSelect = new NoteSelect<Note, INoteable>(guest.Guest.Name, null, null);
            var guestNotes = guest.Notes.Select(n => new NoteSelect<Note, INoteable>(
                Persistence.Notes.ReadNote(n), n, guest));
            if (guestNotes.Any())
                notePrompt.AddChoiceGroup(guestSelect, guestNotes);
        }

        var selection = AnsiConsole.Prompt(notePrompt);
        var note = selection.Data;

        var edited_note = Persistence.Notes.EditNote(note);

        if (edited_note == null)
        {
            AnsiConsole.Confirm(
                "Could not find an editor, please ensure one of the following is installed an on the PATH (vscode, notepad, emacs, vi). (Enter to Continue)"
            );
        }
        var text = Persistence.Notes.ReadNote(note);
        if (string.IsNullOrWhiteSpace(text))
        {
            selection.Group.RemoveNote(note);
            Persistence.Notes.RemoveNote(note);
            Persistence.EventData.WriteEvent(Event);
        }
    }
}

class ShowAllNotes : INestedMenu
{
    public string MenuName { get; } = "Show All Notes";

    public void Run(Event Event)
    {
        Console.Clear();
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