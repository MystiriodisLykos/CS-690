namespace PlannerService;

using StorageService;

public static class Events {
    internal static void Save(Event Event) {
	Storage.WriteData("event", Event);
    }

    public static Event Read() {
	GuestList.Load();
	DietaryRequirements.Load();
	return Storage.ReadData<Event>("event") ?? new();
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
