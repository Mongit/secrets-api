#!/bin/bash

DBFILE="secrets.db"
SCRIPTFILE="secrets.sql"
PROJ_DIR="SecretsAPI"

if [ ! -f "./$SCRIPTFILE" ]; then
    echo "./$SCRIPTFILE does not exist."
    exit 0
fi

remove_file_if_exists () {    
    if [ -f "$1" ]; then
	    rm $1
    fi
}

OLD="../$PROJ_DIR/Db/$DBFILE"
remove_file_if_exists $OLD

remove_file_if_exists "./$DBFILE"


cat $SCRIPTFILE | sqlite3 $DBFILE
mv $DBFILE $OLD