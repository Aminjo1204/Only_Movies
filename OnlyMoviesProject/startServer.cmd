rd /S /Q .vs 2> nul
rd /S /Q OnlyMoviesProject.Application/.vs 2> nul
rd /S /Q OnlyMoviesProject.Application/bin 2> nul
rd /S /Q OnlyMoviesProject.Application/obj 2> nul
rd /S /Q OnlyMoviesProject.Webapi/.vs 2> nul
rd /S /Q OnlyMoviesProject.Webapi/bin 2> nul
rd /S /Q OnlyMoviesProject.Webapi/obj 2> nul

:start
dotnet watch run -c Debug --project OnlyMoviesProject.Webapi
goto start
