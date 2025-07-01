package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func ReadAddressesFromClient(id uint) ([]Address, error) {
	var result []Address

	if err := getConnection().Where("client_id = ?", id).Find(&result).Error; err != nil {
		return []Address{}, err
	}
	return result, nil
}

func ReadAddressById(Id uint, Preloads ...string) (Address, error) {
	var result Address
	query := getConnection().Where("ID = ?", Id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return Address{}, err
	}
	return result, nil
}

func CreateAddress(address Address) error {
	if err := getConnection().Create(&address).Error; err != nil {
		return err
	}
	return nil
}

func UpdateAddress(student Address, id uint) error {
	if err := getConnection().Model(&Address{}).Where("id = ?", id).Updates(&student).Error; err != nil {
		return err
	}
	return nil
}
