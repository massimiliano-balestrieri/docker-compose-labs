#!/bin/sh

# redirect stdout and stderr to files
exec >/results/results.txt
exec 2>/results/results.txt

# now run the requested CMD without forking a subprocess
exec "$@"