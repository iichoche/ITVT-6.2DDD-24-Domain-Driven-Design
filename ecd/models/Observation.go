package models

import (
	"time"

	"gorm.io/gorm"
)

type Observation struct {
	gorm.Model
	ClientID    uint
	DateNoted   time.Time
	Description string
	Measurments Measurment
	Diagnosis   []Diagnosis
}

type Measurment struct {
	DateTaken   time.Time
	Description string
}
