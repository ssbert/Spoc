CREATE TABLE IF NOT EXISTS `exam_policy_item_label` (
  `id` char(36) NOT NULL,
  `itemId` char(36) NOT NULL,
  `labelId` char(36) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;