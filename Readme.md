# Manage Courses UI project

[<img src="https://api.travis-ci.org/DFE-Digital/manage-courses-ui.svg?branch=master">](https://api.travis-ci.org/DFE-Digital/manage-courses-ui.svg?branch=master)

## About

This repo provides a dotnet core solution containing the main client for managing courses.

It should be [12 factor](https://12factor.net/), notably config from environment variables.

## Requirements

* EITHER dotnet core, OR msbuid+Visual studio
* Nodejs https://nodejs.org/ for the asset pipeline - worked with version 8.11.2 LTS
* Access to a running instance of [manage-courses-api](https://github.com/DFE-Digital/manage-courses-api), e.g. [the dev one](https://manage-courses-ui-bat-development.e4ff.pro-eu-west-1.openshiftapps.com)
* Access to an oauth server. To use the DfE Sign-in sandbox environment, ask the team for the current client secret, or get it from the DfE single sign-in project by emailing [DfE.SIGNIN@education.gov.uk](mailto:DfE.SIGNIN@education.gov.uk)

# Build and run in dotnet core

## Build

In the repository root, run:
```
dotnet restore
cd src\ui
npm install
npm run-script generate
```

Go to `https://localhost:44364` in your browser

## Run

### App settings variables
You will need to set the app settings (prefered to be store as `user-secrets`):

* **ApiConnection:url** - the location of your [manage-courses-api](https://github.com/DFE-Digital/manage-courses-api) deployment. You can use a local one or perhaps [the one in the Dev Environment](https://manage-courses-ui-bat-development.e4ff.pro-eu-west-1.openshiftapps.com) if you are lazy
* **DFE_SIGNIN_CLIENT_SECRET** - the client secret of your oath server
* **auth:oidc:metadataAddress** - the .well-known config URL of your oauth server, if you don't want to use the default sandbox one

You may also set the following optional ones:

* **auth:oidc:clientId** (optional) - the Client ID to be used with your oauth server, if you don't want to use the default one (`bats`)


### System Environment variables

* **ASPNET_ENVIRONMENT** - set to `Development`
* **MANAGE_COURSES_API_Development_PORT** (optional) - the port to run locally on `Development` valid port number betweeen `49152 - 65535` otherwise it will use the default port `44364` if port is unset or port is invalid/out of bounds


#### Notes

* ensure the overriden or default port used (**MANAGE_COURSES_API_Development_PORT**), is available for use.
* app settings can also be defined as System Enviroment Variables or as `user-secrets`

The best way to set and store them is [user-secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1), e.g.
```powershell
cd src\ui
dotnet user-secrets set ASPNET_ENVIRONMENT Development
dotnet user-secrets set ApiConnection:url https://manage-courses-api-bat-development.e4ff.pro-eu-west-1.openshiftapps.com
dotnet user-secrets set DFE_SIGNIN_CLIENT_SECRET <the client secret>
```

And then just launch the UI (in folder `src/ui`)
```powershell
dotnet run
```

Once you're up and running, navigate to https://localhost:44364. Your browser will alert you that the certificate has no trusted root, but navigate to the page anyway.


# Build and run in Visual Studio with msbuild (Legacy only - it's better to use dotnet core)

## Setup

* Add `DFE_SIGNIN_CLIENT_SECRET` to your user secrets for the ManageCoursesUI project (right-click the project in VS, "Manage user secrets".
* Run `npm install` in `src\ui\` to get the asset pipeline dependencies (gulp) before opening visual studio.
* Must be run with SSL/TLS. The DfE Sign-in service will not allow non-https redirect urls

## Coding

This repo can be worked on both Visual Studio 2017 [see IIS Express (Windows-only](###iis-Express-(windows-only)) & Visual Studio Code.

### Asset pipeline

* See https://docs.microsoft.com/en-us/aspnet/core/client-side/using-gulp?view=aspnetcore-2.1 for how gulp is used.
* To get visual studio to download the node dependencies open the package.json file and save it. Take a look at the output window (select "Bower/npm" in the output dropdown).
* Once you've done that you can right-click gulpfile.js in solution explorer and open "Task Runner Explorer" to see that it's been understood.
* Run a build and it should generate all the files needed under folder wwwroot.

### IIS Express (Windows-only)

You may need the following if you have port-clash problems. To be revised as we learn more...

In this file
.\manage-courses-ui\.vs\config\applicationhost.config

Check that the bindings for https settings are for port 44364 (you may need to run the solution first to have it auto generate it first)
```xml
      <site name="ManageCoursesUi" id="2">
        <application path="/" applicationPool="Clr4IntegratedAppPool">
          <virtualDirectory path="/" physicalPath=".\manage-courses-ui\src" />
        </application>
        <bindings>
          <binding protocol="http" bindingInformation="*:5002:localhost" />
          <binding protocol="https" bindingInformation="*:44364:localhost" />
        </bindings>
      </site>
```

# Miscellaneous

## Auth workflow

1. localhost hits login button
2. opens up url to external auth
3. user logins in on external auth
4. user is then redirected back to localhost  (redirect url provided from localhost in step 2)
5. localhost then extract the user details

## Sign-in service links

* https://signin-test-sup-as.azurewebsites.net/users

* https://signin-test-sup-as.azurewebsites.net/users/80BA7FAF-D6E1-47B4-9EDD-4539F53C8B9E/audit - The data
  here is always 15 mins out of date as its on an event stream (its more realtime in prod envs.)

## Additional Msbuild Target

The default value of `InputSwaggerJson` is `manage-courses-api-swagger.json`.
The default value of `OutputSwaggerGeneration` is `Generated\ManageCoursesApiClient.cs`

```bash
msbuild
```

```bash
dotnet msbuild /t:compile /p:InputSwaggerJson={url to json| file path to json}
```

For development or previewing
The default value is `Generated\ManageCoursesApiClient.cs`
```bash
cd src\api-client
dotnet msbuild /t:compile  /t:NSwag /p:OutputSwaggerGeneration={file path}
```
