# Getting Started With GRPC in .NET

## Intro

1. What is gRPC?
    * RPC Framework
    * vs. REST
        * Binary vs. human-readable serialization
        * No HTTP verb nomenclature
        * Shared contract
        * Requires special tooling
2. HTTP2
    * multiplexing / streaming
    * One connection, multiple bi-directional requests/responses
3. Companies that use it:
    * Uber, Netflix, Spotify, DropBox
4. Cross-platform
    * Supported languages: [Documentation | gRPC](https://grpc.io/docs/)

## Demo

### Creating Server

1. [Run Server Setup Script](src/initial-setup-grpc-server.ps1)
2. Open sln
3. Add to appsettings.json (this adds verbosity to logging, making it more clear when GRPC service is invoked):

    ```json
        "Microsoft.AspNetCore.Hosting": "Information",
        "Microsoft.AspNetCore.Routing.EndpointMiddleware": "Information"
    ```

4. Show [`greet.proto`](src/Grpc.Demo/Server/Grpc.Demo.Server/Protos/greet.proto)
    * Explain.
    * Show Properties.
5. Show [`GreeterService.cs`](src/Grpc.Demo/Server/Grpc.Demo.Server/Services/GreeterService.cs)
    * Show Generated class
    * Demonstrate updating
6. Run
    * Demonstrate trying to access in browser

### Creating Client

1. [Run Client Setup Script](src/initial-setup-grpc-client.ps1)
2. Open sln
3. Copy Protos folder from Server
4. Open Greet.proto, change namespace
5. Add to .csproj (need to unload and reload)

    ```xml
    <ItemGroup>
        <Protobuf Include="Protos\greet.proto" GrpcServices="Client" />
    </ItemGroup>
    ```

6. Run, demonstrating communication with server.
7. Add [Greet Client Program.cs](src/greet-client-program.cs)

### New Service

#### Create new Service On Server

1. Create [order.proto](src/order.proto)
    * Talk about datetime. Include this link:
        * [Protobuf scalar data types - gRPC for WCF developers | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/architecture/grpc-for-wcf-developers/protobuf-data-types)
2. Create a C# [OrderService](src/OrderService-v1.cs)
    * Try to inherit from OrderProtoService.
    * It doesn't work, why not?
    * Need to change compile options.
3. Add to `Program.cs`

#### Update Client to Consume New Service

1. Copy `order.proto` from server to client.
2. Add to .csproj
3. Add [order-client-program-v1.cs](src/order-client-program-v1.cs)
4. Demonstrate

### Using Streams

1. Add delay to order provider to simulate server latency.
2. Demonstrate how latency creates inefficicies with unary calls.
3. Update order.proto with [order-v2.proto](src/order-v2.proto), which adds a streaming method.
4. Update order service with [OrderService-v2.cs](src/OrderService-v2.cs).
5. Update client with [order-client-program-v2.cs](src/order-client-program-v2.cs).
6. Re-run server and client.
7. Demonstrate how streaming method is more performant.

### Client Side Load Balancing / Transient Fault Handling

1. Update client with [order-client-program-v3](src/order-client-program-v3.cs)
2. Explain DNS service discovery
    * Can use DNS service (SRV) records.
3. Start multiple instances of server, demonstrating load balancing / failover:

    ```powershell
    dotnet run --no-build --urls="http://localhost:5126/" --project "S:\Projects\Grpc.Demo\Server\Grpc.Demo.Server\Grpc.Demo.Server.csproj"
    dotnet run --no-build --urls="http://localhost:5127/" --project "S:\Projects\Grpc.Demo\Server\Grpc.Demo.Server\Grpc.Demo.Server.csproj"
    dotnet run --no-build --urls="http://localhost:5128/" --project "S:\Projects\Grpc.Demo\Server\Grpc.Demo.Server\Grpc.Demo.Server.csproj"
    ```

4. Switch round robin config to `PickFirstConfig`.

### .NET Framework Support

1. Requiress 4.6.1 or later
2. Requires Grpc.Core, a maintenance mode project
3. Doesn't support outbound streams

### Authentication

[Authentication and authorization in gRPC for ASP.NET Core | Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/grpc/authn-and-authz?view=aspnetcore-6.0)

* Supported:
  * Simplest -- bearer token
  * Certificate
  * JWT Tokens
  * OAuth
  * OpenId Connect
* Similar to securing a Web API app (authorization attributes, etc.)

## Sample Projects

* [GRPC Server](/src/Grpc.Demo/Server/) (final version created in this demo)
* [GRPC Client](/src/Grpc.Demo/Client/) (final version created in this demo)
* [GRPC Legacy (.NET Framework) Client](/src/Grpc.Demo/Grpc.Legacy/)

## Links / Additional Reading

### GRPC

* [Create a .NET Core gRPC client and server in ASP.NET Core | Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-3.0&tabs=visual-studio)
* [Use gRPC client with .NET Standard 2.0 | Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/grpc/netstandard?view=aspnetcore-6.0)
* [Versioning gRPC services | Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/grpc/versioning?view=aspnetcore-6.0)
* [Performance best practices with gRPC | Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/grpc/performance?view=aspnetcore-6.0)

### Proto

* [protobuf-net/protobuf-net: Protocol Buffers library for idiomatic .NET (github.com)](https://github.com/protobuf-net/protobuf-net)
* [Language Guide  |  Protocol Buffers  |  Google Developers](https://developers.google.com/protocol-buffers/docs/proto)
* [Protobuf scalar data types - gRPC for WCF developers | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/architecture/grpc-for-wcf-developers/protobuf-data-types)
