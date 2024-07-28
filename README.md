# api-dotnet-mssql-base #

Run a simple `CRUD` API using `C#` and [MS SQL Server](https://www.microsoft.com/en-au/sql-server/sql-server-downloads) at `localhost`. Feel free to adapt it for your needs. 

The `Photo` class was used to represent an image uploaded to cloud storage.

## Setup ##

**NOTE**: The following was tested on Ubuntu `22.04`, so adjust your instructions accordingly.

### Prerequisites: ###

[Docker Desktop](https://docs.docker.com/desktop/install/ubuntu/)\
Why install multiple versions of multiple databases? Connect to local databases running in Docker!

[Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=linux%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp)\
Tested on `v4.0.5198`. Verify the installation:
```
func --version
```
[Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-linux?pivots=apt)\
Tested on `v2.57.0`. Verify the installation:
```
az --version
```

[Install the .NET SDK](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-install?pivots=os-linux-ubuntu-2204&tabs=dotnet8)\
Tested on `v8.0.303`. Summary of the steps:
- To remove previous versions of dotnet:
```
sudo apt remove dotnet* aspnetcore* netstandard*
```
- Install `.NET version 8`:
```
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0
```
- Verify the installation:
```
dotnet --list-sdks
```
**NOTE**: If you have several versions installed then you can switch between them. Create a file called `global.json` somewhere in your folder path and enter:
```
{
  "sdk": {
    "version": "8.0.303"
  }
}
```
I connect to the database using [DBeaver](https://dbeaver.io/download/).

I use [Postman](https://www.postman.com/downloads/) to test the endpoints.

### Database server setup ###
Summary of the steps:

[Pull the latest SQL Server 2022 (16.x) Linux container image](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash&tabs=cli#pull-the-container-from-the-registry-2):
```
docker pull mcr.microsoft.com/mssql/server:2022-latest
```
[Create and run the container](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash&tabs=cli#run-the-container-2):
```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong@Passw0rd>" -p 1433:1433 --name msss-container --hostname msss-host -d mcr.microsoft.com/mssql/server:2022-latest
```
[Change the SA password](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash&tabs=cli#sapassword):
```
docker exec -it msss-container /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "$(read -sp "Enter current SA password: "; echo "${REPLY}")" -Q "ALTER LOGIN SA WITH PASSWORD=\"$(read -sp "Enter new SA password: "; echo "${REPLY}")\""
```

### Code Setup ###
```
git clone git@github.com:scottocorp/api-dotnet-mssql-base.git
```

### Database setup ###

[Create the database by running commands from Docker](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash&tabs=cli#create-a-new-database):
```
docker exec -it msss-container "bash"           # connect to the container
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA  # connect to the database
CREATE DATABASE [msss-db];
GO
exit                                            # exit the database
exit                                            # exit the container
```
Create the table by running a local script uploaded to Docker:
```
cd api-dotnet-mssql-base/scripts
docker cp photos.sql msss-container:/tmp
docker exec -it msss-container "bash"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -i /tmp/photos.sql
exit
```
### To launch the API at localhost ###
	
Create a file in the repository folder called `local.settings.json` and add the following:
```
{
    "IsEncrypted": false,
    "Values": {
      "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
      "SqlConnectionString": "Server=localhost,1433;Initial Catalog=msss-db;Persist Security Info=False;User ID=SA;Password=<YourStrong@Passw0rd>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
    },
    "Host": {
      "CORS": "*"
    }
  }
```		  
[Run the function locally](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=linux%2Cazure-cli#run-the-function-locally):
```
func start
```

Use postman to test:
- Create a new `POST` tab and enter the following url:\
 `http://localhost:7071/api/function_photos`
- select `Body -> raw -> JSON`, and paste in:
```
{ 
  "Filename": "xyz",
  "Bearing": 1,
  "Pitch": 2,
  "Zoom": 3
}
```
- `Send`

Other endpoints:
- `GET` http://localhost:7071/api/photos
- `GET` http://localhost:7071/api/photos/`id`
- `DELETE` http://localhost:7071/api/photos/`id`
- `PUT` http://localhost:7071/api/photos/`id`


## TODO ##

This is definitely a "work in progress". More work is needed to: 
- Set up a test suite.
- etc

## NOTES ##
- Looking at the git history, the `initial commit` comprised boilerplate code generated with the [following commands](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=linux%2Cazure-cli#create-a-local-function-project):
```
func init api-dotnet-mssql-base --worker-runtime dotnet-isolated --target-framework net8.0
cd api-dotnet-mssql-base
func new --name function_photos --template "HTTP trigger" --authlevel "anonymous"
```			


## License

See [LICENSE.md](./LICENSE.md).
