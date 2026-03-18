namespace PlannerService;

public class Guest
{
    public string Name;

    public Guest(string name)
    {
        Name = name;
    }

    // Needed for serialization
    public Guest() {}
}

public class Invitation
{
    public Guest Guest;

    public Invitation(Guest guest)
    {
        Guest = guest;
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