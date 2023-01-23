docker rm -f mariadb_onlymovies 2> nul
docker run --name mariadb_onlymovies -d -p 13306:3306 -e MARIADB_USER=root -e MARIADB_ROOT_PASSWORD=MySecretPassword mariadb:10.10.2
ping -n 3 127.0.0.1
dotnet build OnlyMoviesProject/OnlyMoviesProject.Webapi --no-incremental --force
dotnet run -c Debug --project OnlyMoviesProject/OnlyMoviesProject.Webapi
