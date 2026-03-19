namespace PlannerService;

public class Note
{
    public Guid Guid;
    public string Path;

    public Note()
    {
        Guid = Guid.NewGuid();
        Path = Guid.ToString();
    }
}

public class Guest
{
    public string Name;
    public Guid Guid;

    public Guest(string name)
    {
        Name = name;
        Guid = Guid.NewGuid();
    }

    // Needed for serialization
    public Guest() {}
}

public class Invitation
{
    public Guest Guest;
    public Guid Guid;
    public List<Note> Notes { get; }

    public Invitation(Guest guest)
    {
        Guest = guest;
        Guid = Guid.NewGuid();
        Notes = [];
    }

    //  Needed for serialization
    public Invitation()
    {
        Notes = [];
    }

    public void AddNote(Note note)
    {
        Notes.Add(note);
    }
}


public class KnownGuests
{
    public List<Guest> GuestList { get; set; }

    public KnownGuests()
    {
        GuestList = [];
    }

    public IEnumerable<Guest> FindGuest(string name)
    {
        return
            from guest in GuestList
            where guest.Name == name
            select guest;
    }

    public void AddGuest(Guest guest)
    {
        GuestList.Add(guest);
    }
}

public class Event
{
    public List<Invitation> Guests { get; }
    public List<Note> Notes { get; }

    public Event()
    {
        Guests = [];
        Notes = [];
    }

    public void InviteGuest(Guest guest)
    {
        Guests.Add(new Invitation(guest));
    }

    public void AddNote(Note note)
    {
        Notes.Add(note);
    }
}