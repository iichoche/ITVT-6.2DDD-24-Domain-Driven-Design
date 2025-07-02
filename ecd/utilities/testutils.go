package utilities

import (
	"log"
	"os"
)

const UNITTEST_DATABASE_ORIGINAL = "databasetesting/database.sqlite"
const UNITTEST_DATABASE_TESTING = "databasetesting/testing.sqlite"

func SetTestMode() {
	os.Setenv("APP_MODE", "testing")
	os.Setenv("UNITTEST_DATABASE", UNITTEST_DATABASE_TESTING)

	goToProjectRoot()
	copyTestDatabase()
}

func goToProjectRoot() error {
	for {
		dirs, err := os.ReadDir(".")
		if err != nil {
			return err
		}
		for _, dir := range dirs {
			if dir.Name() == "databasetesting" {
				return nil
			}
		}
		err = os.Chdir("..")
		if err != nil {
			return err
		}
	}
}

func copyTestDatabase() error {
	original, err := os.ReadFile(UNITTEST_DATABASE_ORIGINAL)
	if err != nil {
		log.Panic(err)
		return err
	}
	err = os.WriteFile(UNITTEST_DATABASE_TESTING, original, 0660)
	if err != nil {
		log.Panic(err)
		return err
	}

	return err
}
