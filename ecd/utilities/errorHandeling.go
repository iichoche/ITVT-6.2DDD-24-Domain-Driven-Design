package utilities

import "log"

func FailOnError(msg string, err error) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}
}

func FailOnStatement(msg string, statement bool) {
	if statement {
		log.Fatalf("%s", msg)
	}
}
