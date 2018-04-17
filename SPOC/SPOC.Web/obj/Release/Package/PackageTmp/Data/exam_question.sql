call proc_add_field('exam_question', 'inputParam', 'longtext', ' comment ''输入流参数'' after `param`');
call proc_add_field('exam_question', 'multiTest', 'tinyint(1) default 0', ' comment ''多次测试'' after `inputParam`');
call proc_add_field('exam_question', 'preinstallCode', 'longtext', ' comment ''预设代码'' after `multiTest`');
call proc_add_field('challenge_question', 'inputParam', 'longtext', ' comment ''输入流参数'' after `param`');
call proc_add_field('challenge_question', 'multiTest', 'tinyint(1) default 0', ' comment ''多次测试'' after `inputParam`');
call proc_add_field('challenge_question', 'preinstallCode', 'longtext', ' comment ''预设代码'' after `multiTest`');
