drop procedure if exists `excuteproc`;
create procedure  `excuteproc`(sqltext varchar(2000))
	comment '执行存储过程'
begin
	set @temp=sqltext;
	prepare stmt from @temp;
	execute stmt;
end;

drop procedure if exists `del_idx`;
create procedure  `del_idx`(IN p_tablename varchar(50), IN p_idxname varchar(50))
	comment '删除索引'
begin
	set @schema_name = database();	
	set @str=concat('drop index ',p_idxname,' on ', @schema_name, '.', p_tablename, ';');
	select count(*) into @cnt from information_schema.statistics where TABLE_SCHEMA=@schema_name and table_name=p_tablename and index_name=p_idxname;

	if @cnt > 0 then
		call excuteproc(@str);
	end if;
end; 

drop procedure if exists `del_FK`;
create procedure `del_FK`(IN p_table_name varchar(50), IN p_column_name varchar(50))
	comment '根据表名和字段名删除外键'
begin
	set @schema_name = database();
	select count(*) into @cnt from information_schema.key_column_usage where constraint_schema=@schema_name and table_name=p_table_name and column_name=p_column_name;
	
	if @cnt > 0 then
		select constraint_name into @fk_name from information_schema.key_column_usage where constraint_schema=@schema_name and table_name=p_table_name and column_name=p_column_name;
		set @str=concat('alter table ', @schema_name, '.', p_table_name, ' drop foreign key ', @fk_name, ';');
		call excuteproc(@str);
	end if;
end; 

drop procedure if exists `proc_add_field`;
create procedure `proc_add_field`(in table_name varchar(50),in column_name varchar(50),in column_type varchar(100),in column_other varchar(100))
	comment '新增字段'
begin
	set @schema_name = database();
	set @str=concat('select count(0) into @t_count from information_schema.columns where table_schema=''',@schema_name,''' and table_name=''',table_name,''' and column_name=''',column_name,'''');
	call excuteproc(@str);
	if (@t_count=0) then
		set @str=concat('alter table `',@schema_name,'`.`',table_name,'` add column `',column_name,'` ',column_type,' ',column_other);
		call excuteproc(@str);
	end if;
end;

drop procedure if exists `proc_update_field`;
create  procedure `proc_update_field`(in table_name varchar(50),in column_name varchar(50),in column_type varchar(100),in column_other varchar(100))
    comment '修改字段'
begin
	set @schema_name = database();
	set @str=concat('select count(0) into @t_count from information_schema.columns where table_schema=''',@schema_name,''' and table_name=''',table_name,''' and column_name=''',column_name,'''');
	call excuteproc(@str);
	if (@t_count=1) then
		set @str=concat('alter table `',@schema_name,'`.`',table_name,'` modify column `',column_name,'` ',column_type,' ',column_other);
		call excuteproc(@str);
	end if;
end;

drop procedure if exists `proc_update_column_name`;
create  procedure `proc_update_column_name`(in table_name varchar(50),in column_name varchar(50), in new_column_name varchar(50),in column_type varchar(100))
    comment '修改字段名称'
begin
	set @schema_name = database();
	set @str=concat('select count(0) into @t_count from information_schema.columns where table_schema=''',@schema_name,''' and table_name=''',table_name,''' and column_name=''',column_name,'''');
	call excuteproc(@str);
	if (@t_count=1) then
		set @str=concat('alter table `',@schema_name,'`.`',table_name,'` change column `',column_name, '` `',new_column_name,'` ', column_type);
		call excuteproc(@str);
	end if;
end;

drop procedure if exists `proc_delete_column`;
create  procedure `proc_delete_column`(in table_name varchar(50),in column_name varchar(50))
    comment '删除字段'
begin
	set @schema_name = database();
	set @str=concat('select count(0) into @t_count from information_schema.columns where table_schema=''',@schema_name,''' and table_name=''',table_name,''' and column_name=''',column_name,'''');
	call excuteproc(@str);
	if (@t_count=1) then
		set @str=concat('alter table `',@schema_name,'`.`',table_name,'` drop column `',column_name,'`');
		call excuteproc(@str);
	end if;
end;

drop procedure if exists `proc_change_index`;
CREATE PROCEDURE `proc_change_index`(in table_name varchar(50),in index_field varchar(50),in index_name varchar(50),index_type varchar(50))
    COMMENT '创建索引'
begin
	-- set @str = concat('drop index if exists ',index_name,' on ',table_name,' ;');
	call del_idx(table_name, index_name); -- mysql不能使用if exists语句判断索引，需要单独处理
	set @str = concat('alter table ',table_name,' add ',index_type,' ',index_name,' (',index_field,');');
	call excuteproc(@str);

end;





drop procedure if exists `proCheckUserExtendExam`;
CREATE  PROCEDURE `proCheckUserExtendExam`(pUserUid varchar(36),pExamUid varchar(36),isForbitExamWhenPass varchar(1),OUT examGradeUid varchar(36),OUT returnCode varchar(36))
label_a:begin

declare strsql nvarchar(4000);
declare currTime int;
declare beginTime_1 int;
declare endTime_1 int;
declare forbidTime_1 int;
declare isExamination_1 varchar(1);
declare examinationCount_1 int;
declare maxExamNum_1 int;
declare hasUseTime int;
declare allowExamTime_1 int;
declare count int;
declare paperUid_1 varchar(36);


declare examGradeTableName varchar(36);

set examGradeTableName='exam_grade';
#获取数据
set examGradeUid='';
select UNIX_TIMESTAMP(now()) into @nowTime;
set currTime=@nowTime;

set @examUid=pExamUid;

#select UNIX_TIMESTAMP(beginTime),UNIX_TIMESTAMP(endTime),isExamination into beginTime_1,endTime_1,isExamination_1 from exam_arrange  where id=examArrangeUid;
select forbidTime,maxExamNum,examinationCount,paperUid, UNIX_TIMESTAMP(beginTime),UNIX_TIMESTAMP(endTime), isExamination into forbidTime_1,maxExamNum_1,examinationCount_1,paperUid_1, beginTime_1,endTime_1, isExamination_1 from exam_exam where id=@examUid;



#检查是否指定试卷
if (paperUid_1='' or paperUid_1 is null)
then
	set returnCode='-8'; #没有指定试卷
select returnCode,examGradeUid;
	 leave label_a;
end IF;

#检查考试时间
if (currTime<beginTime_1 and beginTime_1 <> 0)
then
	set returnCode='-1'; #还未达到开始时间
select returnCode,examGradeUid;
	 leave label_a;
end if;
if (currTime>endTime_1 and endTime_1 <> 0) 
then
	set returnCode='-2'; #考试时间已过
select returnCode,examGradeUid;
	leave label_a;
end if;

if (forbidTime_1>0)
then
	if (beginTime_1 < (currTime-forbidTime_1))
	then
		set returnCode='-3'; #未按规定的时间进入考试
select returnCode,examGradeUid;
		leave label_a;
	end if;
end if;


#检查是否可以参加该考试


#检查考试记录
 select id,examTime,allowExamTime into examGradeUid,hasUseTime,allowExamTime_1 from exam_grade  where userUid=pUserUid and examUid=pExamUid and gradeStatusCode in ('examing','pause') limit 1;
-- set  strsql =CONCAT("select id,examTime,allowExamTime into @examGradeUid,@hasUseTime,@allowExamTime_1 from exam_grade  where userUid='",@userUid, "' and examUid='",@examUid, "' and gradeStatusCode in ('examing','pause') limit 1;");
-- call excuteproc(strsql);


if (examGradeUid is NOT NULL AND LENGTH(examGradeUid)>=1)
then 
if (hasUseTime is null)
then
		set @hasUseTime=0;
end if;
if (allowExamTime_1>0 and hasUseTime>allowExamTime_1)
	then
		set examGradeUid='';
		set returnCode='-5'; #您上次未成功交卷
		select returnCode,examGradeUid;
		leave label_a;
	END if;
ELSE
	set examGradeUid='';
	if(isExamination_1='Y')
	THEN
			select count(1) into @count from exam_grade  where userUid=pUserUid and examUid=pExamUid and isExamination='Y';
			
			if(examinationCount_1>0 and count>=examinationCount_1)
			then
				set returnCode='-6'; #超过最大考试次数
				select returnCode,examGradeUid;
				leave label_a;
			end if;
	ELSE
			select @countOut=count(1) into @count from exam_grade  where userUid=pUserUid and examUid=pExamUid and isExamination='N';
		
			if (maxExamNum_1>0 and count>=maxExamNum_1 )
			then
				set returnCode='-6'; #超过最大考试次数
				select returnCode,examGradeUid;
				leave label_a;
			end if;
	end if;
end if;

if (isForbitExamWhenPass = 'Y')
then
 select @countOut=count(1) into @count from exam_grade  where userUid=pUserUid and examUid=pExamUid and isPass='Y';
	if (@count>0)
then
		set returnCode='-7';#超过最大考试次数
select returnCode,examGradeUid;
		leave label_a;
	end if;
end if;
set returnCode='0';
select returnCode,examGradeUid;
END;






drop procedure if exists `proInitUserExam`;
CREATE  PROCEDURE `proInitUserExam`(userUid varchar(36),examUid varchar(36),examGradeUid varchar(36),attendIP varchar(36), OUT gradePaperUid varchar(36))
BEGIN
	#Routine body goes here...   
declare strsql nvarchar(4000);
declare hasUseTime int;
declare allowExamTime_1 int;
declare isExamination_1 varchar(1);
declare examTimeModule_1 varchar(16);
declare paperUid_1 varchar(36);
declare paperTypeCode_1 varchar(36);
declare bufferPaperNum_1 int;
declare paperTotalScore numeric(18,2);
declare endTime_1 int;
declare beginTime_1 int;
declare examGradeTableName varchar(36);
declare strNewTime varchar(19);
declare sqlPara nvarchar(1000);
declare emptyGuid varchar(36);

set emptyGuid = '00000000-0000-0000-0000-000000000000';
set examGradeTableName='exam_grade';
set paperTotalScore=0;
select UNIX_TIMESTAMP(now()) into @nowTime;
set strNewTime=@nowTime;
-- select UNIX_TIMESTAMP(endTime),isExamination into endTime_1,isExamination_1 from exam_arrange  where id=examArrangeUid;

#select UNIX_TIMESTAMP(beginTime),isExamination into beginTime_1,isExamination_1 from exam_arrange  where id=examArrangeUid;

select paperUid,examTime,paperTypeCode,bufferPaperNum,examTimeModule, UNIX_TIMESTAMP(beginTime), UNIX_TIMESTAMP(endTime), isExamination into paperUid_1,allowExamTime_1,paperTypeCode_1,bufferPaperNum_1,examTimeModule_1, beginTime_1, endTime_1, isExamination_1 from  exam_exam where id=examUid;



IF (examGradeUid<>'' and examGradeUid is not null) #gradeUId不为空的情况
THEN
set  strsql =CONCAT("SELECT paperUid into @gradePaperUid ", " from ",examGradeTableName, " where id='",examGradeUid  ,"';");    
call excuteproc(strsql);
set gradePaperUid=@gradePaperUid;
#更新成绩表信息
set  strsql =CONCAT("update  exam_grade set lastExamIp='",attendIP, "',lastUpdateTime='",now(), "',isExamination='",isExamination_1, "' where id='",examGradeUid  ,"';");
call excuteproc(strsql);

 
ELSE
IF (paperTypeCode_1='random') #随机试卷
THEN
set  strsql =CONCAT("SELECT exam_exam_paper.paperUid into @paperUid from exam_exam_paper left join exam_grade on exam_exam_paper.paperUid=exam_grade.paperUid and exam_grade.examUid='",examUid ,"' and exam_grade.userUid='",userUid, "' where exam_exam_paper.examUid='",examUid ,"' and exam_exam_paper.isActive='Y' and exam_grade.id is null order by rand() limit 1;");
call excuteproc(strsql);
if(@paperUid is null) #查不到paperUid
THEN
set gradePaperUid='00000000-0000-0000-0000-000000000000';
ELSE                #返回随机到的paperUid
set gradePaperUid=@paperUid;

end if;
ELSE
set gradePaperUid=paperUid_1;
end IF;

if(examTimeModule_1='end_time') #倒计时  
THEN
if(endTime_1>strNewTime)
THEN
if(allowExamTime_1>(strNewTime-endTime_1))
THEN
set allowExamTime_1=(strNewTime-endTime_1);
end IF;
end IF;
end IF;

#生成考试记录
	if gradePaperUid <> '00000000-0000-0000-0000-000000000000'
then
		select totalScore into paperTotalScore from exam_paper  where id=gradePaperUid;

end if;
select uuid() into examGradeUid;

set strsql=CONCAT("insert into exam_grade(id,userUid,paperUid,paperTotalScore,beginTime,allowExamTime,examUid,lastUpdateTime,gradeStatusCode,hasSaveAnswerToDb,lastExamIp,isExamination,noAnswerQuestionNum,isPass,examTime,judgeUserUid,userAnswerUid,isCompiled)values('",examGradeUid ,"','",userUid,"','",gradePaperUid,"',",paperTotalScore,",'",now(),"',", allowExamTime_1 ,",'",examUid,"','",now(),"','examing','N','",attendIP,"','",isExamination_1,"',0,'N',0,'",emptyGuid,"','",emptyGuid,"',0);");    

call excuteproc(strsql);
end IF;
select * from exam_grade  where id=examGradeUid;

END;






drop procedure if exists `proc_get_rownum`;
CREATE  PROCEDURE `proc_get_rownum`(in table_name varchar(50),in rowid varchar(36),in orderby varchar(100))
    COMMENT '根据ID|排序条件获取数据所在的位置'
begin
	set @rownum = 0;
	set @str=concat('select ROWNUM from (select (@rownum := @rownum + 1) as ROWNUM,id from ',table_name,' order by ',orderby,') as A where id=''',rowid,'''');

	 call excuteproc(@str);
end;

drop PROCEDURE if exists `proc_CheckTradeStatus`;
CREATE  PROCEDURE `proc_CheckTradeStatus`()
BEGIN
    declare tradeDate datetime;			#校验日期
    DECLARE t_error INTEGER DEFAULT 0;  
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET t_error=1;  
    set tradeDate=ADDDATE(NOW(),-1); #真实校验日期 超过24小时的订单需要删除
    START TRANSACTION;  
     #删除无效的未支付的订单相关数据
		DELETE from order_member where orderId in (select id from orders where createTime<=tradeDate and status='created');
    DELETE from ordercouponusages where orderId in (select id from orders where createTime<=tradeDate and status='created');
    #删除无效的未支付的订单数据
		DELETE from orders where createTime<=tradeDate and status='created';
	IF t_error = 1 THEN 
     ROLLBACK; 
   ELSE  
    COMMIT;  
  END IF;  
END;

#每天凌晨自动执行订单清除事件
SET GLOBAL event_scheduler = 1;
drop EVENT if exists `CheckTradeStatus`;
CREATE  EVENT CheckTradeStatus
  ON SCHEDULE EVERY 1 day STARTS
    date_add(concat(current_date(), ' 00:00:00'), interval 0 second)
  ON COMPLETION PRESERVE ENABLE
DO
call proc_CheckTradeStatus();
