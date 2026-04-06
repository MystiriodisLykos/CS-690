namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

class AddDietaryMenu {
    // fake requirement to add a new requirement.
    protected readonly static Note fake_requirement = new Note();

    protected string NoteConverter(Note note) {
	if (note == fake_requirement) {
	    return "Add New";
	}
	return Notes.ReadNote(note);
    }

    public void Run(Event Event, IDietaryRequirements noted) {
	var requirement = AnsiConsole.Prompt(
	    new SelectionPrompt<Note>()
	    .Title("Select a known dietary requirement or add a new one")
	    .WrapAround()
	    .AddChoices(DietaryRequirements.KnownRestrictions)
	    .AddChoices(fake_requirement)
	    .UseConverter(NoteConverter)
	);

	if (requirement == fake_requirement) {
	    requirement = new Note();
	    DietaryRequirements.EditRequirement(Event, noted, requirement);
	    return;
	}

	DietaryRequirements.AddRequirement(Event, noted, requirement);
    }
}
