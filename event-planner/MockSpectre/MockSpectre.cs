namespace Spectre.Console;

public interface IPrompt<T> { }
public interface IRenderable { }

public class TextPrompt<T> : IPrompt<T>
{
    public TextPrompt(T prompt) {
	return;
    }

    public TextPrompt<T> AllowEmpty() { return this; }
}

public enum SelectionMode {
    Leaf = 0
}

public class SelectionPrompt<T> : IPrompt<T> {
    public SelectionPrompt<T> Title(string title) { return this; }
    public SelectionPrompt<T> WrapAround() { return this; }
    public SelectionPrompt<T> AddChoiceGroup(T group, IEnumerable<T> choices) { return this; }
    public SelectionPrompt<T> AddChoices(IEnumerable<T> choices) { return this; }
    public SelectionPrompt<T> AddChoices(T choice) { return this; }
    public SelectionPrompt<T> UseConverter(Func<T, string> converter) { return this; }
    public SelectionPrompt<T> Mode(SelectionMode mode) { return this; }
}

public class Tree : IRenderable {
    public Tree(string root) { }
    public Tree Guide(TreeGuide guide) { return this; }
    public Tree Style(Style style) { return this; }
    public Tree AddNode(string node) { return this; }
}

public enum TreeGuide {
    BoldLine = 0
}

public class Style {
    public static Style Parse(string style) {
	return new();
    }
}

public static class AnsiConsole
{
    private static Queue<object> queue = new();

    public static void QueueResponse(object response) {
	queue.Enqueue(response);
    }

    public static T Prompt<T>(IPrompt<T> prompter) {
	return (T)queue.Dequeue();
    }

    public static bool Confirm(string prompt) {
	return (bool)queue.Dequeue();
    }

    public static void Write(IRenderable renderable) { }
}
