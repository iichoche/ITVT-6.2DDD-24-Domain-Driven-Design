FROM golang:1.24-alpine

WORKDIR /ecd

COPY go.mod ./
COPY go.sum ./
RUN go mod download

COPY . ./

RUN go build -o ./ecd

EXPOSE 8080

CMD [ "./ecd" ]