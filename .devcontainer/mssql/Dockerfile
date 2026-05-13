FROM mcr.microsoft.com/azure-sql-edge

USER root
# Install wget for the backup download in entrypoint.sh
# mssql-tools18 is not available for arm64 on Ubuntu 18.04;
# use go-sqlcmd instead, which has ARM64 support.
RUN sed -i 's|http://archive.ubuntu.com/ubuntu|http://old-releases.ubuntu.com/ubuntu|g' /etc/apt/sources.list \
    && sed -i 's|http://security.ubuntu.com/ubuntu|http://old-releases.ubuntu.com/ubuntu|g' /etc/apt/sources.list \
    && apt-get update \
    && apt-get install -y wget curl \
    && rm -rf /var/lib/apt/lists/* \
    && curl -fsSL https://github.com/microsoft/go-sqlcmd/releases/download/v1.8.0/sqlcmd-linux-arm64.tar.bz2 \
       | tar -xj -C /usr/local/bin sqlcmd \
    && chmod +x /usr/local/bin/sqlcmd

COPY entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

# Stay as root so entrypoint can fix data directory ownership before starting SQL Server
ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
