package main

import (
	"electronic-client-dossier/handlers"

	"github.com/labstack/echo/v4"
)

func main() {
	server := echo.New()

	server.GET("/", handlers.Hello)

}
