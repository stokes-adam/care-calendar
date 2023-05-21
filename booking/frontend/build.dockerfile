FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine

RUN apk update \
    && apk add git \
    && apk add docker-cli \
    && apk add nodejs \
    && apk add npm
    

ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=${GITHUB_TOKEN}
ARG GITHUB_RUN_NUMBER
ENV GITHUB_RUN_NUMBER=${GITHUB_RUN_NUMBER}

RUN echo "${GITHUB_TOKEN}" | docker login ghcr.io -u "ci" --password-stdin

COPY . ./repo/

WORKDIR /repo/wwwroot

RUN npm install
RUN PUBLIC_URL=https://static.microcosmos.live npm run build

WORKDIR /repo
