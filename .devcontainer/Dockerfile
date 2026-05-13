FROM mcr.microsoft.com/devcontainers/dotnet:1-10.0-noble

# Remove the Yarn apt repository whose GPG key (62D54FD4003F6525) is no longer
# valid. Its presence causes `apt-get update` to fail with a non-zero exit code,
# which in turn breaks the docker-outside-of-docker feature install.
RUN rm -f /etc/apt/sources.list.d/yarn.list \
    && apt-get clean
