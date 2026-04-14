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
	    new AddExpense(),
	    new RemoveExpense(),
	    new ShowExpenses()
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

class RemoveExpense : INestedMenu {
    public string MenuName { get; } = "Remove Expense";

    // Fake expense for existing without removing any
    private readonly static Expense fake_expense = new Expense(null, 0.0);

    protected string ExpenseConverter(Expense expense) {
	if (expense == fake_expense) {
	    return "Quit";
	}
	return $"{expense.Amount} for:\n{Notes.ReadNote(expense.Item)}";
    }

    public void Run(Event Event) {
	var expense = AnsiConsole.Prompt(
	    new SelectionPrompt<Expense>()
	    .Title("Select Expense To Remove")
	    .WrapAround()
	    .AddChoices(Event.Expenses)
	    .AddChoices(new[] {fake_expense})
	    .UseConverter(ExpenseConverter)
	);

	if (expense != fake_expense) {
	    Events.RemoveExpense(Event, expense);
	}
    }
}

class ShowExpenses : INestedMenu {
    public string MenuName { get; } = "Show Expenses";

    public void Run(Event Event) {
	AnsiConsole.Clear();

	var total = 0.0;
	var tree = new Tree("")
	    .Guide(TreeGuide.BoldLine)
	    .Style(Style.Parse("dim"));

	foreach (var expense in Event.Expenses) {
	    total += expense.Amount;
	    var node = tree.AddNode($"{expense.Amount} for");
	    node.AddNode(Notes.ReadNote(expense.Item));
	}

	AnsiConsole.Write(tree);

	AnsiConsole.Write(new Text($"Total Expenses: {total}"));
	AnsiConsole.WriteLine();
    }
}
