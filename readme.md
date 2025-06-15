# ITVT-6.2DDD-24-Domain-Driven-Design
Repository voor DDD de beste groep

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
