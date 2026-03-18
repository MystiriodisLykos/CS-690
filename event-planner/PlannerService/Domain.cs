namespace PlannerService;

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

    public Invitation(Guest guest)
    {
        Guest = guest;
        Guid = Guid.NewGuid();
    }

    //  Needed for serialization
    public Invitation() {}
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

    public Event()
    {
        Guests = [];
    }

    public void InviteGuest(Guest guest)
    {
        Guests.Add(new Invitation(guest));
    }
}