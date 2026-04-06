namespace StorageService;

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

public static class Storage
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

    public static string? ReadData(string path)
    {
	try {
	    var file_path = PersistPath(path);
	    Directory.CreateDirectory(Path.GetDirectoryName(file_path));
	    return File.ReadAllText(file_path);
	} catch (FileNotFoundException) {
	    return null;
	}
    }

    public static T ReadData<T>(string path) {
	try {
	    var file_path = PersistPath(path);
	    DataContractSerializer serializer = new(typeof(T));
	    using (var reader = XmlReader.Create(file_path)) {
		return (T)serializer.ReadObject(reader);
	    }
	} catch (FileNotFoundException) {
	    return default(T);
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

    public static bool EditPath(string editor, string path, string args)
    {
	/* Use `editor` with `args` to edit the `path`
	   returns boolean based on if the editing worked or not.
	 */
	var file_path = PersistPath(path);
        try
        {
            var process = new Process();
            process.StartInfo.FileName = editor;
            process.StartInfo.Arguments = file_path + args;
            process.Start();
            process.WaitForExit();
            return true;
        } catch (Win32Exception) {
            return false;
        }
    }

    public static bool EditPath(string path) {
	/* Edit the `path` trying a handful of common editors.
	   returns true when editing was successful,
	   false when editing doesn't work or we can't find an editor.
	 */
        List<string> editors = ["notepad", "emacs", "vi"];

        // Try to use vscode first because it has special args
        if (! EditPath("code", path, " --wait"))
        {
            // If we can't open with vscode try some other common apps
            foreach (var editor in editors)
            {
                if (EditPath(editor, path, ""))
                {
                    return true;
                }
            }
	} else {
	    // vscode editing worked
	    return true;
	}
	// Could not find an editor.
	return false;
    }
}
