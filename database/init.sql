create table IF NOT EXISTS Payments
(
    Id         char(36)   not null,
    OrderId    char(36)   not null,
    Status     int        not null,
    Created    datetime   null,
    Updated    datetime   null,
    PaymentType int       not null,
    ExternalReference     varchar(36) not null,
    Amount     decimal(10,2) not null,
    PRIMARY KEY (Id, OrderId)
);
