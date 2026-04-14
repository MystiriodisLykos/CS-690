namespace PlannerService;

using StorageService;

public static class Notes
{
    private static string NotePath(Note note) {
	return Path.Combine("notes", note.Path);
    }

    public static string? ReadNote(Note note)
    {
	return Storage.ReadData(NotePath(note));
    }

    public static void MarkTodo(Event Event, Note note) {
	var can_mark = false;
	foreach (var guest in Event.Guests) {
	    if (guest.Notes.Contains(note)) {
		can_mark = true;
		break;
	    }
	}
	if (! (can_mark || Event.Notes.Contains(note))) return;
	foreach (var todo in Event.TodoNotes) {
	    // Don't add notes already marked as todo.
	    if (todo == note) return;
	}
	Event.TodoNotes.Add(note);
	Events.Save(Event);
    }

    public static void UnMarkTodo(Event Event, Note note) {	
	Event.TodoNotes.Remove(note);
	Events.Save(Event);
    }

    public static string? EditNote(Event Event, INoted On, Note note)
    {
        if (Storage.EditPath(NotePath(note)))
        {
	    var text = ReadNote(note);
	    StoreNote(Event, On, note, text);
	    return text;
	}
        // Found no editor apps, return
        return null;
    }

    public static void StoreNote(
	Event Event,
	INoted On,
	Note note,
	string text)
    {
	/* Store the `note` `On` with `text` for `Event` */

	// TODO: do I need to pass `note` or can I make a new one on the fly?
	//   Check call sites.
	//   In fact I think notes should only be returned objects, so that
	//   Only notes with existing text have associated objects.
	var path = NotePath(note);
	/* Store note if associated text is not empty */
	if (string.IsNullOrWhiteSpace(text)) {
	    Storage.RemoveData(path);
	    On.Notes.Remove(note);
	    UnMarkTodo(Event, note);
	} else {
	    Storage.WriteData(path, text);
	    On.Notes.Add(note);
	}
	Events.Save(Event);
    }

    public static IEnumerable<(A, IEnumerable<B>)> NoteTree<A, B>(
	Func<Event, A> project_event,
	Func<Invitation, A> project_invitation,
	Func<Note, B> project_note,
	Event Event
    ) {
	/* Build an enumerable grouping notes by the Event/Guest.
	   Projected with the associated functions
	*/
	var event_notes = (project_event(Event),
			   from note in Event.Notes
			   select project_note(note));

	var guest_notes = 
	    from guest in Event.Guests
	    select (project_invitation(guest),
		    from note in guest.Notes
		    select project_note(note));

	return guest_notes.Prepend(event_notes);
    }

    public static IEnumerable<((string, INoted), IEnumerable<(string, Note)>)> NoteLabeledTree(Event Event) {
	/* Build an enumerable grouping of notes by the Event/Guest.
	   Encluding a string representation of the object.
	 */
	return NoteTree<(string, INoted), (string, Note)>(
	    e => ("event", e),
	    i => (i.Guest.Name, i),
	    n => (ReadNote(n), n),
	    Event);
    }

    public static IEnumerable<(string, IEnumerable<string>)> NoteLabelTree(Event Event) {
	/* Build an enumerable grouping of note text by the Event/Guest name.
	   Not be be confused with NoteLabeledTree.
	 */
	return NoteTree(
	    e => "event",
	    i => i.Guest.Name,
	    n => ReadNote(n),
	    Event);
    }
}
