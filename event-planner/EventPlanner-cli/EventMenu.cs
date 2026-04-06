namespace EventPlannerCLI;

using PlannerService;

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
    protected static readonly AddNoteMenu noteMenu = new AddNoteMenu();

    public void Run(Event Event)
    {
	noteMenu.Run(Event, Event);
    }
}
