namespace PlannerService;

using StorageService;

public static class Events {

    private static string EventsDir(string name) {
	return Path.Combine("events", name);
    }

    internal static void Save(Event Event) {
	Storage.WriteData(EventsDir(Event.Name), Event);
    }

    public static Event Read(string name) {
	GuestList.Load();
	DietaryRequirements.Load();
	var Event = Storage.ReadData<Event>(EventsDir(name)) ?? new(name);
	Event.Name = name;  // Set name if migrating from v3
	Save(Event);
	return Event;
    }

    public static void Delete(Event Event) {
	Storage.RemoveData(EventsDir(Event.Name));
    }

    public static IEnumerable<string> ListEvents() {
	return Storage.ListDir(EventsDir("."));
    }

    public static Invitation InviteGuest(Event Event, Guest Guest) {
	GuestList.AddGuest(Guest);
	foreach (var invitation in Event.Guests) {
	    // Only one invite per guest
	    if (invitation.Guest == Guest) return invitation;
	}
	var new_invitation = new Invitation(Guest);
	Event.Guests.Add(new_invitation);
        Save(Event);
	return new_invitation;
    }

    public static void RejectInvitation(Event Event, Invitation Invitation) {
	Invitation.InvitationStatus = InvitationStatus.Rejected;
	Events.Save(Event);
    }

    public static void AcceptInvitation(Event Event, Invitation Invitation) {
	Invitation.InvitationStatus = InvitationStatus.Accepted;
	Events.Save(Event);
    }

    public static void AddExpense(Event Event, double amount) {
	var item = new Note();
	var text = Notes.EditNote(Event, new ExpensesOf(Event), item);
	if (text != null) {
	    var expense = new Expense(item, amount);
	    Event.Expenses.Add(expense);
	    Save(Event);
	}
    }

    public static void RemoveExpense(Event Event, Expense expense) {
	Event.Expenses.Remove(expense);
	Notes.StoreNote(Event, new ExpensesOf(Event), expense.Item, "");
	Save(Event);
    }
}
