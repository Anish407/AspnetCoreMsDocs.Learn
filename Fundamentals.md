# ASP.NET Core fundamentals overview

### Dependency injection (services)
* ASP.NET Core includes dependency injection (DI) that makes configured services available throughout an app. Services are added to the DI container with WebApplicationBuilder.Services, builder.Services in the preceding code. **When the WebApplicationBuilder is instantiated, many framework-provided services are added. builder is a WebApplicationBuilder in the following code**:
 </br> `var builder = WebApplication.CreateBuilder(args);`
<br> <a href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0#framework-provided-services">Additional resources that are added when CreateBuilder() is called </a>
![image](https://github.com/Anish407/AspnetCoreMsDocs.Learn/assets/51234038/75099403-bfa1-4d85-9b59-5932505b2ef0)

# Middleware
The request handling pipeline is composed as a series of middleware components. Each component performs operations on an HttpContext and either invokes the next middleware in the pipeline or terminates the request.

By convention, a middleware component is added to the pipeline by invoking a Use{Feature} extension method. Middleware added to the app is highlighted in the following code:

# Host
On startup, an ASP.NET Core app builds a host. The host encapsulates all of the app's resources, such as:

An HTTP server implementation
* Middleware components
* Logging
* Dependency injection (DI) services
* Configuration

**The ASP.NET Core WebApplication and WebApplicationBuilder types are recommended and used in all the ASP.NET Core templates. WebApplication behaves similarly to the .NET Generic Host and exposes many of the same interfaces but requires less callbacks to configure. The ASP.NET Core WebHost is available only for backward compatibility.**
# Servers
An ASP.NET Core app uses an HTTP server implementation to listen for HTTP requests. ** The server surfaces requests to the app as a set of request features composed into an HttpContext.**

ASP.NET Core provides the following server implementations:

* **Kestrel** is a cross-platform web server. Kestrel is often run in a reverse proxy configuration using IIS. In ASP.NET Core 2.0 or later, Kestrel can be run as a public-facing edge server exposed directly to the Internet.
* **IIS HTTP Server** is a server for Windows that uses IIS. With this server, the ASP.NET Core app and IIS run in the same process.
* **HTTP.sys** is a server for Windows that isn't used with IIS.

# Environments
Specify the environment an app is running in by setting the ASPNETCORE_ENVIRONMENT environment variable. ASP.NET Core reads that environment variable at app startup and stores the value in an _IWebHostEnvironment_ implementation. This implementation is available anywhere in an app via dependency injection (DI).
This is another feature that is setup when the CreateBuilder() is called during startup.






