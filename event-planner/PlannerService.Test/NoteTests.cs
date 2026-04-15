namespace PlannerService.Test;

using PlannerService;
using StorageService;

[Collection("Serial")]
public class NoteTests : IDisposable
{
    private Note note1;
    private Guest guest1;
    private Event testEvent;

    public NoteTests() {
        note1 = new Note();
	guest1 = new Guest("guest1");
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

    [Fact]
    public void Note_edit_returns_text() {
	Notes.StoreNote(testEvent, testEvent, note1, "a");

	Storage.SetEditCallback(t => "b");

	var result = Notes.EditNote(testEvent, testEvent, note1);
	Assert.Equal("b", result);
    }

    [Fact]
    public void Notes_can_be_added_to_guest_invitations() {
	var invitation = Events.InviteGuest(testEvent, guest1);
	Notes.StoreNote(testEvent, invitation, note1, "guest note");

	Assert.Equal("guest note", Notes.ReadNote(note1));
	Assert.Contains(note1, invitation.Notes);
    }

    [Fact]
    public void Noted_labled_tree_puts_note_text_as_label() {
	var invitation = Events.InviteGuest(testEvent, guest1);
	Notes.StoreNote(testEvent, invitation, note1, "guest note");
	var note2 = new Note();
	Notes.StoreNote(testEvent, testEvent, note2, "event note");

	foreach (var (_, notes) in Notes.NoteLabeledTree(testEvent)) {
	    foreach (var (label, note) in notes) {
		Assert.Equal(Notes.ReadNote(note), label);
	    }
	}
    }

    [Fact]
    public void Noted_labled_tree_puts_noted_names_as_label() {
	var guest1 = new Guest("Guest 1");
	var invitation = Events.InviteGuest(testEvent, guest1);
	Notes.StoreNote(testEvent, invitation, note1, "guest note");
	var note2 = new Note();
	Notes.StoreNote(testEvent, testEvent, note2, "event note");

	foreach (var ((label, noted), _) in Notes.NoteLabeledTree(testEvent)) {
	    if (noted == guest1) {
		Assert.Equal(guest1.Name, label);
	    } else if (noted == testEvent) {
		Assert.Equal("event", label);
	    }
	}
    }

    [Fact]
    public void Noted_labled_tree_puts_notes_under_corret_noted() {
	var guest1 = new Guest("Guest 1");
	var invitation = Events.InviteGuest(testEvent, guest1);
	Notes.StoreNote(testEvent, invitation, note1, "guest note");
	var note2 = new Note();
	Notes.StoreNote(testEvent, testEvent, note2, "event note");

	foreach (var ((_, noted), notes) in Notes.NoteLabeledTree(testEvent)) {
	    foreach (var (label, note) in notes) {
		Assert.Contains(note, noted.Notes);
	    }
	}
    }

    [Fact]
    public void notes_can_be_marked_as_todo() {
	Notes.StoreNote(testEvent, testEvent, note1, "test todo");
	Notes.MarkTodo(testEvent, note1);

	Assert.Single(testEvent.TodoNotes);
	Assert.Contains(note1, testEvent.TodoNotes);
    }

    [Fact]
    public void guest_notes_can_be_marked_as_todo() {
	var invitation = Events.InviteGuest(testEvent, guest1);
	Notes.StoreNote(testEvent, invitation, note1, "test todo");
	Notes.MarkTodo(testEvent, note1);

	Assert.Single(testEvent.TodoNotes);
	Assert.Contains(note1, testEvent.TodoNotes);
    }

    [Fact]
    public void notes_can_be_unmarked_as_todo() {
	Notes.StoreNote(testEvent, testEvent, note1, "test todo");
	Notes.MarkTodo(testEvent, note1);	
	Notes.UnMarkTodo(testEvent, note1);

	Assert.Empty(testEvent.TodoNotes);
    }

    [Fact]
    public void notes_not_on_event_or_guest_cannot_be_marked_as_todo() {
	Notes.MarkTodo(testEvent, note1);

	Assert.Empty(testEvent.TodoNotes);
    }

    [Fact]
    public void when_note_is_removed_it_is_removed_from_todos() {
	Notes.StoreNote(testEvent, testEvent, note1, "test todo");
	Notes.MarkTodo(testEvent, note1);

	Storage.SetEditCallback(t => " ");

	Notes.EditNote(testEvent, testEvent, note1);

	Assert.Empty(testEvent.TodoNotes);
    }

    [Fact]
    public void todo_marking_order_is_preserved() {
	Notes.StoreNote(testEvent, testEvent, note1, "a");

	var note2 = new Note();
	Notes.StoreNote(testEvent, testEvent, note2, "b");

	Notes.MarkTodo(testEvent, note2);
	Notes.MarkTodo(testEvent, note1);

	Assert.Equal([note2, note1], testEvent.TodoNotes);
    }

    [Fact]
    public void marking_a_not_todo_twice_is_the_same_as_once() {
	Notes.StoreNote(testEvent, testEvent, note1, "a");

	Notes.MarkTodo(testEvent, note1);
	Notes.MarkTodo(testEvent, note1);

	Assert.Equal([note1], testEvent.TodoNotes);
    }
}
