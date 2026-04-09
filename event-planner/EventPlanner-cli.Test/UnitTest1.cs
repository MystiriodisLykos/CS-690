namespace EventPlanner_cli.Test;

using EventPlannerCLI;
using PlannerService;

// Mocked namespaces
using Spectre.Console;
using StorageService;

// Demo test of the CLI menus using a mock AnsiConsole.
public class UnitTest1
{
    [Fact]
    public void EvenMenu_add_note_prompts_user_when_edit_fails()
    {
	var addEventNoteMenu = new AddEventNote();
	var Event = Events.Read();

	var noteText = "Mock Ansi Response";

	AnsiConsole.QueueResponse(noteText);
	Storage.UseEditor = false;

	addEventNoteMenu.Run(Event);

	Assert.Equal(1, Event.Notes.Count);
	Assert.Equal(noteText, Notes.ReadNote(Event.Notes.FirstOrDefault()));
    }
}
