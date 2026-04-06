namespace PlannerService.Test;

using PlannerService;
using StorageService;

[Collection("Serial")]
public class EventTests : IDisposable
{
    private Event Event;
    private Guest Guest1;

    public EventTests() {
	Event = Events.Read();
	Guest1 = new Guest("guest 1");
    }

    public void Dispose() {
	Storage.Clear();
    }

    [Fact]
    public void Inviting_new_guest_adds_them_to_guest_list() {
	Events.InviteGuest(Event, Guest1);
        Assert.Contains(Guest1, GuestList.KnownGuests(Guest1));
    }

    [Fact]
    public void Inviting_an_already_invited_guest_does_not_add_invitation() {
	Events.InviteGuest(Event, Guest1);
	Events.InviteGuest(Event, Guest1);
	Assert.Equal(1, Event.Guests.Count());
    }
}
