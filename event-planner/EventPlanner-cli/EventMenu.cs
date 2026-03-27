namespace EvenPlannerCLI;

using Spectre.Console;
using PlannerService;
using Persistence = PlannerService.Storage;

class EventMenu : INestedMenu {
    public string MenuName { get; } = "Event";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {new AddEventNote()},
        "What would you like to do?"
    );
    public void Run(Event Event) {
        Menu.Run(Event);
    }
}

class AddEventNote : INestedMenu
{
    public string MenuName { get; } = "Add A Note";

    public void Run(Event Event)
    {
        var text = AnsiConsole.Prompt(new TextPrompt<string>("Note:").AllowEmpty());
        var note = new Note();
        if (string.IsNullOrWhiteSpace(text)) return;
        Persistence.Notes.WriteNote(note, text);
        Event.AddNote(note);
        Persistence.EventData.WriteEvent(Event);
    }
}