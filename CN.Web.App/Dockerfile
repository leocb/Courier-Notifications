FROM nginx:latest
# Copy publish files to image
COPY ./publish/wwwroot /usr/share/nginx/html
# Delete local appsettings (users provide their own)
RUN rm /usr/share/nginx/html/appsettings.json
# Nginx configuration
COPY ./nginx.conf /etc/nginx/conf.d/default.conf