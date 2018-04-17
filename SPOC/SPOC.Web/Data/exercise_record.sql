CREATE TABLE IF NOT EXISTS `exercise_record` (
  `id` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `exerciseId` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `userId` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `isPass` tinyint(1) NOT NULL,
  `beginTime` datetime NOT NULL,
  `endTime` datetime DEFAULT NULL,
  `compiledResults` longtext DEFAULT NULL, 
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

call proc_add_field('exercise_record', 'compiledResults', 'longtext', ' after `endTime`');