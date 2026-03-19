namespace EvenPlannerCLI;

using Spectre.Console;
using PlannerService;
using Persistence = PlannerService.Storage;

class Program {
    static void Main(string[] args) {
        var Event = Persistence.EventData.ReadEvent();
        EventPlanner.Run(Event);
    }
}

interface INestedMenu
{
    public string MenuName { get; }

    public void Run(Event Event)
    {
        throw new NotImplementedException();
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
        }
    }
}

class EventPlanner {

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {new GuestPlanner(), new EventMenu(), new NoteMenu()},
        "Select what to Manage",
        "Exit"
    );
    public static void Run(Event Event) {
        Menu.Run(Event);
    }
}
