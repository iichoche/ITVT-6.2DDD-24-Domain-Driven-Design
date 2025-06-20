package models

import (
	"time"

	"gorm.io/gorm"
)

type Client struct {
	gorm.Model
	Active    bool
	BirthDate time.Time
	Name      HumanName
	Telecom   []ContactPoint
	Address   []Address

	CareNeeds    []CareNeed
	Observations []Observation
	Diagnosis    []Diagnosis
}
