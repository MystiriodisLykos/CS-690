namespace PlannerService.Test;

using PlannerService;
using StorageService;

[Collection("Serial")]
public class DietaryRequirementsTests : IDisposable
{
    private Note note1;
    private Guest guest1;
    private Event testEvent;

    public DietaryRequirementsTests() {
        note1 = new Note();
	guest1 = new Guest("guest1");
	testEvent = Events.Read();
	// Mock storage needs note to exist before it can edit it.
	Notes.StoreNote(testEvent, testEvent, note1, "b");
    }

    public void Dispose() {
	Storage.Clear();
    }

    [Fact]
    public void Empty_dietary_requirements_are_not_added() {
	Storage.SetEditCallback(t => " ");

	DietaryRequirements.EditRequirement(testEvent, testEvent, note1);

	Assert.DoesNotContain(note1, DietaryRequirements.KnownRestrictions);
	Assert.DoesNotContain(note1, testEvent.DietaryRequirements);
    }

    [Fact]
    public void dietary_requirements_can_be_added_to_event_through_edit() {
	Storage.SetEditCallback(t => "a");

	DietaryRequirements.EditRequirement(testEvent, testEvent, note1);

	Assert.Contains(note1, DietaryRequirements.KnownRestrictions);
	Assert.Contains(note1, testEvent.DietaryRequirements);
    }

    [Fact]
    public void dietary_requirements_can_be_added_to_guest_through_edit() {
	Storage.SetEditCallback(t => "a");

	DietaryRequirements.EditRequirement(testEvent, guest1, note1);

	Assert.Contains(note1, DietaryRequirements.KnownRestrictions);
	Assert.Contains(note1, guest1.DietaryRequirements);
    }

    [Fact]
    public void Adding_the_same_requirement_to_both_event_and_guest_only_adds_it_one() {
	DietaryRequirements.AddRequirement(testEvent, guest1, note1);
	DietaryRequirements.AddRequirement(testEvent, guest1, note1);

	Assert.Equal([note1], DietaryRequirements.KnownRestrictions);
    }
}
