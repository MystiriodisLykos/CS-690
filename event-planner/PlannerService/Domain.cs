namespace PlannerService;

using System.Runtime.Serialization;

[DataContract(Name = "note")]
public class Note
{
    [DataMember(Name = "Path")]
    public string Path { get; internal set; }

    public Note()
    {
        Path = Guid.NewGuid() + ".txt";
    }
}

public interface INoted
{
    public HashSet<Note> Notes { get; }
}

public interface IDietaryRequirements
{
    public HashSet<Note> DietaryRequirements { get; }
}

internal class DietaryRequirementsOf : INoted
{
    /* wrapper class to expose dietary requirements to the note systems */
    protected IDietaryRequirements Of;
    public DietaryRequirementsOf(IDietaryRequirements of) {
	Of = of;
    }

    public HashSet<Note> Notes {
	get => Of.DietaryRequirements;
    }
}

[DataContract(Name = "guest")]
public class Guest : IDietaryRequirements
{
    [DataMember(Name = "Name")]
    public string Name { get; internal set; }
    [DataMember(Name = "Guid")]
    public Guid Guid { get; internal set; }
    [DataMember(Name = "DietaryRequirements")]
    public HashSet<Note> DietaryRequirements { get; internal set; }

    public Guest(string name)
    {
        Name = name;
        Guid = Guid.NewGuid();
	DietaryRequirements = new();
    }
}

[DataContract(Name = "guests")]
internal class KnownGuests
{
    [DataMember(Name = "GuestList")]
    internal HashSet<Guest> GuestList { get; set; }

    internal KnownGuests() {
	GuestList = new();
    }
}

public enum InvitationStatus {
    Pending,
    Accepted,
    Rejected
}

public static class InvitationStatusExtensions {
    public static string FriendlyToString(this InvitationStatus s) {
	switch(s) {
	    case InvitationStatus.Pending:
		return "Pending";
	    case InvitationStatus.Accepted:
		return "Attending";
	    case InvitationStatus.Rejected:
		return "Not Attending";
	    default:
		// Should never be thrown, type checker should verify this and it shouldn't be needed.
		throw new Exception("cannot create to string for unknown InvitationStatus value");
	}
    }
}

[DataContract(Name = "invitation")]
public class Invitation : INoted
{
    [DataMember(Name = "Guest")]
    public Guest Guest { get; internal set; }
    [DataMember(Name = "Notes")]
    public HashSet<Note> Notes { get; internal set; }
    [DataMember(Name = "Invitation Status")]
    public InvitationStatus InvitationStatus { get; internal set; }

    internal Invitation(Guest guest)
    {
        Guest = guest;
        Notes = new();
        InvitationStatus = InvitationStatus.Pending;
    }
}

[DataContract(Name = "event")]
public class Event : INoted, IDietaryRequirements
{
    [DataMember(Name = "Guests")]
    public HashSet<Invitation> Guests { get; internal set; }
    [DataMember(Name = "Notes")]
    public HashSet<Note> Notes { get; internal set; }
    [DataMember(Name = "DietaryRequirements")]
    public HashSet<Note> DietaryRequirements { get; internal set; }

    internal Event() {
	Guests = new();
	Notes = new();
	DietaryRequirements = new();
    }
}
