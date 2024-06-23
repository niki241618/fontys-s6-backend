@echo off
setlocal

rem Define the directory containing the Dockerfiles
set "DOCKERFILE_DIR=."

rem Docker Hub username
set "DOCKERHUB_USERNAME=niki241618"

set "GKE_PROJ=europe-west4-docker.pkg.dev/astral-option-427116-t2/audio-oasis"

rem Build Docker images
docker build -t %GKE_PROJ%/book-info-service -f %DOCKERFILE_DIR%\Dockerfile.bookinfo %DOCKERFILE_DIR%
docker build -t %GKE_PROJ%/book-streaming-service -f %DOCKERFILE_DIR%\Dockerfile.bookstream %DOCKERFILE_DIR%
docker build -t %GKE_PROJ%/logging-service -f %DOCKERFILE_DIR%\Dockerfile.logger %DOCKERFILE_DIR%
docker build -t %GKE_PROJ%/users-service -f %DOCKERFILE_DIR%\Dockerfile.usersservice %DOCKERFILE_DIR%

rem Ask user if they want to upload images to Docker Hub
set /p answer="Would you want to upload the images to Docker Hub? (y/n): "
if /I "%answer%"=="y" (
    docker push %GKE_PROJ%/book-info-service
    docker push %GKE_PROJ%/book-streaming-service
    docker push %GKE_PROJ%/logging-service
    docker push %GKE_PROJ%/users-service
) else (
    echo Skipping GKE upload.
)

echo All Docker images have been processed.
endlocal
