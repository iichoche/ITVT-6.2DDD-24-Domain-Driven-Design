# BCImplementatie & BFFApi

Deze repository bevat twee projecten:

1. **BCImplementatie** – de core-API voor het registreren en beheren van “gebruik” (sessions), zorgbehoeften, ervaringen en lookup-data (`NeedCategory`).  
2. **BFFApi** – een kleine Backend-For-Frontend laag die de core-API orkestreert en vereenvoudigde endpoints biedt voor de front-end.
3. **SQLServer**  de relationele database waarin de `BCImplementatie`-data wordt opgeslagen

---

## 🌐 BCImplementatie

### Wat doet het?
- **Start gebruik**: begin een nieuwe gebruikssessie voor een cliënt en product.  
- **CareNeeds**:  
  - Aanmaken, bijwerken, verwijderen van individuele noden (o.a. medicatie­herinnering).  
  - Ophalen één / alle care-needs.  
- **Ervaringen**:  
  - Registreren, bijwerken en verwijderen van gebruikers­ervaringen.  
  - Ophalen één / alle ervaringen.  
- **NeedCategories**: lookup-data voor categorieën van behoeften („Gezondheid”, „Onderwijs”, …).  

### Technische opzet
- **.NET 8 Web API** met **MediatR** (CQRS-patroon).  
- **Domain-Driven Design**: entiteiten (`Gebruik`, `CareNeed`, `Ervaring`, `NeedCategory`) + EF Core DbContext (+ migrations).  
- **Dependency Injection**: 
  - `IGebruikRepository` + repository-implementatie  
  - `IEventPublisher` voor domain-events  
- **DTO-laag** voor alle in/uitgaande JSON.  
- **Swagger / OpenAPI** voor interactieve API-documentatie.  
- **Docker** + **docker-compose** (inclusief SQL Server-container) voor lokale full-stack.

---

## 🚧 BFFApi

### Wat doet het?
- Exposeert één enkele “orchestratie” endpoint(s), bijvoorbeeld:
  - **`POST /api/util/registreerGebruik`**  
    - Roept achtereenvolgens de core-API aan om product- en cliënt-IDs op te halen,  
      en dan `POST /api/gebruik` om een sessie te starten.
- Vereenvoudigt front-end code en hoofdtaken (aggregation, API-key injectie, fallback).

### Technische opzet
- **.NET 8 Web API**  
- **IHttpClientFactory** met een named client `"BFF"` die automatisch de `X-API-KEY` header toevoegt.  
- **Configuratie** via `IMPLEMENTATIE_API_URL` + `X_API_KEY` in `appsettings.json` of omgevingsvariabelen.  
- **Controllers** met één actie per use-case, die intern naar `http://bcimplementatie/api/...` forwarden.  
- **Swagger** voor documentatie van de BFF-endpoints.  
- **Eigen Dockerfile** en `docker-compose.yml` entry, draait onafhankelijk op poort 3000.

---

## 🚀 Lokale opstart

1. **Clone** deze repo.  
2. **Configureer** (optioneel) je eigen secrets in `appsettings.json` of via omgevingsvariabelen.  
3. **Start alles** via Docker Compose in de root:
   ```bash
   docker-compose up --build -d

   Azure URLs:
https://implementatieapp-fucddyd2hqaaa8a7.northeurope-01.azurewebsites.net/

https://bffapiapp-gte3e8bybhf6hxa6.northeurope-01.azurewebsites.net/
