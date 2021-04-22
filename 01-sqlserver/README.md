# Docker compose - Database

## PR1 - SqlServer - Base solution

 - [PR](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs/pullrequest/1)
 - [build](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_build/results?buildId=14&view=results)

```cmd
dotnet new api -n FriendsApi.Host -o src/FriendsApi.Host
dotnet new classlib -n FriendsApi -o src/FriendsApi
dotnet new classlib -n FriendsApi.Types -o src/FriendsApi.Types
dotnet new nunit -n FriendsApi.SelfHostedTests -o tests/FriendsApi.SelfHostedTests
dotnet new nunit -n FriendsApi.UnitTests -o tests/FriendsApi.UnitTests
```
### Nuget packages

- Create sln in the root and add projects
- Updates packages
- Install tests libraries
### Test and run

```cmd
PS C:\Github\docker-compose-labs\01-sqlserver> dotnet test
Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: 67 ms - FriendsApi.SelfHostedTests.dll (net5.0)
Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: 69 ms - FriendsApi.UnitTests.dll (net5.0)
PS C:\Github\docker-compose-labs\01-sqlserver> dotnet run --project .\src\FriendsApi.Host\FriendsApi.Host.csproj
```

 - navigate https://localhost:5001/swagger/index.html

## PR2 - SqlServer - Friend Crud + InMemory Tests  

 - [PR](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs/pullrequest/2)
 - [build](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_build/results?buildId=17&view=results)
 - [code coverage](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_build/results?buildId=17&view=codecoverage-tab)

### Database

- docker-compose

```yaml
version: "3"
services:
    db:
        image: custom-mssql
        container_name: custom-mssql
        build:
            context: ./database
            dockerfile: Dockerfile
        networks:
            - dbnet
        ports:
            - "1433:1433"        
networks:
    dbnet:
        name: dbnet
```

- database/Dockerfile
  
```dockerfile
FROM mcr.microsoft.com/mssql/server:2017-latest

RUN mkdir /work
COPY /friends.tar.gz /work
COPY /restore.sql /work
COPY /restore.sh /work

WORKDIR /work
RUN tar -xzvf ./friends.tar.gz -C ./
RUN chmod 666 friends.bak
RUN chmod 755 restore.sh

EXPOSE 1433
RUN ./restore.sh restore.sql

```

 - database/restore.sh

```bash
echo "Setting Environment variables."
export ACCEPT_EULA=Y
export SA_PASSWORD=yourStrong1234!Password
echo "Environment variables set."
echo "Starting SqlServr"
/opt/mssql/bin/sqlservr &
sleep 60 | echo "Waiting for 60s to start Sql Server"
# echo "Setting RAM to 2GB usage."
# /opt/mssql/bin/mssql-conf set memory.memorylimitmb 2048
# echo "Restarting to apply the changes."
# systemctl restart mssql-server.service
echo "Restoring DB."
/opt/mssql-tools/bin/sqlcmd -S127.0.0.1 -U sa -P $SA_PASSWORD -i $1
echo "DB restored."
echo "Deleting backup files."
rm -rf /work/*.bak
```

 - database/restore.sql

```sql
USE master
GO
PRINT 'Restoring db'
 -------------------------------------------------
--> Restoring friends 
-------------------------------------------------
RESTORE DATABASE friends
FROM DISK =  N'/work/friends.bak'
WITH FILE = 1,
     MOVE N'friends'
     TO  N'/var/opt/mssql/data/friends.mdf',
     MOVE N'friends_log'
     TO  N'/var/opt/mssql/data/friends_log.ldf',
     NOUNLOAD,
     STATS = 5;
GO

-- -------------------------------------------------
-- --> Adding user AdventureUser 
-- -------------------------------------------------
-- USE master;
-- GO
-- CREATE LOGIN AdventureUser
-- WITH PASSWORD = N'Adventure.@2018',
--      DEFAULT_DATABASE = AdventureWorks2017
-- GO
-- -------------------------------------------------
-- --> Adding permissions to AdventureUser
-- -------------------------------------------------
-- USE AdventureWorks2017
-- GO
-- CREATE USER AdventureUser FOR LOGIN AdventureUser
-- GO
-- USE AdventureWorks2017
-- GO
-- ALTER ROLE db_owner ADD MEMBER AdventureUser
-- GO
```

### Run database

 - docker-compose build
 - docker-compose up

### How to backup

- from database

```sql
BACKUP DATABASE friends TO DISK='/tmp/friends.bak'
```

- from docker host
 
```cmd
docker exec -it custom-mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "BACKUP DATABASE friends TO DISK='/tmp/friends.bak'"
```

- check bak file

```cmd
docker exec -it custom-mssql ls /tmp/ -la

total 5604
drwxrwxrwt 1 root root    4096 Apr  1 05:46 .
drwxr-xr-x 1 root root    4096 Apr  1 05:13 ..
-rw-r----- 1 root root 5726208 Apr  1 05:49 friends.bak
```
 - tar.gz 

```cmd
docker exec -it custom-mssql tar -czvf /tmp/friends.tar.gz -C /tmp friends.bak
```

- copy tar.gz into host

```cmd
docker cp custom-mssql:/tmp/friends.tar.gz C:\Github\docker-compose-labs\01-sqlserver\database
```

- [original playground](https://github.com/sheltertake/dotnetcore-api-template/blob/main/docs/README-1-1-docker-primer.md) 

### Nuget packages

- FriendsApi.SelfHostedTests.csproj
```xml
  <ItemGroup>
    <PackageReference Include="coverlet.msbuild" Version="3.0.3">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" Version="5.10.3" />
    <PackageReference Include="Moq" Version="4.16.1" />
    <PackageReference Include="NUnit" Version="3.13.1" />
    <PackageReference Include="NUnit3TestAdapter" Version="3.17.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.9.1" />
    
    <PackageReference Include="Microsoft.AspNetCore.TestHost" Version="5.0.4" />
    <PackageReference Include="Microsoft.AspNet.WebApi.Client" Version="5.2.7" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="5.0.4" />
  </ItemGroup>
```

- FriendsApi.csproj
```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="5.0.4" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.4" />
  </ItemGroup>
```

- FriendsApi.Host.csproj

```xml
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.1.1" />
```

### Browse solution

 - [controller](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs?path=%2F01-sqlserver%2Fsrc%2FFriendsApi.Host%2FControllers%2FFriendsController.cs)
 - [repository](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs?path=%2F01-sqlserver%2Fsrc%2FFriendsApi%2FRepositories%2FFriendsRepository.cs)

### SelfHosted Test

 - [crud test](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs?path=%2F01-sqlserver%2Ftests%2FFriendsApi.SelfHostedTests%2FControllers%2FFriendsControllerTest.cs)
 - [helper](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs?path=%2F01-sqlserver%2Ftests%2FFriendsApi.SelfHostedTests%2FHelpers%2FTestHelper.cs)

## Pipeline

 - [FriendsApi pipeline](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_build?definitionId=6)


```yaml

pool:
  name: Default

steps:

- task: UseDotNet@2
  displayName: 'Use .NET Core sdk 5.0.201'
  inputs:
    version: 5.0.201

- task: DotNetCoreCLI@2
  displayName: 'dotnet restore'
  inputs:
    command: restore
    projects: '**/*.csproj'

- task: DotNetCoreCLI@2
  displayName: 'dotnet build'
  inputs:
    projects: '**/*.csproj'
    arguments: '--configuration Release /p:Version=$(build.buildNumber)'

- task: DotNetCoreCLI@2
  displayName: 'dotnet test unit'
  inputs:
    command: test
    projects: '**/*tests/*.UnitTests/*.UnitTests.csproj'
    arguments: '--filter=TestCategory!=local --configuration Release --logger trx;logfilename=TestResults-UnitTests.trx /p:CollectCoverage=true /p:CoverletOutput=TestResults/results.xml /p:CoverletOutputFormat=cobertura'

- task: DotNetCoreCLI@2
  displayName: 'dotnet test selfhosted'
  inputs:
    command: test
    projects: '**/*tests/*.SelfHostedTests/*.SelfHostedTests.csproj'
    arguments: '--filter=TestCategory!=local --configuration Release  --logger trx;logfilename=TestResults-SelfHostedTests.trx /p:CollectCoverage=true /p:CoverletOutput=TestResults/results.xml /p:CoverletOutputFormat=cobertura'

- task: Palmmedia.reportgenerator.reportgenerator-build-release-task.reportgenerator@4
  displayName: ReportGenerator
  inputs:
    reports: '**/TestResults/results.xml'
    targetdir: '$(Build.SourcesDirectory)/CodeCoverage'
    assemblyfilters: '+FriendsApi*'

- task: PublishCodeCoverageResults@1
  displayName: 'Publish code coverage from $(Build.SourcesDirectory)/CodeCoverage/Cobertura.xml'
  inputs:
    codeCoverageTool: Cobertura
    summaryFileLocation: '$(Build.SourcesDirectory)/CodeCoverage/Cobertura.xml'
    reportDirectory: '$(Build.SourcesDirectory)/CodeCoverage'

- task: DotNetCoreCLI@2
  displayName: 'dotnet publish'
  inputs:
    command: publish
    arguments: '--configuration Release --output $(build.artifactstagingdirectory)  /p:Version=$(build.buildNumber)'

- task: PublishBuildArtifacts@1
  displayName: 'Publish Artifact: drop'

```

## PR3 - SqlServer - db in pipeline

 - [PR](https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs/pullrequest/4)
 - build
