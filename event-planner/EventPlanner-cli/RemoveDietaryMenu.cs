namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

class RemoveDietaryMenu {
    // fake requirement to add a new requirement.
    protected readonly static Note fake_requirement = new Note();

    protected string NoteConverter(Note note) {
	if (note == fake_requirement) {
	    return "Quit";
	}
	return Notes.ReadNote(note);
    }

    public void Run(Event Event, IDietaryRequirements noted) {
	var requirement = AnsiConsole.Prompt(
	    new SelectionPrompt<Note>()
	    .Title("Select a dietary requirement to remove")
	    .WrapAround()
	    .AddChoices(noted.DietaryRequirements)
	    .AddChoices(fake_requirement)
	    .UseConverter(NoteConverter)
	);

	if (requirement == fake_requirement) {
	    return;
	}

	DietaryRequirements.RemoveRequirement(Event, noted, requirement);
    }
}
