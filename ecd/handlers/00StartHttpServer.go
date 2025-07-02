package handlers

import (
	"context"
	"fmt"
	"log"
	"time"

	"github.com/labstack/echo/v4"
)

func InitHTTPServer(ctx context.Context) error {
	server := echo.New()
	initApiEindpoints(server)

	// Start Server in Goroutine
	serverErr := make(chan error, 1)
	go func() {
		log.Println("Echo server started on :8080")
		serverErr <- server.Start(":8080")
	}()

	select {
	case <-ctx.Done():
		log.Println("Shutting down Echo server...")
		shutdownCtx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
		defer cancel()
		return server.Shutdown(shutdownCtx)
	case err := <-serverErr:
		if err != nil {
			return fmt.Errorf("echo server error: %w", err)
		}
	}

	return nil
}

func initApiEindpoints(server *echo.Echo) {
	// All routes
	server.GET("api/clients", GetAllClients)

	server.POST("api/client", CreateNewClient)
	server.GET("api/client/:id", GetClientById)
	server.PUT("api/client/:id", UpdateExistingClient)

	server.GET("api/client/:clientid/addresses/", GetAddressesFromClient)
	server.GET("api/client/:clientid/contact-points/", GetContactPointsFromClient)
	server.GET("api/client/:clientid/observations/", GetObservationsFromClient)
	server.GET("api/client/:clientid/diagnoses/", GetDiagnosisFromClient)
	server.GET("api/client/:clientid/care-needs/", GetCareNeedsFromClient)

	// address operations
	server.POST("api/address/", AddAddressToClient)
	server.GET("api/address/:id", GetContactPointById)
	server.PUT("api/address/:id", UpdateContactPoint)

	// contactpoint operations
	server.POST("api/contact-point/", AddContactPointToClient)
	server.GET("api/contact-point/:id", GetContactPointById)
	server.PUT("api/contact-point/:id", UpdateContactPoint)

	// observation operations
	server.POST("api/observation/", AddObservationsToClient)
	server.GET("api/observation/:id", GetObservationsById)
	server.PUT("api/observation/:id", UpdateObservation)

	// observation Diagnosis
	server.POST("api/diagnoses/", AddDiagnosessToClient)
	server.GET("api/diagnoses/:id", GetDiagnosessById)
	server.PUT("api/diagnoses/:id", UpdateDiagnoses)

	// careneed operations
	server.POST("api/care-need/", AddCareNeedToClient)
	server.GET("api/care-need/:id", GetCareNeedById)
	server.PUT("api/care-need/:id", UpdateCareNeed)

	// Classification operations
	server.GET("api/classification/", GetAllClassifications)
	server.POST("api/classification/", AddClassification)
	server.GET("api/classification/:id", GetClassificationById)
	server.PUT("api/classification/:id", UpdateClassification)

	// Classification operations
	server.GET("api/classification/", GetAllClassifications)
	server.POST("api/classification/", AddClassification)
	server.GET("api/classification/:id", GetClassificationById)
	server.PUT("api/classification/:id", UpdateClassification)
}
