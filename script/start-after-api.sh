#!/bin/sh

# Abort on any error (including if wait-for-it fails).
set -e

# Wait for the backend to be up, if we know where it is.
/app/wait-for-it.sh api:5010 -t 300 --strict -- echo "api is up"

# Run the main container command.
exec "$@"