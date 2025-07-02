package messages

import (
	"context"
	"encoding/json"
	"fmt"

	"github.com/Azure/go-amqp"
)

type recommendationsDTO struct {
	CareNeedId      uint `json:"careneedid"`
	RecommendedTech int  `json:"recommended_healthcareTech"`
	Ranking         []struct {
		HealthcareTech uint    `json:"healthcareTech"`
		Percetage      float64 `json:"percetage"`
	} `json:"healthcareTech_ranking"`
}

func ReceiveRecommendations(ctx context.Context, msg *amqp.Message) error {
	data := msg.GetData()
	if len(data) == 0 {
		return fmt.Errorf("received message with no data")
	}

	var recommendations recommendationsDTO
	err := json.Unmarshal(msg.GetData(), &recommendations)

	return err
}
