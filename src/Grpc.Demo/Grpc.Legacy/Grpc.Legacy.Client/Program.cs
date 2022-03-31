using Grpc.Core;
using Grpc.Demo.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Legacy.Client
{
    internal class Program
    {
        static async Task Main()
        {
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Channel channel = new Channel("127.0.0.1:5286", ChannelCredentials.Insecure);

            try
            {
                while (true)
                {
                    var client = new OrderProtoService.OrderProtoServiceClient(channel);

                    Console.Write("Enter # of Orders to Get: ");

                    var request = new OrderRequest();
                    request.Count = int.Parse(Console.ReadLine());
                    Console.WriteLine();

                    using (var response = client.GetStream(request))
                    {
                        while (await response.ResponseStream.MoveNext(cancellationToken))
                        {
                            var order = response.ResponseStream.Current;
                            WriteOrderData(order);
                        }
                    }

                    Console.WriteLine();
                }

            } finally
            {
                if(channel.State != ChannelState.Shutdown)
                    await channel.ShutdownAsync();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void WriteOrderData(Order order)
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

    }

}
