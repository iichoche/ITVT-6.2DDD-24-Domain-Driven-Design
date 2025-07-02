package handlers_test

import (
	"electronic-client-dossier/handlers"
	"electronic-client-dossier/reposetories"
	"electronic-client-dossier/utilities"
	"net/http"
	"net/http/httptest"
	"strings"
	"testing"

	"github.com/labstack/echo/v4"
	"github.com/stretchr/testify/assert"
)

const PATH_CLIENT string = "/api/client"

func Test_CreateNewClient_GoodJson_200(t *testing.T) {
	utilities.SetTestMode()

	body := `{"BirthDate":"2000-07-11T00:00:00+02:00","active":true,"text":"Mårten Margit Khalil","family":" Layth","given":"Mårten"}`

	// Setup
	e := echo.New()
	req := httptest.NewRequest(http.MethodPost, PATH_CLIENT, strings.NewReader(body))
	req.Header.Set(echo.HeaderContentType, echo.MIMEApplicationJSON)
	rec := httptest.NewRecorder()
	c := e.NewContext(req, rec)

	assert.NoError(t, handlers.CreateNewClient(c))
	assert.Equal(t, http.StatusCreated, rec.Code)
}

func Test_CreateNewClient_NoDatabase_200(t *testing.T) {
	utilities.SetTestMode()
	reposetories.Disconect()

	body := `{"BirthDate":"2019-02-01T00:00:00+01:00","active":true,"text":"Mårten Margit Khalil","family":" Layth","given":"Mårten"}`

	// Setup
	e := echo.New()
	req := httptest.NewRequest(http.MethodPost, PATH_CLIENT, strings.NewReader(body))
	req.Header.Set(echo.HeaderContentType, echo.MIMEApplicationJSON)
	rec := httptest.NewRecorder()
	c := e.NewContext(req, rec)

	assert.NoError(t, handlers.CreateNewClient(c))
	assert.Equal(t, http.StatusInternalServerError, rec.Code)
}
