namespace EventPlannerCLI;

using Spectre.Console;
using PlannerService;

class NoteMenu : INestedMenu {
    public string MenuName { get; } = "Notes";

    protected static MenuOfMenus Menu = new(
        new List<INestedMenu> {new EditNote(), new ShowAllNotes()},
        "What would you like to do?"
    );
    public void Run(Event Event) {
        Menu.Run(Event);
    }
}

class EditNote : INestedMenu
{
    public string MenuName { get; } = "Edit Note";

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

    public void Run(Event Event)
    {

        var notePrompt = new SelectionPrompt<GroupSelect<Note, INoted>>()
            .Title("Select Note to Edit")
            .WrapAround()
            .Mode(SelectionMode.Leaf);

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
	    }
	}

        var selection = AnsiConsole.Prompt(notePrompt);
	var note = selection.Data;
	var noted = selection.Group;

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
