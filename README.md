# LibraryCop Backend

The backend is a REST API implemented in .NET 6 (hopefully upgraded soon to .NET 7) and runs in Azure Functions.

## Structure

### BackendFunctions

Project that contains the Azure Functions along with configuration.

### BusinessLogic

The backend is implemented mostly in [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). This project contains controllers (data access), use cases (business logic, interactors) and entities.

### UnitTest

Sadly there are not enough tests. But the fewes that have been implemented are stored here and they are executed in the pipeline.

## Building it

Build using the .NET Core CLI, which is installed with [the .NET SDK](https://www.microsoft.com/net/download) or directly in Visual Studio. Then run
these commands from the CLI in the directory of any sample:

```console
dotnet build
dotnet run
```


These will install any needed dependencies, build the project, and run the project respectively.