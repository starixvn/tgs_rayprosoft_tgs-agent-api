FROM node:14.15.1-stretch-slim AS build

WORKDIR /dest

# Copy package.json and package-lock.json
COPY ./src/Web/backoffice/package* ./
# Copy snowpack env file
COPY ./src/Web/backoffice/snowpack.env ./.env

# Install packages
RUN npm install

# Copy source code
COPY ./src/Web/backoffice/ .

# TODO: RUN npm run test
RUN npm run build

# Copy build to caddy server
FROM caddy:2.1.1-alpine

# Debug Tool
# RUN apk --no-cache add curl

COPY --from=build /dest/caddy/Caddyfile /etc/caddy/Caddyfile
COPY --from=build /dest/build /usr/share/caddy

# HEALTHCHECK --start-period=5s\
#     CMD curl -fs http://localhost/healthcheck || exit 1


# Start caddy server
# CMD caddy run --config /etc/caddy/Caddyfile
# ENTRYPOINT ["caddy", "run", "--config", "/etc/caddy/Caddyfile"]
# EXPOSE 80