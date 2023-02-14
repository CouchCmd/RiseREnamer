using System.Threading;
using RiseFileRenamer;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Please make sure to put mhrisePC_GamePass.list in same folder as executable");
var myLocation = Directory.GetCurrentDirectory();

if (!File.Exists($"{myLocation}\\mhrisePC_GamePass.list"))
{
    Console.WriteLine($"\nmhrisePC_GamePass.lis missing at \n{myLocation}");
    Thread.Sleep(5000);
}

Console.WriteLine("\nPlease enter folder location of mod you would like to update for gamepass or exit to quite");
var modLocation = Console.ReadLine();

while (string.IsNullOrEmpty(modLocation) || modLocation.Equals("exit", StringComparison.OrdinalIgnoreCase) || !Directory.Exists(modLocation) || !modLocation.EndsWith("natives", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine($"\nPlease enter a valid location (must start at the natives folder) or exit");
    modLocation = Console.ReadLine();
}

if (modLocation.Equals("exit", StringComparison.OrdinalIgnoreCase))
    System.Environment.Exit(0);

var locations = await File.ReadAllLinesAsync($"{myLocation}\\mhrisePC_GamePass.list");

FileTraversalHelper.FileTraversal(modLocation, locations.ToList(), modLocation.Replace("natives", string.Empty));

Console.WriteLine("Completed Conversion");
Thread.Sleep(5000);