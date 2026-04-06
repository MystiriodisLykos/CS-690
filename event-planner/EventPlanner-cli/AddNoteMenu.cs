namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

class AddNoteMenu {
    protected static TextPrompt<string> note_prompt =
	new TextPrompt<string>("Note:").AllowEmpty();

    public void Run(Event Event, INoted noted) {
	Note note = new Note();
	// Use editor by default
	var text = Notes.EditNote(Event, noted, note);
	if (text == null) {
	    text = AnsiConsole.Prompt(note_prompt);
	}
	Notes.StoreNote(Event, noted, note, text);
    }
}
