#!/bin/bash

az login

# opret ressourcegruppe
ResourceGroup=AuktionsHusetRG
az group create --name $ResourceGroup --location northeurope

# deploy bicep fil
az deployment group create --resource-group $ResourceGroup --template-file auctionsGO.bicep --verbose

# verificer ressourcer i ressourcegruppen
az resource list --resource-group $ResourceGroup


vault kv put -mount secret secrets SecretKey=5Jw9yT4fb9T5XrwKUz23QzA5D9BuY3p6 IssuerKey=gAdDxQDQq7UYNxF3F8pLjVmGuU5u8g3y MongoConnectionString=mongodb+srv://admin:1234@4semproj.npem60f.mongodb.net/
vault kv put -mount secret secrets SecretKey=5Jw9yT4fb9T5XrwKUz23QzA5D9BuY3p6 IssuerKey=gAdDxQDQq7UYNxF3F8pLjVmGuU5u8g3y MongoConnectionString=mongodb+srv://admin:<password>@4semproj.npem60f.mongodb.net/?retryWrites=true&w=majority&appName=4Semproj"
