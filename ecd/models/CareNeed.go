package models

import "gorm.io/gorm"

type CareNeed struct {
	gorm.Model
	ClientID       uint
	Description    string
	Classification CareNeedClassification
	ProductAvice   int
}

type CareNeedClassification struct {
	Name        string
	Description string
}
