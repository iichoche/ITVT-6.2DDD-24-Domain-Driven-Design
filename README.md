# ITVT-6.2DDD-24-Domain-Driven-Design
Repository voor de SCA-recommandatie API van DDD de beste groep

Alles in deze branch is gemaakt door: Lucas

# installatiestappen
# Stap 1: Installeer anaconda
installeer conda van de onderstaande link
https://www.anaconda.com/download
# Step 2: installeer de environment
Installeer de environment. run de command:
``` conda env create -f environment.yaml ```
gebruik de onderstaande command om de environment te openen
``` conda activate advice ```
# Step 3: installeer de dependencies
in de envionment, installeer de dependencies met de onderstaande command
``` pip install -r requirements.txt ```

run het programma: ```app.py``` om de het lokaal te testen. 

De demo app maakt gebruik van de azure live deployment. zorg ervoor dat de Azure web service live aanstaat.

# Dockerfile
om de app als een dockerfile te runnen gebruik de volgende command
``` docker-compose up --build```
daarna, run de image in docker. 

# JWT token
dit programma maakt gebruik van een JWT token. deze token is aangeleverd in het ingeleverde document op itslearning.

# testen met postman
open postman, hieronder volgt de link om het te downloaden
https://www.postman.com/downloads/


gebruik de POST locatie van ```/get_advice``` om de POST te testen. 

in authrorization, selecteer Authorization en voeg het type 'Bearer token' in
voeg de JWT token toe.

gebruik de onderstaande body voor een werkend response.
```
{
    "Categories": [7,11]
}
```


