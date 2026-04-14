namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

class NoteMenu : INestedMenu {
    public string MenuName { get; } = "Notes";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {
	    new ManageTodos(),
	    new ShowTodos(),
	    new EditNote(),
	    new ShowAllNotes()},
        "What would you like to do?"
    );
    public void Run(Event Event) {
        Menu.Run(Event);
    }
}

static class SelectNoteMenu
{
    protected class GroupSelect<T, V>
    {
	protected string Name;
        public T Data;
        public V Group;
        public static GroupSelect<T, V> ChildSelect(string name, T data, V group)
        {
	    var selector = new GroupSelect<T, V>();
	    selector.Name = name;
            selector.Data = data;
            selector.Group = group;
	    return selector;
        }

	public static GroupSelect<T, V> ParentSelect(string name) {
	    var selector = new GroupSelect<T, V>();
	    selector.Name = name;
	    return selector;
	}

        public override string ToString()
        {
            return Name;
        }
    }

    public static (INoted, Note) SelectNote(Event Event, string title) {
	var notePrompt = new SelectionPrompt<GroupSelect<Note, INoted>>()
            .Title(title)
            .WrapAround()
            .Mode(SelectionMode.Leaf);

	var notes_exist = false;
	foreach (var ((name, noteable), notes) in Notes.NoteLabeledTree(Event)) {
	    // Sentinal object of selction screen, not actually selectable
	    var group_ = GroupSelect<Note, INoted>.ParentSelect(name);

	    IEnumerable<GroupSelect<Note, INoted>> notes_ = [];
	    foreach (var (note_text, note_) in notes) {
		notes_ = notes_.Append(GroupSelect<Note, INoted>.ChildSelect(
		    note_text, note_, noteable)
		);
	    }

	    if (notes_.Any()) {
		notePrompt.AddChoiceGroup(group_, notes_.ToList());
		notes_exist = true;
	    }
	}

	if (! notes_exist) {
	    return (null, null);
	}

        var selection = AnsiConsole.Prompt(notePrompt);
	return (selection.Group, selection.Data);
    }
}

class EditNote : INestedMenu
{
    public string MenuName { get; } = "Edit Note";

    public void Run(Event Event)
    {
	var (noted, note) = SelectNoteMenu.SelectNote(Event, "Select Note to Edit");

	if (note == null) {
	    AnsiConsole.Confirm("No Notes found, please add at least one before trying to edit (Enter to Confrim");
	    return;
	}

	var text = Notes.EditNote(Event, noted, note);
        if (text == null)
        {
            AnsiConsole.Confirm(
                "Could not find an editor, please ensure one of the following is installed an on the PATH (vscode, notepad, emacs, vi). (Enter to Continue)"
            );
	    return;
        }
    }
}

class ShowAllNotes : INestedMenu
{
    public string MenuName { get; } = "Show All Notes";

    public void Run(Event Event)
    {
        Console.Clear();

	var allNotes = new Tree("")
	    .Guide(TreeGuide.BoldLine)
	    .Style(Style.Parse("dim"));

	foreach (var (node, notes) in Notes.NoteLabelTree(Event)) {
	    var nodeNotes = allNotes.AddNode($"Notes for {node}");

	    foreach (var note in notes) {
		nodeNotes.AddNode(note);
            }
	}

        AnsiConsole.Write(allNotes);
    }
}

class ManageTodos : INestedMenu
{
    public string MenuName { get; } = "Manage Todos";

    public void Run(Event Event) {
	Console.Clear();

	var (_, note) = SelectNoteMenu.SelectNote(Event, "Select Note to mark/umark as Todo");

	if (note == null) {
	    AnsiConsole.Confirm("No Notes found, please add at least one before trying to mark todos (Enter to Confrim");
	    return;
	}

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

class ShowTodos : INestedMenu
{
    public string MenuName { get; } = "Show Todos";

    public void Run(Event Event) {
	Console.Clear();

	var TodoNotes = new Tree("")
	    .Guide(TreeGuide.BoldLine)
	    .Style(Style.Parse("dim"));

	foreach (var note in Event.TodoNotes) {
	    TodoNotes.AddNode(Notes.ReadNote(note));
	}

        AnsiConsole.Write(TodoNotes);
    }
}
