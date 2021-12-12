eval $(minikube docker-env)
export APP_VERSION=$(TZ=Europe/Copenhagen date "+%y%m%d%H%M%S")
export BUILD_TAG=app:1.0.$APP_VERSION
docker build . -t $BUILD_TAG -t app:latest --build-arg APP_VERSION=$APP_VERSION