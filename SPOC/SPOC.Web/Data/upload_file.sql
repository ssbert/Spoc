CREATE TABLE IF NOT EXISTS `upload_file` (
  `id` char(36) NOT NULL,
  `size` bigint(20) NOT NULL,
  `fileName` varchar(512) NOT NULL,
  `source` varchar(64) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;