package reposetories

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/utilities"

	"log"
	"os"

	"github.com/glebarez/sqlite"
	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

var connection *gorm.DB

func Connected() bool {
	return !getConnection().Config.DryRun
}

func Disconect() {
	database, _ := getConnection().DB()
	database.Close()
}

func getConnection() *gorm.DB {
	if connection != nil {
		return connection
	}

	var db *gorm.DB
	var err error

	if os.Getenv("APP_MODE") == "testing" {
		var dsn = utilities.UNITTEST_DATABASE_ORIGINAL
		db, err = gorm.Open(sqlite.Open(dsn))
	} else {
		var dns = os.Getenv("DNS")
		db, err = gorm.Open(mysql.Open(dns))
	}

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

func Create(Data interface{}) {
	getConnection().Create(Data)
}
