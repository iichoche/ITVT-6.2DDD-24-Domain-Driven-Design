package messages

import (
	"context"
	"electronic-client-dossier/utilities"
	"fmt"
	"log"
	"os"
	"sync"

	"github.com/Azure/go-amqp"
)

type HandlerFunc func(ctx context.Context, msg *amqp.Message) error

type amqpManager struct {
	context context.Context

	connection *amqp.Conn
	session    *amqp.Session

	senderMu   sync.Mutex
	receiverMu sync.Mutex

	senders   map[string]*amqp.Sender
	receivers map[string]*amqp.Receiver
}

var manager *amqpManager

// func makeConnectionString(format string) string {
// 	var username = os.Getenv("DATABASE_USERNAME")
// 	var password = os.Getenv("DATABASE_PASSWORD")
// 	var server = os.Getenv("DATABASE_SERVER")
// 	var port = os.Getenv("DATABASE_PORT")
// 	var name = os.Getenv("DATABASE_NAME")
// 	return fmt.Sprintf(format, username, password, server, port, name)
// }

func NewAMQPManager(ctx context.Context) error {
	conn, err := amqp.Dial(ctx, os.Getenv("ASB"), &amqp.ConnOptions{})
	if err != nil {
		return fmt.Errorf("failed to dial AMQP: %w", err)
	}

	session, err := conn.NewSession(ctx, nil)
	if err != nil {
		return fmt.Errorf("failed to create AMQP session: %w", err)
	}

	manager = &amqpManager{
		context:    ctx,
		connection: conn,
		session:    session,
		senders:    make(map[string]*amqp.Sender),
		receivers:  make(map[string]*amqp.Receiver),
	}

	return nil
}

func (m *amqpManager) getSender(queue string) (*amqp.Sender, error) {
	m.senderMu.Lock()
	defer m.senderMu.Unlock()

	if sender, ok := m.senders[queue]; ok {
		return sender, nil
	}

	sender, err := m.session.NewSender(m.context, queue, nil)
	if err != nil {
		return nil, fmt.Errorf("failed to create sender for %s: %w", queue, err)
	}

	m.senders[queue] = sender
	return sender, nil
}

func (m *amqpManager) getReceiver(ctx context.Context, queue string) (*amqp.Receiver, error) {
	m.receiverMu.Lock()
	defer m.receiverMu.Unlock()

	if receiver, ok := m.receivers[queue]; ok {
		return receiver, nil
	}

	receiver, err := m.session.NewReceiver(ctx, queue, nil)
	if err != nil {
		return nil, fmt.Errorf("failed to create receiver for %s: %w", queue, err)
	}

	m.receivers[queue] = receiver
	return receiver, nil
}

func (m *amqpManager) Close(ctx context.Context) {
	for _, sender := range m.senders {
		sender.Close(ctx)
	}
	for _, receiver := range m.receivers {
		receiver.Close(ctx)
	}
	m.session.Close(ctx)
	m.connection.Close()
}

func StartAMQPReceiver(ctx context.Context, queue string, handler HandlerFunc) error {
	receiver, err := manager.getReceiver(ctx, queue)
	if err != nil {
		utilities.FailOnError("[AMQP] Failed to create receiver for "+queue, err)
	}

	log.Printf("[AMQP] Receiver started for queue: %s", queue)

	for {
		select {
		case <-ctx.Done():
			log.Printf("[AMQP] Shutting down receiver for queue: %s", queue)
			return nil
		default:
			msgCtx, cancel := context.WithCancel(ctx)
			msg, err := receiver.Receive(msgCtx, &amqp.ReceiveOptions{})
			cancel()

			if err != nil {
				return fmt.Errorf("failed to receive message: %w", err)
			}

			if err := receiver.AcceptMessage(ctx, msg); err != nil {
				log.Printf("Failed to accept message: %v", err)
			}

			if err := handler(ctx, msg); err != nil {
				log.Printf("[AMQP] Handler error for %s: %v", queue, err)
				receiver.RejectMessage(ctx, msg, nil)
			} else {
				log.Printf("[AMQP] Handler Received message %s", queue)
				receiver.AcceptMessage(ctx, msg)
			}
		}
	}
}

func NewTextMessage(body []byte) *amqp.Message {
	return amqp.NewMessage(body)
}
