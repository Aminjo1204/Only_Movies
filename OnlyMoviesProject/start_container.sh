DOCKER_IMAGE=onlymovies_app
SQL_IMAGE=OnlyMovies_sqlserver
# Use INTERNAL port for the communication inside the docker network (1433 not 11433)
CONN_STR="Server=10.0.38.3,1433;Initial Catalog=OnlyMoviesDB;User Id=sa;Password=SqlServer2019;TrustServerCertificate=true"
# Generate random secret (the secret in appsettings.json is empty)
SECRET=$(dd if=/dev/random bs=128 count=1 2> /dev/null | base64)

# Cleanup
docker rm -f $DOCKER_IMAGE
docker rm -f $SQL_IMAGE
docker volume prune -f
docker image prune -f
docker network prune -f


# Create a docker network.
docker network create --subnet=10.0.38.0/24 sqlserver_network
# Run SQL Server container with assigned ip in docker network.
docker run -d -p 11433:1433 --network=sqlserver_network --ip=10.0.38.3 --name $SQL_IMAGE \
    -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" \
    mcr.microsoft.com/azure-sql-edge:latest
# Build and run app container.
docker build -t $DOCKER_IMAGE . 
docker run -d -p 5000:80 --network=sqlserver_network --ip=10.0.38.2 --name $DOCKER_IMAGE \
    -e "ASPNETCORE_ENVIRONMENT=Production" \
    -e "CONNECTIONSTRINGS__DEFAULT=$CONN_STR" \
    -e "SECRET=$SECRET" \
    $DOCKER_IMAGE