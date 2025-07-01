package models

import "gorm.io/gorm"

type Classification struct {
	gorm.Model
	Name        string
	Description string
}
