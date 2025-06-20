package models

import (
	"time"

	"gorm.io/gorm"
)

type Diagnosis struct {
	gorm.Model
	ClientID     uint
	DateNoted    time.Time
	Description  string
	Observations []Observation
}
