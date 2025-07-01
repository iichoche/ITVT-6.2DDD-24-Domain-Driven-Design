package handlers

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/reposetories"
	"log"
	"net/http"
	"strconv"

	"github.com/labstack/echo/v4"
)

type contactPointDTO struct {
	ContactPointID uint   `json:"contactpointid" form:"contactpointid" query:"contactpointid"`
	ClientId       uint   `json:"clientid" form:"clientid" query:"clientid"`
	Value          string `json:"value" form:"value" query:"value"`
	Rank           uint16 `json:"rank" form:"rank" query:"rank"`

	Use    models.ContactPointUse    `json:"use" form:"use" query:"use"`
	System models.ContactPointSystem `json:"system" form:"system" query:"system"`
}

func (dto *contactPointDTO) populate(contactPoint models.ContactPoint) {
	dto.ContactPointID = contactPoint.ID
	dto.ClientId = contactPoint.ClientId
	dto.System = contactPoint.System
	dto.Value = contactPoint.Value
	dto.Use = contactPoint.Use
	dto.Rank = contactPoint.Rank
}

func (dto contactPointDTO) toModel() models.ContactPoint {
	return models.ContactPoint{
		ClientId: dto.ClientId,
		System:   dto.System,
		Value:    dto.Value,
		Use:      dto.Use,
		Rank:     dto.Rank,
	}
}

func GetContactPointsFromClient(c echo.Context) error {
	clientId, err := strconv.ParseUint(c.Param("clientid"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	allContactPoints, err := reposetories.ReadContactPointsFromClient(uint(clientId))
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	var allContactPointsDTO []contactPointDTO
	for _, contactPoint := range allContactPoints {
		dto := new(contactPointDTO)
		dto.populate(contactPoint)
		allContactPointsDTO = append(allContactPointsDTO, *dto)
	}

	return c.JSON(http.StatusOK, allContactPointsDTO)
}

func AddContactPointToClient(c echo.Context) error {
	newContactPoint := new(contactPointDTO)
	if err := c.Bind(newContactPoint); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	createContactPoint := newContactPoint.toModel()
	if err := reposetories.CreateContactPoint(createContactPoint); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}

func GetContactPointById(c echo.Context) error {
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	result, err := reposetories.ReadContactPointById(uint(id))
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	response := new(contactPointDTO)
	response.populate(result)
	return c.JSON(http.StatusOK, response)
}

func UpdateContactPoint(c echo.Context) error {
	contactPointDTO := new(contactPointDTO)
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	if err := c.Bind(contactPointDTO); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	updateContactPoint := contactPointDTO.toModel()
	if err := reposetories.UpdateContactPoint(updateContactPoint, uint(id)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}
