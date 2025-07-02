# Demo Gebruik
Testing command:
dotnet test
dotnet test --logger "console;verbosity=detailed"

Om de frontend los te runnen:
cd .\frontend-vanilla
python -m http.server 8080

De backend los te runnen en het testen:
cd zorgtechnologieproduct.api
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run

(hiervoor moet de azure firewall worden opgezet van de database (staat uit vanwege credits))

Om een docker build te gebruiken:
docker-compose down
docker-compose up --build

Frontend beschikbaar op: http://localhost:3000
API beschikbaar op: http://localhost:5000/swagger

Hiervoor moet nog wel apart een .env worden gemaakt met de passwords etc

