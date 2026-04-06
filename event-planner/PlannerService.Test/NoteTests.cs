namespace PlannerService.Test;

using PlannerService;
using StorageService;

[Collection("Serial")]
public class NoteTests : IDisposable
{
    private Note note1;
    private Event testEvent;

    public NoteTests() {
        note1 = new Note();
	testEvent = Events.Read();
    }

    public void Dispose() {
	Storage.Clear();
    }

    [Fact]
    public void Empty_Note_is_not_stored() {
	Notes.StoreNote(testEvent, testEvent, note1, " ");
	Assert.Null(Notes.ReadNote(note1));
    }

    [Fact]
    public void Read_non_existing_note_is_null() {
	Assert.Null(Notes.ReadNote(note1));
    }

    [Fact]
    public void Stored_notes_are_readable() {
	Notes.StoreNote(testEvent, testEvent, note1, "a");
	Assert.Equal("a", Notes.ReadNote(note1));
    }

    [Fact]
    public void Stored_notes_add_to_noted() {
	Notes.StoreNote(testEvent, testEvent, note1, "a");
	Assert.Contains(note1, testEvent.Notes);
    }

    [Fact]
    public void Note_editted_to_empty_is_removed() {
	Notes.StoreNote(testEvent, testEvent, note1, "a");

	Storage.SetEditCallback(t => " ");

	Notes.EditNote(testEvent, testEvent, note1);
	Assert.DoesNotContain(note1, testEvent.Notes);
	Assert.Null(Notes.ReadNote(note1));
    }

    [Fact]
    public void Note_editted_does_not_add_note() {
	Notes.StoreNote(testEvent, testEvent, note1, "a");

	Storage.SetEditCallback(t => "b");

	Notes.EditNote(testEvent, testEvent, note1);

	Assert.Equal([note1], testEvent.Notes);
    }

    [Fact]
    public void Note_editted_saves_note() {
	Notes.StoreNote(testEvent, testEvent, note1, "a");

	Storage.SetEditCallback(t => "b");

	Notes.EditNote(testEvent, testEvent, note1);

	Assert.Equal("b", Notes.ReadNote(note1));
    }
}
