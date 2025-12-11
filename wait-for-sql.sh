#!/bin/sh
echo "Aguardando SQL Server na porta 1433..."

while ! nc -z sqlserver 1433; do
    echo "SQL Server ainda iniciando..."
    sleep 3
done

echo "SQL Server pronto!"
exec "$@"