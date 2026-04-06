namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

class GuestPlanner : INestedMenu {
    public string MenuName { get; } = "Guests";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {
	    new InviteGuest(),
	    new AddGuestNote(),
	    new AddGuestDietaryRequirement()
			      },
        "What would you like to do?"
    );
    public void Run(Event Event) {
        Menu.Run(Event);
    }
}

class InviteGuest : INestedMenu {
    public string MenuName { get; } = "Invite a Guest";

    // fake guest to have a "no/quit" option in guest selection prompts.
    private readonly static Guest fake_guest = new Guest("No/Create New");

    public void Run(Event Event)
    {
        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter Guests Name:")
        );

        var guest = new Guest(name);
        var known_guests = GuestList.KnownGuests(guest);

        if (known_guests.Any())
        {
            var known_guest = AnsiConsole.Prompt(
                new SelectionPrompt<Guest>()
                .Title("A Guest with that name already exists in the system, use them?")
                .WrapAround()
                .AddChoices(known_guests)
                .AddChoices(new[] {fake_guest})
                .UseConverter(option => option.Name)
            );

            if (known_guest != fake_guest)
            {
                guest = known_guest;
            }
        }
	Events.InviteGuest(Event, guest);
    }
}

static class SelectGuestMenu
{
    public static Invitation? SelectGuest(Event Event, string title) {
	if (! Event.Guests.Any())
        {
            AnsiConsole.Confirm("Must have at least one Guest Invited. (Enter to Continue)");
            return null;
        }

        return AnsiConsole.Prompt(
            new SelectionPrompt<Invitation>()
	    .Title(title)
            .WrapAround()
            .AddChoices(Event.Guests)
            .UseConverter(option => option.Guest.Name)
        );
    }
}

class AddGuestNote : INestedMenu
{
    public string MenuName { get; } = "Add A Note";
    protected static readonly AddNoteMenu noteMenu = new AddNoteMenu();

    public void Run(Event Event)
    {

        var invitation = SelectGuestMenu.SelectGuest(Event, "Select Guest to Add a Note to");

	if (invitation != null)
	    noteMenu.Run(Event, invitation);
    }
}

class AddGuestDietaryRequirement : INestedMenu
{
    public string MenuName { get; } = "Add A Dietary Requirement";
    protected static readonly AddDietaryMenu dietMenu = new AddDietaryMenu();

    public void Run(Event Event)
    {
	var invitation = SelectGuestMenu.SelectGuest(Event, "Select Guest to Add a Requirement to");
	if (invitation != null)
	    dietMenu.Run(Event, invitation.Guest);
    }
}
