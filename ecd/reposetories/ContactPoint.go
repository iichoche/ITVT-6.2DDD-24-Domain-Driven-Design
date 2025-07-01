package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func ReadContactPointsFromClient(id uint) ([]ContactPoint, error) {
	var result []ContactPoint

	if err := getConnection().Where("client_id = ?", id).Find(&result).Error; err != nil {
		return []ContactPoint{}, err
	}
	return result, nil
}

func ReadContactPointById(id uint, preloads ...string) (ContactPoint, error) {
	var result ContactPoint
	query := getConnection().Where("ID = ?", id).Preload(clause.Associations)
	for _, preload := range preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return ContactPoint{}, err
	}
	return result, nil
}

func CreateContactPoint(contactPoint ContactPoint) error {
	if err := getConnection().Create(&contactPoint).Error; err != nil {
		return err
	}
	return nil
}

func UpdateContactPoint(contactPoint ContactPoint, id uint) error {
	if err := getConnection().Model(&ContactPoint{}).Where("id = ?", id).Updates(&contactPoint).Error; err != nil {
		return err
	}
	return nil
}
