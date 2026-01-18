FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app
# copy the soltion and project files
COPY ./src .
# restore packages
RUN dotnet restore
# publish
RUN dotnet publish -c Release -o /publish

# run test
FROM build-env AS testrunner
WORKDIR "/app/PowerPositionExtractor.Logic.Tests"
RUN dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# build docker runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /publish
# copy the build output 
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "PowerPositionExtractor.Worker.dll"]