/* ============================================================
   INSURANCE MANAGEMENT – PROTOTYPE DATABASE (MySQL 8+)
   ------------------------------------------------------------
   This database is designed for an insurance company system.
   - Customers own policies
   - Policies may have claims
   - Employees manage policies/claims
   - Users table provides logins for both customers & employees
   - Sessions table stores active logins (like cookies)
   - Documents table attaches files to customers, policies, or claims
   - Releases table tracks system version updates
   ============================================================ */

-- Make sure we start fresh by removing old tables (safe for dev)
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

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
-- Customers: who owns policies
-- ======================
CREATE TABLE customers (
  customer_id   INT AUTO_INCREMENT PRIMARY KEY, -- unique ID for each customer
  name          VARCHAR(80)  NOT NULL,          -- company or person name
  email         VARCHAR(120),                   -- contact email
  phone         VARCHAR(20),                    -- phone number
  addr_line1    VARCHAR(80),                    -- address fields
  addr_line2    VARCHAR(80),
  city          VARCHAR(50),
  state_code    VARCHAR(10),
  zip_code      VARCHAR(12),
  created_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP -- when the record was created
) ENGINE=InnoDB;

CREATE INDEX ix_customers_name ON customers (name); -- speed up customer search by name

-- ======================
-- Employees: staff or agents
-- ======================
CREATE TABLE employees (
  employee_id   INT AUTO_INCREMENT PRIMARY KEY, -- unique ID for each employee
  name          VARCHAR(80)  NOT NULL,          -- full name
  title         VARCHAR(40),                    -- job title (Agent, Adjuster, etc.)
  email         VARCHAR(120),                   -- email contact
  phone         VARCHAR(20),
  status        VARCHAR(20) DEFAULT 'Active',   -- free text: Active / Inactive
  created_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE INDEX ix_employees_name  ON employees (name);
CREATE INDEX ix_employees_email ON employees (email);

-- ======================
-- Users: login accounts
-- ======================
-- A "user" is someone who can log in:
--   - If role = 'Customer', it links to a customer_id
--   - If role = 'Employee' or 'Admin', it links to an employee_id
CREATE TABLE users (
  user_id       INT AUTO_INCREMENT PRIMARY KEY,
  email         VARCHAR(120) UNIQUE NOT NULL,   -- login username
  password_hash VARCHAR(200) NOT NULL,          -- hashed password (bcrypt/argon2)
  role          ENUM('Customer','Employee','Admin') NOT NULL, -- role for permissions
  customer_id   INT NULL,                       -- link if customer
  employee_id   INT NULL,                       -- link if employee
  is_active     BOOLEAN DEFAULT TRUE,           -- disable logins if needed
  created_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_users_customer FOREIGN KEY (customer_id) REFERENCES customers(customer_id),
  CONSTRAINT fk_users_employee FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
) ENGINE=InnoDB;

CREATE INDEX ix_users_email ON users (email);
CREATE INDEX ix_users_role  ON users (role);

-- ======================
-- Policies: insurance policies
-- ======================
CREATE TABLE policies (
  policy_id       INT AUTO_INCREMENT PRIMARY KEY,
  account_number  VARCHAR(24) UNIQUE NOT NULL,  -- external policy/account reference
  customer_id     INT NOT NULL,                 -- who owns the policy
  manager_id      INT NULL,                     -- employee managing it
  policy_type     VARCHAR(30),                  -- e.g., Auto, Property, Liability
  status          ENUM('Active','Pending','Cancelled','Expired') DEFAULT 'Active',
  start_date      DATE,
  end_date        DATE,
  exposure_amount DECIMAL(13,2),                -- coverage amount
  loc_addr1       VARCHAR(80),                  -- policy location
  loc_addr2       VARCHAR(80),
  loc_city        VARCHAR(50),
  loc_state       VARCHAR(10),
  loc_zip         VARCHAR(12),
  created_at      TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_policies_customer FOREIGN KEY (customer_id) REFERENCES customers(customer_id),
  CONSTRAINT fk_policies_manager  FOREIGN KEY (manager_id) REFERENCES employees(employee_id)
) ENGINE=InnoDB;

CREATE INDEX ix_policies_customer ON policies (customer_id);
CREATE INDEX ix_policies_status   ON policies (status);

-- ======================
-- Claims: insurance claims under a policy
-- ======================
CREATE TABLE claims (
  claim_id       INT AUTO_INCREMENT PRIMARY KEY,
  policy_id      INT NOT NULL,                 -- which policy the claim belongs to
  claim_number   VARCHAR(24) UNIQUE NOT NULL,  -- external claim reference
  status         ENUM('Open','Closed','Pending') DEFAULT 'Open',
  date_of_loss   DATE,                         -- when the damage/loss occurred
  date_reported  DATE,                         -- when it was reported
  reserve_amount DECIMAL(13,2) DEFAULT 0,      -- money set aside
  paid_amount    DECIMAL(13,2) DEFAULT 0,      -- money already paid
  memo           VARCHAR(250),                 -- notes
  created_at     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_claims_policy FOREIGN KEY (policy_id) REFERENCES policies(policy_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_claims_policy ON claims (policy_id);
CREATE INDEX ix_claims_status ON claims (status);

-- ======================
-- Documents: upload/attach files
-- ======================
-- Can attach to any entity type: Customer, Policy, Claim
CREATE TABLE documents (
  document_id      INT AUTO_INCREMENT PRIMARY KEY,
  file_name        VARCHAR(160) NOT NULL,      -- file name
  url              VARCHAR(400) NOT NULL,      -- storage location (URL or path)
  uploaded_by      INT NULL,                   -- employee who uploaded
  uploaded_at      TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  attached_to_type ENUM('Customer','Policy','Claim') NOT NULL,
  attached_to_id   INT NOT NULL,               -- ID of the entity it’s attached to
  description      VARCHAR(250),
  CONSTRAINT fk_documents_employee FOREIGN KEY (uploaded_by) REFERENCES employees(employee_id)
) ENGINE=InnoDB;

CREATE INDEX ix_documents_target ON documents (attached_to_type, attached_to_id);

-- ======================
-- Releases: system version notes
-- ======================
CREATE TABLE releases (
  version        VARCHAR(20) PRIMARY KEY,      -- version name
  start_date     DATE,
  rollout_date   DATE,
  complete_date  DATE,
  notes          VARCHAR(250),
  hotfix_notes   VARCHAR(250)
) ENGINE=InnoDB;

-- ======================
-- Sessions: active login sessions
-- ======================
-- Stores login sessions like cookies; ties them to users
CREATE TABLE sessions (
  session_id    CHAR(36) PRIMARY KEY DEFAULT (UUID()), -- unique session ID
  user_id       INT NOT NULL,
  session_hash  VARCHAR(200) NOT NULL,   -- hash of random cookie token
  created_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  expires_at    TIMESTAMP NOT NULL,      -- when session expires
  revoked_at    TIMESTAMP NULL,          -- set when user logs out
  ip_address    VARCHAR(45),             -- track login IP
  user_agent    VARCHAR(300),            -- track browser/device
  CONSTRAINT fk_sessions_user FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE INDEX ix_sessions_user_active ON sessions (user_id, expires_at);
CREATE INDEX ix_sessions_expiry      ON sessions (expires_at);

-- ============================================================
-- 2) SEED DATA (example rows for testing)
-- ============================================================

-- Example customer
INSERT INTO customers (name, email, phone, city, state_code)
VALUES ('ABC Manufacturing', 'abc@client.com', '401-555-1000', 'Providence', 'RI');

-- Example employee
INSERT INTO employees (name, title, email)
VALUES ('Jane Doe', 'Agent', 'jane.doe@insco.com');

-- Example users (replace hashes with real bcrypt/argon2 hashes)
INSERT INTO users (email, password_hash, role, customer_id)
VALUES ('abc@client.com', '$2b$10$replace_me_customer', 'Customer', 1);

INSERT INTO users (email, password_hash, role, employee_id)
VALUES ('jane.doe@insco.com', '$2b$10$replace_me_employee', 'Employee', 1);

-- Example policy
INSERT INTO policies (account_number, customer_id, manager_id, policy_type, status, start_date, end_date, exposure_amount, loc_city, loc_state)
VALUES ('POL-0001', 1, 1, 'Property', 'Active', CURDATE(), DATE_ADD(CURDATE(), INTERVAL 1 YEAR), 250000, 'Cranston', 'RI');

-- Example claim
INSERT INTO claims (policy_id, claim_number, status, date_of_loss, date_reported, reserve_amount, paid_amount, memo)
VALUES (1, 'CLM-0001', 'Open', DATE_SUB(CURDATE(), INTERVAL 10 DAY), DATE_SUB(CURDATE(), INTERVAL 7 DAY), 5000, 0, 'Roof damage');

-- Example document attached to a policy
INSERT INTO documents (file_name, url, uploaded_by, attached_to_type, attached_to_id, description)
VALUES ('inspection.pdf', 'https://files.example/inspection.pdf', 1, 'Policy', 1, 'Initial inspection report');

-- Example release entry
INSERT INTO releases (version, start_date, notes) 
VALUES ('v0.1.0', CURDATE(), 'Prototype seed data loaded');
