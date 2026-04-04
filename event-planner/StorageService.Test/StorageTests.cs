namespace StorageService.Test;

using StorageService;
using System.Runtime.Serialization;

public class StorageTests : IDisposable
{
    private readonly string test_home = "test_home";
    private readonly string test_data;
    public StorageTests() {
	Environment.SetEnvironmentVariable(Persist.EnvHome, @test_home);
	test_data = Path.Combine(test_home, Persist.DataFolder);
    }

    public void Dispose() {
	var dir = new DirectoryInfo(test_data);
	// foreach(var file in dir.GetFiles()) file.Delete();
	dir.Delete(true);
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
    public void Persist_writes_data_without_failing()
    {
	Persist.WriteData("data", test_data1);
    }

    [Fact]
    public void Persist_writes_text_without_failing()
    {
	Persist.WriteData("data", "test string");
    }

    [Fact]
    public void Persist_writes_to_expected_folder()
    {
	var file_path = Path.Combine("dataF", "data");
	Persist.WriteData(file_path, "test string");
	var out_path = Path.Combine(test_data, file_path);

	Assert.True(File.Exists(out_path));
    }

    [Fact]
    public void Persist_read_write_is_identity() {
	Persist.WriteData("data", test_data1);
	var read = Persist.ReadData<TestData>("data");

	Assert.Equal(test_data1.A, read.A);
	Assert.Equal(test_data1.B, read.B);
    }

    [Fact]
    public void Persist_overwrites_file() {
	var test_data2 = new TestData("other data", 20);

	Persist.WriteData("data", test_data1);
	Persist.WriteData("data", test_data2);

	var read = Persist.ReadData<TestData>("data");
	Assert.Equal(test_data2.A, read.A);
	Assert.Equal(test_data2.B, read.B);
    }
}
