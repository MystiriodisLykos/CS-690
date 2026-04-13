namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

class GuestPlanner : INestedMenu {
    public string MenuName { get; } = "Guests";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {
	    new InviteGuest(),
	    new AddGuestNote(),
	    new AddGuestDietaryRequirement(),
	    new ChangeStatusMenu()
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

    protected static readonly InvitationStatusMenu invitationMenu = new InvitationStatusMenu();

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
	var invitation = Events.InviteGuest(Event, guest);

	invitationMenu.Run(Event, invitation);
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

class ChangeStatusMenu : INestedMenu
{
    public string MenuName { get; } = "Change Guest Invitation Status";
    protected static readonly InvitationStatusMenu invitationMenu = new InvitationStatusMenu();

    public void Run(Event Event) {
	var invitation = SelectGuestMenu.SelectGuest(Event, "Select Guest to change the status of");
	if (invitation != null)
	    invitationMenu.Run(Event, invitation);
    }
}

class InvitationStatusMenu {

    public void Run(Event Event, Invitation Invitation) {

	// Uses 'Pending' as the quit option.
	var new_status = AnsiConsole.Prompt(
	    new SelectionPrompt<InvitationStatus>()
	    .Title("Select Invitation Status")
	    .AddChoices([
			    InvitationStatus.Accepted,
			    InvitationStatus.Rejected,
			    InvitationStatus.Pending
			])
	    .UseConverter(n => n == InvitationStatus.Pending ? "Quit" : n.FriendlyToString())
	);

	if (new_status == InvitationStatus.Rejected) {
	    Events.RejectInvitation(Event, Invitation);
	} else if (new_status == InvitationStatus.Rejected) {
	    Events.AcceptInvitation(Event, Invitation);
	}
    }
}
