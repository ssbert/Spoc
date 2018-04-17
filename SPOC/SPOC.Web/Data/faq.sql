CREATE TABLE IF NOT EXISTS `faq` (
  `id` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `title` varchar(50) DEFAULT NULL,
  `content` longtext,
  `folderId` char(36)  NOT NULL,
  `updateTime` datetime NOT NULL,
 `userFul` int NOT NULL,
 `userLess` int NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

call proc_add_field('faq', 'seq', 'int', ' comment ''序号'' after `updateTime`');
call proc_add_field('faq', 'isActive', 'tinyint(1) DEFAULT 1', ' comment ''状态'' after `seq`');
call proc_add_field('faq', 'formCloud', 'tinyint(1) DEFAULT 0', ' comment ''来自云'' after `isActive`');