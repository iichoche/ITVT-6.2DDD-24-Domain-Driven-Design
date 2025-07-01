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

type diagnosesDTO struct {
	DiagnosesID uint           `json:"diagnosesid" form:"diagnosesid" query:"diagnosesid"`
	ClientID    uint           `json:"clientid" form:"clientid" query:"clientid"`
	DateNoted   datatypes.Date `json:"datenoted" form:"datenoted" query:"datenoted"`
	Description string         `json:"description" form:"description" query:"description"`
}

func (dto *diagnosesDTO) populate(model models.Diagnoses) {
	dto.DiagnosesID = model.ID
	dto.ClientID = model.ClientId
	dto.DateNoted = model.DateNoted
	dto.Description = model.Description
}

func (dto diagnosesDTO) toModel() models.Diagnoses {
	return models.Diagnoses{
		ClientId:    dto.ClientID,
		DateNoted:   dto.DateNoted,
		Description: dto.Description,
	}
}

func GetDiagnosisFromClient(c echo.Context) error {
	clientId, err := strconv.ParseUint(c.Param("clientid"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	results, err := reposetories.ReadDiagnosisFromClient(uint(clientId))
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	var responces []diagnosesDTO
	for _, diagnoses := range results {
		dto := new(diagnosesDTO)
		dto.populate(diagnoses)
		responces = append(responces, *dto)
	}

	return c.JSON(http.StatusOK, responces)
}

func AddDiagnosessToClient(c echo.Context) error {
	request := new(diagnosesDTO)
	if err := c.Bind(request); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	CreateDiagnoses := request.toModel()
	if err := reposetories.CreateDiagnoses(CreateDiagnoses); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}

func GetDiagnosessById(c echo.Context) error {
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	result, err := reposetories.ReadDiagnosesById(uint(id))
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	response := new(diagnosesDTO)
	response.populate(result)
	return c.JSON(http.StatusOK, response)
}

func UpdateDiagnoses(c echo.Context) error {
	diagnosesDTO := new(diagnosesDTO)
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	if err := c.Bind(diagnosesDTO); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}
	UpdateDiagnoses := diagnosesDTO.toModel()

	if err := reposetories.UpdateDiagnoses(UpdateDiagnoses, uint(id)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}
