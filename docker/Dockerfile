FROM drupal:10-apache

RUN apt-get update && apt-get install -y \
    git \
    unzip \
    libpng-dev \
    && docker-php-ext-install pdo pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*

# Install Composer
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

WORKDIR /var/www/html

# Copy composer files
COPY composer.json composer.lock* ./

RUN composer install --no-dev --optimize-autoloader 2>/dev/null || true

# Copy module
COPY src/modules/pie_shop web/modules/custom/pie_shop

# Apache config
RUN a2enmod rewrite

EXPOSE 80
