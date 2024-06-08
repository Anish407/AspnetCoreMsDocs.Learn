## Managing the global packages, cache, and temp folders
Whenever you install, update, or restore a package, NuGet manages packages and package information in several folders outside of your project structure

### global-packages:
The global-packages folder is where NuGet installs any downloaded package. Each package is fully expanded into a subfolder that matches the package identifier and version number.
Projects using the PackageReference format always use packages directly from this folder. When using the packages.config, packages are installed to the global-packages folder, then copied into the project's packages folder.
   - Windows: %userprofile%\.nuget\packages 
   - Mac/Linux: ~/.nuget/packages

### http-cache
The Visual Studio Package Manager (NuGet 3.x+) and the dotnet tool store copies of downloaded packages in this cache (saved as .dat files), organized into subfolders for each package source. Packages are not expanded, and the cache has an expiration time of 30 minutes.
  - Windows: %localappdata%\NuGet\v3-cache
  - Mac/Linux: ~/.local/share/NuGet/v3-cache
Override using the NUGET_HTTP_CACHE_PATH environment variable.

### temp
A folder where NuGet stores temporary files during its various operations.
 - Windows: %temp%\NuGetScratch
 - Mac: /tmp/NuGetScratch
 - Linux: /tmp/NuGetScratch<username>
Override using the NUGET_SCRATCH environment variable.

### plugins-cache
A folder where NuGet stores the results from the operation claims request.
 - Windows: %localappdata%\NuGet\plugins-cache
 - Mac/Linux: ~/.local/share/NuGet/plugins-cache
Override using the NUGET_PLUGINS_CACHE_PATH environment variable.

~~~
NuGet 3.5 and earlier uses packages-cache instead of the http-cache, which is located in %localappdata%\NuGet\Cache.
~~~

By using the cache and global-packages folders, NuGet generally avoids downloading packages that already exist on the computer, improving the performance of install, update, and restore operations. When using PackageReference, the global-packages folder also avoids keeping downloaded packages inside project folders, where they might be inadvertently added to source control, and reduces NuGet's overall impact on computer storage.

When asked to retrieve a package, NuGet first looks in the global-packages folder. If the exact version of package is not there, then NuGet checks all non-HTTP package sources. If the package is still not found, NuGet looks for the package in the http-cache unless you specify --no-http-cache with dotnet.exe commands or -NoHttpCache with nuget.exe commands. If the package is not in the cache, or the cache isn't used, NuGet then retrieves the package over HTTP 

### Viewing folder locations
~~~
nuget locals all -list
DotnetCli: dotnet nuget locals all --list
~~~

## Reference
- https://learn.microsoft.com/en-us/nuget/consume-packages/managing-the-global-packages-and-cache-folders
