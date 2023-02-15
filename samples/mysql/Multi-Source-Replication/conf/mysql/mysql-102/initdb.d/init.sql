create user master2@'%' identified with mysql_native_password by 'root';
grant replication slave on *.* to master2@'%' with grant option;
flush privileges;