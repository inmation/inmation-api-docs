# Configure inmation Web API to communicate over HTTPS and Secure WebSockets (WSS)

inmation Web API is a Self Hosted API in Windows Service, on HTTP. This document describes how to add a SSL layer on the top of it to provide additional transport layer security.

If you have a SSL Certificate from a commercial CA, you can directly go to ‘Configuring Netsh to bind certificate to Port’

## Creating a self signed certificate

```powershell
New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -DnsName "%FQDN%"
```

The option -DnsName specifies one or more DNS names to put into the subject alternative name extension of the certificate. The first DNS name is also saved as the Subject Name. If no signing certificate is specified, the first DNS name is also saved as the Issuer Name.

We recommend to use the fully qualified domain name (FQDN) of the machine were the inmation Web API is hosted as value for the -DnsName parameter.

### Example creating a self signed certificate

```powershell
New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -DnsName "srv01.inmation.local", "srv01"
```

This example creates a self-signed SSL server certificate in the computer MY (Personal) store with the subject and issuer set to the fully qualified domain name `srv01.inmation.local`, and the subject alternative names to:

- DNS Name=srv01.inmation.local
- DNS Name=srv01

Note: A new self signed certificate can only be installed into MY store. It can be moved (manually) to an other certificate store.

## Configuring Netsh to bind certificate to Port

Since the inmation Web API is hosted as an windows service, the SSL certificate is needed to bind to the configured HTTP port, so that all the requests to the port may be handed over. This is done over machine level using a netsh command.

Make sure the certificate is installed on local machine level and knows its private key. The recommended certificate store for a self signed certificate is the MY (Personal) store. A SLL certificate from a Commercial Certificate Authority should be installed in the Certificate Store ‘Trusted Root Certification Authorities’.

This can be verified by using the Microsoft Management Console (MMC):

- Open MMC console in administrator mode.
- Add the Certificates snap-in for ‘Computer account’. (File —> Add/Remove Snap-in... )
- Browse to the certificate store where the certificate is installed:
    * Certificates (Local Computer) —> Trusted Root Certification Authorities —> Certificates.

        or

    * Certificates (Local Computer) —> Personal —> Certificates.

- Double click the certificate, at the bottom of the ‘General’ tab you see: ‘You have a private key that corresponds to this certificate’. In case the private key is missing, the binding command results in the following error:

```cmd
SSL Certificate add failed, Error: 1312
A specified logon session does not exist. It may already have been terminated.
```

The netsh command needs as parameters the certificate store name, application identifier and the Certificate Hash of the SSL certificate.

### Certificate store name (%STORE%)

- ‘Root’ in case the certificate is installed in the certificate store ‘Trusted Root Certification Authorities’.
- ‘My’ in case the certificate is installed in the ‘Personal’ certificate store.

### Application ID (%APP_ID%)

Any valid GUID can be used to identify the inmation Web API. You can also use the GUID, which is defined in the inmation Web API Windows Service project: 0100f20b-b700-46a0-b175-d4486ce21df4.

### Certificate Hash / Thumbprint (%THUMBPRINT%)

To get the thumbprint, open the certificate and select the ‘Details’ Tab. Select the Thumbprint’ field in the list and copy the thumbprint hexadecimal string. If necessary remove the spaces to get a continuous string.

Open the command prompt in administrator mode and execute the following command to register the SSL with port. Replace the fields `%STORE%`, `%APP_ID%` and `%THUMBPRINT%`

```cmd
netsh http add sslcert ipport=0.0.0.0:8002 certstorename=%Store% appid={%APP_ID%} certhash=%THUMBPRINT%
```

### Example Netsh port binding

```cmd
netsh http add sslcert ipport=0.0.0.0:8002 certstorename=Root appid={0100f20b-b700-46a0- b175-d4486ce21df4} certhash=13ee8cdd0c823431b2056ef9673e22a53e3e6e28
```

- ipport=0.0.0:8002: Means, listen on all IP’s on port number 8002.

Information about the SSL Certificate bindings can be verified using following command:

```cmd
netsh http show sslcert ipport=0.0.0.0:8002
```

A SSL Certificate binding can be deleted using following command:

```cmd
netsh http delete sslcert ipport=0.0.0.0:8002
```

## Web API configuration

Make sure the Web API Server Object 'Base Address' property is configured to use https.

**!!! Restart the inmation Web API Windows Service after the configuration change !!!**