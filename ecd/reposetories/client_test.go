package reposetories_test

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/reposetories"
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
	"gorm.io/datatypes"
)

func Test_CreateClient_withExtra(t *testing.T) {
	println(reposetories.Connected())

	updateClient := models.Client{
		Active:    true,
		BirthDate: datatypes.Date(time.Now()),
		Name: models.HumanName{
			Text:   "newClient.Text",
			Family: "newClient.Family",
			Given:  "newClient.Given",
			Prefix: "newClient.Prefix",
			Suffix: "newClient.Suffix",
		},
	}

	err := reposetories.CreateClient(updateClient)
	println(err)

	assert.NoError(t, err)

}

func Test_UpdateClient_withExtra(t *testing.T) {
	println(reposetories.Connected())

	updateClient := models.Client{
		Active:    true,
		BirthDate: datatypes.Date(time.Now()),
	}

	updateName := models.HumanName{
		Text:   "hhhhhhhhhhh",
		Family: "hhhhhhhhhhh",
		Given:  "hhhhhhhhhhh",
		Prefix: "hhhhhhhhhhh",
		Suffix: "hhhhhhhhhhh",
	}

	err1 := reposetories.UpdateClient(updateClient, 1)
	err2 := reposetories.UpdateHumanName(updateName, 1)

	assert.NoError(t, err1)
	assert.NoError(t, err2)
}
