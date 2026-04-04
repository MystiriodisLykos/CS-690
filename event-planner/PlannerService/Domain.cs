namespace PlannerService;

using System.Runtime.Serialization;

[DataContract(Name = "note")]
public class Note
{
    [DataMember(Name = "Guid")]
    public Guid Guid { get; internal set; }
    [DataMember(Name = "Path")]
    public string Path { get; internal set; }

    public Note()
    {
        Guid = Guid.NewGuid();
        Path = Guid.ToString() + ".txt";
    }
}

[DataContract(Name = "guest")]
public class Guest
{
    [DataMember(Name = "Name")]
    public string Name { get; internal set; }
    [DataMember(Name = "Guid")]
    public Guid Guid { get; internal set; }

    public Guest(string name)
    {
        Name = name;
        Guid = Guid.NewGuid();
    }
}

[DataContract(Name = "guests")]
internal class KnownGuests
{
    [DataMember(Name = "GuestList")]
    internal List<Guest> GuestList { get; set; }

    internal KnownGuests() {
	GuestList = [];
    }
}

public interface INoted
{
    public List<Note> Notes { get; }
}

[DataContract(Name = "invitation")]
public class Invitation : INoted
{
    [DataMember(Name = "Guest")]
    public Guest Guest { get; internal set; }
    [DataMember(Name = "Notes")]
    public List<Note> Notes { get; internal set; }
    [DataMember(Name = "Guid")]
    public Guid Guid { get; internal set; }

    protected Invitation(Guest guest)
    {
        Guest = guest;
        Guid = Guid.NewGuid();
        Notes = [];
    }
}

[DataContract(Name = "event")]
public class Event : INoted
{
    [DataMember(Name = "Guests")]
    public List<Invitation> Guests { get; internal set; }
    [DataMember(Name = "Notes")]
    public List<Note> Notes { get; internal set; }

    protected Event() {
	Guests = [];
	Notes = [];
    }
}
