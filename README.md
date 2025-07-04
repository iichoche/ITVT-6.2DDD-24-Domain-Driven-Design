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

# Dockerfile
om de app als een dockerfile te runnen gebruik de volgende command
``` docker-compose up --build```
daarna, run de image in docker. 


# JWT token
dit programma maakt gebruik van een JWT token. deze token is aangeleverd in het ingeleverde document op itslearning

# testen met swagger
Om de app te testen met swagger, open de directie ```/apidocs```
daarin bevindt een swagger omgeving om ```get_advice``` te testen. 


# testen met swagger
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

om de geschreven unit testen te testen, run de onderstaande command:
```python -m unittest tests.unittests```
of run de action in github actions.


# Stappen voor azure deployment (vanaf start)
```
$timestamp = (Get-Date).ToString('yyyyMMddHHmmss')

$RESOURCE_GROUP_NAME = 'api'
$LOCATION = 'eastus'
$CONTAINER_REGISTRY_NAME = "acradvice$timestamp"    # must be globally unique
$PLAN     = 'asp-advice'
$SITE     = "api-advice-$timestamp"  # must be globally unique

az login

az upgrade

## Create a resource group and Azure Container Registry
# create group
az group create --name $RESOURCE_GROUP_NAME --location $LOCATION

# create azure container registry
az acr create --resource-group $RESOURCE_GROUP_NAME --name $CONTAINER_REGISTRY_NAME --sku Basic

## Build the image in Azure Container Registry
# build image
# Log in to ACR
az acr login --name $CONTAINER_REGISTRY_NAME

# Build the Docker image locally
docker build -t "${CONTAINER_REGISTRY_NAME}.azurecr.io/webappsimple:latest" .

# Push the image to your ACR
docker push "${CONTAINER_REGISTRY_NAME}.azurecr.io/webappsimple:latest"

## Deploy web app to service
# create app service plan
az appservice plan create --name $PLAN --resource-group $RESOURCE_GROUP_NAME --sku B1 --is-linux

# set env vironment as my subscription ID
$SUBSCRIPTION_ID=$(az account show --query id --output tsv)

export MSYS_NO_PATHCONV=1 # This line is for Windows users to prevent path conversion issues in Git Bash.
az webapp create --resource-group $RESOURCE_GROUP_NAME --plan $PLAN --name $CONTAINER_REGISTRY_NAME --assign-identity [system] --role AcrPull --scope /subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP_NAME --acr-use-identity --acr-identity [system] --container-image-name $CONTAINER_REGISTRY_NAME.azurecr.io/webappsimple:latest
```
# CI/CD Deployment
voor CI/CD is de deployment uitgevoerd door middel van een Docker container registry. hieronder volgt de code om de deployment up te daten: (niet geautomatiseerd om te controleren van een deployment)
## updaten azure deployment via docker
open docker
```
# update time and name accordingly to webname
$WEBAPP_NAME = "acradvice20250622213824"  
# make sure you're in the correct work folder
cd adv
rebuilding docker and pushing
docker build -t acradvice20250622213824.azurecr.io/webappsimple:latest .
az acr login --name $WEBAPP_NAME
docker push acradvice20250622213824.azurecr.io/webappsimple:latest
# restart app
az webapp restart --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP_NAME
```


# Cross cutting innovaties en concerns in de API
1: data valitdatie of de ingevulde input correct is
2: om de taak van API aan te houden, wordt er geen informatie opgeslagen in een database.
3: Er is een configuratie file aangemaakt voor het aanpassen van environment variables en er zijn geen local variables
4: logging toegepast om problemen op te sporen
5: om privacy te voorkomen wordt er in de API en het trainen van het model geen cliënten informatie meegenomen
6: er is een API key die met JWT beveiligd is 
7: er zijn verschillende github actions voor zelfgeschreven testen en een swagger omgeving om uitgebreid te testen met de API
8: er is CI/CD in de vorm van een Docker container registry