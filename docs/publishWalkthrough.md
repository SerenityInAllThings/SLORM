cd SLORM.Application
dotnet pack -c Release
dotnet nuget push SLORM.x.x.x.nupkg -k APIKEY -s https://api.nuget.org/v3/index.json