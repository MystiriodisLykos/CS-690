namespace PlannerService;

using StorageService;

public static class GuestList {
    private static string path = "guest-list";

    private static KnownGuests guestList;

    public static void Load() {
	guestList = Storage.ReadData<KnownGuests>(path) ?? new();
    }

    internal static void Save() {
	Storage.WriteData(path, guestList);
    }

    public static bool AddGuest(Guest Guest) {
	var new_item = guestList.GuestList.Add(Guest);
	Save();
	return new_item;
    }

    public static IEnumerable<Guest> KnownGuests(Guest Guest) {
	return
	    from guest in guestList.GuestList
	    where guest.Name == Guest.Name
	    select guest;
    }
}
