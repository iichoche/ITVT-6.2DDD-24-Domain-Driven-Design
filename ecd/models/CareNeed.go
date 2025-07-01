package models

import "gorm.io/gorm"

type CareNeed struct {
	gorm.Model
	ClientId         uint
	Description      string
	ClassificationId uint
	Classification   Classification
	ProductedAvice   uint
}
