package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func ReadCareNeedsFromClient(id uint, Preloads ...string) ([]CareNeed, error) {
	var result []CareNeed

	query := getConnection().Where("client_id = ?", id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}

	if err := query.Find(&result).Error; err != nil {
		return []CareNeed{}, err
	}
	return result, nil
}

func ReadCareNeedById(Id uint, Preloads ...string) (CareNeed, error) {
	var result CareNeed
	query := getConnection().Where("ID = ?", Id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return CareNeed{}, err
	}
	return result, nil
}

func CreateCareNeed(careNeed CareNeed) error {
	if err := getConnection().Create(&careNeed).Error; err != nil {
		return err
	}
	return nil
}

func UpdateCareNeed(careNeed CareNeed, id uint) error {
	if err := getConnection().Model(&CareNeed{}).Where("id = ?", id).Updates(&careNeed).Error; err != nil {
		return err
	}
	return nil
}
