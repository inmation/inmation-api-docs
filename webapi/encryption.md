# Configure inmation Web API to communicate over HTTPS and Secure WebSockets (WSS)

inmation Web API is a Self Hosted API in Windows Service, on HTTP. This document describes how to add a SSL layer on the top of it to provide additional transport layer security.
If you have a SSL Certificate from a commercial CA, you can directly go to ‘Configuring Netsh to bind certificate to Port’

## Creating a self signed certificate

New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -DnsName "%FQDN%"

The option -DnsName specifies one or more DNS names to put into the subject alternative name extension of the certificate. The first DNS name is also saved as the Subject Name. If no signing certificate is specified, the first DNS name is also saved as the Issuer Name.

We recommend to use the fully qualified domain name (FQDN) of the machine were the inmation Web API is hosted as value for the -DnsName parameter.

### Example

netsh http add sslcert ipport=0.0.0.0:8002 certstorename=Root appid={0100f20b-b700-46a0- b175-d4486ce21df4} certhash=13ee8cdd0c823431b2056ef9673e22a53e3e6e28

- ipport=0.0.0:8002: Means, listen on all IP’s on port number 8002.

Information about the SSL Certificate bindings can be verified using following command: netsh http show sslcert ipport=0.0.0.0:8002

A SSL Certificate binding can be deleted using following command.

netsh http delete sslcert ipport=0.0.0.0:8002

## Web API configuration

Make sure the Web API is configured to use https. This can be configured with the setting:

<add key="inmation.Api.Http.BaseAddress" value="https://*:8002" /> The default location of the configuration file is:

C:\inmation.root\webapi\inmation.WebApi.WindowsService.exe.config

**!!! Restart the inmation Web API Windows Service after the configuration change !!!**