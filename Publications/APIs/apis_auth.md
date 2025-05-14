## Overview

OAuth 2.0 was designed to answer the concern of **authorization** and not **authentication.** There is no defined way in the OAuth protocol to get resource owner information. Also, there's no common set of scopes. This is why the solution that needs to be build must use both OAuth for authorization and OpenID (or SAML or something like it) for authentication. 

## Quick implementation

## 

##  multimedia

## [Edit file](https://invisionapp.atlassian.net/wiki/download/attachments/898013279/OAuth-OpenID-using-ory-Hydra.mov?version=1&modificationDate=1573666223341&cacheVersion=1&api=v2)

[ORY Hydra](https://github.com/ory/hydra) is an open-source OAuth2 Server and OpenID Certified Service Provider implementation. The implementation is incredibly simple though we would need to consider previous work and our current infrastructure constraints.  We would be remiss to not look more deeply into this solution or even attempt to roll our own.  See below for some quick implementation details (pulled from this [article](https://www.ory.sh/run-oauth2-server-open-source-api-security/)). I left them broken out so that we could clearly see the flow behind thew Hydra implementation. See below.

Auth Squad has also done some initial works on Hydra to examine how to integrate it into our current infrastructure and existing services. With minimal coding, we were able to create a common flow for React and mobile clients to interact with an OAuth 2 protected API. We also tried using Hydra without OpenID to integrate our [scim-api](https://github.com/InVisionApp/scim) with IDPs who support OAuth 2, namely Okta, Azure AD, and OneLogin. The initial tests look promising and we're looking into integrating with these vendors (using Hydra) in Q1/Q2.

## Intent

**The intent of this document is primarily for ideation and to drive some discussions around all of the previous work that has been done and thought through as well as what we can do now based on new needs and directions.**

## Our identity use cases

* OAuth 2.0: Delegated authorization \- giving scoped access to a user's data to a 3rd party without compromising the resource owner's credentials  
* OpenID: Authentication \- Identifying a user of a given system

## OpenID \+ OAuth 2.0 general flow

* **Resource owner** is on the **client** site\\app  
* **Client** app communicates to the **Resource owner** that authorization is needed from an **authorization server**  
* **Resource owner** takes an action to let the **client** app know that they want to request Authorization  
* **Resource owner** is directed to **authorization server's** auth login  
  * With  
    * Client Id  
    * Redirect URI  
    * Response Type  
    * Scope (openId) \+ actual scopes  
    * State  
* **Authorization code flow** begins and the **authorization server** communicates to the **resource owner** what the **client** wants access to \- to initialize an **authorization grant** (in the form of a **code grant**)  
* If User affirms then the **authorization server** redirects the **resource owner** back to the client using a **redirect URI** with an **authorization code**  
* The **client** uses the **authorization code** to request an **access token** that is **scoped** based on the **scopes** that were provided during the **Authorization code flow** to the **authorization server**  
  * With  
    * code  
    * Client Id  
    * Client Secret  
    * Grant Type  
* The **authorization server** validates the **authorization code** and returns an **access token** if validation is successful  
  * Content body of  
    * access token  
    * ID token (JWT)  
    * expires in (in seconds)  
    * token type (typically "Bearer")  
* The **client** calls **resource server** APIs on the **resource owner's** behalf using the **access token** and **client secret**  
  * Header of  
    * Authorization: Bearer \[**access token\]**  
  * The Client can also call the /userinfo endpoint to get resource owner authentication information  
* The **resource server** validates the **access token** (is it valid, has it expired) then evaluates the scope for authorization

## OpenID Terminology

* **ID token** \- represents the ID of the resource owner  
* **UserInfo endpoint** \- an endpoint on the authorization server that can be used to get more user information  
* **Standard Scope set \-** self-defined see scope under OAuth 2.0 terminology

## OAuth 2.0 Terminology

* **Access token** \- Token that the Client uses to ask for permission  
* **Authorization code** \- Sent back from the authorization server to the Client to be used in an exchange for an access token  
* **Authorization code flow** \- The workflow used to obtain an authorization code grant  
* **Authorization grant** (different types \- code grant) \- Proof of consent  
* **Authorization server** \- The system that the client can use to obtain consent  
* **Client** \- The consumer application  
* **Client Id** \- a unique value that is used by the client when making the initial request to the authorization server  
* **Client secret** \- a key only known to the client that is used in back-channel requests to the authorization server and resource server  
* **Consent** \- permission provided by the resource owner to the authorization server to indicate that the resource owner wants to allow the client to access data on the resource owner's behalf  
* **Grant type** \- there are multiple OAuth 2.0 flows. These flows are dependent on network channel visibility and can enable various consumer models  
  * **authorization code** (common 3rd party): front and back channel considerations  
  * **implicit**: front channel only  
  * **resource owner credentials based** (1st party consumption): back-channel only  
  * **client credentials** (common 1st party consumption, server to server): back-channel only  
* **Redirect URI** \- The location provided to the authorization server so that it can return the user to origin  
* **Resource owner** \- The user  
* **Resource server** \- API or system that is used to get data  
* **Response type** \- the type of expected response when the client is making a request to the authorization server  
  * **code** \- will result in the authorization server returning an authorization code  
  * **token** \- will result in the authorization server returning an access token  
* **Scope** \- permissive bounds that can be used by the client when requesting auth and making requests to the resource server  
* **State** \- a text value that is sometimes sent to the authorization server and then returned from the authorization server to the client after the initial authorization code is requested

## Other Terminology

* Back-channel (secure) \- Complete trust. When the client makes requests to and from it's backend server to other services as the client  
* Front channel (less secure) \- Implies less than complete trust.

## Needs

## ORY OAuth 2.0 / OpenID Connect Implementation Quick Commands

**Check for ports in use**

sudo netstat \-tuplen | grep '9000\\|9001\\|9010\\|9020'  
**Check for previous instances of Hydra**

docker ps | grep 'hydra'  
**Create a network**

docker network create hydraguide  
**Install and run PostgreSQL**

docker run \--network hydraguide \\

  \--name ory-hydra-example--postgres \\

  \-e POSTGRES\_USER=hydra \\

  \-e POSTGRES\_PASSWORD=secret \\

  \-e POSTGRES\_DB=hydra \\

  \-d postgres:9.6  
**Setup the config for the SP**

\# The system secret can only be set against a fresh database. This

\# secret is used to encrypt the database and needs to be set to the same value every time the process (re-)starts.

\# You can use /dev/urandom to generate a secret. But make sure that the secret must be the same anytime you define it.

\# You could, for example, store the value somewhere.

$ export SECRETS\_SYSTEM=$(export LC\_CTYPE=C; cat /dev/urandom | tr \-dc 'a-zA-Z0-9' | fold \-w 32 | head \-n 1\)

\#

\# Alternatively you can obviously just set a secret:

\# $ export SECRETS\_SYSTEM=this\_needs\_to\_be\_the\_same\_always\_and\_also\_very\_$3cuR3-.\_

**Setup the database**

docker run \-it \--rm \\

  \--network hydraguide \\

  oryd/hydra:v1.0.8 \\

  migrate sql \--yes $DSN

**Start/Run the OAuth2 Server**

docker run \-d \\

  \--name ory-hydra-example--hydra \\

  \--network hydraguide \\

  \-p 9000:4444 \\

  \-p 9001:4445 \\

  \-e SECRETS\_SYSTEM=$SECRETS\_SYSTEM \\

  \-e DSN=$DSN \\

  \-e URLS\_SELF\_ISSUER=http://127.0.0.1:9000/ \\

  \-e URLS\_CONSENT=http://127.0.0.1:9020/consent \\

  \-e URLS\_LOGIN=http://127.0.0.1:9020/login \\

  oryd/hydra:v1.0.8 serve all \--dangerous-force-http

**Run the "live" check**

docker logs ory-hydra-example--hydra  
**Optional: Run for hydra help**

docker run \--rm \-it \\

  oryd/hydra:v1.0.8 \\

  Help

**Perform the OAuth2 Client Credentials Flow**

docker run \--rm \-it \\

  \--network hydraguide \\

  oryd/hydra:v1.0.8 \\

  clients create \\

    \--endpoint http://ory-hydra-example--hydra:4445 \\

    \--id some-consumer \\

    \--secret some-secret \\

    \--grant-types client\_credentials \\

    \--response-types token,code  
**Issue an OAuth2 Access Token**

docker run \--rm \-it \\

  \--network hydraguide \\

  oryd/hydra:v1.0.8 \\

  token client \\

    \--client-id some-consumer \\

    \--client-secret some-secret \\

    \--endpoint http://ory-hydra-example--hydra:4444  
**Validate the OAuth2 Access Token**

docker run \--rm \-it \\

  \--network hydraguide \\

  oryd/hydra:v1.0.8 \\

  token introspect \\

    \--client-id some-consumer \\

    \--client-secret some-secret \\

    \--endpoint http://ory-hydra-example--hydra:4445 \\

    \[ACCESS TOKEN FROM THE ABOVE EXECUTION\]

{

        "active": true,

        "client\_id": "some-consumer",

        "exp": 1528901511,

        "iat": 1528897911,

        "iss": "http://127.0.0.1:9000/",

        "sub": "facebook-photo-backup",

        "token\_type": "access\_token"

}

#### **User login / concent flow**

**Run the user login & consent app**

docker run \-d \\  
  \--name ory-hydra-example--consent \\  
  \-p 9020:3000 \\  
  \--network hydraguide \\  
  \-e HYDRA\_ADMIN\_URL=http://ory-hydra-example--hydra:4445 \\  
  \-e NODE\_TLS\_REJECT\_UNAUTHORIZED=0 \\  
  oryd/hydra-login-consent-node:v1.0.8

#### **OAuth2 with OpenID Connect (OIDC) Authorize Flow**

**Request tokens**

docker run \--rm \-it \\

  \--network hydraguide \\

  oryd/hydra:v1.0.8 \\

  clients create \\

    \--endpoint http://ory-hydra-example--hydra:4445 \\

    \--id another-consumer \\

    \--secret consumer-secret \\

    \-g authorization\_code,refresh\_token \\

    \-r token,code,id\_token \\

    \--scope openid,offline \\

    \--callbacks http://127.0.0.1:9010/callback

**Perform OAuth2 Authorize Code Flow**

docker run \--rm \-it \\

  \--network hydraguide \\

  \-p 9010:9010 \\

  oryd/hydra:v1.0.8 \\

  token user \\

    \--port 9010 \\

    \--auth-url http://127.0.0.1:9000/oauth2/auth \\

    \--token-url http://ory-hydra-example--hydra:4444/oauth2/token \\

    \--client-id another-consumer \\

    \--client-secret consumer-secret \\

    \--scope openid,offline \\

    \--redirect http://127.0.0.1:9010/callback

## Tools

[oauthdebugger.com](http://oauthdebugger.com/)

[oidcdebugger.com](http://oidcdebugger.com/)

[jsonwebtoken.io](http://jsonwebtoken.io/)

## References

[oauth.com](http://oauth.com/)

[openid.net/connect/](http://openid.net/connect/)

[https://www.ory.sh/run-oauth2-server-open-source-api-security/](https://www.ory.sh/run-oauth2-server-open-source-api-security/)

[https://www.youtube.com/watch?v=996OiexHze0](https://www.youtube.com/watch?v=996OiexHze0)  
