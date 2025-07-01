package reposetories

import (
	. "electronic-client-dossier/models"

	"gorm.io/gorm/clause"
)

func CreateClient(client Client) (error, uint) {
	if err := getConnection().Create(&client).Error; err != nil {
		return err, 0
	}
	return nil, client.ID
}

func ReadAllClients() ([]Client, error) {
	var result []Client
	if err := getConnection().Preload("Name").Find(&result).Error; err != nil {
		return []Client{}, err
	}
	return result, nil
}

func ReadClientById(Id uint, Preloads ...string) (Client, error) {
	var result Client
	query := getConnection().Where("ID = ?", Id).Preload(clause.Associations)
	for _, preload := range Preloads {
		query.Preload(preload)
	}
	if err := query.First(&result).Error; err != nil {
		return Client{}, err
	}
	return result, nil
}

func UpdateClient(client Client, id uint) error {
	if err := getConnection().Model(&Client{}).Where("id = ?", id).Updates(&client).Error; err != nil {
		return err
	}
	return nil
}
