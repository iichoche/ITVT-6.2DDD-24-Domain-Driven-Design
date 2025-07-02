package models

import "gorm.io/gorm"

type CareNeed struct {
	gorm.Model
	ClientId        uint
	Description     string
	Classifications []Classification `gorm:"many2many:classifications_careneeds;"`
	ProductedAvice  uint
}
