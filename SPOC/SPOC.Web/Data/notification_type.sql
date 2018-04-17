
insert into notification_type(id,code,`name`) 
select 'b407358a-d0eb-11e7-95a3-507b9dbddb3e','system','系统'
from dual 
where not exists (select * from notification_type where id='b407358a-d0eb-11e7-95a3-507b9dbddb3e');
insert into notification_type(id,code,`name`) 
select 'b4073dbb-d0eb-11e7-95a3-507b9dbddb3e','exam','考试'
from dual 
where not exists (select * from notification_type where id='b4073dbb-d0eb-11e7-95a3-507b9dbddb3e');
insert into notification_type(id,code,`name`) 
select 'b4073e42-d0eb-11e7-95a3-507b9dbddb3e','exercise','练习'
from dual 
where not exists (select * from notification_type where id='b4073e42-d0eb-11e7-95a3-507b9dbddb3e');
insert into notification_type(id,code,`name`) 
select 'b4073e61-d0eb-11e7-95a3-507b9dbddb3e','announcement','公告'
from dual 
where not exists (select * from notification_type where id='b4073e61-d0eb-11e7-95a3-507b9dbddb3e');
