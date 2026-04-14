namespace StorageService;

public static class Storage
{
    /* Mock Storage system for testing the other services. */

    public static readonly string EnvHome = "EVENT_PLANNER_HOME";

    // Mock editting function for when EditPath is called.
    static Func<object, object> EditCallback;

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

    public static void SetEditCallback(Func<object, object> callback) {
	EditCallback = callback;
    }

    public static bool EditPath(string path) {
	var text = EditCallback(Data.GetValueOrDefault(path, ""));
	if (text != null) {
	    Data[path] = text;
	    return true;
	}
	return false;
    }

    public static void Clear() {
	Data.Clear();
	EditCallback = null;
    }
}
