# Authentication

The Web API supports token authentication based on inmation profiles and Windows domain and local accounts. Windows domain accounts are validated against the Active Directory (AD), local accounts are validated against the local Security Account Manager (SAM) store of the machine where the Web API Service is hosted. The sections Active directory integration and SAM integration describe the these integrations in more detail.

The Web API uses JSON Web Tokens (JWT). JWT is an open standard (RFC 7519) that defines a compact and self-contained way for securely transmitting information between parties as a JSON object. This information can be verified and trusted because it is digitally signed.

## Token request

Documentation about a token request can be found in the Swagger help page of the Web API:

`<baseaddress>`/api-docs/index#!/API/post_api_oauth2_token

## Token payload

The payload of a JWT can be inspected by using the Debugger section of the [JWT.IO](https://jwt.io) site.

A token payload contains the following claims:

```json
{
  "sub": "DOMAIN\\USERNAME",
  "in_prf": [
    "PowerUser",
    "Operator"
  ],
  "iat": 1534327142,
  "aud": [
    "inmation Web API"
  ],
  "exp": 1534328342,
  "iss": "inmation Web API",
  "nbf": 1534327142
}
```

| Claim | Description |
| ----- | ----------- |
| sub | Identifies the principal that is the subject of the JWT. Contains a username in Down-Level Logon Name or User Principle Name (UPN) format.|
| in_prf | Array of (enabled and Web Data Access granted) profile names of which the 'sub' is member of.
| iat | The time at which the JWT was issued.|
| aud | Identifies the inmation Web API instance(s) that the JWT is intended for.|
| exp | Contains the expiration time of the token. This time is calculated based on the 'iat' and the Web API Server Object property '.WebAPIAuthentication.AccessTokenLifeTime'.|
| iss | Identifies the inmation Web API that issued the JWT.|
| nbf | Identifies the time before which the JWT MUST NOT be accepted for processing.|

## Active directory integration

For Active Directory integration it is required that the Web API is hosted on a machine which is part of the domain. In a token request the 'authority' parameter has to be provided with value 'ad'.

The following diagram shows the OAuth 2 flow for Windows Active Directory users:

```ascii
                +------------------------------+
                |            Web API           |
+--------+      | +--------+        +--------+ |      +--------+        +--------+
|        |      | |        |        |        | |      |        |        |        |
|  User  |      | |  Auth  |        |Resource| |      |  Core  |        |   AD   |
|        |      | |        |        |        | |      |        |        |        |
+---+----+      +-----+-----------------+------+      +---+----+        +---+----+
    |                 |                 |                 |                 |
    |  Token request  | Validate credentials              |                 |
    +---------------> +---------------------------------------------------> |
    |                 |                 |                 |                 |
    |                 | <---------------------------------------------------+
    |                 |                 |                 |                 |
    |                 | Fetch authorization groups        |                 |
    |                 +---------------------------------------------------> |
    |                 |                 |                 |                 |
    |                 | <---------------------------------------------------+
    |                 |                 |                 |                 |
    |                 |  Fetch authorization info         |                 |
    |                 +---------------------------------> |                 |
    |                 |                 |                 |                 |
    |   Access token  | <---------------------------------+                 |
    | <---------------+                 |                 |                 |
    |                 |                 |                 |                 |
    |  Request with access token        |                 |                 |
    +---------------------------------->+                 |                 |
    |                 |                 +---------------> |                 |
    |                 |                 |                 |                 |
    |                 |                 | <---------------+                 |
    | <---------------------------------+                 |                 |
    |                 |                 |                 |                 |


```

The Web API validates the provided username and password in the token request against the Active Directory. In case the credentials are valid, the Web API queries the AD for security groups the user is member of. The Web API uses the Web API Server object property 'LDAP Directory Query Root' as query root.

Based on the provided username and authorization groups a mapping to inmation profiles takes place. The rules used to map a inmation profile are:

1) The Profile must exist in the inmation Access Model.
2) The Profile must be enabled.
3) The Profile must be authorized for SCI calls (under General Authorization in Object Properties panel)
4) A User object exist beneath the Profile of which the authenticating domain and account name matches the provided username.
5) A Group object exist beneath the Profile of which the authenticating domain and account name matches one of the security group fetched from the AD.

Based on the fetched authorization information a JWT will be created and signed with the 'Access Token Secret' defined in the Web API Server Object.

## Security Account Manager (SAM) integration

Local Machine account integration supports only inmation profile / user mapping based on username. Authorization groups are NOT fetched from the SAM. In a token request the 'authority' parameter has to be provided with value 'machine'.

## Authorization

The endpoints in the 'V2' namespace of the Web API can consume access token for authorization. In the request the 'Authorization' HTTP header has to be provided. The value of this header must be set to 'Bearer <access_token>'.