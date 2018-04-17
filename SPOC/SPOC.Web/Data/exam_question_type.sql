call proc_add_field('exam_question_type', 'defaultQuestionScore', 'decimal(9,2) not null default 0', ' comment ''默认分数'' after `questionTypeName`');

call proc_add_field('exam_question_type', 'judgeDeviceName', 'varchar(255) default ''''', ' comment ''评分组件名'' after `defaultQuestionScore`');

insert into exam_question_type(id,questionBaseTypeCode,questionTypeName,defaultQuestionScore,judgeDeviceName,listOrder) 
select '16d8bb70-41c5-11e6-b3b7-005056c00008', 'single', '单选题', 0, '', 0 
from dual 
where not exists (select * from exam_question_type where id='16d8bb70-41c5-11e6-b3b7-005056c00008');

insert into exam_question_type(id,questionBaseTypeCode,questionTypeName,defaultQuestionScore,judgeDeviceName,listOrder) 
select '16d8c0a0-41c5-11e6-b3b7-005056c00008', 'multi', '多选题', 0, '', 1
from dual 
where not exists (select * from exam_question_type where id='16d8c0a0-41c5-11e6-b3b7-005056c00008');

insert into exam_question_type(id,questionBaseTypeCode,questionTypeName,defaultQuestionScore,judgeDeviceName,listOrder) 
select '16d8c117-41c5-11e6-b3b7-005056c00008', 'judge', '判断题', 0, '', 2
from dual 
where not exists (select * from exam_question_type where id='16d8c117-41c5-11e6-b3b7-005056c00008');

insert into exam_question_type(id,questionBaseTypeCode,questionTypeName,defaultQuestionScore,judgeDeviceName,listOrder) 
select '16d8c134-41c5-11e6-b3b7-005056c00008', 'fill', '填空题', 0, '', 3
from dual 
where not exists (select * from exam_question_type where id='16d8c134-41c5-11e6-b3b7-005056c00008');

insert into exam_question_type(id,questionBaseTypeCode,questionTypeName,defaultQuestionScore,judgeDeviceName,listOrder) 
select '16d8c41f-41c5-11e6-b3b7-005056c00008', 'program', '编程题', 0, '', 10
from dual 
where not exists (select * from exam_question_type where id='16d8c41f-41c5-11e6-b3b7-005056c00008');

insert into exam_question_type(id,questionBaseTypeCode,questionTypeName,defaultQuestionScore,judgeDeviceName,listOrder) 
select 'a0d97daf-f426-11e7-9611-507b9dbddb3e', 'program_fill', '编程填空题', 0, '', 11
from dual 
where not exists (select * from exam_question_type where id='a0d97daf-f426-11e7-9611-507b9dbddb3e');