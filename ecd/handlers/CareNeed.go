package handlers

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/reposetories"
	"log"
	"net/http"
	"strconv"

	"github.com/labstack/echo/v4"
)

type careNeedDTO struct {
	CareNeedId       uint   `json:"careneedid" form:"careneedid" query:"careneedid"`
	ClientId         uint   `json:"clientid" form:"clientid" query:"clientid"`
	Description      string `json:"description" form:"description" query:"description"`
	ClassificationId uint   `json:"classificationid" form:"classificationid" query:"classificationid"`
	ProductedAvice   uint   `json:"productedavice" form:"productedavice" query:"productedavice"`
}

func (dto *careNeedDTO) populate(model models.CareNeed) {
	dto.CareNeedId = model.ID
	dto.ClientId = model.ClientId
	dto.Description = model.Description
	dto.ClassificationId = model.ClassificationId
	dto.ProductedAvice = model.ProductedAvice
}

func (dto careNeedDTO) toModel() models.CareNeed {
	return models.CareNeed{
		ClientId:         dto.ClientId,
		Description:      dto.Description,
		ClassificationId: dto.ClassificationId,
	}
}

func GetCareNeedsFromClient(c echo.Context) error {
	clientId, err := strconv.ParseUint(c.Param("clientid"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	results, err := reposetories.ReadCareNeedsFromClient(uint(clientId))
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	var responces []careNeedDTO
	for _, careNeed := range results {
		dto := new(careNeedDTO)
		dto.populate(careNeed)
		responces = append(responces, *dto)
	}

	return c.JSON(http.StatusOK, responces)
}

func AddCareNeedToClient(c echo.Context) error {
	request := new(careNeedDTO)
	if err := c.Bind(request); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	CreateCareNeed := request.toModel()
	if err := reposetories.CreateCareNeed(CreateCareNeed); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}

func GetCareNeedById(c echo.Context) error {
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	result, err := reposetories.ReadCareNeedById(uint(id))
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	response := new(careNeedDTO)
	response.populate(result)
	return c.JSON(http.StatusOK, response)
}

func UpdateCareNeed(c echo.Context) error {
	careNeedDTO := new(careNeedDTO)
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	if err := c.Bind(careNeedDTO); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	UpdateCareNeed := careNeedDTO.toModel()
	if err := reposetories.UpdateCareNeed(UpdateCareNeed, uint(id)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}
