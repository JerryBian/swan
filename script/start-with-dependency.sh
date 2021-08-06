#!/bin/sh

# Abort on any error (including if wait-for-it fails).
set -e

# Wait for the backend to be up, if we know where it is.
if [ -n "$ENV_API_LOCAL_ENDPOINT" ]; then
  /app/wait-for-it.sh "$ENV_API_LOCAL_ENDPOINT:${ENV_API_PORT:-5010}"
fi

# Run the main container command.
exec "$@"