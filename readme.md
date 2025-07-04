# ITVT-6.2DDD-24 - Domain-Driven Design

## Testen

Om de backend tests uit te voeren, gebruik het volgende commando in de hoofdmap van het project:
```powershell
dotnet test
dotnet test --logger "console;verbosity=detailed"
```
Deze tests worden ook automatisch uitgevoerd bij elke push en pull op de zorgtechnologie-product branch bij de tab actions in github
## Frontend afzonderlijk draaien

Navigeer naar de map `frontend-vanilla` en start een eenvoudige webserver via Python op poort 8080:
```powershell
cd .\frontend-vanilla
python -m http.server 8080
```
De frontend is dan bereikbaar via [http://localhost:8080](http://localhost:8080).

## Backend afzonderlijk draaien en testen

Navigeer naar de map `zorgtechnologieproduct.api`, zet de omgeving op `Development` en start de applicatie:
```powershell
cd zorgtechnologieproduct.api
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```
De API is dan bereikbaar via [http://localhost:5000/swagger](http://localhost:5000/swagger).

> **Let op:** Zorg dat de Azure Firewall van de database actief is als je verbinding maakt met Azure SQL. Standaard staat deze uitgeschakeld.

## Applicatie draaien met Docker

Stop eerst actieve containers en bouw de applicatie opnieuw. Hiermee worden zowel de backend als de frontend opgestart:
```powershell
docker-compose down
docker-compose up --build
```
- Frontend: [http://localhost:3000](http://localhost:3000)
- API: [http://localhost:5000/swagger](http://localhost:5000/swagger)

## Belangrijke opmerking

Maak een `.env` bestand aan in de root van het project met de benodigde wachtwoorden en secrets voordat je de applicatie volledig gebruikt. Deze informatie is te vinden in het documentatie document H10

## Live applicatie

De applicatie kan ook gehost worden op Azure (voor demo-doeleinden). Houd rekening met het verbruik van credits.

- Frontend: [https://zorgtech-frontend.graypebble-998bdebe.westeurope.azurecontainerapps.io](https://zorgtech-frontend.graypebble-998bdebe.westeurope.azurecontainerapps.io)
- API: [https://zorgtech-api.graypebble-998bdebe.westeurope.azurecontainerapps.io/swagger/index.html](https://zorgtech-api.graypebble-998bdebe.westeurope.azurecontainerapps.io/swagger/index.html)
