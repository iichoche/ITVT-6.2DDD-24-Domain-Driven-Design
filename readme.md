# ITVT-6.2DDD-24 - Domain-Driven Design

## Testen

Om de backend tests uit te voeren, gebruik je het volgende commando:
dotnet test
dotnet test --logger "console;verbosity=detailed"

## Frontend afzonderlijk draaien

Ga naar de map `frontend-vanilla` en start een eenvoudige webserver via Python op poort 8080.

Het commando hiervoor is:
cd .\frontend-vanilla
python -m http.server 8080

## Backend afzonderlijk draaien en testen

Ga naar de map `zorgtechnologieproduct.api`, zet de omgeving op `Development` en start vervolgens de applicatie. Houd er rekening mee dat de Azure Firewall van de database hiervoor actief moet zijn. Standaard staat deze uitgeschakeld vanwege de credits.
Het commando voor dit is:
cd zorgtechnologieproduct.api
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run

## Applicatie draaien met Docker

Stop eerst eventueel actieve containers en bouw de applicatie opnieuw. Hiermee wordt zowel de backend als de frontend opgestart.
De frontend is daarna bereikbaar via [http://localhost:3000](http://localhost:3000) en de API via [http://localhost:5000/swagger](http://localhost:5000/swagger).

Het commando voor dit is
docker-compose down
docker-compose up --build

## Belangrijke opmerking

Er moet apart een `.env` bestand worden aangemaakt met de benodigde wachtwoorden etc voordat de applicatie voledig te gebruiken is.

## Live applicatie
De applicatie kan ook vanuit azure worden gehost worden, dit is voor de demo vooral, omdat het permanent aan laten staan van de omgeving veel credits zal gaan kosten.
wanneer deze live staan zijn ze te bereiken op:

[https://zorgtech-frontend.graypebble-998bdebe.westeurope.azurecontainerapps.io](https://zorgtech-frontend.graypebble-998bdebe.westeurope.azurecontainerapps.io).
[https://zorgtech-api.graypebble-998bdebe.westeurope.azurecontainerapps.io/swagger/index.html](https://zorgtech-api.graypebble-998bdebe.westeurope.azurecontainerapps.io/swagger/index.html).
