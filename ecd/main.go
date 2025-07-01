package main

import (
	"electronic-client-dossier/handlers"
	"electronic-client-dossier/messages"
	"electronic-client-dossier/reposetories"

	"github.com/labstack/echo/v4"
)

func main() {
	messages.InitMessaging()
	reposetories.Connected()
	server := echo.New()

	server.GET("api/clients", handlers.GetAllClients)

	server.POST("api/client", handlers.CreateNewClient)
	server.GET("api/client/:id", handlers.GetClientById)
	server.PUT("api/client/:id", handlers.UpdateExistingClient)

	server.GET("api/client/:clientid/addresses/", handlers.GetAddressesFromClient)
	server.GET("api/client/:clientid/contact-points/", handlers.GetContactPointsFromClient)
	server.GET("api/client/:clientid/observations/", handlers.GetObservationsFromClient)
	server.GET("api/client/:clientid/diagnoses/", handlers.GetDiagnosisFromClient)
	server.GET("api/client/:clientid/care-needs/", handlers.GetCareNeedsFromClient)

	// address operations
	server.POST("api/address/", handlers.AddAddressToClient)
	server.GET("api/address/:id", handlers.GetContactPointById)
	server.PUT("api/address/:id", handlers.UpdateContactPoint)

	// contactpoint operations
	server.POST("api/contact-point/", handlers.AddContactPointToClient)
	server.GET("api/contact-point/:id", handlers.GetContactPointById)
	server.PUT("api/contact-point/:id", handlers.UpdateContactPoint)

	// observation operations
	server.POST("api/observation/", handlers.AddObservationsToClient)
	server.GET("api/observation/:id", handlers.GetObservationsById)
	server.PUT("api/observation/:id", handlers.UpdateObservation)

	// observation Diagnosis
	server.POST("api/diagnoses/", handlers.AddDiagnosessToClient)
	server.GET("api/diagnoses/:id", handlers.GetDiagnosessById)
	server.PUT("api/diagnoses/:id", handlers.UpdateDiagnoses)

	// careneed operations
	server.POST("api/care-need/", handlers.AddCareNeedToClient)
	server.GET("api/care-need/:id", handlers.GetCareNeedById)
	server.PUT("api/care-need/:id", handlers.UpdateCareNeed)

	// Classification operations
	server.GET("api/classification/", handlers.GetAllClassifications)
	server.POST("api/classification/", handlers.AddClassification)
	server.GET("api/classification/:id", handlers.GetClassificationById)
	server.PUT("api/classification/:id", handlers.UpdateClassification)

	// Classification operations
	server.GET("api/classification/", handlers.GetAllClassifications)
	server.POST("api/classification/", handlers.AddClassification)
	server.GET("api/classification/:id", handlers.GetClassificationById)
	server.PUT("api/classification/:id", handlers.UpdateClassification)

	// Start server
	server.Logger.Fatal(server.Start(":8080"))
}
