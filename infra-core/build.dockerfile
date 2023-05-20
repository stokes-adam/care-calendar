FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine

RUN apk update \
    && apk add git \
    && apk add docker-cli


ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=${GITHUB_TOKEN}
ARG GITHUB_RUN_NUMBER
ENV GITHUB_RUN_NUMBER=${GITHUB_RUN_NUMBER}

# using password-stdin to avoid exposing the token in the build log
RUN echo "${GITHUB_TOKEN}" | docker login ghcr.io -u "ci" --password-stdin

COPY . ./repo/

WORKDIR /repo
