FROM alpine/git as git
WORKDIR /app
RUN  git clone https://github.com/midianok/MultiLoader.git

FROM microsoft/dotnet:2.2-sdk-alpine as build
WORKDIR /app
COPY --from=git /app .
RUN dotnet restore ./MultiLoader/MultiLoader.TelegramFacade
RUN dotnet publish ./MultiLoader/MultiLoader.TelegramFacade -c Release -o out

FROM microsoft/dotnet:2.2-runtime-alpine
WORKDIR /app
COPY --from=build /app/MultiLoader/MultiLoader.TelegramFacade/out .
COPY appsettings.json .

CMD ["ping", "ya.ru"]