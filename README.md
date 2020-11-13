# TraceSANCertificate
PowerShell Cmdlet to Trace Hostnames in SAN Certificates

## About

This Module contains a Cmdlet that can be used to retrieve the valid hostnames specified as Subject Alternative Names (SANs) in a given certificate, and retrieves the certificate from each hostname.
Useful to see if the hostnames specified in a certificate are using the same certificate as the source certificate/host.

## Requirements
This Module is built upon .NET Core 3.1 and PowerShell 7.\
Tested against PowerShell 7.1 on Windows and Linux.

## Usage
```powershell
Trace-SANCertificate -Host www.example.com -Port 443 -TryPorts 443, 993 -Timeout 1000
```

This example will retrieve the certificate at the host "www.example.com" on TCP Port 443. It will then parse any SANs in the certificate and attempt to connect to each SAN hostname on the ports specified by "-TryPorts" parameter
