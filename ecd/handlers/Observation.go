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

type observationDTO struct {
	ObservationID uint           `json:"observationid" form:"observationid" query:"observationid"`
	ClientID      uint           `json:"clientid" form:"clientid" query:"clientid"`
	DateNoted     datatypes.Date `json:"datenoted" form:"datenoted" query:"datenoted"`
	Description   string         `json:"description" form:"description" query:"description"`
}

func (dto *observationDTO) populate(model models.Observation) {
	dto.ObservationID = model.ID
	dto.ClientID = model.ClientId
	dto.DateNoted = model.DateNoted
	dto.Description = model.Description
}

func (dto observationDTO) toModel() models.Observation {
	return models.Observation{
		ClientId:    dto.ClientID,
		DateNoted:   dto.DateNoted,
		Description: dto.Description,
	}
}

func GetObservationsFromClient(c echo.Context) error {
	clientId, err := strconv.ParseUint(c.Param("clientid"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	results, err := reposetories.ReadObservationsFromClient(uint(clientId))
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	var responces []observationDTO
	for _, observation := range results {
		dto := new(observationDTO)
		dto.populate(observation)
		responces = append(responces, *dto)
	}

	return c.JSON(http.StatusOK, responces)
}

func AddObservationsToClient(c echo.Context) error {
	request := new(observationDTO)
	if err := c.Bind(request); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	CreateObservation := request.toModel()
	if err := reposetories.CreateObservation(CreateObservation); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}

func GetObservationsById(c echo.Context) error {
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	result, err := reposetories.ReadObservationById(uint(id))
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	response := new(observationDTO)
	response.populate(result)
	return c.JSON(http.StatusOK, response)
}

func UpdateObservation(c echo.Context) error {
	observationDTO := new(observationDTO)
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	if err := c.Bind(observationDTO); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}
	UpdateObservation := observationDTO.toModel()

	if err := reposetories.UpdateObservation(UpdateObservation, uint(id)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}
