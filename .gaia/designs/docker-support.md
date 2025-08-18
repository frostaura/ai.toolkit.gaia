[<< Back](../README.md)

# Docker Support
![Docker Badge](https://dockeri.co/image/<DOCKER_USERNAME>/<APPLICATION>)
## Local
The project supports being run as a container and is in fact indended to. In order to run this service locally, simply run `docker-compose up` in the directory where the `docker-compose.yml` file resides. The service will now run.
## Docker Hub
Automated builds are set up for Docker Hub. To use this service without the source code running, use
- `docker pull <DOCKER_USERNAME>/<APPLICATION>` or 
- Visit https://hub.docker.com/repository/docker/<DOCKER_USERNAME>/<APPLICATION>.
### Docker Compose Example
    version: "3"
        services:
            <APPLICATION>:
                image: "<DOCKER_USERNAME>/<APPLICATION>"
            restart: unless-stopped
            container_name: <APPLICATION>
            ports:
                - 8000:9999

## How To
### Getting Started
#### Docker Requirement
- Install Docker Desktop from here: https://www.docker.com/products/docker-desktop
- After installation, ensure that if you're on windows, you switch docker from windows containers to linux containers. This can be done via the Docker icon in the system tray.

[<< Back](../README.md)
