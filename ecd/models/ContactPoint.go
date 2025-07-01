package models

import "gorm.io/gorm"

type ContactPointSystem int

const (
	CPSPhone ContactPointSystem = iota
	CPSFax   ContactPointSystem = iota
	CPSEmail ContactPointSystem = iota
	CPSPager ContactPointSystem = iota
	CPSUrl   ContactPointSystem = iota
	CPSSms   ContactPointSystem = iota
	CPSOther ContactPointSystem = iota
)

type ContactPointUse int

const (
	CPUHome   ContactPointUse = iota
	CPUWork   ContactPointUse = iota
	CPUTemp   ContactPointUse = iota
	CPUMobile ContactPointUse = iota
	CPUOld    ContactPointUse = iota
)

type ContactPoint struct {
	gorm.Model
	ClientId uint
	System   ContactPointSystem
	Value    string
	Use      ContactPointUse
	Rank     uint16
}
