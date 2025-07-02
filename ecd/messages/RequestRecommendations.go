package messages

import (
	"electronic-client-dossier/models"
	"encoding/json"

	"github.com/Azure/go-amqp"
)

type recReqDTO struct {
	CareNeedId uint   `json:"careneedid"`
	Categories []uint `json:"categories"`
}

func (r *recReqDTO) populate(m models.CareNeed) {
	r.CareNeedId = m.ID
	for _, cat := range m.Classifications {
		r.Categories = append(r.Categories, cat.ID)
	}
}

func RequestRecommendations(careNeed models.CareNeed) error {
	recReq := new(recReqDTO)
	recReq.populate(careNeed)
	messageData, err := json.Marshal(recReq)
	if err != nil {
		return err
	}

	sender, err := manager.getSender("generate_recommendations")
	if err != nil {
		return err
	}

	message := amqp.NewMessage(messageData)
	sender.Send(manager.context, message, &amqp.SendOptions{})

	return nil
}
