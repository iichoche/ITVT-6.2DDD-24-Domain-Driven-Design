package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func ReadObservationsFromClient(clientId uint) ([]Observation, error) {
	var result []Observation
	if err := getConnection().Where("client_id = ?", clientId).Find(&result).Error; err != nil {
		return []Observation{}, err
	}
	return result, nil
}

func ReadObservationById(Id uint, Preloads ...string) (Observation, error) {
	var result Observation
	query := getConnection().Where("ID = ?", Id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return Observation{}, err
	}
	return result, nil
}

func CreateObservation(observation Observation) error {
	if err := getConnection().Create(&observation).Error; err != nil {
		return err
	}
	return nil
}

func UpdateObservation(observation Observation, id uint) error {
	if err := getConnection().Model(&Observation{}).Where("id = ?", id).Updates(&observation).Error; err != nil {
		return err
	}
	return nil
}
