# MSA2022.Phase2.Backend

## Section One
Three configuration files were created: Development, Prodaction and Debug. Development and Prodation have different databases and the Debug config is the same
as Development but the GET /Cats endpoint returns raw API response from https://cataas.com. To switch between the configuration files, you need to
go to Debug.Properties and change the ASPNETCORE_ENVIRONMENT environment variable to Development, Prodaction or Debug.

## Section Two

Demonstrate an understanding of how these middleware via DI (dependency injection) simplifies your code.



## Section Three

Demonstrate the use of NUnit to unit test your code.
Use at least one substitute to test your code.
Demonstrate an understanding of why the middleware libraries made your code easier to test.
