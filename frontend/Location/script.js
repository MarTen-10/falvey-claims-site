document.addEventListener("DOMContentLoaded", () => {
  const logoutBtn = document.getElementById("logoutBtn");
  logoutBtn.addEventListener("click", (e) => {
    e.preventDefault();
    if (confirm("Are you sure you want to log out?")) {
      localStorage.removeItem("loggedInUser");
      window.location.href = "../login.html";
    }
  });

  /*COMPANY SELECTOR */
  const companySelect = document.querySelector(".company_select");
  companySelect.addEventListener("change", () => {
    const selected = companySelect.value;
    console.log(`Selected company: ${selected}`);

    const notice = document.createElement("div");
    notice.textContent = `✔ Company switched to ${selected}`;
    notice.style.cssText = `
      position: fixed; bottom: 20px; right: 20px;
      background: #1a2e59; color: white;
      padding: 10px 20px; border-radius: 8px;
      font-size: 14px;
    `;
    document.body.appendChild(notice);
    setTimeout(() => notice.remove(), 2500);
  });

  /* SEARCH BOX */
  const searchBtn = document.querySelector(".search-btn");
  const searchInput = document.querySelector(".search-box input");

  searchBtn.addEventListener("click", () => {
    const query = searchInput.value.trim();
    if (query.length < 3) {
      alert("Please enter at least 3 characters.");
      return;
    }
    window.location.href = `../search.html?q=${encodeURIComponent(query)}`;
  });

  /* FILE UPLOAD preview & download */
  const fileInput = document.getElementById("fileInput");
  const dropZone = document.getElementById("dropZone");
  const tableBody = document.querySelector(".files-uploaded tbody");

  let uploadedFiles = [];

  if (localStorage.getItem("uploadedFiles")) {
    uploadedFiles = JSON.parse(localStorage.getItem("uploadedFiles"));
    renderTable();
  }

  // Handle drag & drop
  dropZone.addEventListener("dragover", (e) => {
    e.preventDefault();
    dropZone.style.backgroundColor = "#e0e7ff";
  });
  dropZone.addEventListener("dragleave", () => {
    dropZone.style.backgroundColor = "";
  });
  dropZone.addEventListener("drop", (e) => {
    e.preventDefault();
    dropZone.style.backgroundColor = "";
    handleFiles(e.dataTransfer.files);
  });

  // Handle click to browse
  dropZone.addEventListener("click", () => fileInput.click());
  fileInput.addEventListener("change", (e) => handleFiles(e.target.files));

  function handleFiles(files) {
    Array.from(files).forEach((file) => {
      const fileURL = URL.createObjectURL(file);
      const now = new Date().toLocaleString();
      const fileInfo = {
        type: getFileType(file.name),
        name: file.name,
        time: now,
        url: fileURL,
      };
      uploadedFiles.push(fileInfo);
    });

    // Save to localStorage
    localStorage.setItem("uploadedFiles", JSON.stringify(uploadedFiles));
    renderTable();
  }

  function getFileType(fileName) {
    const ext = fileName.split(".").pop().toLowerCase();
    if (["jpg", "jpeg", "png", "gif"].includes(ext)) return "Image";
    if (ext === "pdf") return "PDF";
    if (["doc", "docx"].includes(ext)) return "Word";
    return "Other";
  }

  function renderTable() {
    tableBody.innerHTML = "";
    if (uploadedFiles.length === 0) {
      tableBody.innerHTML =
        '<tr><td colspan="4" class="no-files">No files uploaded yet.</td></tr>';
      return;
    }

    uploadedFiles.forEach((file, index) => {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${file.type}</td>
        <td>${file.name}</td>
        <td>${file.time}</td>
        <td>
          <a href="${file.url}" download="${file.name}" class="download-btn">⬇</a>
          <button class="delete-btn" data-index="${index}" style="margin-left:8px;">🗑️</button>
        </td>
      `;
      tableBody.appendChild(row);
    });

    // Handle delete buttons
    document.querySelectorAll(".delete-btn").forEach((btn) => {
      btn.addEventListener("click", () => {
        const idx = parseInt(btn.dataset.index);
        uploadedFiles.splice(idx, 1);
        localStorage.setItem("uploadedFiles", JSON.stringify(uploadedFiles));
        renderTable();
      });
    });
  }

  /*SHARE LOCATION*/
  const shareBtn = document.querySelector(".share-btn");
  shareBtn.addEventListener("click", async () => {
    const locationURL = window.location.href;
    try {
      await navigator.clipboard.writeText(locationURL);
      alert("📍 Location info copied to clipboard!");
    } catch (err) {
      console.error("Clipboard failed", err);
      alert("Could not copy location link.");
    }
  });

  /***  REQUEST LOCATION INFO */
  const requestBtn = document.querySelector(".btn.secondary");
  const requestModal = document.getElementById("requestModal");
  const cancelRequest = document.getElementById("cancelRequest");
  const cancelOnsite = document.getElementById("cancelOnsite");
  const tabButtons = document.querySelectorAll(".tab-button");
  const tabContents = document.querySelectorAll(".tab-content");

  function closeModal() {
    requestModal.classList.remove("active");
    document.body.style.overflow = "";
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  requestBtn.addEventListener("click", () => {
    requestModal.classList.add("active");
    document.body.style.overflow = "hidden";
  });

  cancelRequest.addEventListener("click", closeModal);
  if (cancelOnsite) cancelOnsite.addEventListener("click", closeModal);

  requestModal.addEventListener("click", (e) => {
    if (e.target === requestModal) {
      closeModal();
    }
  });

  tabButtons.forEach((btn) => {
    btn.addEventListener("click", () => {
      tabButtons.forEach((b) => b.classList.remove("active"));
      tabContents.forEach((tab) => tab.classList.remove("active"));
      btn.classList.add("active");
      document.getElementById(btn.dataset.tab).classList.add("active");
    });
  });

  document.getElementById("saveDigital").addEventListener("click", () => {
    alert("✅ Digital COPE Supplement saved successfully!");
    closeModal();
  });

  document.getElementById("saveOnsite").addEventListener("click", () => {
    alert("✅ Onsite COPE Supplement saved successfully!");
    closeModal();
  });
});
