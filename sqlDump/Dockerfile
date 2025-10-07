FROM mysql:8.0

ENV MYSQL_USER=mrs_root
ENV MYSQL_PASSWORD=toor
ENV MYSQL_ROOT_PASSWORD=toor
ENV MYSQL_DATABASE=mrs

COPY ./mrs.sql /docker-entrypoint-initdb.d/