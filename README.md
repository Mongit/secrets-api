# secretapi
api designed to store secret string content

## Install
In order to install SQLite library uncomment
nugget.config. To install the rest of the liraries
add standard nugget.org repo.

## dep-raspi folder
It is built from source specific for raspberry pi os-64
https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
download
https://system.data.sqlite.org/downloads/1.0.115.5/sqlite-netFx-source-1.0.115.5.zip
``` bash
sudo apt-get update
sudo apt-get install build-essential
cd <source root>/Setup
chmod +x compile-interop-assembly-release.sh
./compile-interop-assembly-release.sh
```
Now, you will have a freshly built library file called libSQLite.Interop.so in the <source root>/bin/2013/Release/bin directory. This file might have execution permission which isn’t relevant for a library, so remove it by
chmod -x <source root>/bin/2013/Release/bin/libSQLite.Interop.so
Copy libSQLite.Interop.so the directory where your application’s binaries reside (not the x64 or x86 subdirectories containing SQLite.Interop.dll), and you’re set to go.


## Token
When trying token from https://localhost:7070/swagger
url paste token as


bearer {token}

## Curl
If you are testing localhost with https you can
use -k in curl so that ignores self signed ssl certs

## podman
``` bash
podman build -f Containerfile -t secret-api-img
podman images
podman run -d --name secret-api -p 5000:5000 secret-api-img
podman ps
podman logs secret-api
podman exec -it secret-api bash
```
## clean up
``` bash
podman stop secret-api && \
podman rm secret-api && \
podman rmi secret-api-img
```

## run
``` bash
podman build -f Containerfile -t secret-api-img && podman run -e JWT_TOKEN -v $(pwd)/SecretAPI/Db:/app/Db -d --name secret-api -p 5000:5000 secret-api-img
```

## User & Groups
``` bash
id
id -u $USER
id -g $USER
```

## Usernamespaces
``` bash
lsns -t user
```

