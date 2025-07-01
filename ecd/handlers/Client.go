package handlers

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/reposetories"
	"log"
	"net/http"
	"strconv"

	"github.com/labstack/echo/v4"
	"gorm.io/datatypes"
)

type clientDTO struct {
	ClientId     uint           `json:"clientid" form:"clientid" query:"clientid"`
	BirthDate    datatypes.Date `json:"BirthDate" form:"BirthDate" query:"BirthDate"`
	DeceasedDate datatypes.Date `json:"DeceasedDate" form:"DeceasedDate" query:"DeceasedDate"`
	Active       bool           `json:"active" form:"active" query:"active"`
	Text         string         `json:"text" form:"text" query:"text"`
	Family       string         `json:"family" form:"family" query:"family"`
	Given        string         `json:"given" form:"given" query:"given"`
	Prefix       string         `json:"prefix" form:"prefix" query:"prefix"`
	Suffix       string         `json:"suffix" form:"suffix" query:"suffix"`

	Addresses     []addressDTO      `json:"addresses" form:"addresses" query:"addresses"`
	ContactPoints []contactPointDTO `json:"contactpoints" form:"contactpoints" query:"contactpoints"`
	Observations  []observationDTO  `json:"observations" form:"observations" query:"observations"`
	Diagnosis     []diagnosesDTO    `json:"diagnosis" form:"diagnosis" query:"diagnosis"`
	CareNeeds     []careNeedDTO     `json:"careneeds" form:"careneeds" query:"careneeds"`
}

func (client *clientDTO) populate(model models.Client) {
	client.ClientId = model.ID
	client.Active = model.Active
	client.BirthDate = model.BirthDate
	client.DeceasedDate = model.DeceasedDate
	client.Text = model.Name.Text
	client.Family = model.Name.Family
	client.Given = model.Name.Given
	client.Prefix = model.Name.Prefix
	client.Suffix = model.Name.Suffix

	for _, address := range model.Addresses {
		var addressDTO addressDTO
		addressDTO.populate(address)
		client.Addresses = append(client.Addresses, addressDTO)
	}
	for _, contactpoint := range model.Telecoms {
		var contactPointDTO contactPointDTO
		contactPointDTO.populate(contactpoint)
		client.ContactPoints = append(client.ContactPoints, contactPointDTO)
	}
	for _, observation := range model.Observations {
		var observationDTO observationDTO
		observationDTO.populate(observation)
		client.Observations = append(client.Observations, observationDTO)
	}
	for _, diagnoses := range model.Diagnosis {
		var diagnosesDTO diagnosesDTO
		diagnosesDTO.populate(diagnoses)
		client.Diagnosis = append(client.Diagnosis, diagnosesDTO)
	}
	for _, careneed := range model.CareNeeds {
		var careneedDTO careNeedDTO
		careneedDTO.populate(careneed)
		client.CareNeeds = append(client.CareNeeds, careneedDTO)
	}
}

func (client *clientDTO) toModel() models.Client {
	return models.Client{
		Active:       client.Active,
		BirthDate:    client.BirthDate,
		DeceasedDate: client.DeceasedDate,
		Name: models.HumanName{
			Text:   client.Text,
			Family: client.Family,
			Given:  client.Given,
			Prefix: client.Prefix,
			Suffix: client.Suffix,
		},
	}
}

func GetAllClients(c echo.Context) error {
	clients, err := reposetories.ReadAllClients()
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	var response []clientDTO
	for _, client := range clients {
		dto := new(clientDTO)
		dto.populate(client)
		response = append(response, *dto)
	}

	return c.JSON(http.StatusOK, response)
}

func CreateNewClient(c echo.Context) error {
	newClient := new(clientDTO)
	if err := c.Bind(newClient); err != nil {
		log.Output(1, err.Error())
		return c.String(http.StatusBadRequest, "bad Request")
	}

	createClient := newClient.toModel()
	err, clientId := reposetories.CreateClient(createClient)
	if err != nil {
		return c.String(http.StatusInternalServerError, "Internal Server Error")
	}

	return c.JSON(http.StatusCreated, clientId)
}

func GetClientById(c echo.Context) error {
	clientId, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	preloads := []string{"Name", "Addresses", "Telecoms", "Observations", "Diagnosis", "CareNeeds"}
	client, err := reposetories.ReadClientById(uint(clientId), preloads...)
	if err != nil {
		log.Output(1, err.Error())
		return c.String(http.StatusNotFound, "record not found")
	}

	response := new(clientDTO)
	response.populate(client)

	return c.JSON(http.StatusOK, response)
}

func UpdateExistingClient(c echo.Context) error {
	newClient := new(clientDTO)
	clientId, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	if err := c.Bind(newClient); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	updateClient := newClient.toModel()

	if err := reposetories.UpdateClient(updateClient, uint(clientId)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	if err := reposetories.UpdateHumanName(updateClient.Name, uint(clientId)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}
