-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: insurancedb
-- ------------------------------------------------------
-- Server version	8.0.43

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `announcements`
--

DROP TABLE IF EXISTS `announcements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `announcements` (
  `announcement_id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(160) NOT NULL,
  `body` text NOT NULL,
  `publish_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `expire_at` timestamp NULL DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`announcement_id`),
  KEY `fk_announce_user` (`created_by`),
  KEY `ix_announce_pub` (`publish_at`),
  KEY `ix_announce_exp` (`expire_at`),
  CONSTRAINT `fk_announce_user` FOREIGN KEY (`created_by`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `announcements`
--

LOCK TABLES `announcements` WRITE;
/*!40000 ALTER TABLE `announcements` DISABLE KEYS */;
INSERT INTO `announcements` VALUES (1,'Welcome to the Employee Hub','Please review MFA policy.','2025-09-24 18:49:35',NULL,2,'2025-09-24 18:49:35');
/*!40000 ALTER TABLE `announcements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim_notes`
--

DROP TABLE IF EXISTS `claim_notes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claim_notes` (
  `note_id` int NOT NULL AUTO_INCREMENT,
  `claim_id` int NOT NULL,
  `author_user_id` int DEFAULT NULL,
  `note_text` text NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`note_id`),
  KEY `fk_notes_author` (`author_user_id`),
  KEY `ix_notes_claim_time` (`claim_id`,`created_at`),
  CONSTRAINT `fk_notes_author` FOREIGN KEY (`author_user_id`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_notes_claim` FOREIGN KEY (`claim_id`) REFERENCES `claims` (`claim_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claim_notes`
--

LOCK TABLES `claim_notes` WRITE;
/*!40000 ALTER TABLE `claim_notes` DISABLE KEYS */;
INSERT INTO `claim_notes` VALUES (1,1,2,'Initial assessment completed.','2025-09-24 18:49:35');
/*!40000 ALTER TABLE `claim_notes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claims`
--

DROP TABLE IF EXISTS `claims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `claims` (
  `claim_id` int NOT NULL AUTO_INCREMENT,
  `policy_id` int NOT NULL,
  `claim_number` varchar(32) NOT NULL,
  `status` enum('Open','Investigating','Pending','Approved','Denied','Closed') NOT NULL DEFAULT 'Open',
  `date_of_loss` date DEFAULT NULL,
  `date_reported` date DEFAULT NULL,
  `reserve_amount` decimal(13,2) NOT NULL DEFAULT '0.00',
  `paid_amount` decimal(13,2) NOT NULL DEFAULT '0.00',
  `memo` varchar(300) DEFAULT NULL,
  `assigned_to` int DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`claim_id`),
  UNIQUE KEY `claim_number` (`claim_number`),
  KEY `fk_claims_creator` (`created_by`),
  KEY `ix_claims_policy` (`policy_id`),
  KEY `ix_claims_status` (`status`),
  KEY `ix_claims_assignee` (`assigned_to`),
  CONSTRAINT `fk_claims_assignee` FOREIGN KEY (`assigned_to`) REFERENCES `employees` (`employee_id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_claims_creator` FOREIGN KEY (`created_by`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_claims_policy` FOREIGN KEY (`policy_id`) REFERENCES `policies` (`policy_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `claims`
--

LOCK TABLES `claims` WRITE;
/*!40000 ALTER TABLE `claims` DISABLE KEYS */;
INSERT INTO `claims` VALUES (1,1,'CLM-0001','Open','2025-09-14','2025-09-17',5000.00,0.00,'Roof damage',1,2,'2025-09-24 18:49:35');
/*!40000 ALTER TABLE `claims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_records`
--

DROP TABLE IF EXISTS `customer_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_records` (
  `record_id` int NOT NULL AUTO_INCREMENT,
  `record_name` varchar(200) NOT NULL,
  `url` varchar(500) NOT NULL,
  `uploaded_by` int DEFAULT NULL,
  `uploaded_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `attached_to_type` enum('Customer','Policy','Claim') NOT NULL,
  `attached_to_id` int NOT NULL,
  `description` varchar(300) DEFAULT NULL,
  PRIMARY KEY (`record_id`),
  KEY `fk_documents_employee` (`uploaded_by`),
  KEY `ix_documents_target` (`attached_to_type`,`attached_to_id`),
  KEY `ix_documents_time` (`uploaded_at`),
  CONSTRAINT `fk_documents_employee` FOREIGN KEY (`uploaded_by`) REFERENCES `employees` (`employee_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_records`
--

LOCK TABLES `customer_records` WRITE;
/*!40000 ALTER TABLE `customer_records` DISABLE KEYS */;
INSERT INTO `customer_records` VALUES (1,'inspection.pdf','https://files.example/inspection.pdf',1,'2025-09-24 18:49:35','Policy',1,'Initial inspection report'),(2,'images.pdf','https://files.example/inspection.pdf',3,'2025-09-24 18:49:35','Policy',3,'Images'),(3,'other.pdf','https://files.example/other.pdf',2,'2025-09-24 18:49:35','Policy',2,'Other');
/*!40000 ALTER TABLE `customer_records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customers` (
  `customer_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `email` varchar(120) DEFAULT NULL,
  `phone` varchar(25) DEFAULT NULL,
  `addr_line1` varchar(120) DEFAULT NULL,
  `addr_line2` varchar(120) DEFAULT NULL,
  `city` varchar(80) DEFAULT NULL,
  `state_code` varchar(10) DEFAULT NULL,
  `zip_code` varchar(12) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`customer_id`),
  KEY `ix_customers_name` (`name`),
  KEY `ix_customers_email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES (1,'ABC Manufacturing','abc@client.com','401-555-1000','1 Brick Rd','Bldg. 3','Providence','RI','02860','2025-09-24 18:49:34'),(2,'Chad Smith','csmith_1234@email.ric.edu','4015552384','3 Mulberry Ln','','Woonsocket','RI','02895','2025-09-23 04:00:00');
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `employees`
--

DROP TABLE IF EXISTS `employees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employees` (
  `employee_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `title` varchar(60) DEFAULT NULL,
  `email` varchar(120) DEFAULT NULL,
  `phone` varchar(25) DEFAULT NULL,
  `status` enum('Active','Inactive','Leave','Terminated') NOT NULL DEFAULT 'Active',
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`employee_id`),
  UNIQUE KEY `email` (`email`),
  KEY `ix_employees_name` (`name`),
  KEY `ix_employees_status` (`status`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employees`
--

LOCK TABLES `employees` WRITE;
/*!40000 ALTER TABLE `employees` DISABLE KEYS */;
INSERT INTO `employees` VALUES (1,'Jane Doe','Agent','jane.doe@emailplace.com','4015557896','Active','2025-09-24 18:49:34'),(2,'John Doe','Agent','john.doe@emailplace.com','4015557897','Active','2025-09-24 18:49:34'),(3,'Martin Doe','Agent','martin.doe@emailplace.com','4015557807','Active','2025-09-24 18:49:34'),(4,'Eric Doe','Agent','eric.doe@emailplace.com','4015557891','Active','2025-09-24 18:49:34'),(5,'Fred Fox','Agent','fred.fox@emailplace.com','4015557000','Active','2025-09-24 18:49:34');
/*!40000 ALTER TABLE `employees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `login_audit`
--

DROP TABLE IF EXISTS `login_audit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `login_audit` (
  `audit_id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `event` enum('LOGIN_SUCCESS','LOGIN_FAIL','LOGOUT') NOT NULL,
  `ip_address` varchar(45) DEFAULT NULL,
  `user_agent` varchar(300) DEFAULT NULL,
  `occurred_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`audit_id`),
  KEY `ix_login_user_time` (`user_id`,`occurred_at`),
  KEY `ix_login_event_time` (`event`,`occurred_at`),
  CONSTRAINT `fk_login_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `login_audit`
--

LOCK TABLES `login_audit` WRITE;
/*!40000 ALTER TABLE `login_audit` DISABLE KEYS */;
INSERT INTO `login_audit` VALUES (1,2,'LOGIN_SUCCESS','127.0.0.1','Chrome','2025-09-23 18:49:36'),(2,2,'LOGOUT','127.0.0.1','Chrome','2025-09-24 06:49:36');
/*!40000 ALTER TABLE `login_audit` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `policies`
--

DROP TABLE IF EXISTS `policies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `policies` (
  `policy_id` int NOT NULL AUTO_INCREMENT,
  `account_number` varchar(32) NOT NULL,
  `customer_id` int NOT NULL,
  `manager_id` int DEFAULT NULL,
  `policy_type` enum('Auto','Property','Liability','Commercial','Marine','Other') DEFAULT 'Other',
  `status` enum('Active','Pending','Cancelled','Expired') NOT NULL DEFAULT 'Active',
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `exposure_amount` decimal(13,2) DEFAULT NULL,
  `loc_addr1` varchar(120) DEFAULT NULL,
  `loc_addr2` varchar(120) DEFAULT NULL,
  `loc_city` varchar(80) DEFAULT NULL,
  `loc_state` varchar(10) DEFAULT NULL,
  `loc_zip` varchar(12) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`policy_id`),
  UNIQUE KEY `account_number` (`account_number`),
  KEY `fk_policies_manager` (`manager_id`),
  KEY `ix_policies_customer` (`customer_id`),
  KEY `ix_policies_status` (`status`),
  KEY `ix_policies_dates` (`start_date`,`end_date`),
  CONSTRAINT `fk_policies_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_policies_manager` FOREIGN KEY (`manager_id`) REFERENCES `employees` (`employee_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `policies`
--

LOCK TABLES `policies` WRITE;
/*!40000 ALTER TABLE `policies` DISABLE KEYS */;
INSERT INTO `policies` VALUES (1,'POL-0001',1,1,'Property','Active','2025-09-24','2026-09-24',250000.00,'1 Lake Dr.','','Woonsocket','RI','02895','2025-09-24 18:49:35'),(2,'FIG-67601',1,1,'Property','Active','2025-09-24','2026-09-24',250000.00,'1 Pine Rd','Bldg. 4','Woonsocket','RI','02895','2025-09-24 18:49:35');
/*!40000 ALTER TABLE `policies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `releases`
--

DROP TABLE IF EXISTS `releases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `releases` (
  `version` varchar(30) NOT NULL,
  `start_date` date DEFAULT NULL,
  `rollout_date` date DEFAULT NULL,
  `complete_date` date DEFAULT NULL,
  `notes` varchar(400) DEFAULT NULL,
  `hotfix_notes` varchar(400) DEFAULT NULL,
  PRIMARY KEY (`version`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `releases`
--

LOCK TABLES `releases` WRITE;
/*!40000 ALTER TABLE `releases` DISABLE KEYS */;
INSERT INTO `releases` VALUES ('v0.2.0','2025-09-24',NULL,NULL,'Employee hub improvements + audit + announcements',NULL);
/*!40000 ALTER TABLE `releases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sessions`
--

DROP TABLE IF EXISTS `sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sessions` (
  `session_id` char(36) NOT NULL DEFAULT (uuid()),
  `user_id` int NOT NULL,
  `session_hash` varchar(200) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `expires_at` timestamp NOT NULL,
  `revoked_at` timestamp NULL DEFAULT NULL,
  `ip_address` varchar(45) DEFAULT NULL,
  `user_agent` varchar(300) DEFAULT NULL,
  PRIMARY KEY (`session_id`),
  KEY `ix_sessions_user_active` (`user_id`,`expires_at`),
  KEY `ix_sessions_expiry` (`expires_at`),
  CONSTRAINT `fk_sessions_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sessions`
--

LOCK TABLES `sessions` WRITE;
/*!40000 ALTER TABLE `sessions` DISABLE KEYS */;
/*!40000 ALTER TABLE `sessions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `email` varchar(120) NOT NULL,
  `password_hash` varchar(200) NOT NULL,
  `role` enum('Customer','Employee','Admin') NOT NULL,
  `customer_id` int DEFAULT NULL,
  `employee_id` int DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `email` (`email`),
  KEY `fk_users_customer` (`customer_id`),
  KEY `fk_users_employee` (`employee_id`),
  KEY `ix_users_role` (`role`),
  KEY `ix_users_active` (`is_active`),
  CONSTRAINT `fk_users_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_users_employee` FOREIGN KEY (`employee_id`) REFERENCES `employees` (`employee_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'abc@client.com','$2b$10$replace_me_customer','Customer',1,NULL,1,'2025-09-24 18:49:34',NULL),(2,'jane.doe@emailplace.com','$2b$10$replace_me_employee','Employee',NULL,1,1,'2025-09-24 18:49:35',NULL),(3,'martin.doe@emailplace.com','$2b$10$replace_me_employee','Employee',NULL,3,1,'2025-09-29 00:05:34',NULL);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'insurancedb'
--

--
-- Dumping routines for database 'insurancedb'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-09-28 20:33:35
