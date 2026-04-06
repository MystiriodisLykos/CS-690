namespace EvenPlannerCLI;

using Spectre.Console;
using PlannerService;

class GuestPlanner : INestedMenu {
    public string MenuName { get; } = "Guests";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {new InviteGuest(), new AddGuestNote()},
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

class AddGuestNote : INestedMenu
{
    public string MenuName { get; } = "Add A Note";
    protected static readonly AddNoteMenu noteMenu = new AddNoteMenu();

    public void Run(Event Event)
    {
        if (! Event.Guests.Any())
        {
            AnsiConsole.Confirm("Must have at least one Guest Invited to add Notes to Guests. (Enter to Continue)");
            return;
        }

        var invitation = AnsiConsole.Prompt(
            new SelectionPrompt<Invitation>()
            .Title("Select Guest to Add a Note to")
            .WrapAround()
            .AddChoices(Event.Guests)
            .UseConverter(option => option.Guest.Name)
        );

	noteMenu.Run(Event, invitation);
    }
}
