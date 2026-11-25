# Etapa 1: gera os arquivos finais do Vue (pasta dist)
FROM node:24-alpine AS compilacao
WORKDIR /codigo
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

# Etapa 2: o Nginx entrega os arquivos prontos para o navegador
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=compilacao /codigo/dist /usr/share/nginx/html
EXPOSE 80
