create user master1@'%' identified with mysql_native_password by 'root';
grant replication slave on *.* to master1@'%' with grant option;
flush privileges;