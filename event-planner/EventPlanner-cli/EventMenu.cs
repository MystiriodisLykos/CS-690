namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

class EventMenu : INestedMenu {
    public string MenuName { get; } = "Event";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {
	    new AddEventNote(),
	    new AddEventDietaryRequirement(),
	    new ShowAllRequirements(),
	    new ShowInvitations(),
	    new AddExpense()
			      },
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

class AddEventDietaryRequirement : INestedMenu
{
    public string MenuName { get; } = "Add A Dietary Requirement";
    protected static readonly AddDietaryMenu dietMenu = new AddDietaryMenu();

    public void Run(Event Event)
    {
	dietMenu.Run(Event, Event);
    }
}

class ShowAllRequirements : INestedMenu {
    public string MenuName { get; } = "Show All Dietary Requirements";

    public void Run(Event Event)
    {
	Console.Clear();

	var requirements = DietaryRequirements.AllRequirements(Event);

        AnsiConsole.Write(new Rows(
	    from requirement in requirements
	    select new Text(requirement)
	));
    }
}

class ShowInvitations : INestedMenu {
    public string MenuName { get; } = "Show all Invitations";

    public void Run(Event Event) {
	AnsiConsole.Clear();

	var invitations = Event.Guests.ToLookup(i => i.InvitationStatus, i => i.Guest);

	var allInvitations = new Tree("")
	    .Guide(TreeGuide.BoldLine)
	    .Style(Style.Parse("dim"));

	foreach (var group_ in invitations) {
	    var groupNode = allInvitations.AddNode(group_.Key.FriendlyToString());
	    foreach (var invitation in group_) {
		groupNode.AddNode(invitation.Name);
	    }
	}

	AnsiConsole.Write(allInvitations);
    }
}

class AddExpense : INestedMenu {
    public string MenuName { get; } = "Add Expense";

    public void Run(Event Event) {
	double amount = AnsiConsole.Ask<double>("Expense Amount: ");
	Events.AddExpense(Event, amount);
    }
}
