create database dev_training;
;
create table if not exists users
(
    id        int primary key unique,
    user_name varchar(50) not null,
    email     varchar(50),
    password  varchar(20) not null,
    address   varchar(20),
    rec_date  timestamp   not null default now(),
    status    int         not null default 1
    );

select *
from users;