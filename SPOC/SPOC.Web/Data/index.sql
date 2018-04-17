
call proc_change_index('exam_exam_paper','examUid','idxExamId','index');
call proc_change_index('exam_grade','examUid,userUid','idxExamUserId','index');
call proc_change_index('exam_paper','folderUid','idxFolder','index');
call proc_change_index('exam_paper_node','paperUid,questionTypeUid','idxPaperNode','index');
call proc_change_index('exam_paper_node_question','paperUid,paperNodeUid,questionUid','idxNodeQuestion','index');
call proc_change_index('exam_policy','folderUid','idxFolder','index');
call proc_change_index('exam_policy_item','policyNodeUid,questionTypeUid','idxPolicyItem','index');
call proc_change_index('exam_policy_node','policyUid,questionTypeUid','idxPolicyNode','index');
call proc_change_index('exam_publish','examUid,ownerUid','idxPublish','index');
call proc_change_index('exam_question','questionTypeUid','idxQTypeId','index');


call proc_change_index('student_info','userId','idxUserId','index');

call proc_change_index('teacher_info','userId','idxUserId','index');

call proc_change_index('users','userLoginName(36)','idxLoginName','index');
call proc_change_index('users','userMobile','idxMobile','index');
call proc_change_index('users','userEmail(36)','idxEmail','index');
