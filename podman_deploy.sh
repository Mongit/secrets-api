#!/bin/bash

CONTAINER_NAME="secrets-api"
IMAGE_NAME="secrets-api-img"
PROJ_DIR="SecretsAPI"
CONTAINER_FILE="./Containerfile"
DB_PATH="./$PROJ_DIR/Db"
CONT_DB_PATH="/app/Db"
PORT="5000:5000"

# Get latest
echo "--> Updating git repository"
git pull

## Stop if running 
if podman container exists $CONTAINER_NAME; then
    echo "--> Stopping and removing container: $CONTAINER_NAME"
    podman stop $CONTAINER_NAME
    podman rm $CONTAINER_NAME
fi

if podman image exists $IMAGE_NAME; then
    echo "--> Removing image: $IMAGE_NAME"
    podman rmi $IMAGE_NAME
fi

## Build image
podman build -f $CONTAINER_FILE -t $IMAGE_NAME

## Run
podman run -e JWT_TOKEN -v $DB_PATH:$CONT_DB_PATH -d --name $CONTAINER_NAME -p $PORT $IMAGE_NAME

podman ps
