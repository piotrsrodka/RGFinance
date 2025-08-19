# Build stage
FROM node:16-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci --legacy-peer-deps

# Copy source and build
COPY . .
RUN npm run build

# Serve stage with nginx
FROM nginx:alpine
COPY --from=build /app/dist/* /usr/share/nginx/html/
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80