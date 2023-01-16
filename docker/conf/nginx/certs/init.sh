#!/bin/sh
openssl genrsa 2048 > ./localhost.key
openssl req -new -key ./localhost.key > ./localhost.csr
openssl req -x509 -days 3650 -key ./localhost.key -in ./localhost.csr > ./localhost.crt
