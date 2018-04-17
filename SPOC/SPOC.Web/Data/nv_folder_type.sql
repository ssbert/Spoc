insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select 'ea0dd552-44e0-11e6-ac47-d4ae528cba72','course','课程类型','1',null 
from dual 
where not exists (select * from nv_folder_type where id='ea0dd552-44e0-11e6-ac47-d4ae528cba72');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select '0ff502c5-36b1-11e6-b1d0-d4ae528cba72','question_bank','题库类型','2',null 
from dual 
where not exists (select * from nv_folder_type where id='0ff502c5-36b1-11e6-b1d0-d4ae528cba72');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select '0ff8bbb3-36b1-11e6-b1d0-d4ae528cba72','exam_paper','试卷类型','3',null 
from dual 
where not exists (select * from nv_folder_type where id='0ff8bbb3-36b1-11e6-b1d0-d4ae528cba72');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select '14bf0454-2d30-45a3-8808-bfd8fdf06f54','exercisesBank','子题库类型','4',null 
from dual 
where not exists (select * from nv_folder_type where id='14bf0454-2d30-45a3-8808-bfd8fdf06f54');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select 'df21b93f-5878-4c0c-a868-82ac943573ee','exercisesCourse','题库类型','5',null 
from dual 
where not exists (select * from nv_folder_type where id='df21b93f-5878-4c0c-a868-82ac943573ee');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select 'bf8fce61-c94e-469e-81ec-b334b681f0f5','examCourse','考试类型','6',null 
from dual 
where not exists (select * from nv_folder_type where id='bf8fce61-c94e-469e-81ec-b334b681f0f5');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select '22c4a9f9-1a94-11e7-82d3-d4ae528cba72','live','直播类型','7',null 
from dual 
where not exists (select * from nv_folder_type where id='22c4a9f9-1a94-11e7-82d3-d4ae528cba72');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select '7c4ddacc-ddb5-4646-88cb-d8f9fc793dd4','courseware','课件类型','8',null 
from dual 
where not exists (select * from nv_folder_type where id='7c4ddacc-ddb5-4646-88cb-d8f9fc793dd4');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select '55e282c9-d195-11e7-9350-3497f653649d','challenge_cpp','C++挑战题库','9',null 
from dual 
where not exists (select * from nv_folder_type where id='55e282c9-d195-11e7-9350-3497f653649d');


-- 改名
update nv_folder_type set folderTypeName='试题类型' where id='0ff502c5-36b1-11e6-b1d0-d4ae528cba72' and folderTypeName='题库类型';
update nv_folder_type set folderTypeName='子题库类型' where id='14bf0454-2d30-45a3-8808-bfd8fdf06f54' and folderTypeName='练习题库类型';

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select 'd3a56414-fb39-11e7-bb36-3497f653649d','lib_label','知识点管理','9',null 
from dual 
where not exists (select * from nv_folder_type where id='d3a56414-fb39-11e7-bb36-3497f653649d');

insert into nv_folder_type(id,folderTypeCode,folderTypeName,listOrder,remarks) 
select '7d56b9ac-33ea-11e8-905f-7824af8c98ea','faq','Faq帮助手册','10',null 
from dual 
where not exists (select * from nv_folder_type where id='7d56b9ac-33ea-11e8-905f-7824af8c98ea');

