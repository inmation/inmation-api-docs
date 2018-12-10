# NGINX Plus for system:inmation

## 1. Introduction

NGINX is a software for a *web serving*, *reverse proxying*, *caching*, *load balancing*, *media streaming* and more. As a web Server NGINX was created for a fast performance, originally released for a simple HTML pages now supports all the components of the modern Web, including WebSocket, HTTP/2 and streaming of multiple video formats. NGINX outperforms other servers in benchmarks based on the web server performance measurements, including Apache.\
Though NGINX became famous as the fastest web server, the scalable underlying architecture has proved ideal for many web tasks beyond serving content. Because it can handle a high volume of connections, NGINX is commonly used as a reverse proxy and load balancer to manage incoming traffic and to distribute it to slower upstream servers.
NGINX offers the (Open Source Software) `OSS` and the commercial `NGINX Plus` software versions, in the table below most important differences are highlighted.

## 2. Benefits of using NGINX Plus with system:inmation

| Features                                                         | `OSS`                 | `NGINX Plus` |
| ---------------------------------------------------------------- |:--------------------:| ------------:|
| NGINX PlusHTTP/TCP/UDP load balancer                             | Yes                  | Yes          |
| Layer 7 request routing                                          | Yes                  | Yes          |
| `Active health checks`                                           |                      | Yes          |
| DNS service‑discovery integration                                |                      | Yes          |
| Security controls                                                |                      | Yes          |
| JWT authentication/OpenID Connect SSO                            |                      | Yes          |
| NGINX Web Application Firewall (additional cost)                 |                      | Yes          |
| Activity monitoring                                              | Yes                  | Yes          |
| `Extended status with 90 additional metrics`                     |                      | Yes          |
| Active‑active, active‑passive HA with config sync, state sharing |                      | Yes          |
| `NGINX Plus API for dynamic reconfiguration`                     |                      | Yes          |

NGINX Plus is a powerful load balancer, web server, and content cache build on top of the open source NGINX. The **biggest differences** between the `NGINX OSS` and `NGINX Plus` for **inmation customers** are:

* `Active health checks`. The open source software performs basic checks on responses from the upstream servers, Plus version adds out-of-band health checks and adds the new and recovered servers into the load-balanced group.
* `Extended status with 90 metrics`. NGINX Plus enables live monitoring of 90 data points, displayed in a dashboard. OSS version is capable of returning 7 basic data points.
* `Dynamic reconfiguration`. Dynamic reconfiguration allows to manage servers - turn off, add, remove and switch between servers. This feature eliminates downtime during upgrades and patch fixes.

While NGINX open source version would be possible to use with inmation Web API it would require extra configuration. Errors might occur due to the absents of the active health checks and the high amount of new version releases in the NGINX OSS are highly labor intensive.

### 2.1 Component Overview

The high availability load balancing provided by NGINX Plus for system:inmation consists of multiple components as listed below.

* **NGINX for inmation** this is a partially pre-configured docker image for running an inmation compatible NGINX Plus server.
* **NGINX metrics import** this lua script imports the data provided by the NGINX status API back into system:inmation.

#### 2.3.1 NGINX for inmation

*NGINX for inmation* is a pre-configured docker image for running an instance of NGINX Plus together with a standard config file that works together with the *Web API Service* and the *NGINX metrics import*.

#### 2.3.2 NGINX metrics import

The Lua 'nginx-metrics' script can be used to import various NGINX metrics into system:inmation. NGINX's API offers the ability to retrieve information about the server's statuses. More information about the metrics can be found [here](./metrics/README.md).

## 3. Installation Guide

NGINX for system:inmation consists of two components in the following chapters we will walk through the requirements and installation of the *NGINX for inmation docker* and the *NGINX metrics import*.

### 3.1 NGINX for inmation docker

#### 3.1.1 Requirements

The minimum requirements for installing *NGINX for inmation* are listed below.

* Docker ( > 18 )
* NGINX Plus license

#### 3.1.2 Installation

Download the 'inmation-nginx' folder from the [inmation-nginx](https://github.com/inmation/inmation-nginx/tree/master/README.md) git repository on your docker host.

```bash
git clone https://github.com/inmation/inmation-nginx.git
```

Now move your working directory into the 'inmation-nginx' folder

```bash
cd 'inmation-nginx'
```

while in the correct folder give execute permission to the bash files.

```bash
chmod +x *.sh
```

and place your own NGINX Plus license ('nginx-repo.crt' and 'nginx-repo.key')  in this folder.

after this is complete you can run the build.sh file to add the docker image to your system.

```:card_index:
./build.sh
```

after docker finished building your image you are ready to continue to the configuration of your system.

#### 3.1.3 Configuration

This chapter will explain how to configure different parts  of NGINX for inmation

##### 3.1.3.1 backend servers

To configure your *NGINX for inmation* install edit the config file located at:

```:bust_in_silhouette:
/NGINX for inmation/conf.d/inmation.conf
```

Inside this config you can add your backend servers under the *upstream* directions.

##### 3.1.3.2 NGINX Metrics password

To change the user accounts capable of reaching the NGINX metrics panel open the following file.

```:bust_in_silhouette:
/NGINX for inmation/conf.d/api_users
```

And change or add your users to the file.

### 3.2 NGINX metrics import

#### 3.2.1 Requirements

This script depends on the following scripts and versions.

* [esi-lcurl-http-client](https://github.com/inmation/inmation-ESI/tree/master/lib/http/lib) ( > 0.1.1 20181029.2 )

#### 3.2.2 Installation

TODO: Link to documentation on installing an ESI script.

#### 3.2.3 Configuration

To use this script, two variables must be provided by the user.

* The URL at which the NGINX API is exposed. This should have the format: website/api/version. The URL is to be stored in the variable "url"

```cfg
  "http://username:password@example.com/api/3"
```

* The root path at which the datastudio directories should be created. The user should create a folder and pass the full path to the script in the variable "base"

```cfg
  "/System/Core/LocalConnector/nginxMetrics"
```

Here the folder "nginxMetrics" was created, and it's path passed to the script.
