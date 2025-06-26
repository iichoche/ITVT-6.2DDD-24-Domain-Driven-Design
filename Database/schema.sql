
CREATE DATABASE IF NOT EXISTS ZorgtechnologieProductDB;
USE ZorgtechnologieProductDB;

CREATE TABLE ZorgtechnologieProduct (
    Id CHAR(36) PRIMARY KEY,
    Naam VARCHAR(255) NOT NULL,
    Omschrijving TEXT,
    Type VARCHAR(100),
    Kosten DECIMAL(10,2),
    Eigenschappen TEXT
);

CREATE TABLE Zorginstellingslocatie (
    Id CHAR(36) PRIMARY KEY,
    Naam VARCHAR(255) NOT NULL,
    Adres VARCHAR(255),
    Stad VARCHAR(100)
);

CREATE TABLE ZorgtechnologieProductItem (
    Id CHAR(36) PRIMARY KEY,
    ZorgTechnologieProductId CHAR(36),
    AanschafDatum DATE,
    AanschafKosten DECIMAL(10,2),
    InGebruik BOOLEAN,
    LocatieId CHAR(36),
    FOREIGN KEY (ZorgTechnologieProductId) REFERENCES ZorgtechnologieProduct(Id),
    FOREIGN KEY (LocatieId) REFERENCES Zorginstellingslocatie(Id)
);

CREATE TABLE Gebruik (
    Id CHAR(36) PRIMARY KEY,
    ZorgtechnologieProductItemId CHAR(36),
    StartTime DATETIME,
    EndTime DATETIME,
    InGebruik BOOLEAN,
    CareNeeds TEXT,
    Categorie VARCHAR(100),
    Ervaringen TEXT,
    FOREIGN KEY (ZorgtechnologieProductItemId) REFERENCES ZorgtechnologieProductItem(Id)
);
