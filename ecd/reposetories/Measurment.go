package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func ReadMeasurmentById(Id uint, Preloads ...string) (Measurment, error) {
	var result Measurment
	query := getConnection().Where("ID = ?", Id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return Measurment{}, err
	}
	return result, nil
}

func CreateMeasurment(measurment Measurment) error {
	if err := getConnection().Create(&measurment).Error; err != nil {
		return err
	}
	return nil
}

func UpdateMeasurment(measurment Measurment, id uint) error {
	if err := getConnection().Model(&Measurment{}).Where("id = ?", id).Updates(&measurment).Error; err != nil {
		return err
	}
	return nil
}
