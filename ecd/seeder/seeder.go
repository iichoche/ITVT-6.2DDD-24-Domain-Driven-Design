package main

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/reposetories"
	"encoding/json"
	"log"
	"os"
)

type JsonData struct {
	Clients       []models.Client       `json:"Clients"`
	HumanNames    []models.HumanName    `json:"HumanNames"`
	Addresses     []models.Address      `json:"Addresses"`
	ContactPoints []models.ContactPoint `json:"ContactPoints"`
}

func main() {
	seeder()
}

func seeder() {
	var Data JsonData
	file, err := os.ReadFile("seeder/seed.json")
	if err != nil {
		log.Output(1, err.Error())
	}
	json.Unmarshal(file, &Data)

	reposetories.Create(Data.Clients)
	reposetories.Create(Data.HumanNames)
	reposetories.Create(Data.Addresses)
	reposetories.Create(Data.ContactPoints)

}
