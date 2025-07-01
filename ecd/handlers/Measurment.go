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

type measurmentsDTO struct {
	MeasurmentsId uint           `json:"measurmentsid" form:"measurmentsid" query:"measurmentsid"`
	ObservationId uint           `json:"observationid" form:"observationid" query:"observationid"`
	DateTaken     datatypes.Date `json:"datetaken" form:"datetaken" query:"datetaken"`
	Description   string         `json:"description" form:"description" query:"description"`
}

func (dto *measurmentsDTO) populate(model models.Measurment) {
	dto.MeasurmentsId = model.ID
	dto.ObservationId = model.ObservationId
	dto.DateTaken = model.DateTaken
	dto.Description = model.Description
}

func (dto measurmentsDTO) toModel() models.Measurment {
	return models.Measurment{
		ObservationId: dto.ObservationId,
		DateTaken:     dto.DateTaken,
		Description:   dto.Description,
	}
}

func AddMeasurmentsToObservation(c echo.Context) error {
	request := new(measurmentsDTO)
	if err := c.Bind(request); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	CreateMeasurment := request.toModel()
	if err := reposetories.CreateMeasurment(CreateMeasurment); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}

func GetMeasurmentsById(c echo.Context) error {
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	result, err := reposetories.ReadMeasurmentById(uint(id))
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	response := new(measurmentsDTO)
	response.populate(result)
	return c.JSON(http.StatusOK, response)
}

func UpdateMeasurment(c echo.Context) error {
	observationDTO := new(measurmentsDTO)
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	if err := c.Bind(observationDTO); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}
	UpdateMeasurment := observationDTO.toModel()

	if err := reposetories.UpdateMeasurment(UpdateMeasurment, uint(id)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}
