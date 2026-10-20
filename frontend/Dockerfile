FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Ajusta "cambio-front" al nombre de tu proyecto (carpeta dentro de dist/)
COPY --from=build /app/dist/cambio-moneda-front/browser /usr/share/nginx/html
EXPOSE 80