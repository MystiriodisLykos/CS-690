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
}
