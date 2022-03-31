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

var channel = GrpcChannel.ForAddress(
    "static:///feed-host",
    new GrpcChannelOptions
    {
        Credentials = ChannelCredentials.Insecure,
        ServiceProvider = services.BuildServiceProvider(),
        ServiceConfig = new ServiceConfig { 
            LoadBalancingConfigs = {
                new RoundRobinConfig()
            },
            MethodConfigs = { defaultMethodConfig }
        }
    });

while (true)
{
    var client = new OrderProtoService.OrderProtoServiceClient(channel);

    var request = new OrderRequest();
    request.Count = 1;
    Console.WriteLine();

    var response = await client.GetAsync(request);

    foreach (var order in response.Orders)
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

    Console.WriteLine();
}