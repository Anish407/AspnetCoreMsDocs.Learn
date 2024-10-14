## HTTP CLIENT

Though this class implements IDisposable, declaring and instantiating it within a using statement is not preferred because when the HttpClient object gets disposed of, the underlying socket is not immediately released, which can lead to a socket exhaustion problem.
For more information about this issue, see the blog post https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/

<p>Therefore, HttpClient is intended to be instantiated once and reused throughout the life of an application. Instantiating an HttpClient class for every request will exhaust the number of sockets available under heavy loads. That issue will result in SocketException errors. Possible approaches to solve that problem are based on the creation of the HttpClient object as singleton or static, as explained in this Microsoft article on HttpClient usage.
  This can be a good solution for short-lived console apps or similar, that run a few times a day.</p>

<p>However, the issue isn't really with HttpClient per se, but with the default constructor for HttpClient, because it creates a new concrete instance of HttpMessageHandler, which is the one that has sockets exhaustion and DNS changes issues mentioned above.

To address the issues mentioned above and to make HttpClient instances manageable, .NET Core 2.1 introduced two approaches, one of them being IHttpClientFactory. It's an interface that's used to configure and create HttpClient instances in an app through Dependency Injection (DI).
It also provides extensions for Polly-based middleware to take advantage of delegating handlers in HttpClient.</p>


> [!TIP]
> The HttpClient instances injected by DI can be disposed of safely, because the associated HttpMessageHandler is managed by the factory. Injected HttpClient instances are Transient from a DI perspective, while HttpMessageHandler instances can be regarded as Scoped. HttpMessageHandler instances have their own DI scopes,
 separate from the application scopes (for example, ASP.NET incoming request scopes). For more information, see Using HttpClientFactory in .NET.

> [!IMPORTANT]
> The implementation of IHttpClientFactory (DefaultHttpClientFactory) is tightly tied to the DI implementation in the Microsoft.Extensions.DependencyInjection NuGet package. If you need to use HttpClient without DI or with other DI implementations, consider using a static or singleton HttpClient with PooledConnectionLifetime set up. For more information, see HttpClient guidelines for .NET.

This advice in .NET Core is primarily about avoiding unintended memory and resource management issues, particularly when dealing with the HttpMessageHandler instances, which are typically used to send HTTP requests in .NET through HttpClient. Here's a breakdown of the key points in this guidance:

1. HttpMessageHandler and Lifecycle
HttpMessageHandler is an abstraction that handles HTTP requests and responses.
It is often reused across multiple requests when you instantiate an HttpClient. Reusing these handlers improves performance, reduces the number of socket connections, and minimizes resource consumption.
2. Scope-Related Data (e.g., HttpContext)
HttpContext contains request-specific data (e.g., user information, authentication tokens, request headers).
HttpContext is scoped: meaning its data is only relevant during a single HTTP request, and it should not persist beyond that scope. Once the request completes, that data becomes irrelevant and is cleaned up by the framework.
3. Caching Scope-Related Data
If you cache or store HttpContext-related information inside an HttpMessageHandler:

That data may live much longer than it should, because HttpMessageHandler is reused for multiple HTTP requests.
This causes data leaks, where one user's sensitive data (like authentication tokens) could accidentally be accessed during another request, or for another user.
Such a situation is a security risk, as sensitive information, such as tokens or user data, could inadvertently be exposed.
4. Scoped Dependencies
In .NET Core Dependency Injection (DI), dependencies can have different lifetimes:

Transient: Created every time they are requested.
Scoped: Created once per HTTP request.
Singleton: Created once and shared throughout the application’s lifetime.
Using scoped services inside a singleton or long-lived object (like HttpMessageHandler) is dangerous because scoped services are tied to the current HTTP request.

> [!CAUTION]
> If you cache a scoped service, like an HttpContext, it could be reused across different requests, leading to similar issues where sensitive information from one request "leaks" into another request.


## References
- [USING HTTPCLIENTFACTORY](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)
- [HTTP CLIENT GUIDELINES](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
