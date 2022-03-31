set-location S:\Projects
new-item -path Grpc.Demo\Client -ItemType Directory
set-location Grpc.Demo\Client

# create new empty solution
dotnet new sln -n Grpc.Demo.Client

# create new console app
dotnet new console -n Grpc.Demo.Client -lang C#

# add project to solution
dotnet sln add Grpc.Demo.Client\Grpc.Demo.Client.csproj

# install packages
set-location Grpc.Demo.Client
dotnet add package Grpc.Net.Client
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools
dotnet add package Microsoft.Extensions.DependencyInjection

