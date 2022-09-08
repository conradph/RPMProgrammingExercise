USE RPMProgrammingExercise;

drop table GasPrices;

CREATE TABLE GasPrices(
	recordDate varchar(8),
	price float
)


select * from GasPrices;

insert into GasPrices
values ('20220808', 6.99);