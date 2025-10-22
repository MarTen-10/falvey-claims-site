document.addEventListener("DOMContentLoaded", () => {
  /* ==== LOGOUT BUTTON ==== */
  const logoutBtn = document.getElementById("logoutBtn");
  if (logoutBtn) {
    logoutBtn.addEventListener("click", (e) => {
      e.preventDefault();
      if (confirm("Are you sure you want to log out?")) {
        localStorage.removeItem("loggedInUser");
        window.location.href = "../login.html";
      }
    });
  }

  /* COMPANY SELECTOR */
  const companySelect = document.querySelector(".company_select");
  if (companySelect) {
    companySelect.addEventListener("change", () => {
      const selected = companySelect.value;
      console.log(`Selected company: ${selected}`);

      const notice = document.createElement("div");
      notice.textContent = `✔ Company switched to ${selected}`;
      notice.style.cssText = `
        position: fixed; 
        bottom: 20px; 
        right: 20px; 
        background: #1a2e59; 
        color: white; 
        padding: 10px 20px; 
        border-radius: 8px; 
        font-size: 14px; 
        z-index: 1000;
      `;
      document.body.appendChild(notice);
      setTimeout(() => notice.remove(), 2500);
    });
  }

  /* ==== SEARCH BOX ==== */
  const searchBtn = document.querySelector(".search-btn");
  const searchInput = document.querySelector(".search-box input");

  if (searchBtn && searchInput) {
    searchBtn.addEventListener("click", () => {
      const query = searchInput.value.trim();
      if (query.length < 3) {
        alert("Please enter at least 3 characters.");
        return;
      }
      window.location.href = `../search.html?q=${encodeURIComponent(query)}`;
    });
  }

  /* ==== SHARE LOCATION BUTTON ==== */
  const shareBtn = document.querySelector(".share-btn");
  if (shareBtn) {
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
  }

  /* RECOMMENDATION / MEMOS TABS */
  const subTabs = document.querySelectorAll(".sub");
  const subContents = document.querySelectorAll(".sub-content");

  subTabs.forEach((tab) => {
    tab.addEventListener("click", () => {
      // Remove active class from all
      subTabs.forEach((t) => t.classList.remove("active"));
      subContents.forEach((c) => c.classList.remove("active"));

      // Add active class to the clicked one
      tab.classList.add("active");
      const targetId = tab.dataset.tab;
      const targetContent = document.getElementById(targetId);

      if (targetContent) {
        targetContent.classList.add("active");
      }
      if (targetId === "recommendation") {
        initializeRecommendationGrid();
      }
    });
  });
  function initializeRecommendationGrid() {
    // Your code to initialize or re-render the 8-box grid
    console.log("Recommendation grid initialized");
  }
});
