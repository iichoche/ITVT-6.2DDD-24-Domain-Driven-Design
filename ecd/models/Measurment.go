package models

import (
	"gorm.io/datatypes"
	"gorm.io/gorm"
)

type Measurment struct {
	gorm.Model
	ObservationId uint
	DateTaken     datatypes.Date
	Description   string
}
