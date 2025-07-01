package handlers

import (
	"electronic-client-dossier/models"
	"electronic-client-dossier/reposetories"
	"log"
	"net/http"
	"strconv"

	"github.com/labstack/echo/v4"
)

type addressDTO struct {
	AddressId  uint   `json:"addressid" form:"addressid" query:"addressid"`
	ClientId   uint   `json:"clientid" form:"clientid" query:"clientid"`
	Line       string `json:"line" form:"line" query:"line"`
	City       string `json:"city" form:"city" query:"city"`
	Province   string `json:"province" form:"province" query:"province"`
	PostalCode string `json:"postalcode" form:"postalcode" query:"postalcode"`
	Country    string `json:"country" form:"country" query:"country"`

	Use  models.AddressUse  `json:"use" form:"use" query:"use"`
	Type models.AddressType `json:"type" form:"type" query:"type"`
}

func (dto *addressDTO) populate(addr models.Address) {
	dto.ClientId = addr.ClientId
	dto.AddressId = addr.ID
	dto.Use = addr.Use
	dto.Type = addr.Type
	dto.Line = addr.Line
	dto.City = addr.City
	dto.Province = addr.Province
	dto.PostalCode = addr.PostalCode
	dto.Country = addr.Country
}

func (dto addressDTO) toModel() (result models.Address) {
	text := dto.Line + ", " + dto.PostalCode + " " + dto.City + ", " + dto.Country
	result = models.Address{
		ClientId:   dto.ClientId,
		Use:        dto.Use,
		Type:       dto.Type,
		Text:       text,
		Line:       dto.Line,
		City:       dto.City,
		Province:   dto.Province,
		PostalCode: dto.PostalCode,
		Country:    dto.Country,
	}
	return result
}

func GetAddressesFromClient(c echo.Context) error {
	clientId, err := strconv.ParseUint(c.Param("clientid"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	allAddresses, err := reposetories.ReadAddressesFromClient(uint(clientId))
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	var allAddressesDTO []addressDTO
	for _, address := range allAddresses {
		dto := new(addressDTO)
		dto.populate(address)
		allAddressesDTO = append(allAddressesDTO, *dto)
	}

	return c.JSON(http.StatusOK, allAddressesDTO)
}

func AddAddressToClient(c echo.Context) error {
	newAddress := new(addressDTO)
	if err := c.Bind(newAddress); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	createAddress := newAddress.toModel()
	if err := reposetories.CreateAddress(createAddress); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}

func GetAddressById(c echo.Context) error {
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	result, err := reposetories.ReadAddressById(uint(id))
	if err != nil {
		log.Output(1, err.Error())
		return c.NoContent(http.StatusInternalServerError)
	}

	response := new(addressDTO)
	response.populate(result)
	return c.JSON(http.StatusOK, response)
}

func UpdateAddress(c echo.Context) error {
	addressDTO := new(addressDTO)
	id, err := strconv.ParseUint(c.Param("id"), 10, 64)
	if err != nil {
		return c.NoContent(http.StatusNoContent)
	}

	if err := c.Bind(addressDTO); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	UpdateAddress := addressDTO.toModel()
	if err := reposetories.UpdateAddress(UpdateAddress, uint(id)); err != nil {
		return c.String(http.StatusBadRequest, "bad request")
	}

	return c.JSON(http.StatusAccepted, "")
}
