CREATE TABLE IF NOT EXISTS `exam_program_result` (
  `id` char(36) NOT NULL COMMENT 'ID',
  `gradeId` char(36) NOT NULL COMMENT '成绩ID',
  `questionId` char(36) NOT NULL COMMENT '编程题ID',
  `result` longtext NOT NULL COMMENT '编译运行结果',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;