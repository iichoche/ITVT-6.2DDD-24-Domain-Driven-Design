package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func ReadHumanNameById(Id uint, Preloads ...string) (HumanName, error) {
	var result HumanName
	query := getConnection().Where("ID = ?", Id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return HumanName{}, err
	}
	return result, nil
}

func CreateHumanName(humanName HumanName) error {
	if err := getConnection().Create(&humanName).Error; err != nil {
		return err
	}
	return nil
}

func UpdateHumanName(humanName HumanName, id uint) error {
	if err := getConnection().Model(&HumanName{}).Where("client_id = ?", id).Updates(&humanName).Error; err != nil {
		return err
	}
	return nil
}
