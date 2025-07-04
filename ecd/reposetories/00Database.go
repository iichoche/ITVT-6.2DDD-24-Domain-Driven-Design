package reposetories

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/utilities"
	"fmt"

	"log"
	"os"

	"github.com/glebarez/sqlite"
	"gorm.io/driver/mysql"
	"gorm.io/driver/sqlserver"
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

func makeConnectionString(format string) string {
	var username = os.Getenv("DATABASE_USERNAME")
	var password = os.Getenv("DATABASE_PASSWORD")
	var server = os.Getenv("DATABASE_SERVER")
	var port = os.Getenv("DATABASE_PORT")
	var name = os.Getenv("DATABASE_NAME")
	return fmt.Sprintf(format, username, password, server, port, name)
}

func getConnection() *gorm.DB {
	if connection != nil {
		return connection
	}

	var db *gorm.DB
	var err error

	switch os.Getenv("APP_MODE") {
	case "testing":
		var dsn = utilities.UNITTEST_DATABASE_ORIGINAL
		db, err = gorm.Open(sqlite.Open(dsn))
	case "dev":
		var dns = makeConnectionString("%s:%s@tcp(%s:%s)/%s?charset=utf8mb4&parseTime=True&loc=Local")
		db, err = gorm.Open(mysql.Open(dns))
	case "cloud":
		db, err = gorm.Open(sqlserver.Open(os.Getenv("AZURE_SQL_CONNECTIONSTRING")))
	default:
		err = fmt.Errorf("Incorrecte APP_MODE" + os.Getenv("APP_MODE"))
	}

	utilities.FailOnError("Database connection failed", err)
	log.Output(1, "Database connection successful")

	err = automigrate(db)
	utilities.FailOnError("Database migration failed", err)
	log.Output(1, "Database migration successful")

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
