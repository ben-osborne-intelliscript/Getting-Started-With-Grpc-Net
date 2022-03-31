set-location S:\Projects
new-item -path Grpc.Demo\Server -ItemType Directory
set-location Grpc.Demo\Server

# create new empty solution
dotnet new sln -n Grpc.Demo.Server

# create new grpc project
dotnet new grpc -n Grpc.Demo.Server -lang C#

# add project to solution
dotnet sln add Grpc.Demo.Server\Grpc.Demo.Server.csproj

<#
install packages

The default grpc template includes the packages necessary for grpc.
#>
set-location Grpc.Demo.Server
dotnet add package Bogus
dotnet add package AutoBogus

