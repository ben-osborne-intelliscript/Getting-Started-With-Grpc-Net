using Grpc.Net.Client;
using Grpc.Demo.Client;

CancellationToken cancellationToken = new CancellationTokenSource().Token;

using var channel = GrpcChannel.ForAddress("https://localhost:7273/", new GrpcChannelOptions() { MaxReceiveMessageSize = null });

while (true)
{
    var client = new OrderProtoService.OrderProtoServiceClient(channel);

    Console.Write("Enter # of Orders to Get: ");

    var request = new OrderRequest();
    request.Count = int.Parse(Console.ReadLine());
    Console.WriteLine();

    var response = await client.GetAsync(request);

    foreach(var order in response.Orders)
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
