# Serilog & Humio toghether

This simple console application allow to test 
Serilog library in conjunction with Humio online service.

This simple project was developed in Visual Studio 2022 with
versions for .NET Framework (4.7.2) and .NET Core (6.x). 
The underlying code is the same for both versions.

## Serilog

[Serilog](https://serilog.net/) is a library that provides logging
features. A key feature is the ability to manage complex structured data
in addition to classical log messages.

At this scope Serilog manage concepts like context information
and [enrichment](https://github.com/serilog/serilog/wiki/Enrichment).

## Humio service

[Humio](https://www.humio.com/) is a log service management that 
offer a free community  plan to work with, at the moment 
(June 2022) the free plan allow a single
repository with 16GB daily data and a retention period 
of 7 days (all without the needs to add credit card information).

So is a very starting point to play with a very comprehensive logging
platform.

### Registration

The registration process require few information and in a couple
of days you will receive an email with account information to play with.

### Create a repo and get an Ingest Token

To start playing with humio first create a repo from the (Humio home 
page)[https://cloud.community.humio.com/home] 

Then in the new repo navigate to the *Settings* tab and select 
*Ingest Tokens*. 

Create a new ingest Ingest Token using standard `serilog-jsonformatter`
as parser.

## The sample code

All the sample code is in the [Program.cs](./Program.cs) file.

The first part set up Humio sink configuration. 
In particular a Json formatter is assigned as a *TextFormatter* 
(see [Humio docs](https://library.humio.com/stable/docs/parsers/built-in-parsers/#serilog-jsonformatter))

```C#
var humioConfig = new HumioSinkConfiguration
{
    IngestToken = "{IngestToken}",
    TextFormatter = new JsonFormatter(renderMessage: true),
    Url = "https://cloud.community.humio.com"
};
```

> In .NET 6 program the token is loaded from *user secrets* saved locally. You can use `dotnet user-secrets init`
and `dotnet user-secrets set "HumioIngestToken" "{tokenHere}"` to save token in a safe place.

Regarding Serilog configuration it's important to include 
`.Enrich.FromLogContext()` to allow the Humio sink to receive
context information in json formatted data, and therfore be able to
process this data in the service.

```C#
// Serilog configuration
using var log = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.HumioSink(humioConfig)
    .CreateLogger();
```

The next part simply set a default logger to use with static
method of `Log` class. 

```C#
Log.Logger = log;
```

> For test purposes I simply use Log static methods. Obviously the use of static method versus dependency injection is
an architectural decision that needs to be taken in account when 
starting a new project. 

Then I add  context information to be added to the log.   
I also added args parameters of console application, so if you start
the console application with some args that data will be added to the log.

```C#
using (LogContext.PushProperty("args", args))
using (LogContext.PushProperty("assembly", typeof(Program).Assembly.FullName))
using (LogContext.PushProperty("class", typeof(Program).FullName))
```

And finally I add a log message to be sent to the Humio service.

```C#
Log.Information("This is a message from a test Console Application that use Serilog with Humio");
```

## Acknowledgements

I started learning about Serilog and its ecosystem thanks to
the course [Developing .NET Core 5 Apps with Docker](https://app.pluralsight.com/library/courses/docker-dot-net-core-apps-developing/table-of-contents)
by [Erik Dahl](https://app.pluralsight.com/profile/author/erik-dahl).
So thanks Erik for alle the usefull information that I got from your courses.





