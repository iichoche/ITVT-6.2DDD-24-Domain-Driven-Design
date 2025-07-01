package models

import "gorm.io/gorm"

type HumanName struct {
	gorm.Model
	ClientId uint
	Text     string
	Family   string
	Given    string
	Prefix   string
	Suffix   string
}
