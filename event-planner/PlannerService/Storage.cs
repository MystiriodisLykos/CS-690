namespace PlannerService.Storage;

using System.Xml.Serialization;
using System.IO;

// using PlannerService;

static class Persist
{
    private static readonly string directory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".cs-690.bd.event-planner"
    );

    // Replicated from:
    //   https://blog.danskingdom.com/saving-and-loading-a-c-objects-data-to-an-xml-json-or-binary-file/
    public static void WriteData<T>(string path, T data, bool append = false) where T : new()
    {
        Directory.CreateDirectory(directory);
        TextWriter writer = null;
        try
        {
            XmlSerializer serializer = new(typeof(T));
            writer = new StreamWriter(Path.Combine(directory, path), append);
            serializer.Serialize(writer, data);
        } finally {
            writer?.Close();
        }
    }

    public static T ReadData<T>(string path) where T : new() {
        Directory.CreateDirectory(directory);
        Console.WriteLine(directory);
        TextReader reader = null;
        try {
            XmlSerializer serializer = new(typeof(T));
            reader = new StreamReader(Path.Combine(directory, path));
            return (T)serializer.Deserialize(reader);
        } finally {
            reader?.Close();
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