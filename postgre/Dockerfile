FROM postgis/postgis:16-3.4 as builder
RUN apt update
RUN apt install -y postgis
RUN which shp2pgsql

FROM postgis/postgis:16-3.4

COPY --from=builder /usr/bin/shp2pgsql /usr/bin/shp2pgsql

COPY ./shp /my/data

ARG TARGET_ROADS
RUN shp2pgsql -I -s 4326 -W "latin1" /my/data/${TARGET_ROADS} Regions > /docker-entrypoint-initdb.d/roads.sql