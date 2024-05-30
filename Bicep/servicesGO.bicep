@description('Location for all resources.')
param location string = resourceGroup().location

param vnetname string = 'GOAuctionHouseVNet'
param subnetName string = 'GOServicesSubnet'
param storageAccountName string = 'storageAccount'
param dnsRecordName string ='backendhostname'
param dnszonename string='GOAuctionHouse.dk'


// --- Get a reference to our existing Virtual Network ---
resource VNET 'Microsoft.Network/virtualNetworks@2020-11-01' existing = {
  name: vnetname
  resource subnet 'subnets@2022-01-01' existing = {
    name: subnetName
  }
}

// --- Get a reference to the existing storage ---
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: storageAccountName
}

// --- Create the services container group ---
@description('auktionsHusetServicesGroup')
resource auktionsHusetServicesGroup 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
  name: 'auktionsHusetServicesGroup'
  location: location
  properties: {
    sku: 'Standard'
    containers: [
      {
        name: 'biddingservice'
        properties: {
          image: 'rasmusdd/biddingservice-image:1.0.0' 
          // command:[
          //   'tail', '-f', '/dev/null'
          // ]
          ports: [
            {
              port: 8081 // --- spørg Henrik om porte (intern/ekstern)
            }
          ]
          environmentVariables: [
            {
              name: 'ASPNETCORE_HTTP_PORTS'
              value: '8081'
            }
            {
              name: 'vaultConnectionString'
              value: 'http://backend:8200' //--- backendGO:8201
            }
            {
              name: 'QueueHostName'
              value: 'backend'
            }
          ]
          resources: {
            requests: {
              memoryInGB: json('1.0')
              cpu: json('0.5')
            }
          }
        }
      }
      
      {
        name: 'auctionservice'
        properties:{
          image: 'jakobhaulund/auction-service-image:latest'
          ports:[
            {
              port: 8082
            }
          ]
          environmentVariables:[
            {
              name: 'ASPNETCORE_HTTP_PORTS'
              value: '8082'
            }
            { 
              name: 'ConnectionString'
              value: 'mongodb://admin:1234@mongodb:27017'
            }
            {
              name: 'DatabaseName'
              value: '4SemProj'
            }
            {
              name: 'collectionName'
              value: 'Auction'
            }
            {
              name: 'QueueHostName'
              value: 'backend'
            }
            {
              name: 'vaultConnectionString'
              value: 'http://backend:8200'
            }
          ]
          resources: {
            requests: {
              memoryInGB: json('1.0')
              cpu: json('0.5')
            }
          }
        }
      }
      {
        name: 'userservice'
        properties: {
          image: 'jakobhaulund/bruger-service-image:latest'
          ports: [
            {
              port: 8084
            }                   // --- kan man mappe i Bicep?
          ]
          environmentVariables: [
            {
              name: 'ASPNETCORE_HTTP_PORTS'
              value: '8084'
            }
            {
              name: 'ConnectionString'
              value: 'mongodb://admin:1234@mongodb:27017'
            }
            {
              name: 'DatabaseName'
              value: '4SemProj'
            }
            {
              name: 'collectionName'
              value: 'User'
            }
            {
              name: 'vaultConnectionString'
              value: 'http://backend:8200'
            }
          ]
          resources: {
            requests: {
              memoryInGB: json('1.0')
              cpu: json('0.5')
            }
          }
        }
      }
      {
        name: 'catalogservice'
        properties: {
          image: 'rasmusdd/catalogservicegit-image:latest'
          ports: [
            {
              port: 8085
            }                   // --- kan man mappe i Bicep?
          ]
          environmentVariables: [
            {
              name: 'ASPNETCORE_HTTP_PORTS'
              value: '8085'
            }
            {
              name: 'DatabaseName'
              value: '4SemProj'
            }
            {
              name: 'collectionName'
              value: 'Catalog'
            }
            {
              name: 'vaultConnectionString'
              value: 'http://backend:8200'
            }
          ]
          resources: {
            requests: {
              memoryInGB: json('1.0')
              cpu: json('0.5')
            }
          }
        }
      }
      {
        name: 'authenticationservice'
        properties: {
          image: 'rasmusdd/authenticationservice-image:latest'
          ports: [
            {
              port: 8083
            }                   // --- kan man mappe i Bicep?
          ]
          environmentVariables: [
            {
              name: 'ASPNETCORE_HTTP_PORTS'
              value: '8083'
            }
            {
              name: 'userservicehost'
              value: 'http://localhost:8084'
            }
            {
              name: 'vaultConnectionString'
              value: 'http://backend:8200'
            }
          ]
          resources: {
            requests: {
              memoryInGB: json('1.0')
              cpu: json('0.5')
            }
          }
        }
      }
      {
        name: 'nginx'
        properties: {
          image: 'nginx:latest'
          ports: [
            {
              port: 4000
            }                   // --- kan man mappe i Bicep?
          ]
          environmentVariables: [
            {
              name: 'ASPNETCORE_HTTP_PORTS'
              value: '4000'
            }
          ]
          volumeMounts: [
            {
              name: 'nginx'
              mountPath: '/etc/nginx/' //--- bicep er forvirrende
            }
          ]
          resources: {
            requests: {
              memoryInGB: json('1.0')
              cpu: json('0.5')
            }
          }
        }
      }
      
    ]
    initContainers: []
    restartPolicy: 'Always'
    ipAddress: {
      ports: [
        {
          port: 4000
        }
        
      ]
      ip: '10.0.3.4'
      type: 'Private'
    }
    osType: 'Linux'
    volumes: [
      {
        name: 'nginx'
        azureFile: {
          shareName: 'storagenginx'
          storageAccountName: storageAccount.name
          storageAccountKey: storageAccount.listKeys().keys[0].value
        }
      }
    ]
    subnetIds: [
      {
        id: VNET::subnet.id
      }
    ]
    dnsConfig: {
      nameServers: [
        '10.0.0.10'
        '10.0.0.11'
      ]
      searchDomains: dnszonename
    }
  }

}


// --- Get a reference to the existing DNS Zone ---
resource dnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: dnszonename
}

// --- Create the DNS record for the service container group ---
resource dnsRecord 'Microsoft.Network/privateDnsZones/A@2020-06-01' = {
  name: dnsRecordName
  parent: dnsZone
  properties: {
    ttl: 3600
    aRecords: [
      {
        ipv4Address: auktionsHusetServicesGroup.properties.ipAddress.ip
      } // --- spørg henrik ---
    ]
  }
}

output containerIPAddressFqdn string = auktionsHusetServicesGroup.properties.ipAddress.ip
