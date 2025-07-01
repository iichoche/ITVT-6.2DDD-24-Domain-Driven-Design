package models

import (
	"gorm.io/datatypes"
	"gorm.io/gorm"
)

type Client struct {
	gorm.Model
	Active       bool
	BirthDate    datatypes.Date
	DeceasedDate datatypes.Date

	Name      HumanName
	Addresses []Address
	Telecoms  []ContactPoint

	Observations []Observation
	Diagnosis    []Diagnoses
	CareNeeds    []CareNeed
}
