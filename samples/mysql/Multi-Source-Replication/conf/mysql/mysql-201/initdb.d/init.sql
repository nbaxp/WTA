create user report@'%' identified with mysql_native_password by 'readonly';
grant select,show view on *.* to report@'%' with grant option;
flush privileges;

CHANGE MASTER TO MASTER_HOST='mysql-101',
MASTER_USER='master1',
MASTER_PORT=3306,
MASTER_PASSWORD='root',
MASTER_AUTO_POSITION=1 FOR CHANNEL 'master1';

CHANGE MASTER TO MASTER_HOST='mysql-102',
MASTER_USER='master2',
MASTER_PORT=3306,
MASTER_PASSWORD='root',
MASTER_AUTO_POSITION=1 FOR CHANNEL 'master2';

--show slave status\G;