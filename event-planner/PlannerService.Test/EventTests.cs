namespace PlannerService.Test;

using PlannerService;
using StorageService;

[Collection("Serial")]
public class EventsTests : IDisposable
{
    public void Dispose() {
	Storage.Clear();
    }

    [Fact]
    public void Events_read_creates_event_in_folder() {
        Events.Read("event");

	Event Event = Storage.ReadData<Event>("events/event");
	Assert.Equal("event", Event.Name);
    }

    [Fact]
    public void Events_read_reads_existing_event() {
	Events.Read("event");
	Event Event = Events.Read("event");
	Assert.Equal("event", Event.Name);
    }

    [Fact]
    public void Events_delete_removes_event() {
	Event Event = Events.Read("event");
	Events.Delete(Event);

	Event NullEvent = Storage.ReadData<Event>("events/event");
	Assert.Null(NullEvent);
    }

    [Fact]
    public void Events_list_has_all_events() {
	Events.Read("event1");
	Events.Read("event2");
	Events.Read("event3");

	Assert.Equal(["event3", "event2", "event1"], Events.ListEvents());
    }
}

[Collection("Serial")]
public class EventTests : IDisposable
{
    private Event Event;
    private Guest Guest1;

    public EventTests() {
	Event = Events.Read("event");
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

    [Fact]
    public void Two_invited_Guests_can_have_the_same_name() {
	var guest1 = new Guest("guest");
	var guest2 = new Guest("guest");

	Events.InviteGuest(Event, guest1);
	Events.InviteGuest(Event, guest2);

	Assert.Equal(2, Event.Guests.Count());
    }

    [Fact]
    public void invitations_are_pending_by_default() {
	var invitation = Events.InviteGuest(Event, Guest1);

	Assert.Equal(InvitationStatus.Pending, invitation.InvitationStatus);
    }

    [Fact]
    public void invitations_can_be_rejected() {
	var invitation = Events.InviteGuest(Event, Guest1);
	Events.RejectInvitation(Event, invitation);

	Assert.Equal(InvitationStatus.Rejected, invitation.InvitationStatus);
    }

    [Fact]
    public void invitations_can_be_accepted() {
	var invitation = Events.InviteGuest(Event, Guest1);
	Events.AcceptInvitation(Event, invitation);

	Assert.Equal(InvitationStatus.Accepted, invitation.InvitationStatus);
    }

    [Fact]
    public void invitations_can_be_accepted_after_rejection() {
	var invitation = Events.InviteGuest(Event, Guest1);
	Events.RejectInvitation(Event, invitation);
	Events.AcceptInvitation(Event, invitation);

	Assert.Equal(InvitationStatus.Accepted, invitation.InvitationStatus);
    }

    [Fact]
    public void invitations_can_be_rejected_after_acceptence() {
	var invitation = Events.InviteGuest(Event, Guest1);
	Events.AcceptInvitation(Event, invitation);
	Events.RejectInvitation(Event, invitation);

	Assert.Equal(InvitationStatus.Rejected, invitation.InvitationStatus);
    }

    [Fact]
    public void events_can_have_expenses_added() {
	Storage.SetEditCallback(t => "b");
	Events.AddExpense(Event, 2.2);

	Assert.Single(Event.Expenses);
	Assert.Equal(2.2, Event.Expenses[0].Amount);
    }

    [Fact]
    public void expenses_can_be_removed_from_events() {
	Storage.SetEditCallback(t => "b");
	Events.AddExpense(Event, 2.2);

	Events.RemoveExpense(Event, Event.Expenses[0]);

	Assert.Empty(Event.Expenses);
    }
}
