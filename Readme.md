# Manage Courses UI project

# Deprecated

Please refer to [manage-courses-frontend](https://github.com/DFE-Digital/manage-courses-frontend)

## About

This repo provides a dotnet core solution containing the main client for managing courses.

It should be [12 factor](https://12factor.net/), notably config from environment variables.

## Requirements

- EITHER dotnet core, OR msbuid+Visual studio
- Nodejs https://nodejs.org/ for the asset pipeline - worked with version 8.11.2 LTS
- Access to a running instance of [manage-courses-api](https://github.com/DFE-Digital/manage-courses-api)
- Access to an oauth server. To use the DfE Sign-in sandbox environment, ask the team for the current client secret, or get it from the DfE single sign-in project by emailing [DfE.SIGNIN@education.gov.uk](mailto:DfE.SIGNIN@education.gov.uk)

## Logging

Serilog has been configured to spit logs out to both the console
(for `dotnet run` testing & development locally) and Application Insights.

Set the `APPINSIGHTS_INSTRUMENTATIONKEY` environment variable to tell Serilog the application insights key.

## Error tracking

This app sends exceptions and errors into [Sentry](https://sentry.io). To enable the integration,
set the SENTRY_DSN environment variable.

# Build and run in dotnet core

## Build

In the repository root, run:

```
dotnet restore
cd src\ui
dotnet run
```

Go to `https://localhost:44364` in your browser

## Run

## Dotnet SDK

You will need to have SDK Version
[2.1.302 (download)](https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.1.302-windows-x64-installer)
of the dotnet SDK installed in order to build and run this. This is due to a
bug in ASP.NET MVC Core which is using inconsistent package versions.
[The bug](https://github.com/aspnet/Mvc/issues/7969) has a fix promised in .NET Core 2.1.3

### App settings variables

You will need to set the app settings (preferred to be store as `user-secrets`):

- **SearchAndCompare:UiBaseUrl** the location of the search and compare UI, for the purpose of linking, e.g. "https://find-postgraduate-teacher-training.gov.uk". Avoid trailing slash.
- **API_URL** - the location of your [manage-courses-api](https://github.com/DFE-Digital/manage-courses-api) deployment. Defaults to development copy when in run in development mode.
- **DFE_SIGNIN_CLIENT_SECRET** - the client secret of your oath server
- **auth:oidc:metadataAddress** - the .well-known config URL of your oauth server, if you don't want to use the default sandbox one
- **auth:oidc:tokenEndpoint** - the /token endpoint as specified in the .well-known config URL of your oauth server

You may also set the following optional ones:

- **auth:oidc:clientId** (optional) - the Client ID to be used with your oauth server, if you don't want to use the default one (`bats`)

### System Environment variables

- **ASPNET_ENVIRONMENT** - set to `Development`
- **MANAGE_COURSES_API_Development_PORT** (optional) - the port to run locally on `Development` valid port number betweeen `49152 - 65535` otherwise it will use the default port `44364` if port is unset or port is invalid/out of bounds

## Running the JS unit tests

To run the JS unit tests (full suite, with coverage output):

```bash
cd src
npm install
npm test
```

To run in watch mode (which also allows you to specify grep patterns to focus only on certain specs/suites, but without coverage output):

```bash
npm run test:watch
```

#### Notes

- ensure the overriden or default port used (**MANAGE_COURSES_API_Development_PORT**), is available for use.
- app settings can also be defined as System Enviroment Variables or as `user-secrets`

The best way to set and store them is [user-secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1), e.g.

```powershell
cd src\ui
dotnet user-secrets set ASPNET_ENVIRONMENT Development
dotnet user-secrets set DFE_SIGNIN_CLIENT_SECRET <the client secret>
```

And then just launch the UI (in folder `src/ui`)

```powershell
dotnet run
```

Once you're up and running, navigate to https://localhost:44364. Your browser will alert you that the certificate has no trusted root, but navigate to the page anyway.

# Build and run in Visual Studio with msbuild (Legacy only - it's better to use dotnet core)

## Setup

- Add `DFE_SIGNIN_CLIENT_SECRET` to your user secrets for the ManageCoursesUI project (right-click the project in VS, "Manage user secrets".
- Run `npm install` in `src\ui\` to get the asset pipeline dependencies (gulp) before opening visual studio.
- Must be run with SSL/TLS. The DfE Sign-in service will not allow non-https redirect urls

## Coding

This repo can be worked on both Visual Studio 2017 [see IIS Express (Windows-only](<###iis-Express-(windows-only)>) & Visual Studio Code.

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

## Using a local version of ManageCourses.ApiClient and SearchAndCompareUi.Shared

If you want to use a version of ManageCourses.ApiClient/SearchAndCompareUi.Shared that is not (yet) on nuget.org, copy `src/ui/dev-mc-api.targets.example` to `src/ui/dev-mc-api.targets` and/or `src/ui/dev-sc-shared.targets.example` to `src/ui/dev-sc-shared.targets` and update the Project path referenced in that file to your local ApiClient project.

## Auth workflow

1. localhost hits login button
2. opens up url to external auth
3. user logins in on external auth
4. user is then redirected back to localhost (redirect url provided from localhost in step 2)
5. localhost then extract the user details

## Sign-in service links

- https://signin-test-sup-as.azurewebsites.net/users

- https://signin-test-sup-as.azurewebsites.net/users/80BA7FAF-D6E1-47B4-9EDD-4539F53C8B9E/audit - The data
  here is always 15 mins out of date as its on an event stream (its more realtime in prod envs.)

## Shutting down the service and showing the off line page.

Rename the file "app_offline.htm.example" in the root folder to "app_offline.htm"
