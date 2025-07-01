package messages

import (
	"context"

	"github.com/Azure/go-amqp"
)

func InitMessaging() error {
	_, err := amqp.Dial(context.TODO(), "amqp[s]://<host name of AMQP 1.0 broker>", nil)
	if err != nil {
		// handle error
	}

	return nil
}
