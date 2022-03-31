using Grpc.Core;
using Grpc.Demo.Server.Providers;

namespace Grpc.Demo.Server.Services
{
    public class OrderService : OrderProtoService.OrderProtoServiceBase
    {
        public override Task<OrderResponse> Get(OrderRequest request, ServerCallContext context)
        {
            var response = new OrderResponse();
            response.Orders.AddRange(OrderProvider.GetOrdersReadyForProcessing(request.Count));
            return Task.FromResult(response);
        }

        public override async Task GetStream(OrderRequest request, IServerStreamWriter<Order> responseStream, ServerCallContext context)
        {
            foreach(var order in OrderProvider.GetOrdersReadyForProcessing(request.Count))
                await responseStream.WriteAsync(order);
            return;
        }
    }
}
