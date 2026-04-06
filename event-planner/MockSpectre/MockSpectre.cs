namespace Spectre.Console;

public interface IPrompt<T> { }

public class TextPrompt<T> : IPrompt<T>
{
    public TextPrompt(T prompt) {
	return;
    }

    public TextPrompt<T> AllowEmpty() { return this; }
}

public class SelectionPrompt<T> : IPrompt<T> {
    public SelectionPrompt<T> Title(string title) { return this; }
    public SelectionPrompt<T> WrapAround() { return this; }
    public SelectionPrompt<T> AddChoices(IEnumerable<T> choices) { return this; }
    public SelectionPrompt<T> UseConverter(Func<T, string> converter) { return this; }
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
}
