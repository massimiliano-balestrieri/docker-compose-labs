## docker-compose-labs

## PRs

- base solution:
https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs/pullrequest/2
- sqlserver in pipeline:
https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs/pullrequest/4
- e2e test coverage:
https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs/pullrequest/7
- load tests:
https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs/pullrequest/11
- memory analysis:
https://dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs/pullrequest/13

## GIT

```cmd
git clone https://massimiliano-balestrieri@dev.azure.com/massimiliano-balestrieri/docker-compose-labs/_git/docker-compose-labs

cd .\docker-compose-labs\

```

## Run solution

```cmd
cd 01-sqlserver\
docker-compose build
docker-compose up
docker-compose down
```

- http://localhost:5001/friends
- http://localhost:5001/swagger/index.html


## SCENARIO 1 - Integration tests vs Server Database 

```cmd
cd 01-sqlserver\

docker-compose -f .\docker-compose-testapi.yml build
docker-compose -f .\docker-compose-testapi.yml up unit integration
docker-compose -f .\docker-compose-testapi.yml up coverage
docker-compose -f .\docker-compose-testapi.yml down
```

explorer:
- .\docker-compose-labs\01-sqlserver\results

## SCENARIO 2 - Pre launch e2e tests - code coverage e2e typed client 

```cmd
cd 02-e2e\
docker-compose -f .\docker-compose-testapi.yml build
docker-compose -f .\docker-compose-testapi.yml up e2e
docker-compose -f .\docker-compose-testapi.yml up coverage
docker-compose -f .\docker-compose-testapi.yml down
```

explorer:
- .\docker-compose-labs\02-e2e\results

## SCENARIO 3 - Run load tests vs API  

```cmd
cd 03-loadtests\
docker-compose -f .\docker-compose-loadtests.yml build
docker-compose -f .\docker-compose-loadtests.yml up warmup
docker-compose -f .\docker-compose-loadtests.yml up warmup
docker-compose -f .\docker-compose-loadtests.yml up wrk6sync 
docker-compose -f .\docker-compose-loadtests.yml up wrk6async 
docker-compose -f .\docker-compose-loadtests.yml up wrk12sync 
docker-compose -f .\docker-compose-loadtests.yml up wrk12async 
docker-compose -f .\docker-compose-loadtests.yml down
```

explorer:
- .\docker-compose-labs\03-loadtests\results


---

## SCENARIO 4 - Analyze memory 

```cmd
cd 03-memorytests\
docker-compose -f .\docker-compose-memorytests.yml build
docker-compose -f .\docker-compose-memorytests.yml up api
docker-compose -f .\docker-compose-memorytests.yml down
```