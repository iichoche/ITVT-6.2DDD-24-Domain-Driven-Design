package models

import (
	"gorm.io/datatypes"
	"gorm.io/gorm"
)

type Observation struct {
	gorm.Model
	ClientId    uint
	DateNoted   datatypes.Date
	Description string
	Measurments []Measurment
	Diagnosis   []Diagnoses `gorm:"many2many:diagnoses_observation;"`
}
