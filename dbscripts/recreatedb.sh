#!/bin/bash

DBFILE="secrets.db"
SCRIPTFILE="secrets.sql"

if [ ! -f "./$SCRIPTFILE" ]; then
    echo "./$SCRIPTFILE does not exist."
    exit 0
fi

remove_file_if_exists () {    
    if [ -f "$1" ]; then
	rm $1
    fi
}

OLD="../SecretAPI/Db/$DBFILE"
remove_file_if_exists $OLD

NEW="./$DBFILE"
remove_file_if_exists $NEW


cat $SCRIPTFILE | sqlite3 $DBFILE
mv $DBFILE $OLD
