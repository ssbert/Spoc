call proc_add_field('nv_folder', 'fullPath', 'varchar(8000) not null default ''''', ' comment ''类型节点全路径'' after `parentUid`');
call proc_add_field('nv_folder', 'isCustomCode', 'tinyint(1) not null default 1', ' comment ''是否是自定义编号'' after `folderCode`');

update nv_folder set parentUid = '00000000-0000-0000-0000-000000000000' where parentUid = '' or parentUid is null;
call proc_update_field('nv_folder', 'parentUid', 'char(36) not null default ''00000000-0000-0000-0000-000000000000''', ' comment ''父级ID'' after `id`');

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select '58e40f38-44e2-11e6-ac47-d4ae528cba72','00000000-0000-0000-0000-000000000000','58e40f38-44e2-11e6-ac47-d4ae528cba72','T00003','0','course','课程分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='58e40f38-44e2-11e6-ac47-d4ae528cba72');

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select 'ec9dca61-3777-11e6-b1d0-d4ae528cba72','00000000-0000-0000-0000-000000000000','ec9dca61-3777-11e6-b1d0-d4ae528cba72','T00001','0','question_bank','试题分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='ec9dca61-3777-11e6-b1d0-d4ae528cba72');

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select 'ec9fa115-3777-11e6-b1d0-d4ae528cba72','00000000-0000-0000-0000-000000000000','ec9fa115-3777-11e6-b1d0-d4ae528cba72','T00002','0','exam_paper','试卷分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='ec9fa115-3777-11e6-b1d0-d4ae528cba72');

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select 'c70d7dd4-f6e9-4185-99db-7e50aaccb9e1','00000000-0000-0000-0000-000000000000','c70d7dd4-f6e9-4185-99db-7e50aaccb9e1','T00004','0','exercisesCourse','题库分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='c70d7dd4-f6e9-4185-99db-7e50aaccb9e1');

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select 'ebf35e27-11c3-43a1-a4fa-bbd42767ad3c','00000000-0000-0000-0000-000000000000','ebf35e27-11c3-43a1-a4fa-bbd42767ad3c','T00005','0','examCourse','考试分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='ebf35e27-11c3-43a1-a4fa-bbd42767ad3c');

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select '652d03c5-1a94-11e7-82d3-d4ae528cba72','00000000-0000-0000-0000-000000000000','652d03c5-1a94-11e7-82d3-d4ae528cba72','T00007','0','live','直播分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='652d03c5-1a94-11e7-82d3-d4ae528cba72');

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select '24ac05fd-be8f-48b0-8b76-d7d3df192d02','00000000-0000-0000-0000-000000000000','24ac05fd-be8f-48b0-8b76-d7d3df192d02','T00008','0','courseware','课件分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='24ac05fd-be8f-48b0-8b76-d7d3df192d02');


insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select '8a8d98f4-d195-11e7-9350-3497f653649d','00000000-0000-0000-0000-000000000000','8a8d98f4-d195-11e7-9350-3497f653649d','T00001','0','challenge_cpp','挑战分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='8a8d98f4-d195-11e7-9350-3497f653649d');


-- 改名
update nv_folder set folderName='试题分类' where id='ec9dca61-3777-11e6-b1d0-d4ae528cba72' and folderName='题库分类';
update nv_folder set folderName='子题库分类' where id='cd5895af-1c47-4bcf-baf2-1c8319f2b556' and folderName='练习题库分类';

-- 删除子题库分类
delete from nv_folder where id='cd5895af-1c47-4bcf-baf2-1c8319f2b556';

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select 'fb65e482-fb39-11e7-bb36-3497f653649d','00000000-0000-0000-0000-000000000000','fb65e482-fb39-11e7-bb36-3497f653649d','T00001','0','lib_label','知识点分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='fb65e482-fb39-11e7-bb36-3497f653649d');

insert into nv_folder(id,parentUid,fullPath,folderCode,isCustomCode,folderTypeCode,folderName,folderLevel,listOrder,hasChild,remarks,creatorUid,createTime) 
select '96e0537e-33ea-11e8-905f-7824af8c98ea','00000000-0000-0000-0000-000000000000','96e0537e-33ea-11e8-905f-7824af8c98ea','T00001','0','faq','Faq分类',0,1,'N','','426c515b-4fe4-11e6-83a1-00ffa2a6b6c2',now() 
from dual 
where not exists (select * from nv_folder where id='96e0537e-33ea-11e8-905f-7824af8c98ea');
