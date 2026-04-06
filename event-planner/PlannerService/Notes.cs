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

    public static string? EditNote(Event Event, INoted On, Note note)
    {
        if (Storage.EditPath(NotePath(note)))
        {
	    var text = ReadNote(note);
	    StoreNote(Event, On, note, text);
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
	var path = NotePath(note);
	/* Store note if associated text is not empty */
	if (string.IsNullOrWhiteSpace(text)) {
	    Storage.RemoveData(path);
	    On.Notes.Remove(note);
	} else {
	    Storage.WriteData(path, text);
	    On.Notes.Add(note);
	}
	Events.Save(Event);
    }

    // public static IEnumerable<(A, IEnumerable<B>)> NoteTree<A, B>(
    // 	Func<Event, A> project_event,
    // 	Func<Invitation, A> project_invitation,
    // 	Func<Note, B> project_note,
    // 	Event Event
    // ) {
    // 	/* Build an enumerable grouping notes by the Event/Guest.
    // 	   Projected with the associated functions
    // 	*/
    // 	var event_notes = (project_event(Event),
    // 			   from note in Event.Notes
    // 			   select project_note(note));

    // 	var guest_notes = 
    // 	    from guest in Event.Guests
    // 	    select (project_invitation(guest),
    // 		    from note in guest.Notes
    // 		    select project_note(note));

    // 	return guest_notes.Prepend(event_notes);
    // }

    // public static IEnumerable<((string, INoted), IEnumerable<(string, Note)>)> NoteLabeledTree(Event Event) {
    // 	/* Build an enumerable grouping of notes by the Event/Guest.
    // 	   Encluding a string representation of the object.
    // 	 */
    // 	return NoteTree<(string, INoted), (string, Note)>(
    // 	    e => ("event", e),
    // 	    i => (i.Guest.Name, i),
    // 	    n => (Model.ReadNote(n), n),
    // 	    Event);
    // }

    // public static IEnumerable<(string, IEnumerable<string>)> NoteLabelTree(Event Event) {
    // 	/* Build an enumerable grouping of note text by the Event/Guest name.
    // 	   Not be be confused with NoteLabeledTree.
    // 	 */
    // 	return NoteTree(
    // 	    e => "event",
    // 	    i => i.Guest.Name,
    // 	    n => Model.ReadNote(n),
    // 	    Event);
    // }
}
