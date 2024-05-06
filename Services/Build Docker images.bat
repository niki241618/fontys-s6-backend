@echo off
setlocal

rem Define the directory containing the Dockerfiles
set "DOCKERFILE_DIR=."

rem Docker Hub username
set "DOCKERHUB_USERNAME=niki241618"

rem Build Docker images
docker build -t %DOCKERHUB_USERNAME%/book-info-service -f %DOCKERFILE_DIR%\Dockerfile.bookinfo %DOCKERFILE_DIR%
docker build -t %DOCKERHUB_USERNAME%/book-streaming-service -f %DOCKERFILE_DIR%\Dockerfile.bookstream %DOCKERFILE_DIR%
docker build -t %DOCKERHUB_USERNAME%/logging-service -f %DOCKERFILE_DIR%\Dockerfile.logger %DOCKERFILE_DIR%

rem Ask user if they want to upload images to Docker Hub
set /p answer="Would you want to upload the images to Docker Hub? (y/n): "
if /I "%answer%"=="y" (
    echo Please login to Docker Hub
    docker login --username %DOCKERHUB_USERNAME%
    docker push %DOCKERHUB_USERNAME%/book-info-service
    docker push %DOCKERHUB_USERNAME%/book-streaming-service
    docker push %DOCKERHUB_USERNAME%/logging-service
) else (
    echo Skipping Docker Hub upload.
)

echo All Docker images have been processed.
endlocal
