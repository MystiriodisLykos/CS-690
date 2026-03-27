namespace EvenPlannerCLI;

using Spectre.Console;
using PlannerService;
using Persistence = PlannerService.Storage;

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
    private static KnownGuests GuestList { get; } = Persistence.Guests.ReadGuests();

    // fake guest to have a "no/quit" option in guest selection prompts.
    private readonly static Guest fake_guest = new Guest("No/Create New");

    public void Run(Event Event)
    {
        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter Guests Name:")
        );

        var guest = new Guest(name);
        var known_guests = GuestList.FindGuest(name);

        var new_guest = true;

        if (known_guests.Count() > 0)
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
                new_guest = false;
            }
        }
        if (new_guest)
        {
            GuestList.AddGuest(guest);
            Persistence.Guests.WriteGuests(GuestList);
        }
        Event.InviteGuest(guest);
        Persistence.EventData.WriteEvent(Event);
    }
}

class AddGuestNote : INestedMenu
{
    public string MenuName { get; } = "Add A Note";

    public void Run(Event Event)
    {
        if (Event.Guests.Count() == 0)
        {
            AnsiConsole.Confirm("Must have at least one Guest Invited to add Notes to Guests.");
            return;
        }

        var invitation = AnsiConsole.Prompt(
            new SelectionPrompt<Invitation>()
            .Title("Select Guest to Add a Note to")
            .WrapAround()
            .AddChoices(Event.Guests)
            .UseConverter(option => option.Guest.Name)
        );


        var text = AnsiConsole.Prompt(new TextPrompt<string>("Note:").AllowEmpty());
        var note = new Note();
        if (string.IsNullOrWhiteSpace(text)) return;
        Persistence.Notes.WriteNote(note, text);
        invitation.AddNote(note);
        Persistence.EventData.WriteEvent(Event);
    }
}