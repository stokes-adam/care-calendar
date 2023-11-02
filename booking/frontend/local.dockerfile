FROM node:18 AS build

WORKDIR /app

COPY wwwroot/package*.json ./

RUN npm install

COPY wwwroot/ ./

RUN npm run build

FROM node:18

WORKDIR /app

COPY --from=build /app/build ./build

RUN npm install -g serve

EXPOSE 5000

CMD ["serve", "-s", "build", "-l", "5000"]
