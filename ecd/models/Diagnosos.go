package models

import (
	"gorm.io/datatypes"
	"gorm.io/gorm"
)

type Diagnoses struct {
	gorm.Model
	ClientId     uint
	DateNoted    datatypes.Date
	Description  string
	Observations []Observation `gorm:"many2many:diagnoses_observation;"`
}
