using Bogus;
using AutoBogus;
using Google.Protobuf.WellKnownTypes;

namespace Grpc.Demo.Server.Services
{
    public class OrderProvider
    {
        public static IEnumerable<Order> GetOrdersReadyForProcessing(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(1000);
                yield return GenerateOrder();
            }
        }
        protected static Order GenerateOrder()
        {
            var itemFaker = new AutoFaker<Item>()
                .RuleFor(x => x.Name, x => new Faker().Commerce.Product())
                .RuleFor(x => x.Description, x => new Faker().Commerce.ProductDescription())
                .RuleFor(x => x.Sku, x => new Faker().Commerce.Ean13())
                .RuleFor(x => x.Quantity, x => new Randomizer().Int(1, 1000))
                .RuleFor(x => x.PricePerUnit, x => new Randomizer().Double(10, 1000))
                ;

            var customerFaker = new AutoFaker<Customer>()
                .RuleFor(x => x.FirstName, x => new Faker().Person.FirstName)
                .RuleFor(x => x.LastName, x => new Faker().Person.LastName)
                .RuleFor(x => x.AddressLine1, x => new Faker().Person.Address.Street)
                .RuleFor(x => x.AddressLine2, x => new Faker().Person.Address.Suite)
                .RuleFor(x => x.City, x => new Faker().Person.Address.City)
                .RuleFor(x => x.State, x => new Faker().Person.Address.State)
                .RuleFor(x => x.Zip, x => new Faker().Person.Address.ZipCode)
                .RuleFor(x => x.BusinessName, x => new Faker().Company.CompanyName())
                ;

            var order = new AutoFaker<Order>()
                .RuleFor(x => x.OrderNumber, new Randomizer().Guid().ToString())
                .RuleFor(x => x.Customer, customerFaker)
                .RuleFor(x => x.Item, itemFaker)
                .RuleFor(x => x.OrderDate, Timestamp.FromDateTime(DateTime.SpecifyKind(new Faker().Date.Recent(), DateTimeKind.Utc)))
                .Generate()
                ;

            order.Item.Total = order.Item.PricePerUnit * order.Item.Quantity;
            return order;
        }

    }
}
