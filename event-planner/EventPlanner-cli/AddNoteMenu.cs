namespace EvenPlannerCLI;

using Spectre.Console;
using PlannerService;

class AddNoteMenu {
    protected static TextPrompt<string> note_prompt =
	new TextPrompt<string>("Note:").AllowEmpty();

    public void Run(Event Event, INoteable noted) {
	Note note = new Note();
	// Use editor by default
	var text = Model.EditNote(note);
	if (text == null) {
	    text = AnsiConsole.Prompt(note_prompt);
	}
	Model.StoreNote(Event, noted, note, text);
    }
}
