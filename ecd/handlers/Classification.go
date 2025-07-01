package handlers

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/reposetories"
	"log"
	"net/http"
	"strconv"

	"github.com/labstack/echo/v4"
)

type classificationDTO struct {
	ClassificationId uint   `json:"classificationid" form:"classificationid" query:"classificationid"`
	Name             string `json:"name" form:"name" query:"name"`
	Description      string `json:"description" form:"description" query:"description"`
}

func (dto *classificationDTO) populate(model models.Classification) {
	dto.ClassificationId = model.ID
	dto.Name = model.Name
	dto.Description = model.Description
}

func (dto classificationDTO) toModel() models.Classification {
	return models.Classification{
		Name:        dto.Name,
		Description: dto.Description,
	}
}

func GetAllClassifications(c echo.Context) error {
	classifications, err := reposetories.ReadAllClassifications()
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	var response []classificationDTO
	for _, classification := range classifications {
		dto := new(classificationDTO)
		dto.populate(classification)
		response = append(response, *dto)
	}

	return c.JSON(http.StatusOK, response)
}

func AddClassification(c echo.Context) error {
	request := new(classificationDTO)

	if err := c.Bind(request); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	createClassification := request.toModel()
	if err := reposetories.CreateClassification(createClassification); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}

func GetClassificationById(c echo.Context) error {
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	result, err := reposetories.ReadClassificationById(uint(id))
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	response := new(classificationDTO)
	response.populate(result)
	return c.JSON(http.StatusOK, response)
}

func UpdateClassification(c echo.Context) error {
	request := new(classificationDTO)
	id, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	if err := c.Bind(request); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}
	updateClassification := request.toModel()

	if err := reposetories.UpdateClassification(updateClassification, uint(id)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}
