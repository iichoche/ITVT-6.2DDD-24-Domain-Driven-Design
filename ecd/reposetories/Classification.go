package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func ReadAllClassifications() ([]Classification, error) {
	var result []Classification
	if err := getConnection().Find(&result).Error; err != nil {
		return []Classification{}, err
	}
	return result, nil
}

func ReadClassificationById(Id uint, Preloads ...string) (Classification, error) {
	var result Classification
	query := getConnection().Where("ID = ?", Id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return Classification{}, err
	}
	return result, nil
}

func CreateClassification(classification Classification) error {
	if err := getConnection().Create(&classification).Error; err != nil {
		return err
	}
	return nil
}

func UpdateClassification(classification Classification, id uint) error {
	if err := getConnection().Model(&Classification{}).Where("id = ?", id).Updates(&classification).Error; err != nil {
		return err
	}
	return nil
}
