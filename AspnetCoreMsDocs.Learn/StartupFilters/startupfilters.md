<h1>IStartupFIlter</h1>
<p>The IStartupFilter interface lives in the Microsoft.AspNetCore.Hosting.Abstractions package.

</p>
<h2> Use IStartupFilter</h2>

<p>To configure middleware at the beginning or end of an app's middleware pipeline without an explicit call to Use{Middleware}. Use IStartupFilter to add defaults to the beginning of the pipeline without explicitly registering the default middleware. IStartupFilter allows a different component to call Use{Middleware} on behalf of the app author.</p>
<p>To create a pipeline of Configure methods. IStartupFilter.Configure can set a middleware to run before or after middleware added by libraries.</p>
<p>IStartupFilter implements Configure, which receives and returns an Action<IApplicationBuilder>. An IApplicationBuilder defines a class to configure an app's request pipeline. For more information, see Create a middleware pipeline with IApplicationBuilder.</p>

Each IStartupFilter can add one or more middlewares in the request pipeline. The filters are invoked in the order they were added to the service container. Filters may add middleware before or after passing control to the next filter, thus they append to the beginning or end of the app pipeline..

<h3>Code</h3>
<ul>
    <p><a href='./SampleFilter.cs'>SampleFilter.cs</a> Contains the StartupFilter that was created. Then we create a middleware (<p><a href='./RequestHandlingMiddleware.cs'>Here</a>) and debug the code to ensure that the filters have run before the middleware. If we have multiple implementations of IStartupFilter, then the order in which they are registered in the DI container will be the order of execution</p>
<p>The startupFilter is registered in the DI container as below</p>
 <code>builder.Services.AddTransient<IStartupFilter, SampleFilter>(); // register the IStartupFilter</code>
</ul>

<h2>Points to remember</h2>
<ul>
<li>IStartupFilter gets executed on every request</li>
<li>Multiple IStartupFilter implementations may interact with the same objects. If ordering is important, order their IStartupFilter service registrations to match the order that their middlewares should run.</li>
<li>Libraries may add middleware with one or more IStartupFilter implementations that run before or after other app middleware registered with IStartupFilter. To invoke an IStartupFilter middleware before a middleware added by a library's IStartupFilter:

    Position the service registration before the library is added to the service container.
    
    To invoke afterward, position the service registration after the library is added.</li>
</ul>
