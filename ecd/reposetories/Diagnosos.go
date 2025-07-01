package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func ReadDiagnosisFromClient(id uint, Preloads ...string) ([]Diagnoses, error) {
	var result []Diagnoses

	query := getConnection().Where("client_id = ?", id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}

	if err := query.Find(&result).Error; err != nil {
		return []Diagnoses{}, err
	}
	return result, nil
}

func ReadDiagnosesById(Id uint, Preloads ...string) (Diagnoses, error) {
	var result Diagnoses
	query := getConnection().Where("ID = ?", Id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return Diagnoses{}, err
	}
	return result, nil
}

func CreateDiagnoses(diagnoses Diagnoses) error {
	if err := getConnection().Create(&diagnoses).Error; err != nil {
		return err
	}
	return nil
}

func UpdateDiagnoses(diagnoses Diagnoses, id uint) error {
	if err := getConnection().Model(&Diagnoses{}).Where("id = ?", id).Updates(&diagnoses).Error; err != nil {
		return err
	}
	return nil
}
