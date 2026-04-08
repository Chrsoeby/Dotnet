# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Api.csproj", "./"]
RUN dotnet restore "Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 Runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose the port (default is 8080 for Render)
EXPOSE 8080

# Set the environment variable for the port
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
ENTRYPOINT ["dotnet", "Api.dll"]