#!/bin/bash
set -e

BACKUP_DIR=/var/opt/mssql/backups
BACKUP_FILE=$BACKUP_DIR/AdventureWorksLT2019.bak
# go-sqlcmd is installed at /usr/local/bin/sqlcmd
SQLCMD=/usr/local/bin/sqlcmd

# Fix ownership so the mssql user (UID 10001) can write to the mounted volumes
chown -R 10001:0 /var/opt/mssql

# Start SQL Server in the background (drops to mssql user internally)
/opt/mssql/bin/sqlservr &
SQL_PID=$!

# Wait for SQL Server to accept TCP connections (up to 60 seconds)
echo "[init] Waiting for SQL Server to start..."
for i in $(seq 1 12); do
  if timeout 1 bash -c 'echo > /dev/tcp/localhost/1433' 2>/dev/null; then
    # Port open — wait a moment for login to be ready, then verify with sqlcmd
    sleep 2
    if $SQLCMD -S localhost -U SA -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" &>/dev/null; then
      echo "[init] SQL Server is ready."
      break
    fi
  fi
  echo "[init] Attempt $i/12 — sleeping 5s..."
  sleep 5
done

# Check if the database already exists (persisted on the volume)
DB_EXISTS=$($SQLCMD -S localhost -U SA -P "$MSSQL_SA_PASSWORD" -C \
  -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = 'AdventureWorksLT2019'" \
  -h-1 2>/dev/null | grep -m1 '[0-9]' | tr -d ' \r')

# Treat empty result (sqlcmd failed to connect) same as not-found
if [ "$DB_EXISTS" != "1" ]; then
  echo "[init] AdventureWorksLT2019 not found — starting restore..."

  if [ ! -f "$BACKUP_FILE" ]; then
    echo "[init] Downloading AdventureWorksLT2019.bak..."
    mkdir -p "$BACKUP_DIR"
    wget -q --show-progress \
      "https://github.com/Microsoft/sql-server-samples/releases/download/adventureworks/AdventureWorksLT2019.bak" \
      -O "$BACKUP_FILE"
  fi

  echo "[init] Restoring database..."
  $SQLCMD -S localhost -U SA -P "$MSSQL_SA_PASSWORD" -C -Q "
RESTORE DATABASE AdventureWorksLT2019
FROM DISK = N'$BACKUP_FILE'
WITH MOVE 'AdventureWorksLT2019_Data' TO '/var/opt/mssql/data/AdventureWorksLT2019.mdf',
     MOVE 'AdventureWorksLT2019_Log'  TO '/var/opt/mssql/data/AdventureWorksLT2019_log.ldf',
     REPLACE
"
  echo "[init] Database restored successfully."
else
  echo "[init] AdventureWorksLT2019 already exists — skipping restore."
fi

# Keep the container alive by waiting on the SQL Server process
wait $SQL_PID
