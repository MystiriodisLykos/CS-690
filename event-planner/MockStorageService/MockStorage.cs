namespace StorageService;

public static class Storage
{
    /* Mock Storage system for testing the other services. */

    public static readonly string EnvHome = "EVENT_PLANNER_HOME";
    
    // Mock editting function for when EditPath is called.
    static Func<object, object> EditCallback { get; set; }
    static Dictionary<string, object> Data = new();

    public static void WriteData(string path, string text) {
	Data[path] = text;
    }
    
    public static void WriteData<T>(string path, T data) {
	Data[path] = data;
    }

    public static string? ReadData(string path) {
	return ReadData<string>(path);
    }

    public static T? ReadData<T>(string path) {
	return (T)Data.GetValueOrDefault(path, null);
    }

    public static void RemoveData(string path) {
	Data.Remove(path);
    }

    public static bool EditPath(string path) {
	if (Data.ContainsKey(path)) {
	    Data.Add(path, EditCallback(Data[path]));
	    return true;
	}
	return false;
    }
}
