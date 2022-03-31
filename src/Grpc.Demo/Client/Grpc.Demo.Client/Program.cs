using Grpc.Net.Client;
using Grpc.Demo.Client;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Core;
using Grpc.Net.Client.Configuration;

CancellationToken cancellationToken = new CancellationTokenSource().Token;

var factory = new StaticResolverFactory(addr => new[]
{
    new BalancerAddress(new System.Net.DnsEndPoint("localhost", 5126)),
    new BalancerAddress(new System.Net.DnsEndPoint("localhost", 5127)),
    new BalancerAddress(new System.Net.DnsEndPoint("localhost", 5128))
});

var services = new ServiceCollection()
    .AddSingleton<ResolverFactory>(factory);

var defaultMethodConfig = new MethodConfig
{
    Names = { MethodName.Default },
    RetryPolicy = new RetryPolicy
    {
        MaxAttempts = 5,
        InitialBackoff = TimeSpan.FromSeconds(1),
        MaxBackoff = TimeSpan.FromSeconds(5),
        BackoffMultiplier = 1.5,
        RetryableStatusCodes = { StatusCode.Unavailable }
    }
};

/*
The current configuration is for demonstrating client-side load balancing. To run the 
client against a single GRPC service, use:

using var channel = GrpcChannel.ForAddress("https://localhost:7296/", new GrpcChannelOptions() { MaxReceiveMessageSize = null });

*/

var channel = GrpcChannel.ForAddress(
    "static:///feed-host",
    new GrpcChannelOptions
    {
        Credentials = ChannelCredentials.Insecure,
        ServiceProvider = services.BuildServiceProvider(),
        ServiceConfig = new ServiceConfig
        {
            LoadBalancingConfigs = {
                new RoundRobinConfig()
            },
            MethodConfigs = { defaultMethodConfig }
        }
    });


while (true)
{
    var client = new OrderProtoService.OrderProtoServiceClient(channel);

    Console.Write("Enter # of Orders to Get: ");

    var request = new OrderRequest();
    request.Count = int.Parse(Console.ReadLine());
    Console.WriteLine();

    using var response = client.GetStream(request);

    while (await response.ResponseStream.MoveNext(cancellationToken))
    {
        var order = response.ResponseStream.Current;
        WriteOrderData(order);
    }

    Console.WriteLine();
}

void WriteOrderData(Order order)
{
    Console.WriteLine($@"ORDER #{order.OrderNumber}
------------------------------------------------------
Date: {order.OrderDate.ToDateTimeOffset().ToLocalTime()}

    ------------------------------------------------------
    Customer Info
    ------------------------------------------------------
    {order.Customer.FirstName} {order.Customer.LastName}
    {order.Customer.BusinessName}
    {order.Customer.AddressLine1}
    {order.Customer.AddressLine2}
    {order.Customer.City}, {order.Customer.State} {order.Customer.Zip}

    ------------------------------------------------------
    Item
    ------------------------------------------------------
    Name: {order.Item.Name}
    Description: {order.Item.Description}
    SKU: {order.Item.Sku}
    Quantity: {order.Item.Quantity}
    Price Per Unit: {order.Item.PricePerUnit:C3}

    ------------------------------------------------------
    Total
    ------------------------------------------------------
    {order.Item.Total:C3}

------------------------------------------------------
");


}

