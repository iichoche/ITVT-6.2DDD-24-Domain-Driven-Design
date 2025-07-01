package models

import "gorm.io/gorm"

type ContactPointSystem string

const (
	CPSPhone ContactPointSystem = "Phone"
	CPSFax   ContactPointSystem = "Fax"
	CPSEmail ContactPointSystem = "Email"
	CPSPager ContactPointSystem = "Pager"
	CPSSms   ContactPointSystem = "sms"
)

type ContactPointUse string

const (
	CPUHome   ContactPointUse = "Home"
	CPUWork   ContactPointUse = "Work"
	CPUTemp   ContactPointUse = "Temp"
	CPUMobile ContactPointUse = "Mobile"
	CPUOld    ContactPointUse = "Old"
)

type ContactPoint struct {
	gorm.Model
	ClientId uint
	System   ContactPointSystem
	Value    string
	Use      ContactPointUse
	Rank     uint16
}
