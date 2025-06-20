package models

type AddressUse int

const (
	AUHome    AddressUse = iota
	AUWork    AddressUse = iota
	AUTemp    AddressUse = iota
	AUOld     AddressUse = iota
	AUBilling AddressUse = iota
)

type AddressType int

const (
	ATPostal   AddressType = iota
	ATPhysical AddressType = iota
	ATBoth     AddressType = iota
)

type Address struct {
	Use        AddressUse
	Type       AddressType
	Text       string
	Line       string
	City       string
	Province   string
	PostalCode string
	Country    string
}
