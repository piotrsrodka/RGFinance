# Use .NET 6 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY Database/Database.csproj Database/
COPY RGFinance/RGFinance.csproj RGFinance/
RUN dotnet restore RGFinance/RGFinance.csproj

# Copy source code and build
COPY . .
RUN dotnet publish RGFinance/RGFinance.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port
EXPOSE 80
ENTRYPOINT ["dotnet", "RGFinance.dll"]