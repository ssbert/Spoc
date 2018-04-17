CREATE TABLE IF NOT EXISTS `user_answer_records` (
  `id` char(36) NOT NULL,
  `questionId` char(36) NOT NULL,
  `labelId` char(36) NOT NULL,
  `userId` char(36) NOT NULL,
  `recordId` char(36) NOT NULL,
  `createTime` datetime NOT NULL,
  `score` int(11) NOT NULL,
  `source` varchar(16) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

call proc_add_field('user_answer_records', 'recordId', 'char(36) not null', ' after `userId`');
call proc_add_field('user_answer_records', 'questionId', 'char(36) not null', ' after `id`');
call proc_add_field('user_answer_records', 'labelId', 'char(36) not null', ' after `questionId`');

call proc_delete_column('user_answer_records', 'questionLabelId');