# ITVT-6.2DDD-24-Domain-Driven-Design
Repository voor DDD de beste groep

Alles in deze branch is gemaakt door: Lucas


# Dockerfile
to run the app using a dockerfile, use the command:
``` docker-compose up --build```
afterwards, turn on the image.

The following installation guide is for Windows.

# Step 1: Install anaconda
install conda from the link below.
https://www.anaconda.com/download
# Step 2: install the environment
Install the environment. run the command:
``` conda env create -f environment.yaml ```
use the command below if to enter the environment:
``` conda activate advice ```
# Step 3: install dependencies
in the newly created environment, run the following command to install the dependencies:
``` pip install -r requirements.txt ```


to train a new model, run the file: ```trainModel.py``` 

to run the API, run the file: ```advice.py```




to test the API you can use postman. below follows the installation  link for postman.
https://www.postman.com/downloads/


Firstly add the ``` X-API-KEY ``` together with the secret key in the authentication. 
When you run the application locally, use the POST location of 
local
http://172.29.158.25//get_advice

Development
acradvice20250622213824.azurewebsites.net/get_advice

Set the body to raw, and select JSON. Then add the following command:
```
{
    "Categories": [7,11]
}
```




Modify the numers within categories as you like
TODO:
- unit tests
- interface tests
- integration tests


cross-cutting examples that can be added:

- Business rules
- Caching
- Code mobility
- Data validation
- Domain-specific optimizations
- Environment variables and other global configuration settings
- Error detection and correction
- Internationalization and localization which includes Language localisation
- Information security
- Logging
- Memory management
- Monitoring
- Persistence
- Product features
- Real-time constraints
- Synchronization
- Transaction processing
- Context-sensitive help
- Privacy
- Computer security

decide later which are needed


mijn idee is om trainModel.py zoveel mogelijk onafhankelijk te maken van advice.py

de focus op de app ligt op advice.py, omdat deze cruciaal is voor de zorg, 
trainModel heeft niets te maken met de zorg, het is alleen een systeem dat ik gebruik, 

version UPDATES todo:

~~4.0 Flasks & Gunicorns update (using gunicorn to adapt to WSGI services)~~ (no llonger needed as deployment automatically does this)
~~1.0.0 Azure & Advice launch (complete Azure deployment)~~
~~1.1.0  API update (adding API keys)~~
~~1.2.0 Tests and Units update~~
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


## maintenance:
```
# update time and name accordingly to webname
$WEBAPP_NAME = "acradvice20250622213824"  

rebuilding docker and pushing
docker build -t acradvice20250622213824.azurecr.io/webappsimple:latest .
docker push acradvice20250622213824.azurecr.io/webappsimple:latest
# restart app
az webapp restart --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP_NAME
```


