namespace PlannerService;

using StorageService;

public static class Events {
    internal static void Save(Event Event) {
	Storage.WriteData("event", Event);
    }

    public static Event Read() {
	return Storage.ReadData<Event>("event") ?? new();
    }

    public static void InviteGuest(Event Event, Guest Guest) {
	GuestList.AddGuest(Guest);
	foreach (var invitation in Event.Guests) {
	    // Only one invite per guest
	    if (invitation.Guest == Guest) return;
	}
	Event.Guests.Add(new Invitation(Guest));
        Save(Event);
    }
}
