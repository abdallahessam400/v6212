/*
 Navicat Premium Dump SQL

 Source Server         : MySql
 Source Server Type    : MySQL
 Source Server Version : 50724 (5.7.24)
 Source Host           : localhost:3306
 Source Schema         : account_cq

 Target Server Type    : MySQL
 Target Server Version : 50724 (5.7.24)
 File Encoding         : 65001

 Date: 06/11/2026 04:00:44
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for city_location
-- ----------------------------
DROP TABLE IF EXISTS `city_location`;
CREATE TABLE `city_location`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ip_from` bigint(20) NULL DEFAULT NULL,
  `ip_to` bigint(20) NULL DEFAULT NULL,
  `country_code` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `country_name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `state` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `city` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `latitude` double(255, 9) NULL DEFAULT NULL,
  `longitude` double(255, 9) NULL DEFAULT NULL,
  `zip_code` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of city_location
-- ----------------------------

-- ----------------------------
-- Table structure for conquer_account
-- ----------------------------
DROP TABLE IF EXISTS `conquer_account`;
CREATE TABLE `conquer_account`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Password` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Salt` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `SrpVerifier` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `AuthorityId` int(11) NULL DEFAULT 1,
  `Flag` int(11) NULL DEFAULT 0,
  `IpAddress` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `MacAddress` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ParentId` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Modified` datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1000017 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of conquer_account
-- ----------------------------
INSERT INTO `conquer_account` VALUES (1000000, '1', '20e1c1c5cd815d6f7f94e3e2034fb4c06d9453e85cc7914107bd98562f5d9359', '91c0e61fb2afad6356b739c26e70e5a57d46995ff9c3fa809be2d399e23332e0', 'a46e5043fec50c9e8306cb122e3519ebfcf95eee8f6d3c0284b27fad48c93289', 2, 0, '127.0.0.1', '02501A2B954A', '00000000-0000-0000-0000-000000000000', '2025-10-17 23:33:09', '2026-11-06 02:45:06', NULL);
INSERT INTO `conquer_account` VALUES (1000005, 'test', 'fdd4ba19984f9b32ed98c86653f595e0475209802c33063f74c5233d8af9e9a1', 'KLnSdzaibuFSGEFpAI2BjsIIA0S6WjAAF5I9', NULL, 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2026-05-12 14:41:01', '2026-05-12 14:41:17', NULL);
INSERT INTO `conquer_account` VALUES (1000006, '2', '7052d842c74bd0316ed7528fad720faa2f45cd78979ca35ddb577a962aa1d445', '025f4931ec7ebc38fc67c9ebc7e02abefbebfa2c4ecde8fa49f1c4cbdbb09b72', 'd3b6097058b460d8ae7b9db6ff732ac6d160c91f7606fd158df9cd9bcbd76049', 1, 0, '127.0.0.1', '', '00000000-0000-0000-0000-000000000000', '2025-11-01 19:00:11', '2026-11-06 02:48:26', NULL);
INSERT INTO `conquer_account` VALUES (1000007, '3', '4007046841a0c8b45bd6cec045011bd5fa7459a5e65ae0119107a841ded2d786', 'f4ebaf8957024667c2b925ee1d9b09e05672c8924f72a6ca81d9c651500ec381', '49f27182363e567ba20776ac8ef0b0f09256f8742c84acffc0c7cabe8cb19a12', 1, 0, '127.0.0.1', '', '00000000-0000-0000-0000-000000000000', '2025-11-11 17:03:44', '2026-11-06 02:48:37', NULL);
INSERT INTO `conquer_account` VALUES (1000008, '4', '599bf2fc664bd9319e8567e0e43eb2795bd883a4b2cc444ce9b408cdc1dc33ec', '98e9993c8a5d11ef0e6c6bbdc586c3246c552596819badc367ee48da1e3177e4', 'fb255fc154edc567d184b7d52c6e8d448ef66752dd8b7d4ebf4801ba486e0585', 1, 0, '127.0.0.1', '', '00000000-0000-0000-0000-000000000000', '2025-11-11 23:38:36', '2026-11-06 02:48:44', NULL);
INSERT INTO `conquer_account` VALUES (1000009, '5', 'c5a2216957a17831a4428dc376071025ac037549e7bf337b7099dbf4a79af743', 'c9e7d5b4edf3d20d59debeb385a2034bd1d01b0a12e38e63252c6f2c63d2236d', '768cdc5507958fce85828be87ecd2e188ba318d965a850c27dbe57a8bbfe8f6c', 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2025-12-11 10:28:38', '2026-11-06 02:48:52', NULL);
INSERT INTO `conquer_account` VALUES (1000010, '6', '53ec34a6fa73d10012d4176fe9ada33bcb98e0bb16a11587466a5f956aeea0e5', 'c63dac08977a719323d6efc91c0bee5161dba5d7af1f0bffbb25078e434b385c', 'c81b94458626424aee931bea825a7dc7ecb486141c1808ed8963ec292309b64d', 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2025-12-22 12:20:10', '2026-11-06 02:48:58', NULL);
INSERT INTO `conquer_account` VALUES (1000011, '7', '246567179a0ff300ef53a384d8d776fb167cd4d7e51a0797f6d1cbe7b3ea2b08', '8fb1099313e57873af5aaf7680717b4db3da9e2b61b2cccbbaf38c475be454d8', 'e06dc174e53ab53b9099c22525072a1b4d3c89f3c53ec80f1bf3954f22324f01', 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2025-12-27 21:49:08', '2026-11-06 02:49:07', NULL);
INSERT INTO `conquer_account` VALUES (1000012, '8', 'fdd4ba19984f9b32ed98c86653f595e0475209802c33063f74c5233d8af9e9a1', 'KLnSdzaibuFSGEFpAI2BjsIIA0S6WjAAF5I9', NULL, 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2025-12-27 21:49:18', '2025-12-27 21:49:24', NULL);
INSERT INTO `conquer_account` VALUES (1000013, '9', 'cb638e9a367f41dfb6f1a97042d70148765196d471f70d046d57352b64471297', '2a91df7ec06d2039dc6cead461fe13fa85ef978c079f7399f5806cc61cede8ef', '2882b399c5669043ad39b8d76608bf86900632f874a2add242e8c320be7b6735', 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2025-12-28 15:44:30', '2026-11-06 02:49:20', NULL);
INSERT INTO `conquer_account` VALUES (1000014, '10', '660d73f0c33a218672936a52d79d6598833859292402349118611e07463e5c08', '91d47a79eae850c29ab471b4435fab4333a1432c748828700e4da954fa6a74bf', 'f3b1626686e70f3855f5599b7bdd4a0aaf73de8189fdd473455d1057d236ea01', 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2025-12-29 07:49:37', '2026-11-06 02:49:25', NULL);
INSERT INTO `conquer_account` VALUES (1000015, '11', '204390f652f00a711a68167f5d92058ff9787fb66b959d486c9fa40d3966b50a', '7e1a4ba813421a16a50886ad4731f7cc08a35c03fd526b5a5ea9092819c1c548', 'f50f6864d104a4ef915f9be59f06b1c352c08e51706c380561dad2b977c2ee11', 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2026-01-08 21:15:25', '2026-11-06 02:49:29', NULL);
INSERT INTO `conquer_account` VALUES (1000016, '12', 'fdd4ba19984f9b32ed98c86653f595e0475209802c33063f74c5233d8af9e9a1', 'KLnSdzaibuFSGEFpAI2BjsIIA0S6WjAAF5I9', NULL, 1, 0, NULL, NULL, '00000000-0000-0000-0000-000000000000', '2026-01-18 13:26:17', '2026-01-18 13:26:25', NULL);

-- ----------------------------
-- Table structure for conquer_account_authority
-- ----------------------------
DROP TABLE IF EXISTS `conquer_account_authority`;
CREATE TABLE `conquer_account_authority`  (
  `Id` int(11) NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `NormalizedName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of conquer_account_authority
-- ----------------------------
INSERT INTO `conquer_account_authority` VALUES (1, 'Player', 'Player');
INSERT INTO `conquer_account_authority` VALUES (2, 'Player', 'Player');

-- ----------------------------
-- Table structure for conquer_account_login_record
-- ----------------------------
DROP TABLE IF EXISTS `conquer_account_login_record`;
CREATE TABLE `conquer_account_login_record`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `AccountId` int(11) NULL DEFAULT NULL,
  `IpAddress` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `LocationId` int(11) NULL DEFAULT NULL,
  `LoginTime` datetime NULL DEFAULT NULL,
  `Success` tinyint(1) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of conquer_account_login_record
-- ----------------------------
INSERT INTO `conquer_account_login_record` VALUES (1, 1000000, '26.172.191.16', 0, '2026-04-18 23:00:34', 1);
INSERT INTO `conquer_account_login_record` VALUES (2, 1000000, '127.0.0.1', 0, '2026-04-18 23:01:56', 1);
INSERT INTO `conquer_account_login_record` VALUES (3, 1000000, '127.0.0.1', 0, '2026-04-18 23:02:21', 1);
INSERT INTO `conquer_account_login_record` VALUES (4, 1000000, '26.172.191.16', 0, '2026-04-18 23:13:39', 1);
INSERT INTO `conquer_account_login_record` VALUES (5, 1000000, '26.172.191.16', 0, '2026-11-06 02:47:56', 0);

-- ----------------------------
-- Table structure for conquer_account_vip
-- ----------------------------
DROP TABLE IF EXISTS `conquer_account_vip`;
CREATE TABLE `conquer_account_vip`  (
  `Id` int(11) NOT NULL,
  `ConquerAccountId` int(11) NULL DEFAULT NULL,
  `VipLevel` tinyint(4) NULL DEFAULT NULL,
  `DurationMinutes` int(11) NULL DEFAULT NULL,
  `StartDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `EndDate` datetime NOT NULL,
  `CreationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of conquer_account_vip
-- ----------------------------

-- ----------------------------
-- Table structure for realm
-- ----------------------------
DROP TABLE IF EXISTS `realm`;
CREATE TABLE `realm`  (
  `RealmID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RealmIdx` int(11) NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `AuthorityID` int(11) NULL DEFAULT 0,
  `GameIPAddress` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `RpcIPAddress` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `GamePort` int(11) NULL DEFAULT 0,
  `RpcPort` int(11) NULL DEFAULT 0,
  `Status` tinyint(4) NULL DEFAULT NULL,
  `Username` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Password` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `LastPing` datetime NULL DEFAULT NULL,
  `DatabaseHost` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DatabaseUser` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DatabasePass` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DatabaseSchema` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DatabasePort` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Active` bit(1) NULL DEFAULT b'1',
  `ProductionRealm` bit(1) NULL DEFAULT b'1',
  `Attribute` int(11) NULL DEFAULT NULL,
  `MasterRealmID` int(11) NULL DEFAULT NULL,
  `CrossPort` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`RealmID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of realm
-- ----------------------------
INSERT INTO `realm` VALUES ('94390aa0-c75d-11ed-9586-0050560401e2', 3, 'Virgo', 6, '26.172.191.16', '127.0.0.1', 5816, 9921, 1, 'KLtXUQ9ACjVByB9yVonh2/xo+WdAysWYQwgwqAF62AE=', 'D4RNWSJMrBpHCINxp0aBw+M1offMzrYY0f4P+vzdNH0=', '2024-04-03 01:37:07', '', '', '', '', '', b'1', b'1', 0, NULL, 9856);
INSERT INTO `realm` VALUES ('b4b3395a-12d5-11ef-9544-0050560401e2', 101, 'Realm1', 6, '26.172.191.16', '127.0.0.1', 9855, 9921, 1, 'KLtXUQ9ACjVByB9yVonh2/xo+WdAysWYQwgwqAF62AE=', 'D4RNWSJMrBpHCINxp0aBw+M1offMzrYY0f4P+vzdNH0=', '2026-05-10 23:06:33', '', '', '', '', '', b'1', b'0', 3, NULL, 9855);

-- ----------------------------
-- Table structure for realm_user
-- ----------------------------
DROP TABLE IF EXISTS `realm_user`;
CREATE TABLE `realm_user`  (
  `PlayerId` int(11) NOT NULL AUTO_INCREMENT,
  `RealmId` int(11) NULL DEFAULT NULL,
  `AccountId` int(11) NULL DEFAULT NULL,
  `CreationDate` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`PlayerId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 87 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of realm_user
-- ----------------------------
INSERT INTO `realm_user` VALUES (8, 1, 8, '2024-07-19 01:32:43');
INSERT INTO `realm_user` VALUES (9, 1, 9, '2024-07-19 01:58:06');
INSERT INTO `realm_user` VALUES (10, 1, 9, '2024-07-19 01:58:33');
INSERT INTO `realm_user` VALUES (11, 1, 9, '2024-07-19 02:01:17');
INSERT INTO `realm_user` VALUES (12, 1, 9, '2024-07-19 02:05:05');
INSERT INTO `realm_user` VALUES (13, 1, 9, '2024-07-19 02:09:44');
INSERT INTO `realm_user` VALUES (14, 1, 9, '2024-07-19 10:20:41');
INSERT INTO `realm_user` VALUES (15, 1, 10, '2024-07-19 13:24:16');
INSERT INTO `realm_user` VALUES (16, 1, 1000001, '2024-07-19 13:42:01');
INSERT INTO `realm_user` VALUES (17, 1, 1000002, '2024-07-19 14:08:07');
INSERT INTO `realm_user` VALUES (18, 1, 1000002, '2024-07-19 14:14:01');
INSERT INTO `realm_user` VALUES (19, 1, 1000003, '2025-10-18 00:03:25');
INSERT INTO `realm_user` VALUES (20, 1, 1000003, '2025-10-18 00:06:03');
INSERT INTO `realm_user` VALUES (21, 1, 1000001, '2025-10-30 03:30:12');
INSERT INTO `realm_user` VALUES (22, 1, 1000001, '2025-11-01 18:43:25');
INSERT INTO `realm_user` VALUES (23, 1, 1000004, '2025-11-01 18:44:49');
INSERT INTO `realm_user` VALUES (24, 1, 1000005, '2025-11-01 18:56:42');
INSERT INTO `realm_user` VALUES (25, 1, 1000006, '2025-11-01 19:01:04');
INSERT INTO `realm_user` VALUES (26, 1, 1000007, '2025-11-11 17:08:23');
INSERT INTO `realm_user` VALUES (27, 1, 1000007, '2025-11-11 17:12:21');
INSERT INTO `realm_user` VALUES (28, 1, 15, '2025-11-11 17:21:04');
INSERT INTO `realm_user` VALUES (29, 1, 1000007, '2025-11-11 17:26:09');
INSERT INTO `realm_user` VALUES (30, 1, 1000008, '2025-11-11 23:39:25');
INSERT INTO `realm_user` VALUES (31, 1, 1000008, '2025-11-24 05:02:37');
INSERT INTO `realm_user` VALUES (32, 1, 1000009, '2025-12-11 10:37:29');
INSERT INTO `realm_user` VALUES (33, 1, 1000010, '2025-12-22 12:24:40');
INSERT INTO `realm_user` VALUES (34, 1, 1000000, '2025-12-25 08:44:45');
INSERT INTO `realm_user` VALUES (35, 1, 1000011, '2025-12-27 21:49:48');
INSERT INTO `realm_user` VALUES (36, 1, 1000013, '2025-12-28 15:45:03');
INSERT INTO `realm_user` VALUES (37, 1, 1000014, '2025-12-29 07:50:46');
INSERT INTO `realm_user` VALUES (38, 1, 1000015, '2026-01-08 21:21:28');
INSERT INTO `realm_user` VALUES (39, 1, 1000015, '2026-01-08 22:19:49');
INSERT INTO `realm_user` VALUES (40, 1, 1000015, '2026-01-09 17:45:00');
INSERT INTO `realm_user` VALUES (41, 1, 1000015, '2026-01-09 18:29:51');
INSERT INTO `realm_user` VALUES (42, 1, 1000015, '2026-01-17 14:36:50');
INSERT INTO `realm_user` VALUES (43, 1, 1000015, '2026-01-17 15:38:56');
INSERT INTO `realm_user` VALUES (44, 1, 1000015, '2026-01-17 15:45:16');
INSERT INTO `realm_user` VALUES (45, 1, 1000015, '2026-01-17 15:46:31');
INSERT INTO `realm_user` VALUES (46, 1, 1000015, '2026-01-17 15:56:12');
INSERT INTO `realm_user` VALUES (47, 1, 1000015, '2026-01-17 16:09:14');
INSERT INTO `realm_user` VALUES (48, 1, 1000015, '2026-01-17 16:14:35');
INSERT INTO `realm_user` VALUES (49, 1, 1000015, '2026-01-17 16:15:19');
INSERT INTO `realm_user` VALUES (50, 1, 1000015, '2026-01-17 16:16:34');
INSERT INTO `realm_user` VALUES (51, 1, 1000015, '2026-01-17 16:18:43');
INSERT INTO `realm_user` VALUES (52, 1, 1000015, '2026-01-17 16:20:33');
INSERT INTO `realm_user` VALUES (53, 1, 1000015, '2026-01-17 20:53:19');
INSERT INTO `realm_user` VALUES (54, 1, 1000015, '2026-01-17 21:26:15');
INSERT INTO `realm_user` VALUES (55, 1, 1000015, '2026-01-18 11:47:47');
INSERT INTO `realm_user` VALUES (56, 1, 1000016, '2026-01-18 13:26:55');
INSERT INTO `realm_user` VALUES (57, 1, 1000016, '2026-01-18 14:35:16');
INSERT INTO `realm_user` VALUES (58, 1, 1000016, '2026-02-21 14:04:18');
INSERT INTO `realm_user` VALUES (59, 1, 1000016, '2026-02-21 14:07:21');
INSERT INTO `realm_user` VALUES (60, 1, 1000016, '2026-02-21 14:08:26');
INSERT INTO `realm_user` VALUES (61, 1, 1000016, '2026-03-18 12:54:34');
INSERT INTO `realm_user` VALUES (62, 1, 1000016, '2026-03-21 12:32:14');
INSERT INTO `realm_user` VALUES (63, 1, 1000016, '2026-03-21 12:34:27');
INSERT INTO `realm_user` VALUES (64, 1, 1000016, '2026-04-14 09:36:34');
INSERT INTO `realm_user` VALUES (65, 1, 1000016, '2026-04-16 00:14:21');
INSERT INTO `realm_user` VALUES (66, 1, 1000016, '2026-04-16 20:17:07');
INSERT INTO `realm_user` VALUES (67, 1, 1000016, '2026-04-17 01:45:41');
INSERT INTO `realm_user` VALUES (68, 3, 1000000, '2026-05-12 14:56:23');
INSERT INTO `realm_user` VALUES (69, 3, 1000000, '2026-05-12 15:14:05');
INSERT INTO `realm_user` VALUES (70, 3, 1000000, '2026-05-12 15:17:29');
INSERT INTO `realm_user` VALUES (71, 3, 1000000, '2026-05-12 15:20:29');
INSERT INTO `realm_user` VALUES (72, 3, 1000000, '2026-05-12 15:27:35');
INSERT INTO `realm_user` VALUES (73, 3, 1000000, '2026-05-12 17:00:45');
INSERT INTO `realm_user` VALUES (74, 3, 1000000, '2026-05-12 17:03:59');
INSERT INTO `realm_user` VALUES (75, 3, 1000006, '2026-05-13 23:21:00');
INSERT INTO `realm_user` VALUES (76, 3, 1000016, '2026-05-31 14:37:13');
INSERT INTO `realm_user` VALUES (77, 3, 1000015, '2026-06-09 13:08:00');
INSERT INTO `realm_user` VALUES (78, 3, 1000016, '2026-06-12 02:27:09');
INSERT INTO `realm_user` VALUES (79, 3, 1000016, '2026-06-12 02:30:07');
INSERT INTO `realm_user` VALUES (80, 3, 1000016, '2026-06-12 02:39:28');
INSERT INTO `realm_user` VALUES (81, 3, 1000016, '2026-06-12 02:40:58');
INSERT INTO `realm_user` VALUES (82, 3, 1000016, '2026-06-12 02:43:47');
INSERT INTO `realm_user` VALUES (83, 3, 1000016, '2026-06-12 02:48:52');
INSERT INTO `realm_user` VALUES (84, 3, 1000016, '2026-06-12 02:53:10');
INSERT INTO `realm_user` VALUES (85, 3, 1000016, '2026-06-12 02:56:24');
INSERT INTO `realm_user` VALUES (86, 3, 1000016, '2026-06-12 03:14:12');

SET FOREIGN_KEY_CHECKS = 1;
