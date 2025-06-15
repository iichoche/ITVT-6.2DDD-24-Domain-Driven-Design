# ITVT-6.2DDD-24-Domain-Driven-Design
Repository voor DDD de beste groep
to run environment, run:
# conda env create -f environment.yaml
to install dependencies, rin
# pip install -r requirements.txt


to train a new model, run the file: `trainModel.py`

to run the API, run the file: `advice.py`

to test the API, create a postmen POST request, use raw json and write
``
{
    "Categories": [7,11]
}
``
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
