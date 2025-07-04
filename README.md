# ITVT-6.2DDD-24-Domain-Driven-Design
Repository voor DDD de beste groep

Alles in deze branch is gemaakt door: Lucas

Deze branch is voor het intern aanmaken van nieuwe modellen. 

omdat dit een intern programma is en dus niet een microservice, is het intern gebouwd.

het idee is dat vanuit de implementatie een get request wordt gemaakt naar de implementatie BC voor de categoriëen en de zorgtechnologie Ids

Omdat de deployment van de implementatiue vrij laat is gekomen, is er een dataset zelf aangemaakt. deze is ook uiteindelijk in de API gebruikt. 
er is een mapping voor verschillende categories, hiermee kon ik een zorgtechnologie producten een naam geven.
 
```
        1: list(range(1, 31)),            # Kraamzorg
        2: [5, 10, 15, 20, 28],            # Rolstoel
        3: [3, 7, 11, 16, 22],             # Bed
        4: [8, 12, 17, 20, 25, 28],        # Traplift
        5: [7, 12, 16, 22, 30],            # Krukken
        6: [9, 14, 19, 25, 29]            # Brees
```

er is een http request gemaakt voor als de implementatie gedeployed is, daarvanuit de echte data opgehaald worden of door middel van een azure service bus. waarschijnlijk zal daar nog een decryptie stap nog komen, daarna volgt het genereren van data stap zoals gewoonlijk.

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

run het programma: ```trainModel.py``` om een nieuw model te trainen. 


to train a new model, run the file: ```trainModel.py``` 


er zal een model aangemaakt en opgeslagen in de log folder. indien er in de toekomst er een database moet komen voor de modellen wordt op dit moment er ook lokaal in .sqlite opgeslagen. 
