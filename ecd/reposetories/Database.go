package reposetories

import (
	"electronic-client-dossier/models"
	"fmt"
	"os"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

var connection *gorm.DB

func GetConnection() {
	host := os.Getenv("DB_HOST")
	user := os.Getenv("DB_USER")
	password := os.Getenv("DB_PASSWORD")
	name := os.Getenv("DB_NAME")
	port := os.Getenv("DB_PORT")
	dns := fmt.Sprintf("host=%s user=%s password=%s dbname=%s port=%s sslmode=disable", host, user, password, name, port)

	db, err := gorm.Open(postgres.Open(dns))
	if err != nil {
		panic("Database connection failed: " + err.Error())
	}

	if db == nil {

	}

}

func automigrate(db *gorm.DB) error {
	err := db.AutoMigrate(&models.Address{}, &models.ContactPoint{}, &models.HumanName{})
	if err != nil {
		panic("Database table loading failed")
	}

	err = db.AutoMigrate(&models.Client{}, &models.CareNeed{}, &models.CareNeedClassification{}, &models.Observation{}, &models.Measurment{}, &models.Diagnosis{})
	if err != nil {
		panic("Database table loading failed")
	}
	return nil
}
