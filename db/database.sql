/* ============================================================
   INSURANCE MANAGEMENT – EMPLOYEE HUB (MySQL 8+)
   ------------------------------------------------------------
   Keeps your original model (customers, policies, claims, docs)
   and improves it for an internal employee hub:
   - Stronger constraints & indexes
   - Employee/Customer logins via users
   - Sessions (server-stored, optional)
   - Announcements (dashboard feed)
   - Login audit (security + dashboard metrics)
   - Claim notes (internal timeline)
   ============================================================ */

SET NAMES utf8mb4;
SET time_zone = '+00:00';

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS login_audit;
DROP TABLE IF EXISTS announcements;
DROP TABLE IF EXISTS claim_notes;
DROP TABLE IF EXISTS sessions;
DROP TABLE IF EXISTS documents;
DROP TABLE IF EXISTS claims;
DROP TABLE IF EXISTS policies;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS employees;
DROP TABLE IF EXISTS customers;
DROP TABLE IF EXISTS releases;
SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================
-- 1) CORE ENTITIES
-- ============================================================

-- ======================
-- Customers
-- ======================
CREATE TABLE customers (
  customer_id   INT AUTO_INCREMENT PRIMARY KEY,
  name          VARCHAR(100) NOT NULL,
  email         VARCHAR(120),
  phone         VARCHAR(25),
  addr_line1    VARCHAR(120),
  addr_line2    VARCHAR(120),
  city          VARCHAR(80),
  state_code    VARCHAR(10),
  zip_code      VARCHAR(12),
  created_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE INDEX ix_customers_name  ON customers (name);
CREATE INDEX ix_customers_email ON customers (email);

-- ======================
-- Employees (staff/agents)
-- ======================
CREATE TABLE employees (
  employee_id   INT AUTO_INCREMENT PRIMARY KEY,
  name          VARCHAR(100) NOT NULL,
  title         VARCHAR(60),
  email         VARCHAR(120) UNIQUE,  -- ensure no duplicate staff emails
  phone         VARCHAR(25),
  status        ENUM('Active','Inactive','Leave','Terminated') NOT NULL DEFAULT 'Active',
  created_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE INDEX ix_employees_name  ON employees (name);
CREATE INDEX ix_employees_status ON employees (status);

-- ======================
-- Users (login accounts: employees/admins and optionally customers)
-- ======================
CREATE TABLE users (
  user_id       INT AUTO_INCREMENT PRIMARY KEY,
  email         VARCHAR(120) NOT NULL UNIQUE,
  password_hash VARCHAR(200) NOT NULL,  -- bcrypt/argon2
  role          ENUM('Customer','Employee','Admin') NOT NULL,
  customer_id   INT NULL,
  employee_id   INT NULL,
  is_active     BOOLEAN NOT NULL DEFAULT TRUE,
  created_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at    TIMESTAMP NULL DEFAULT NULL,
  CONSTRAINT fk_users_customer FOREIGN KEY (customer_id)
    REFERENCES customers(customer_id) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT fk_users_employee FOREIGN KEY (employee_id)
    REFERENCES employees(employee_id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_users_role   ON users (role);
CREATE INDEX ix_users_active ON users (is_active);

-- ======================
-- Policies
-- ======================
CREATE TABLE policies (
  policy_id       INT AUTO_INCREMENT PRIMARY KEY,
  account_number  VARCHAR(32) NOT NULL UNIQUE,
  customer_id     INT NOT NULL,
  manager_id      INT NULL,  -- employee managing it
  policy_type     ENUM('Auto','Property','Liability','Commercial','Marine','Other') DEFAULT 'Other',
  status          ENUM('Active','Pending','Cancelled','Expired') NOT NULL DEFAULT 'Active',
  start_date      DATE,
  end_date        DATE,
  exposure_amount DECIMAL(13,2),
  loc_addr1       VARCHAR(120),
  loc_addr2       VARCHAR(120),
  loc_city        VARCHAR(80),
  loc_state       VARCHAR(10),
  loc_zip         VARCHAR(12),
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_policies_customer FOREIGN KEY (customer_id)
    REFERENCES customers(customer_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_policies_manager  FOREIGN KEY (manager_id)
    REFERENCES employees(employee_id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_policies_customer ON policies (customer_id);
CREATE INDEX ix_policies_status   ON policies (status);
CREATE INDEX ix_policies_dates    ON policies (start_date, end_date);

-- ======================
-- Claims
-- ======================
CREATE TABLE claims (
  claim_id       INT AUTO_INCREMENT PRIMARY KEY,
  policy_id      INT NOT NULL,
  claim_number   VARCHAR(32) NOT NULL UNIQUE,
  status         ENUM('Open','Investigating','Pending','Approved','Denied','Closed') NOT NULL DEFAULT 'Open',
  date_of_loss   DATE,
  date_reported  DATE,
  reserve_amount DECIMAL(13,2) NOT NULL DEFAULT 0,
  paid_amount    DECIMAL(13,2) NOT NULL DEFAULT 0,
  memo           VARCHAR(300),
  assigned_to    INT NULL,       -- employees.employee_id
  created_by     INT NULL,       -- users.user_id (internal creator)
  created_at     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_claims_policy   FOREIGN KEY (policy_id)
    REFERENCES policies(policy_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_claims_assignee FOREIGN KEY (assigned_to)
    REFERENCES employees(employee_id) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT fk_claims_creator  FOREIGN KEY (created_by)
    REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_claims_policy   ON claims (policy_id);
CREATE INDEX ix_claims_status   ON claims (status);
CREATE INDEX ix_claims_assignee ON claims (assigned_to);

-- ======================
-- Claim Notes (internal timeline)
-- ======================
CREATE TABLE claim_notes (
  note_id        INT AUTO_INCREMENT PRIMARY KEY,
  claim_id       INT NOT NULL,
  author_user_id INT NULL,   -- users.user_id
  note_text      TEXT NOT NULL,
  created_at     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_notes_claim  FOREIGN KEY (claim_id)
    REFERENCES claims(claim_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_notes_author FOREIGN KEY (author_user_id)
    REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_notes_claim_time ON claim_notes (claim_id, created_at);

-- ======================
-- Documents (polymorphic: attach to Customer | Policy | Claim)
-- ======================
CREATE TABLE documents (
  document_id      INT AUTO_INCREMENT PRIMARY KEY,
  file_name        VARCHAR(200) NOT NULL,
  url              VARCHAR(500) NOT NULL,   -- storage path or URL
  uploaded_by      INT NULL,                -- employees.employee_id
  uploaded_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  attached_to_type ENUM('Customer','Policy','Claim') NOT NULL,
  attached_to_id   INT NOT NULL,
  description      VARCHAR(300),
  CONSTRAINT fk_documents_employee FOREIGN KEY (uploaded_by)
    REFERENCES employees(employee_id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_documents_target ON documents (attached_to_type, attached_to_id);
CREATE INDEX ix_documents_time   ON documents (uploaded_at);

-- ======================
-- Releases (system version notes / changelog)
-- ======================
CREATE TABLE releases (
  version        VARCHAR(30) PRIMARY KEY,
  start_date     DATE,
  rollout_date   DATE,
  complete_date  DATE,
  notes          VARCHAR(400),
  hotfix_notes   VARCHAR(400)
) ENGINE=InnoDB;

-- ============================================================
-- 2) AUTH SUPPORT (employee hub)
-- ============================================================

-- ----------------------
-- Sessions (optional server-stored sessions; can use JWT instead)
-- ----------------------
CREATE TABLE sessions (
  session_id    CHAR(36) PRIMARY KEY DEFAULT (UUID()),
  user_id       INT NOT NULL,
  session_hash  VARCHAR(200) NOT NULL, -- hash of random cookie token
  created_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  expires_at    TIMESTAMP NOT NULL,
  revoked_at    TIMESTAMP NULL,
  ip_address    VARCHAR(45),
  user_agent    VARCHAR(300),
  CONSTRAINT fk_sessions_user FOREIGN KEY (user_id)
    REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_sessions_user_active ON sessions (user_id, expires_at);
CREATE INDEX ix_sessions_expiry      ON sessions (expires_at);

-- ----------------------
-- Login audit (dashboard + security)
-- ----------------------
CREATE TABLE login_audit (
  audit_id     BIGINT AUTO_INCREMENT PRIMARY KEY,
  user_id      INT NULL,
  login_event        ENUM('LOGIN_SUCCESS','LOGIN_FAIL','LOGOUT') NOT NULL,
  ip_address   VARCHAR(45),
  user_agent   VARCHAR(300),
  occurred_at  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_login_user FOREIGN KEY (user_id)
    REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_login_user_time  ON login_audit (user_id, occurred_at);
CREATE INDEX ix_login_event_time ON login_audit (event, occurred_at);

-- ----------------------
-- Announcements (dashboard feed)
-- ----------------------
CREATE TABLE announcements (
  announcement_id INT AUTO_INCREMENT PRIMARY KEY,
  title           VARCHAR(160) NOT NULL,
  body            TEXT NOT NULL,
  publish_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  expire_at       TIMESTAMP NULL,
  created_by      INT NULL,  -- users.user_id
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_announce_user FOREIGN KEY (created_by)
    REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_announce_pub ON announcements (publish_at);
CREATE INDEX ix_announce_exp ON announcements (expire_at);

-- ============================================================
-- 3) SEED DATA (demo rows)
-- ============================================================

-- Customer + Employee
INSERT INTO customers (name, email, phone, city, state_code)
VALUES ('ABC Manufacturing', 'abc@client.com', '401-555-1000', 'Providence', 'RI');

INSERT INTO employees (name, title, email)
VALUES ('Jane Doe', 'Agent', 'jane.doe@insco.com');

-- Users (replace password_hash with real bcrypt/argon2 before prod)
INSERT INTO users (email, password_hash, role, customer_id, is_active)
VALUES ('abc@client.com',      '$2b$10$replace_me_customer', 'Customer', 1, TRUE);

INSERT INTO users (email, password_hash, role, employee_id, is_active)
VALUES ('jane.doe@insco.com',  '$2b$10$replace_me_employee', 'Employee', 1, TRUE);

-- Policy + Claim
INSERT INTO policies (account_number, customer_id, manager_id, policy_type, status, start_date, end_date, exposure_amount, loc_city, loc_state)
VALUES ('POL-0001', 1, 1, 'Property', 'Active', CURDATE(), DATE_ADD(CURDATE(), INTERVAL 1 YEAR), 250000, 'Cranston', 'RI');

INSERT INTO claims (policy_id, claim_number, status, date_of_loss, date_reported, reserve_amount, paid_amount, memo, assigned_to, created_by)
VALUES (1, 'CLM-0001', 'Open', DATE_SUB(CURDATE(), INTERVAL 10 DAY), DATE_SUB(CURDATE(), INTERVAL 7 DAY), 5000, 0, 'Roof damage', 1, 2);

-- Claim notes + Document
INSERT INTO claim_notes (claim_id, author_user_id, note_text)
VALUES (1, 2, 'Initial assessment completed.');

INSERT INTO documents (file_name, url, uploaded_by, attached_to_type, attached_to_id, description)
VALUES ('inspection.pdf', 'https://files.example/inspection.pdf', 1, 'Policy', 1, 'Initial inspection report');

-- Releases (changelog)
INSERT INTO releases (version, start_date, notes)
VALUES ('v0.2.0', CURDATE(), 'Employee hub improvements + audit + announcements');

-- Announcements + Login audit (for dashboard)
INSERT INTO announcements (title, body, publish_at, created_by)
VALUES ('Welcome to the Employee Hub', 'Please review MFA policy.', NOW(), 2);

INSERT INTO login_audit (user_id, event, ip_address, user_agent, occurred_at) VALUES
(2, 'LOGIN_SUCCESS', '127.0.0.1', 'Chrome', NOW() - INTERVAL 1 DAY),
(2, 'LOGOUT',        '127.0.0.1', 'Chrome', NOW() - INTERVAL 12 HOUR);
