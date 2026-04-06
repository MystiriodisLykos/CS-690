namespace PlannerService.Test;

using PlannerService;
using StorageService;

[Collection("Serial")]
public class GuestListTests : IDisposable
{
    private Guest Guest1;
    private Guest Guest2;

    public GuestListTests() {
	GuestList.Load();
	Guest1 = new Guest("guest1");
	Guest2 = new Guest("guest2");
    }

    public void Dispose() {
	Storage.Clear();
    }

    [Fact]
    public void Adding_a_guest_puts_them_in_known_list() {
	GuestList.AddGuest(Guest1);
	Assert.Contains(Guest1, GuestList.KnownGuests(Guest1));
    }

    [Fact]
    public void Adding_a_guest_is_idempotent() {
	var first = GuestList.AddGuest(Guest1);
	var second = GuestList.AddGuest(Guest1);

	Assert.True(first);
	Assert.False(second);
	
	Assert.Equal([Guest1], GuestList.KnownGuests(Guest1));
    }

    [Fact]
    public void Non_added_guest_is_not_in_guest_list() {
	GuestList.AddGuest(Guest1);
	Assert.DoesNotContain(Guest2, GuestList.KnownGuests(Guest2));
    }

    [Fact]
    public void All_guests_with_the_same_name_in_known_guests() {
	var a1 = new Guest("a");
	var a2 = new Guest("a");
	GuestList.AddGuest(a1);
	Assert.Contains(a1, GuestList.KnownGuests(a2));
    }

    [Fact]
    public void Guest_list_additions_persist_load() {
	GuestList.AddGuest(Guest1);
	GuestList.Load();
	Assert.Contains(Guest1, GuestList.KnownGuests(Guest1));
    }
}
