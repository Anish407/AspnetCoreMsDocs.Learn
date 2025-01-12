## Authentication
Authentication is the process of determining a user's identity. Authorization is the process of determining whether a user has access to a resource. In ASP.NET Core, authentication is handled by the authentication service, IAuthenticationService, which is used by authentication middleware. The authentication service uses registered authentication handlers to complete authentication-related actions. 

<b>Authentication schemes are specified by registering authentication services in Program.cs:

By calling a scheme-specific extension method after a call to AddAuthentication, such as AddJwtBearer or AddCookie. These extension methods use AuthenticationBuilder.AddScheme to register schemes with appropriate settings.
Less commonly, by calling AuthenticationBuilder.AddScheme directly.</b>

~~~
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("JwtSettings", options))
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("CookieSettings", options));
~~~

The AddAuthentication parameter JwtBearerDefaults.AuthenticationScheme is the name of the scheme to use by default when a specific scheme isn't requested.

If multiple schemes are used, authorization policies (or authorization attributes) can specify the authentication scheme (or schemes) they depend on to authenticate the user. In the example above, the cookie authentication scheme could be used by specifying its name (CookieAuthenticationDefaults.AuthenticationScheme by default, though a different name could be provided when calling AddCookie).

* In some cases, the call to AddAuthentication is automatically made by other extension methods. For example, when using ASP.NET Core Identity, AddAuthentication is called internally.

* The Authentication middleware is added in Program.cs by calling UseAuthentication. Calling UseAuthentication registers the middleware that uses the previously registered authentication schemes. Call UseAuthentication before any middleware that depends on users being authenticated. 

## Authentication concepts
Authentication is responsible for providing the ClaimsPrincipal for authorization to make permission decisions against. There are multiple authentication scheme approaches to select which authentication handler is responsible for generating the correct set of claims:

Authentication scheme
The default authentication scheme, discussed in the next two sections.
Directly set HttpContext.User.
When there is only a single authentication scheme registered, it becomes the default scheme. If multiple schemes are registered and the default scheme isn't specified, a scheme must be specified in the authorize attribute, otherwise, the following error is thrown:

> InvalidOperationException: No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).

### DefaultScheme
When there is only a single authentication scheme registered, the single authentication scheme:

Is automatically used as the DefaultScheme.
Eliminates the need to specify the DefaultScheme in AddAuthentication(IServiceCollection) or AddAuthenticationCore(IServiceCollection).
To disable automatically using the single authentication scheme as the DefaultScheme, call 

``` AppContext.SetSwitch("Microsoft.AspNetCore.Authentication.SuppressAutoDefaultScheme"). ```

### Authentication scheme
The authentication scheme can select which authentication handler is responsible for generating the correct set of claims.

An authentication scheme is a name that corresponds to:
- An authentication handler.
- Options for configuring that specific instance of the handler.

When we say AddJwtBearer, we are adding a scheme named JwtBearerDefaults.AuthenticationScheme. This will invoke the handler associated with the JwtBearerDefaults.AuthenticationScheme scheme.
The handler will handle the authentication process and set the HttpContext.User property with the ClaimsPrincipal that represents the authenticated user.
This claims principal will be used in the application to get the user's identity and make authorization decisions.

Similarly, when we say AddScheme, we are adding a scheme with a custom name. This will invoke the handler associated with the custom scheme name. The handler will handle the authentication process and set the HttpContext.User property with the ClaimsPrincipal that represents the authenticated user.
So a customSessionTokenScheme, for example, will invoke the handler associated with the customSessionTokenScheme scheme and generate the claims principal.

### Authentication handler
An authentication handler:

- Is a type that implements the behavior of a scheme.
- Is derived from IAuthenticationHandler or AuthenticationHandler<TOptions>.
- Has the primary responsibility to authenticate users.

Based on the authentication scheme's configuration and the incoming request context, authentication handlers:

- Construct AuthenticationTicket objects representing the user's identity if authentication is successful.
- Return 'no result' or 'failure' if authentication is unsuccessful.
- Have methods for challenge and forbid actions for when users attempt to access resources:
- They're unauthorized to access (forbid).When they're unauthenticated (challenge).

### RemoteAuthenticationHandler<TOptions> vs AuthenticationHandler<TOptions>
RemoteAuthenticationHandler<TOptions> is the class for authentication that requires a remote authentication step. When the remote authentication step is finished, the handler calls back to the CallbackPath set by the handler. The handler finishes the authentication step using the information passed to the HandleRemoteAuthenticateAsync callback path. OAuth 2.0 and OIDC both use this pattern. JWT and cookies don't since they can directly use the bearer header and cookie to authenticate. The remotely hosted provider in this case, Is the authentication provider.
> Examples include Facebook, Twitter, Google, Microsoft, and any other OIDC provider that handles authenticating users using the handlers mechanism.

### Authenticate
An authentication scheme's authenticate action is responsible for constructing the user's identity based on request context. It returns an AuthenticateResult indicating whether authentication was successful and, if so, the user's identity in an authentication ticket. See AuthenticateAsync. Authenticate examples include:

- A cookie authentication scheme constructing the user's identity from cookies.
- A JWT bearer scheme deserializing and validating a JWT bearer token to construct the user's identity.

### How It Works Internally
Here’s a high-level sequence of what the JwtBearerHandler does in the HandleAuthenticateAsync method:

Extract the JWT:

The handler retrieves the JWT from the Authorization header in the HTTP request.
- Validate the Token:

- The handler calls the ValidateToken method on the JwtSecurityTokenHandler, passing the extracted token and the TokenValidationParameters.

#### This step involves:
- Verifying the token signature.
- Checking claims like exp (expiration), iss (issuer), and aud (audience).
- Decoding and validating the payload.
- If validation succeeds, the method returns a SecurityToken and the claims associated with the token.
Create the ClaimsPrincipal:

Using the claims extracted from the validated token, the handler creates a ClaimsIdentity and associates it with the scheme name (e.g., "Bearer").
The ClaimsPrincipal is then created from this ClaimsIdentity.
Return an Authentication Result:

If everything succeeds, the handler returns an AuthenticateResult.Success() with an AuthenticationTicket that includes the ClaimsPrincipal.
If validation fails, it returns AuthenticateResult.Fail().

#### app.UseAuthentication() 

The app.UseAuthentication() middleware in ASP.NET Core is responsible for integrating the authentication system into the HTTP request pipeline. Here's a breakdown of what happens when this middleware is invoked:

## Responsibilities of app.UseAuthentication()
- Attach the Authentication Handler to the Pipeline:

- The middleware ensures that the registered authentication handlers (e.g., JwtBearerHandler, CookieAuthenticationHandler) are initialized for incoming requests.
### Authenticate the Request:

During each request, the middleware calls the *AuthenticateAsync()*  method of the default authentication scheme (or schemes if specified).
This step attempts to validate the credentials (e.g., JWT, cookie) in the request and, if successful, attaches the resulting ClaimsPrincipal to the HttpContext.User.
Set the HttpContext.User:

- If the authentication succeeds:
- The ClaimsPrincipal returned by the authentication handler is assigned to HttpContext.User.
- This makes the user's identity and claims available throughout the request lifecycle, such as in controllers, Razor pages, or other middleware.
- If authentication fails or no valid credentials are provided, HttpContext.User remains unauthenticated (empty or with default identity).

#### When the request hits the app.UseAuthentication() middleware:

The middleware checks the registered schemes in services.AddAuthentication() and invokes their AuthenticateAsync() method.
If a valid ClaimsPrincipal is created, it assigns it to HttpContext.User.
For example, if you're using JWT Bearer authentication, the JwtBearerHandler is invoked to validate the token, extract claims, and create a ClaimsPrincipal.

- Authentication Middleware Calls AuthenticateAsync:

- The middleware starts the authentication process for the default scheme.

>var result = await context.AuthenticateAsync("Bearer");

**The above code shows how we can manually call the AuthenticateAsync method to authenticate the request using the "Bearer" scheme.**

The AuthenticationService retrieves the appropriate handler from the DI container for the scheme being authenticated.
The handler is registered when you configure services.AddAuthentication() with schemes (e.g., .AddJwtBearer()).
Invoke the Handler's AuthenticateAsync Method:

*The handler's implementation of AuthenticateAsync is invoked.*

Each handler (e.g., JwtBearerHandler, CookieAuthenticationHandler) implements its own AuthenticateAsync logic.

### app.UseAuthorization()

The app.UseAuthorization() middleware in ASP.NET Core is responsible for integrating the authorization system into the HTTP request pipeline. Here's a breakdown of what happens when this middleware is invoked:

## Responsibilities of app.UseAuthorization()
- Attach the Authorization Policy to the Pipeline:
- The middleware ensures that the registered authorization policies are initialized for incoming requests.
- Evaluate the Authorization Policy:
- During each request, the middleware evaluates the registered authorization policies against the ClaimsPrincipal attached to HttpContext.User.
- If the user's claims satisfy the policy requirements, the request is allowed to proceed.
- If the user's claims don't meet the policy requirements, the request is denied, and a 403 Forbidden response is returned.

When the request hits the app.UseAuthorization() middleware:
- The middleware evaluates the registered authorization policies against the ClaimsPrincipal attached to HttpContext.User.
- If the user's claims satisfy the policy requirements, the request is allowed to proceed.
- If the user's claims don't meet the policy requirements, the request is denied, and a 403 Forbidden response is returned.


































