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

	SetTodoMenu.SetTodo(Event, note);
    }
}


static class SetTodoMenu {
    public static void SetTodo(Event Event, Note note) {
	var is_todo = AnsiConsole.Prompt(
	    new SelectionPrompt<string>()
	    .Title("Should be Todo note or not?)")
	    .AddChoices(new[] {"Yes", "No"})
	);

	if (is_todo == "Yes") {
	    Notes.MarkTodo(Event, note);
	} else {
	    Notes.UnMarkTodo(Event, note);
	}
    }
}
