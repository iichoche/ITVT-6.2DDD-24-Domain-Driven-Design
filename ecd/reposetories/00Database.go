package reposetories

import (
	"electronic-client-dossier/models"
	"log"

	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

var connection *gorm.DB

func getConnection() *gorm.DB {
	if connection != nil {
		return connection
	}

	var db *gorm.DB
	var err error
	var dns = "service:service@tcp(127.0.0.1:3306)/service?charset=utf8mb4&parseTime=True&loc=Local"

	db, err = gorm.Open(mysql.Open(dns))
	if err != nil {
		panic("Database connection failed: " + err.Error())
	} else {
		log.Output(1, "Database connection successful")
	}

	err = automigrate(db)
	if err != nil {
		panic("Database migration failed: " + err.Error())
	} else {
		log.Output(1, "Database migration successful")
	}

	connection = db
	return db
}

func automigrate(db *gorm.DB) error {
	err := db.AutoMigrate(
		&models.Client{}, &models.Address{}, &models.ContactPoint{}, &models.HumanName{},
		&models.CareNeed{}, &models.Classification{}, &models.Observation{},
		&models.Measurment{}, &models.Diagnoses{},
	)

	return err
}

func Connected() bool {
	return !getConnection().Config.DryRun
}
