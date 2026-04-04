namespace StorageService;

using System.Runtime.Serialization;
using System.Xml;

public static class Persist
{
    public static readonly string EnvHome = "EVENT_PLANNER_HOME";
    public static readonly string DataFolder = ".cs-690.bd.event-planner";

    public static void WriteData(string path, string text)
    {
        var file_path = PersistPath(path);
        File.Create(file_path).Close();
        File.AppendAllText(file_path, text);
    }
    
    public static void WriteData<T>(string path, T data)
    {
        var file_path = PersistPath(path);

        DataContractSerializer serializer = new(typeof(T));
	var output = new StreamWriter(file_path, false);
	using (var writer = new XmlTextWriter(output)) {
	    serializer.WriteObject(writer, data);
	}
    }

    public static string ReadData(string path)
    {
        var file_path = PersistPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(file_path));
        return File.ReadAllText(file_path);
    }

    public static T ReadData<T>(string path) {
	var file_path = PersistPath(path);
	DataContractSerializer serializer = new(typeof(T));
	using (var reader = XmlReader.Create(file_path)) {
	    return (T)serializer.ReadObject(reader);
	}
    }

    public static void RemoveData(string path) {
        var file_path = PersistPath(path);
        File.Delete(file_path);
    }

    private static string PersistPath(string path)
    {
	string directory = Path.Combine(
	    (
		Environment.GetEnvironmentVariable(EnvHome) ??
		Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
	    ),
	    DataFolder
	);
	var file_path = Path.Combine(directory, path);
	Directory.CreateDirectory(Path.GetDirectoryName(file_path));
        return file_path;
    }

    // public static bool EditPath(string editor, string path, string args)
    // {
    //     try
    //     {
    //         var process = new Process();
    //         process.StartInfo.FileName = editor;
    //         process.StartInfo.Arguments = path + args;
    //         process.Start();
    //         process.WaitForExit();
    //         return true;
    //     } catch (Win32Exception) {
    //         return false;
    //     }
    // }
}
