namespace EvenPlannerCLI;

using PlannerService;
using Persistence = PlannerService.Storage;

public class Model {

    private static KnownGuests GuestList { get; } = Persistence.Guests.ReadGuests();

    public static IEnumerable<Guest> KnownGuests(Guest Guest) {
	/* Find guests with the same name as the pass guest */
	return GuestList.FindGuest(Guest.Name);
    }

    public static void InviteGuest(Event Event, Guest Guest, bool newGuest) {
	if (newGuest)
	{
	    GuestList.AddGuest(Guest);
	    Persistence.Guests.WriteGuests(GuestList);
	}
	Event.InviteGuest(Guest);
	Persistence.EventData.WriteEvent(Event);
    }
    
    public static void StoreNote(
	Event Event,
	INoteable On,
	Note note,
	string text)
    {
	/* Store note if associated text is not empty */
	if (string.IsNullOrWhiteSpace(text)) {
	    Persistence.Notes.RemoveNote(note);
	    On.RemoveNote(note);
	} else {
	    Persistence.Notes.WriteNote(note, text);
	    On.AddNote(note);
	}
	Persistence.EventData.WriteEvent(Event);
    }

    public static string? EditNote(Note note) {
	/* Safely edit the given note */
        return Persistence.Notes.EditNote(note);
    }

    public static string? ReadNote(Note note) {
	return Persistence.Notes.ReadNote(note);
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

    public static IEnumerable<((string, INoteable), IEnumerable<(string, Note)>)> NoteLabeledTree(Event Event) {
	/* Build an enumerable grouping of notes by the Event/Guest.
	   Encluding a string representation of the object.
	 */
	return NoteTree<(string, INoteable), (string, Note)>(
	    e => ("event", e),
	    i => (i.Guest.Name, i),
	    n => (Model.ReadNote(n), n),
	    Event);
    }

    public static IEnumerable<(string, IEnumerable<string>)> NoteLabelTree(Event Event) {
	/* Build an enumerable grouping of note text by the Event/Guest name.
	   Not be be confused with NoteLabeledTree.
	 */
	return NoteTree(
	    e => "event",
	    i => i.Guest.Name,
	    n => Model.ReadNote(n),
	    Event);
    }
}
