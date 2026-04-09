namespace EventPlannerCLI;

using PlannerService;

public class EventMenu : INestedMenu {
    public string MenuName { get; } = "Event";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {
	    new AddEventNote(),
	    new AddEventDietaryRequirement()
			      },
        "What would you like to do?"
    );
    public void Run(Event Event) {
        Menu.Run(Event);
    }
}

public class AddEventNote : INestedMenu
{
    public string MenuName { get; } = "Add A Note";
    protected static readonly AddNoteMenu noteMenu = new AddNoteMenu();

    public void Run(Event Event)
    {
	noteMenu.Run(Event, Event);
    }
}

public class AddEventDietaryRequirement : INestedMenu
{
    public string MenuName { get; } = "Add A Dietary Requirement";
    protected static readonly AddDietaryMenu dietMenu = new AddDietaryMenu();

    public void Run(Event Event)
    {
	dietMenu.Run(Event, Event);
    }
}
