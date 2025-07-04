package main

import (
	"context"
	"electronic-client-dossier/handlers"
	"electronic-client-dossier/messages"
	"electronic-client-dossier/reposetories"
	"electronic-client-dossier/utilities"
	"log"
	"os"
	"os/signal"
	"syscall"

	"github.com/joho/godotenv"
	"golang.org/x/sync/errgroup"
)

func main() {
	appmode := os.Getenv("APP_MODE")
	if appmode == "testing" || appmode == "dev" {
		err := godotenv.Load(".env")
		utilities.FailOnError("Error loading .env file", err)
	}

	reposetories.Connected()

	ctx, cancel := context.WithCancel(context.Background())
	defer cancel()
	g, ctx := errgroup.WithContext(ctx)

	sigs := make(chan os.Signal, 1)
	signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)
	go func() {
		sig := <-sigs
		log.Printf("Caught signal: %s", sig)
		cancel()
	}()

	// Echo http http server
	g.Go(func() error {
		return handlers.InitHTTPServer(ctx)
	})

	// AMQP receiver
	err := messages.NewAMQPManager(ctx)
	utilities.FailOnError("Failed to init AMQP", err)

	handlers := map[string]messages.HandlerFunc{
		"receive_recommendations": messages.ReceiveRecommendations,
	}

	for queue, handler := range handlers {
		go func(q string, h messages.HandlerFunc) {
			if err := messages.StartAMQPReceiver(ctx, q, h); err != nil {
				log.Printf("Receiver %s error: %v", q, err)
			}
		}(queue, handler)
	}

	// different
	err = g.Wait()
	utilities.FailOnError("Exited with error", err)

	log.Println("All services exited cleanly.")

	// fggsdgsdfgbsdf := messages.InitMessaging()
	// if fggsdgsdfgbsdf != nil {
	// 	log.Fatalf("Failed to publish message: %v", fggsdgsdfgbsdf)
	// }
	// messages.RereceiveGetRecomendation()

	// reposetories.Connected()
	// server := echo.New()

	// server.GET("api/clients", handlers.GetAllClients)

	// server.POST("api/client", handlers.CreateNewClient)
	// server.GET("api/client/:id", handlers.GetClientById)
	// server.PUT("api/client/:id", handlers.UpdateExistingClient)

	// server.GET("api/client/:clientid/addresses/", handlers.GetAddressesFromClient)
	// server.GET("api/client/:clientid/contact-points/", handlers.GetContactPointsFromClient)
	// server.GET("api/client/:clientid/observations/", handlers.GetObservationsFromClient)
	// server.GET("api/client/:clientid/diagnoses/", handlers.GetDiagnosisFromClient)
	// server.GET("api/client/:clientid/care-needs/", handlers.GetCareNeedsFromClient)

	// // address operations
	// server.POST("api/address/", handlers.AddAddressToClient)
	// server.GET("api/address/:id", handlers.GetContactPointById)
	// server.PUT("api/address/:id", handlers.UpdateContactPoint)

	// // contactpoint operations
	// server.POST("api/contact-point/", handlers.AddContactPointToClient)
	// server.GET("api/contact-point/:id", handlers.GetContactPointById)
	// server.PUT("api/contact-point/:id", handlers.UpdateContactPoint)

	// // observation operations
	// server.POST("api/observation/", handlers.AddObservationsToClient)
	// server.GET("api/observation/:id", handlers.GetObservationsById)
	// server.PUT("api/observation/:id", handlers.UpdateObservation)

	// // observation Diagnosis
	// server.POST("api/diagnoses/", handlers.AddDiagnosessToClient)
	// server.GET("api/diagnoses/:id", handlers.GetDiagnosessById)
	// server.PUT("api/diagnoses/:id", handlers.UpdateDiagnoses)

	// // careneed operations
	// server.POST("api/care-need/", handlers.AddCareNeedToClient)
	// server.GET("api/care-need/:id", handlers.GetCareNeedById)
	// server.PUT("api/care-need/:id", handlers.UpdateCareNeed)

	// // Classification operations
	// server.GET("api/classification/", handlers.GetAllClassifications)
	// server.POST("api/classification/", handlers.AddClassification)
	// server.GET("api/classification/:id", handlers.GetClassificationById)
	// server.PUT("api/classification/:id", handlers.UpdateClassification)

	// // Classification operations
	// server.GET("api/classification/", handlers.GetAllClassifications)
	// server.POST("api/classification/", handlers.AddClassification)
	// server.GET("api/classification/:id", handlers.GetClassificationById)
	// server.PUT("api/classification/:id", handlers.UpdateClassification)

	// // Start server
	// server.Logger.Fatal(server.Start(":8080"))
}
