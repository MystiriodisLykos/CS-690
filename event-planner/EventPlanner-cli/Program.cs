namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

using System.Diagnostics;

class Program {
    static void Main(string[] args) {
        EventPicker.Run();
    }
}

interface INestedMenu
{
    public string MenuName { get; }

    public void Run(Event Event)
    {
        throw new NotImplementedException();
    }

    public bool Break() {
	return false;
    }
}

class MenuOfMenus
{
    protected List<INestedMenu> Menus;
    protected string Prompt;

    // Fake menu to add a "back" or "exit" option to the menu of menus.
    protected class BackMenu : INestedMenu
    {
        public string MenuName { get; }
        public BackMenu(string text)
        {
            MenuName = text;
        }
    }
    protected BackMenu BackOption;

    public MenuOfMenus(List<INestedMenu> menus, string prompt, string backMenuOption = "Go Back")
    {
        Menus = menus;
        Prompt = prompt;
        BackOption = new BackMenu(backMenuOption);
    }

    public void Run(Event Event)
    {
        while(true) {
            var menu = AnsiConsole.Prompt(
                new SelectionPrompt<INestedMenu>()
                    .Title(Prompt)
                    .UseConverter(option => option.MenuName)
                    .WrapAround()
                    .AddChoices(Menus)
                    .AddChoices(new[] {BackOption})
            );
	    if (menu == BackOption) return;
	    menu.Run(Event);
	    if (menu.Break()) return;
        }
    }
}

class EventPicker {
    public static void Run() {
	while(true) {
	    AnsiConsole.Clear();
	    var selection = AnsiConsole.Prompt(
		new SelectionPrompt<string>()
		.Title("Pick event to manage or create new")
		.WrapAround()
		.AddChoices(Events.ListEvents())
		.AddChoices(new[] {"Create New", "Quit"})
	    );

	    if (selection == "Quit") return;
	    if (selection == "Create New")
	    {
		selection = AnsiConsole.Ask<string>("What is the new event's name?");
	    }
	    Event = Events.Read(selection);
	    EventPlanner.Run(Event);
	    Events.Save(Event);
	}
    }
}

class DeleteEventMenu : INestedMenu {
    public string MenuName { get; } = "Delete Event";
    public void Run(Event Event) {
	if (AnsiConsole.Confirm($"Confirm delection of {Event.Name}?", false)) {
	    Events.Delete(Event);
	}
    }
    public bool Break() {
	return true;
    }
}

class EventPlanner {

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {
	    new GuestPlanner(),
	    new EventMenu(),
	    new NoteMenu(),
	    new DeleteEventMenu(),
			      },
        "Select what to Manage",
        "Back"
    );
    public static void Run(Event Event) {
        Menu.Run(Event);
    }
}
