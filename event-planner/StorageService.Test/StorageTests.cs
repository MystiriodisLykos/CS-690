namespace StorageService.Test;

using StorageService;
using System.Runtime.Serialization;

public class StorageTests : IDisposable
{
    private readonly string test_home = "test_home";

    public StorageTests() {
	Environment.SetEnvironmentVariable(Storage.EnvHome, @test_home);
    }

    public void Dispose() {
	if (Directory.Exists(test_home)) {
	    var dir = new DirectoryInfo(test_home);
	    dir.Delete(true);
	}
    }

    [DataContract(Name = "test")]
    private class TestData
    {
	[DataMember(Name = "A")]
	public string A { get; internal set; }
	[DataMember(Name = "B")]
	public int B { get; internal set; }

	public TestData(string a, int b) {
	    A = a;
	    B = b;
	}
    }

    private TestData test_data1 = new TestData("test string", 2);

    [Fact]
    public void Storage_writes_data_without_failing()
    {
	Storage.WriteData("data", test_data1);
    }

    [Fact]
    public void Storage_writes_text_without_failing()
    {
	Storage.WriteData("data", "test string");
    }

    [Fact]
    public void Storage_writes_to_folders_folder()
    {
	var file_path = Path.Combine("dataF", "data");
	Storage.WriteData(file_path, "test string");

	string read = Storage.ReadData(file_path);

	Assert.Equal("test string", read);
    }

    [Fact]
    public void Storage_read_write_is_identity() {
	Storage.WriteData("data", test_data1);
	var read = Storage.ReadData<TestData>("data");

	Assert.Equal(test_data1.A, read.A);
	Assert.Equal(test_data1.B, read.B);
    }

    [Fact]
    public void Storage_overwrites_file() {
	var test_data2 = new TestData("other data", 20);

	Storage.WriteData("data", test_data1);
	Storage.WriteData("data", test_data2);

	var read = Storage.ReadData<TestData>("data");
	Assert.Equal(test_data2.A, read.A);
	Assert.Equal(test_data2.B, read.B);
    }

    [Fact]
    public void Storage_read_non_existent_is_null() {
	Assert.Equal(null, Storage.ReadData("data-bad"));
    }

    [Fact]
    public void Storage_read_non_existent_generic_is_null() {
	Assert.Equal(null, Storage.ReadData<TestData>("data-bad"));
    }

    [Fact]
    public void Storage_ListDir_lists_directory_contents() {
	Storage.WriteData("dir/a1", "a1 data");
	Storage.WriteData("dir/a2", "a2 data");
	Storage.WriteData("dir/a3", "a3 data");

	Storage.WriteData("b1", "b1 data");

	Assert.Equal(["a3", "a2", "a1"], Storage.ListDir("dir/"));
    }
}
