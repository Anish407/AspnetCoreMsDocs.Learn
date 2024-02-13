<h1>IStartupFIlter</h1>
<p>The IStartupFilter interface lives in the Microsoft.AspNetCore.Hosting.Abstractions package.

</p>
Use IStartupFilter:

To configure middleware at the beginning or end of an app's middleware pipeline without an explicit call to Use{Middleware}. Use IStartupFilter to add defaults to the beginning of the pipeline without explicitly registering the default middleware. IStartupFilter allows a different component to call Use{Middleware} on behalf of the app author.
To create a pipeline of Configure methods. IStartupFilter.Configure can set a middleware to run before or after middleware added by libraries.
IStartupFilter implements Configure, which receives and returns an Action<IApplicationBuilder>. An IApplicationBuilder defines a class to configure an app's request pipeline. For more information, see Create a middleware pipeline with IApplicationBuilder.

Each IStartupFilter can add one or more middlewares in the request pipeline. The filters are invoked in the order they were added to the service container. Filters may add middleware before or after passing control to the next filter, thus they append to the beginning or end of the app pipeline.

<h2>Points to remember</h2>
<ul>
<li>IStartupFilter gets executed on every request</li>
<li>Multiple IStartupFilter implementations may interact with the same objects. If ordering is important, order their IStartupFilter service registrations to match the order that their middlewares should run.</li>
<li>Libraries may add middleware with one or more IStartupFilter implementations that run before or after other app middleware registered with IStartupFilter. To invoke an IStartupFilter middleware before a middleware added by a library's IStartupFilter:

    Position the service registration before the library is added to the service container.
    
    To invoke afterward, position the service registration after the library is added.</li>
</ul>