@echo off
echo Starting RGFinance Development Environment...
echo.

echo 1. Starting SQL Server in Docker...
docker-compose -f docker-compose.dev.yml up -d

echo.
echo 2. Waiting for SQL Server to be ready...
@REM timeout /t 10 /nobreak > nul

@REM echo.
@REM echo 3. SQL Server ready on localhost:1433
@REM echo 4. Now start your development:
@REM echo    - Backend: cd WebApi && dotnet run
@REM echo    - Frontend: cd Website && ng serve
@REM echo.
@REM echo Database connection: localhost:1433
@REM echo Username: sa
@REM echo Password: YourStrong!Passw0rd
@REM echo.
pause