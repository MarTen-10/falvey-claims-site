// script.js - Release Notes interactivity
// Uses the ReleasesController API to manage releases

// API endpoints
const API_BASE = 'http://localhost:5113/api/releases';

// Toggle this value to true to show admin controls (in a real app, derive from auth)
let isAdmin = true;

// State management
let releases = [];
let selectedRelease = null;

// Initial fetch of releases
fetch(API_BASE)
  .then(response => {
    if (!response.ok) throw new Error('Failed to fetch releases');
    return response.json();
  })
  .then(data => {
    // Fix: Extract the array from the response object
    console.log('API Response:', data);
    releases = data.data || data; // Handle both wrapped and unwrapped responses
    if (releases.length > 0) {
      setup();
    } else {
      console.warn('No releases found');
    }
  })
  .catch(error => {
    console.error('Error fetching releases:', error);
  });

// Update the details panel with the given release object
function renderDetails(release) {
  const titleEl = document.getElementById('detailsTitle');
  const verEl = document.getElementById('detailsVersion');
  const dateEl = document.getElementById('detailsDate');
  const notesEl = document.getElementById('detailsNotes');

  // Version and dates
  verEl.textContent = release.version;
  dateEl.textContent = release.start_date ? new Date(release.start_date).toLocaleDateString('en-US', {
    year: 'numeric', month: 'long', day: 'numeric'
  }) : 'No start date';

  // Main release notes
  if (release.notes) {
    const lines = release.notes.trim().split('\n').map(l => l.trim()).filter(l => l.length > 0);
    const listItems = lines.filter(l => l.startsWith('-')).map(l => `<li>${l.replace(/^[-\s]+/, '')}</li>`).join('');
    const otherLines = lines.filter(l => !l.startsWith('-')).map(l => `<p>${l}</p>`).join('');

    if (listItems) {
      notesEl.innerHTML = `<ul>${listItems}</ul>${otherLines}`;
    } else {
      notesEl.innerHTML = otherLines;
    }
  } else {
    notesEl.innerHTML = '<p>No notes available.</p>';
  }

  // Hotfix notes section (if you have this element in HTML)
  const hotfixEl = document.getElementById('detailsHotfixNotes');
  if (hotfixEl && release.hotfix_notes) {
    const lines = release.hotfix_notes.trim().split('\n').map(l => l.trim()).filter(l => l.length > 0);
    const listItems = lines.filter(l => l.startsWith('-')).map(l => `<li>${l.replace(/^[-\s]+/, '')}</li>`).join('');
    const otherLines = lines.filter(l => !l.startsWith('-')).map(l => `<p>${l}</p>`).join('');

    hotfixEl.style.display = '';
    hotfixEl.innerHTML = `
      <h3>Hotfix Notes</h3>
      ${listItems ? `<ul>${listItems}</ul>` : ''}
      ${otherLines}
    `;
  } else if (hotfixEl) {
    hotfixEl.style.display = 'none';
  }

  // Replace the details panel save-row with an Edit button when admin; hide otherwise
  const saveRow = document.querySelector('.details .panel .save-row');
  if (saveRow) {
    saveRow.innerHTML = '';
    if (isAdmin) {
      const editBtn = document.createElement('button');
      editBtn.type = 'button';
      editBtn.className = 'edit-details-btn btn-primary';
      editBtn.textContent = 'Edit';
      editBtn.addEventListener('click', () => openReleaseModal('edit', release.version));
      saveRow.appendChild(editBtn);
      saveRow.style.display = '';
    } else {
      saveRow.style.display = 'none';
    }
  }
}

// Render the versions list (left column) and the mobile select dropdown
function renderVersionList() {
  const list = document.getElementById('versionList');
  const select = document.getElementById('versionSelect');
  list.innerHTML = '';
  select.innerHTML = '';

  // sort releases newest first by date so UI is chronological descending
  releases.sort((a, b) => new Date(b.start_date) - new Date(a.start_date));

  releases.forEach(r => {
    // create the card list item for this release
    const li = document.createElement('li');
    li.className = 'card-item version-item';
    li.setAttribute('data-version', r.version);

    // footer will hold the details action and an actions area for edit/delete
    const footer = document.createElement('div');
    footer.className = 'card-footer';

    // edit icon (left) – visible only when isAdmin
    const editBtn = document.createElement('button');
    editBtn.className = 'action-btn edit';
    editBtn.innerHTML = '<span aria-hidden="true">✎</span><span class="label">Edit</span>';
    editBtn.addEventListener('click', (e) => { e.stopPropagation(); openReleaseModal('edit', r.version); });
    if (isAdmin) {
      editBtn.style.display = '';
    } else {
      editBtn.style.display = 'none';
    }

    // details button (selects and shows release details in right panel)
    const detailsBtn = document.createElement('button');
    detailsBtn.className = 'details-btn';
    detailsBtn.setAttribute('aria-controls', 'details');
    detailsBtn.setAttribute('aria-expanded', 'false');
    detailsBtn.textContent = 'Details';
    detailsBtn.addEventListener('click', () => {
      list.querySelectorAll('.details-btn').forEach(b => b.setAttribute('aria-expanded', 'false'));
      detailsBtn.setAttribute('aria-expanded', 'true');
      renderDetails(r);
    });

    // delete icon (right) – visible only when isAdmin
    const deleteBtn = document.createElement('button');
    deleteBtn.className = 'action-btn delete';
    deleteBtn.innerHTML = '<span aria-hidden="true">🗑</span><span class="label">Delete</span>';
    deleteBtn.addEventListener('click', (e) => { e.stopPropagation(); deleteRelease(r.version); });
    if (isAdmin) {
      deleteBtn.style.display = '';
    } else {
      deleteBtn.style.display = 'none';
    }

    footer.appendChild(detailsBtn);
    const actionsDiv = document.createElement('div');
    actionsDiv.className = 'card-actions';
    actionsDiv.appendChild(editBtn);
    actionsDiv.appendChild(deleteBtn);
    footer.appendChild(actionsDiv);
    li.appendChild(footer);

    // body area for card content
    const body = document.createElement('div');
    body.className = 'card-body';
    const title = document.createElement('div');
    title.className = 'card-title';
    title.textContent = `${r.version}`;
    
    const meta = document.createElement('div');
    meta.className = 'card-meta';
    const dates = [];
    if (r.start_date) dates.push(`Started: ${new Date(r.start_date).toLocaleDateString()}`);
    if (r.rollout_date) dates.push(`Rolled out: ${new Date(r.rollout_date).toLocaleDateString()}`);
    if (r.complete_date) dates.push(`Completed: ${new Date(r.complete_date).toLocaleDateString()}`);
    meta.textContent = dates.join(' | ');
    
    const excerpt = document.createElement('div');
    excerpt.className = 'card-excerpt muted';
    if (r.notes) {
      excerpt.textContent = r.notes.trim().split('\n')[0]?.replace(/^-\s*/g, '') || '';
    }

    body.appendChild(title);
    body.appendChild(meta);
    if (r.notes) {
      body.appendChild(excerpt);
    }
    li.insertBefore(body, li.firstChild);

    list.appendChild(li);

    // populate the mobile select option
    const opt = document.createElement('option');
    opt.value = r.version;
    opt.textContent = `${r.version}${r.notes ? ` - ${r.notes.trim().split('\n')[0]?.replace(/^-\s*/g, '')}` : ''}`;
    select.appendChild(opt);
  });

  // mobile select -> render details for the chosen version
  select.onchange = () => {
    const v = select.value;
    const rel = releases.find(x => x.version === v);
    if (rel) {
      const btn = list.querySelector(`.version-item[data-version="${v}"] .details-btn`);
      if (btn) {
        list.querySelectorAll('.details-btn').forEach(b => b.setAttribute('aria-expanded', 'false'));
        btn.setAttribute('aria-expanded', 'true');
      }
      renderDetails(rel);
    }
  };

  // Automatically open the newest release after rendering
  if (releases.length > 0) {
    const first = releases[0];
    const firstBtn = list.querySelector(`.version-item[data-version="${first.version}"] .details-btn`);
    if (firstBtn) firstBtn.click();
  }
}

// Admin functions and modal lifecycle
let _lastActiveElement = null;

function openReleaseModal(mode, version) {
  const modal = document.getElementById('releaseModal');
  const titleEl = document.getElementById('modalTitle');
  const form = document.getElementById('releaseForm');
  document.getElementById('formMode').value = mode;

  if (mode === 'edit') {
    const rel = releases.find(r => r.version === version);
    if (!rel) return;
    titleEl.textContent = 'Edit Release';
    document.getElementById('releaseVersion').value = rel.version;
    document.getElementById('releaseDate').value = rel.start_date || new Date().toISOString().slice(0,10);
    document.getElementById('releaseNotes').value = rel.notes?.trim() || '';
    document.getElementById('originalVersion').value = rel.version;
  } else {
    titleEl.textContent = 'Add Release';
    form.reset();
    document.getElementById('releaseDate').value = new Date().toISOString().slice(0,10);
    document.getElementById('originalVersion').value = '';
  }

  modal.setAttribute('aria-hidden', 'false');
  modal.style.display = 'block';
  _lastActiveElement = document.activeElement;
  document.getElementById('releaseVersion').focus();
  document.addEventListener('keydown', _handleModalKeydown);
}

function closeReleaseModal() {
  const modal = document.getElementById('releaseModal');
  modal.setAttribute('aria-hidden', 'true');
  modal.style.display = 'none';
  document.removeEventListener('keydown', _handleModalKeydown);
  if (_lastActiveElement && typeof _lastActiveElement.focus === 'function') {
    _lastActiveElement.focus();
  }
}

function _handleModalKeydown(e) {
  if (e.key === 'Escape') closeReleaseModal();
}

function deleteRelease(version) {
  openDeleteModal(version);
}

// Delete modal handling
let _pendingDeleteVersion = null;
function openDeleteModal(version) {
  _pendingDeleteVersion = version;
  const modal = document.getElementById('deleteModal');
  document.getElementById('deleteVersionLabel').textContent = version;
  modal.setAttribute('aria-hidden', 'false');
  modal.style.display = 'block';
  document.addEventListener('keydown', _handleDeleteModalKeydown);
}

function closeDeleteModal() {
  const modal = document.getElementById('deleteModal');
  modal.setAttribute('aria-hidden', 'true');
  modal.style.display = 'none';
  _pendingDeleteVersion = null;
  document.removeEventListener('keydown', _handleDeleteModalKeydown);
}

function _handleDeleteModalKeydown(e) {
  if (e.key === 'Escape') closeDeleteModal();
}

async function confirmDelete() {
  if (!_pendingDeleteVersion) return closeDeleteModal();
  
  try {
    const response = await fetch(`${API_BASE}/${_pendingDeleteVersion}`, {
      method: 'DELETE'
    });

    if (!response.ok) throw new Error('Failed to delete release');

    // Refresh releases from server
    const refreshResponse = await fetch(API_BASE);
    if (!refreshResponse.ok) throw new Error('Failed to refresh releases');
    const data = await refreshResponse.json();
    releases = data.data || data;
    
    closeDeleteModal();
    renderVersionList();
  } catch (error) {
    console.error('Error deleting release:', error);
    alert('Failed to delete release. Please try again.');
  }
}

function cancelDelete() {
  closeDeleteModal();
}

// Form submit handler
async function _handleFormSubmit(e) {
  e.preventDefault();
  const mode = document.getElementById('formMode').value;
  const original = document.getElementById('originalVersion').value;
  const version = document.getElementById('releaseVersion').value.trim();
  const start_date = document.getElementById('releaseDate').value;
  let notes = document.getElementById('releaseNotes').value || '';
  const modalError = document.getElementById('modalError');

  if (!version || !start_date) {
    modalError.textContent = 'Please fill in required fields: version and start date.';
    modalError.style.display = 'block';
    return;
  }

  const year = (new Date(start_date)).getFullYear();
  if (isNaN(year) || year < 2024 || year > 2026) {
    modalError.textContent = 'Start date year must be between 2024 and 2026.';
    modalError.style.display = 'block';
    return;
  }

  modalError.textContent = '';
  modalError.style.display = 'none';

  const releaseData = {
    version,
    start_date,
    rollout_date: null,
    complete_date: null,
    notes: notes.trim() || null,
    hotfix_notes: null
  };

  try {
    let response;
    
    if (mode === 'add') {
      response = await fetch(API_BASE, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(releaseData)
      });
    } else if (mode === 'edit') {
      response = await fetch(`${API_BASE}/${original}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(releaseData)
      });
    }

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to save release');
    }

    // Refresh releases
    const refreshResponse = await fetch(API_BASE);
    if (!refreshResponse.ok) throw new Error('Failed to refresh releases');
    const data = await refreshResponse.json();
    releases = data.data || data;
    
    closeReleaseModal();
    renderVersionList();
  } catch (error) {
    console.error('Error saving release:', error);
    modalError.textContent = error.message || 'Failed to save release. Please try again.';
    modalError.style.display = 'block';
  }
}

// Initialization
function setup() {
  renderVersionList();

  const adminControls = document.getElementById('adminControls');
  if (isAdmin) {
    adminControls.setAttribute('aria-hidden', 'false');
    adminControls.style.display = 'block';
    document.getElementById('addReleaseBtn').addEventListener('click', () => openReleaseModal('add'));
    document.getElementById('modalClose').addEventListener('click', closeReleaseModal);
    document.getElementById('modalCancel').addEventListener('click', closeReleaseModal);
    document.getElementById('modalBackdrop').addEventListener('click', closeReleaseModal);
    document.getElementById('releaseForm').addEventListener('submit', _handleFormSubmit);
    document.getElementById('deleteModalClose').addEventListener('click', cancelDelete);
    document.getElementById('cancelDeleteBtn').addEventListener('click', cancelDelete);
    document.getElementById('confirmDeleteBtn').addEventListener('click', confirmDelete);
    document.getElementById('deleteBackdrop').addEventListener('click', cancelDelete);
  } else {
    adminControls.setAttribute('aria-hidden', 'true');
    adminControls.style.display = 'none';
  }
}

// initialize
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', setup);
} else {
  setup();
}