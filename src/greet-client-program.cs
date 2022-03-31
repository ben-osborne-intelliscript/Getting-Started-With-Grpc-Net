using Grpc.Net.Client;
using Grpc.Demo.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:7273/", new GrpcChannelOptions() { MaxReceiveMessageSize = null });

while (true)
{
    var client = new Greeter.GreeterClient(channel);

    Console.Write("What is your name? ");

    var request = new HelloRequest()
    {
        Name = Console.ReadLine()
    };

    var response = await client.SayHelloAsync(request);

    Console.WriteLine(response.Message);
    Console.WriteLine();
}
