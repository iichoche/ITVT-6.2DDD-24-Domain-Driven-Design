# ITVT-6.2DDD-24-Domain-Driven-Design
Repository voor DDD de beste groep

commands:
Voor de testing
dotnet test
dotnet test --logger "console;verbosity=detailed"

De frontend
cd .\frontend-vanilla
python -m http.server 8080

backend
cd zorgtechnologieproduct.api
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
hiervoor moet de azure firewall worden opgezet van de database (staat uit vanwege credits)