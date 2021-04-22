#export ASPNETCORE_URLS="http://localhost:5001"

echo 'starting dottrace in background - timeout 100s'
/tools/dottrace/dotTrace.sh start --framework=NetCore /usr/bin/dotnet /app/FriendsApi.Host.dll --timeout=100s --save-to=/app/snapshots/snapshot.dtp &

#/usr/bin/dotnet /app/FriendsApi.Host.dll 
# &
echo 'sleeping 20s... waiting api up'
sleep 20

# echo 'wget friends'
# wget http://127.0.0.1/friends  > /app/snapshots/wget-results-1.txt
# echo 'wget friends end'

echo 'wrk -t1 -c1 -d30s --latency http://127.0.0.1:80/friends'
/tools/wrk/wrk -t1 -c1 -d30s --latency http://127.0.0.1:80/friends  > /app/snapshots/wrk-results-1.txt

# echo 'killing dotnet'
# kill $(ps aux | grep 'dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}')

echo 'sleeping 150s'
sleep 150

# chmod 666 /app/snapshots/*
# chown 1000 /app/snapshots/*

# sleep 10
