// ====== app.js ======
const express = require("express");
const cors = require("cors");
const bcrypt = require("bcrypt");
const path = require("path");
const { pool } = require("./db.js");

const app = express();
app.use(cors());
app.use(express.json());

// ===== Serve Frontend =====
app.use(express.static(path.join(__dirname, "../frontend/auth")));

app.get("/", (req, res) => {
  res.sendFile(path.join(__dirname, "../frontend/auth/index.html"));
});

// ===== Login API =====
app.post("/api/login", async (req, res) => {
  const { email, password } = req.body;
  if (!email || !password)
    return res.status(400).json({ message: "Email and password required" });

  try {
    const [rows] = await pool.query("SELECT * FROM users WHERE email = ?", [
      email,
    ]);
    const user = rows[0];
    if (!user)
      return res.status(401).json({ message: "Invalid email or password" });

    const match = await bcrypt.compare(password, user.password_hash);
    if (!match)
      return res.status(401).json({ message: "Invalid email or password" });

    res.json({
      message: "Login successful",
      user_id: user.user_id,
      role: user.role,
    });
  } catch (err) {
    console.error(err);
    res.status(500).json({ message: "Server error" });
  }
});

const PORT = 3000;
app.listen(PORT, () =>
  console.log(`✅ Server running on http://localhost:${PORT}`)
);

// ===== SIGN UP API =====
app.post("/api/signup", async (req, res) => {
  const { employee_id, email, password } = req.body;

  if (!email || !password)
    return res.status(400).json({ message: "Email and password required" });

  try {
    // Does the email already exist?
    const [existing] = await pool.query("SELECT * FROM users WHERE email = ?", [email]);
    if (existing.length > 0)
      return res.status(400).json({ message: "Email already exists" });

    const bcrypt = require("bcrypt");
    const hash = await bcrypt.hash(password, 10);
    const role = "Employee";

    // Insert new user, linking to employee if ID provided
    if (employee_id) {
      const [emp] = await pool.query(
        "SELECT employee_id FROM employees WHERE employee_id = ?",
        [employee_id]
      );
      if (emp.length === 0)
        return res.status(400).json({ message: "Employee ID not found" });

      await pool.query(
        "INSERT INTO users (email, password_hash, role, employee_id, is_active) VALUES (?, ?, ?, ?, TRUE)",
        [email, hash, role, employee_id]
      );
    } else {
      await pool.query(
        "INSERT INTO users (email, password_hash, role, is_active) VALUES (?, ?, ?, TRUE)",
        [email, hash, role]
      );
    }

    res.status(201).json({ message: "Account created successfully!" });
  } catch (err) {
    console.error("Signup error:", err);
    res.status(500).json({ message: "Server error" });
  }
});
