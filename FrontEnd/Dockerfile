FROM node:14 AS build
WORKDIR /FrontEnd
COPY . .
RUN npm cache verify && npm install && npm run build

FROM httpd:2.4.48
COPY --from=build /FrontEnd/dist/ /usr/local/apache2/htdocs/
# COPY ./dist/ /usr/local/apache2/htdocs/
COPY ./keycloak.json /usr/local/apache2/htdocs/
COPY ./my-httpd.conf /usr/local/apache2/conf/httpd.conf