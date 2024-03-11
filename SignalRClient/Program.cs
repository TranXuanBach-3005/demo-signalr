

using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("Please specify the URL of SignalR hub");
var url = Console.ReadLine();

var connection = new HubConnectionBuilder().WithUrl(url).Build();

connection.On<string>("ReceiveMessage",
    message => Console.WriteLine($"SignalR Hub Message:{message}"));

try
{

    await connection.StartAsync();
    var running = true;

    while (running)
    {
        var message = string.Empty;

        Console.WriteLine("Please specify the action");
        Console.WriteLine("0 - boardcast to all");
        Console.WriteLine("1 - send to others");
        Console.WriteLine("2 - send to self");
        Console.WriteLine("exit - Exit the program");
        var action = Console.ReadLine();

        Console.WriteLine("Please specify the message:");
        message = Console.ReadLine();

        switch (action)
        {
            case "0":
                await connection.SendAsync("BroadcastMessage", message);
                break;
            case "1":
                await connection.SendAsync("SendToOthers", message);
                break;
            case "2":
                await connection.SendAsync("SendToCaller", message);
                break;
            case "exit":
                running = false;
                break;
            default:
                Console.WriteLine("Invalid action specified");
                break;
        }

    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();
    return;
}
//https://scientificprogrammer.net/2022/09/24/sending-messages-to-individual-signalr-clients-or-groups-of-clients/