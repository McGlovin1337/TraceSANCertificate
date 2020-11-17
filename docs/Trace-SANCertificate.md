---
external help file: TraceSANCertificate.dll-Help.xml
Module Name: TraceSANCertificate
online version:
schema: 2.0.0
---

# Trace-SANCertificate

## SYNOPSIS
Trace Certificates from Hostnames in a Source Certificate

## SYNTAX

```
Trace-SANCertificate [[-Host] <String>] [[-Port] <Int32>] [-TryPorts <Int32[]>] [-Timeout <Int32>]
 [<CommonParameters>]
```

## DESCRIPTION
Trace Certificates from Subject Alternative Names (SAN) Hostnames in a Source Certificate

## EXAMPLES

### Example 1
```powershell
PS C:\> Trace-SANCertificate -Host www.example.com
```

Attempts to retrieve a certificate from the host www.example.com using default TCP Port 443, and then attempt to retrieve a certificate from any valid hostname specified as a Subject Alternative Name (SAN) on TCP Port 443.

### Example 2
```powershell
PS C:\> Trace-SANCertificate -Host host.example.com -Port 993 -TryPorts 443, 993, 8443
```

Attempts to retrieve a certificate from host host.example.com using TCP Port 993, and then attempt to retrieve a certificate from any valid hostname specified as a Subject Alternative Name (SAN) on TCP Ports 443, 993 and 8443.

## PARAMETERS

### -Host
Hostname or IP Address to connect to

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Port
Specify the TCP Port for the Source Host

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Timeout
Specify Connection Timeout in milliseconds (ms)

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TryPorts
Specify TCP Ports to try for each discovered SAN hostname

```yaml
Type: Int32[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

### System.Int32

## OUTPUTS

### TraceSANCertificate.TraceSANCertificate+SANCertificate

## NOTES

## RELATED LINKS
