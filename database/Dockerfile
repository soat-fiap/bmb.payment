# Use the official MySQL image as a base
FROM mysql:8.3.0

# Add our init.sql script to the Docker image
ADD init.sql /docker-entrypoint-initdb.d/

# NOTE: use of "mysql_native_password" is not recommended: https://dev.mysql.com/doc/refman/8.0/en/upgrading-from-previous-series.html#upgrade-caching-sha2-password
# (this is just an example, not intended to be a production configuration)
CMD ["mysqld", "--mysql-native-password=ON"]
