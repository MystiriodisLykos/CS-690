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

    // TODO: I'd really like this value to be static, it shouldn't
    // be unique between instances, but I'm having trouble actually
    // getting access to the value.
    // private static string _MenuName;

    // public string MenuName => _MenuName;

    public void Run(Event Event)
    {
        throw new NotImplementedException();
    }
}

class EventPlanner {
    public static void Run(Event Event) {
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select what to Manage")
                .AddChoices(new[] {
                    "Guests"
                })
        );

        switch(action) {
            case "Guests":
                GuestPlanner.Run(Event);
                break;
        }
    }
}
class GuestPlanner {
    private class InviteGuest : INestedMenu {
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
    public static void Run(Event Event) {
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<INestedMenu>()
                .Title("What would you like to do?")
                .UseConverter(option => option.MenuName)
                .AddChoices(new[] {
                    new InviteGuest()
                })
        );
        action.Run(Event);
    }
}