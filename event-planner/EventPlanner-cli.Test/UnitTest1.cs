namespace EventPlanner_cli.Test;


using Spectre.Console;
using StorageService;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
	AnsiConsole.QueueResponse("hello");
	Storage.SetEditCallback(x => 'a');
    }
}
