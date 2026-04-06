namespace PlannerService;

using StorageService;

public static class DietaryRequirements {
    private static string path = "dietary-requirements";

    public static HashSet<Note> KnownRestrictions { get; private set; }

    public static void Load() {
        KnownRestrictions = Storage.ReadData<HashSet<Note>>(path) ?? new();
    }

    internal static void Save() {
	Storage.WriteData(path, KnownRestrictions);
    }

    public static string EditRequirement(
	Event Event,
	IDietaryRequirements On,
	Note requirement)
    {
	var requirementsOn = new DietaryRequirementsOf(On);
	var text = Notes.EditNote(Event, requirementsOn, requirement);
	Console.WriteLine(text);
	if (string.IsNullOrWhiteSpace(text)) {
	    // blank requirement or editor issue
	    return null;
	}
	AddRequirement(Event, On, requirement);
	return text;
    }

    public static void AddRequirement(
	Event Event,
	IDietaryRequirements On,
	Note requirement)
    {
	var requirementsOn = new DietaryRequirementsOf(On);
	KnownRestrictions.Add(requirement);
	Save();
	requirementsOn.Notes.Add(requirement);
	GuestList.Save();
	Events.Save(Event);
    }
}
