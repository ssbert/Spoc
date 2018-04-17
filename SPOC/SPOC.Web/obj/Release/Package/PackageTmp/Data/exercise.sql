CREATE TABLE IF NOT EXISTS `exercise` (
  `id` char(36) NOT NULL,
  `questionId` char(36) NOT NULL,
  `creatorId` char(36)  NOT NULL,
  `title` varchar(256) NOT NULL,
  `createTime` datetime NOT NULL,
  `endTime` datetime DEFAULT NULL,
  `showAnswer` tinyint(1) DEFAULT 1,
  `showAnswerType` tinyint(2) DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


call proc_add_field('exercise', 'showAnswer', 'tinyint(1) default 1', ' after `endTime`');
call proc_add_field('exercise', 'showAnswerType', 'tinyint(2) default 0', ' after `showAnswer`');