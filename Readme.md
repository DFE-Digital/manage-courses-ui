# Manage Courses UI project

## About

This repo provides a dotnet core solution containing the main client for managing courses.


## Setup

* Add `DFE_SIGNIN_CLIENT_SECRET` to your environment variables and log-out/in.
  Get this from the DfE single sign-in project by emailing [DfE.SIGNIN@education.gov.uk](mailto:DfE.SIGNIN@education.gov.uk)
* Must be run with SSL/TLS. The DfE Sign-in service will not allow non-https redirect urls

## Coding

It can be worked on in Visual Studio 2017 only.

* This uses external site for auth.
* Using vs2017 & iisexpress to  allow for external auth to communicate back via localhost using https.

It can also worked on Visual Studio Code but a reverse proxy from https back to http is required for the external auth.
* Consider ngrok (needs change in external auth)
* Consider reverse proxy from https to http ie nginx handles https and forward it to http localhost

It should be [12 factor](https://12factor.net/), notably config from environment variables.

## Auth workflow

1. localhost hits login button
2. opens up url to external auth
3. user logins in on external auth
4. user is then redirected back to localhost  (redirect url provided from localhost in step 2)
5. localost then extract the user details

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

### Sign-in service links

* https://signin-test-sup-as.azurewebsites.net/users

* https://signin-test-sup-as.azurewebsites.net/users/80BA7FAF-D6E1-47B4-9EDD-4539F53C8B9E/audit - The data
  here is always 15 mins out of date as its on an event stream (its more realtime in prod envs.)
