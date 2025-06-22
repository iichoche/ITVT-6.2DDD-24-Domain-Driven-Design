# ITVT-6.2DDD-24-Domain-Driven-Design
Repository voor DDD de beste groep




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


Firstly add the ``` SECRET_API_KEY ``` together with the secret key value in the header.
create a postmen POST request, use raw json and write
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

4.0 Flasks & Gunicorns update (using gunicorn to adapt to WSGI services)
1.0.0 Azure & Advice launch (complete Azure deployment)



code to run Azure launch
```
# 1) build a timestamp for uniqueness
$timestamp = (Get-Date).ToString('yyyyMMddHHmmss')

# 2) set all the names
$RG       = 'rg-advice-api'
$LOCATION = 'westeurope'
$ACR      = "acradvice$timestamp"    # must be globally unique
$PLAN     = 'asp-advice'
$SITE     = "api-advice-$timestamp"  # must be globally unique

# 3) login and create RG + ACR
az login
az group create --name $RG --location $LOCATION
az acr create --resource-group $RG --name $ACR --sku Basic
az acr login --name $ACR

# 4) build & push your local image into ACR
docker build -t advice-app:latest .
$fullImage = "$($ACR).azurecr.io/advice-app:latest"
docker tag advice-app:latest $fullImage
docker push $fullImage

# 5) make an App Service plan & Web App (Linux + B1 SKU)
az appservice plan create `
  --name $PLAN `
  --resource-group $RG `
  --is-linux `
  --sku B1

az webapp create `
  --resource-group $RG `
  --plan $PLAN `
  --name $SITE `
  --deployment-container-image-name $fullImage

# 6) fetch ACR credentials, wire them into the Web App
$acrCreds = az acr credential show --name $ACR | ConvertFrom-Json
az webapp config container set `
  --name $SITE `
  --resource-group $RG `
  --container-custom-image-name $fullImage `
  --container-registry-server-url https://$($ACR).azurecr.io `
  --container-registry-server-user $acrCreds.username `
  --container-registry-server-password $acrCreds.passwords[0].value

# 7) restart & tail logs to confirm
az webapp restart --resource-group $RG --name $SITE
az webapp log tail --resource-group $RG --name $SITE
```

1.1.0 Tests & Units update (testing my Azure app)

