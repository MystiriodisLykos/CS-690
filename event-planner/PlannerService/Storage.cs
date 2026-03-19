namespace PlannerService.Storage;

using System.Xml.Serialization;
using System.IO;

// using PlannerService;

static class Persist
{
    private static readonly string directory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        // "./", // local testing
        ".cs-690.bd.event-planner"
    );

    public static void WriteData(string path, string text)
    {
        var file_path = Path.Combine(directory, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file_path));
        File.Create(file_path).Close();
        File.AppendAllText(file_path, text + Environment.NewLine);
    }

    // Replicated from:
    //   https://blog.danskingdom.com/saving-and-loading-a-c-objects-data-to-an-xml-json-or-binary-file/
    // Modified to always rewrite the file and use a specfiic directory
    public static void WriteData<T>(string path, T data) where T : new()
    {
        var file_path = Path.Combine(directory, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file_path));
        TextWriter writer = null;
        try
        {
            XmlSerializer serializer = new(typeof(T));
            writer = new StreamWriter(file_path, false);
            serializer.Serialize(writer, data);
        } finally {
            writer?.Close();
        }
    }

    public static string ReadData(string path)
    {
        var file_path = Path.Combine(directory, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file_path));
        return File.ReadAllText(file_path);
    }

    public static T ReadData<T>(string path) where T : new() {
        var file_path = Path.Combine(directory, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file_path));
        Console.WriteLine(Path.GetDirectoryName(file_path));
        TextReader reader = null;
        try {
            XmlSerializer serializer = new(typeof(T));
            reader = new StreamReader(file_path);
            return (T)serializer.Deserialize(reader);
        } finally {
            reader?.Close();
        }
    }
}

public static class Notes
{
    private readonly static string NotesDir = "notes";

    public static void WriteNote(Note note, string text)
    {
        var path = Path.Combine(NotesDir, note.Path);
        Persist.WriteData(path, text);
    }

    public static string? ReadNote(Note note)
    {
        var path = Path.Combine(NotesDir, note.Path);
        try {
            return Persist.ReadData(path);
        } catch (FileNotFoundException)
        {
            return null;
        }
    }
}

public static class Guests
{
    private readonly static string Path = "guest-list";
    public static void WriteGuests(KnownGuests guests)
    {
        Persist.WriteData(Path, guests.GuestList);
    }

    public static KnownGuests ReadGuests()
    {
        var knownGuests = new KnownGuests();
        try {
            knownGuests.GuestList = Persist.ReadData<List<Guest>>(Path);
        } catch (FileNotFoundException) {}
        return knownGuests;
    }
}

public static class EventData
{
    private readonly static string Path = "event";
    public static void WriteEvent(Event Event)
    {
        Persist.WriteData(Path, Event);
    }

    public static Event ReadEvent()
    {
        var Event = new Event();
        try {
            Event = Persist.ReadData<Event>(Path);
        } catch (FileNotFoundException) {}
        return Event;
    }
}