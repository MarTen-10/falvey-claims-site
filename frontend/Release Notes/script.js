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
    console.log('API Response:', data);
    releases = data.data || data;
    console.log('Releases loaded:', releases);
    if (releases.length > 0) {
      setup();
    } else {
      console.warn('No releases found');
      setup(); // Still call setup to show admin controls
    }
  })
  .catch(error => {
    console.error('Error fetching releases:', error);
    //security alert
    alert('Failed to load releases. Please check if the backend is running at ' + API_BASE);
  });

// Update the details panel with the given release object
function renderDetails(release) {
  const titleEl = document.getElementById('detailsTitle');
  const verEl = document.getElementById('detailsVersion');
  const dateEl = document.getElementById('detailsDate');
  const notesEl = document.getElementById('detailsNotes');

  titleEl.textContent = `Release ${release.version}`;
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

  // Hotfix notes section
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

  // Replace the details panel save-row with an Edit button when admin
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

  if (releases.length === 0) {
    list.innerHTML = '<li style="padding: 1rem; color: #6b7280;">No releases yet. Click "Add Release" to create one.</li>';
    return;
  }

  // sort releases newest first by start_date
  releases.sort((a, b) => {
    const dateA = a.start_date ? new Date(a.start_date) : new Date(0);
    const dateB = b.start_date ? new Date(b.start_date) : new Date(0);
    return dateB - dateA;
  });

  releases.forEach(r => {
    // create the card list item for this release
    const li = document.createElement('li');
    li.className = 'card-item version-item';
    li.setAttribute('data-version', r.version);

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
    li.appendChild(body);

    // footer will hold the details action and an actions area for edit/delete
    const footer = document.createElement('div');
    footer.className = 'card-footer';

    // details button
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

    footer.appendChild(detailsBtn);

    // Admin controls
    if (isAdmin) {
      const actionsDiv = document.createElement('div');
      actionsDiv.className = 'card-actions';

      // edit icon
      const editBtn = document.createElement('button');
      editBtn.className = 'action-btn edit';
      editBtn.innerHTML = '<span aria-hidden="true">✎</span><span class="label">Edit</span>';
      editBtn.addEventListener('click', (e) => { e.stopPropagation(); openReleaseModal('edit', r.version); });
      
      // delete icon
      const deleteBtn = document.createElement('button');
      deleteBtn.className = 'action-btn delete';
      deleteBtn.innerHTML = '<span aria-hidden="true">🗑</span><span class="label">Delete</span>';
      deleteBtn.addEventListener('click', (e) => { e.stopPropagation(); deleteRelease(r.version); });
      
      actionsDiv.appendChild(editBtn);
      actionsDiv.appendChild(deleteBtn);
      footer.appendChild(actionsDiv);
    }

    li.appendChild(footer);
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
  const modalError = document.getElementById('modalError');
  
  // Clear any previous errors
  modalError.textContent = '';
  modalError.style.display = 'none';
  
  document.getElementById('formMode').value = mode;

  if (mode === 'edit') {
    const rel = releases.find(r => r.version === version);
    if (!rel) {
      alert('Release not found');
      return;
    }
    titleEl.textContent = 'Edit Release';
    document.getElementById('releaseVersion').value = rel.version;
    document.getElementById('releaseVersion').disabled = true; // Can't change version (it's the primary key)
    document.getElementById('releaseTitle').value = rel.version; // Using version as title
    document.getElementById('releaseDate').value = rel.start_date ? rel.start_date : '';
    document.getElementById('releaseNotes').value = rel.notes || '';
    document.getElementById('originalVersion').value = rel.version;
  } else {
    titleEl.textContent = 'Add Release';
    form.reset();
    document.getElementById('releaseVersion').disabled = false;
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
    console.log('Deleting release:', _pendingDeleteVersion);
    const response = await fetch(`${API_BASE}/${_pendingDeleteVersion}`, {
      method: 'DELETE'
    });

    console.log('Delete response status:', response.status);

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Delete error:', errorText);
      throw new Error(errorText || 'Failed to delete release');
    }

    // Refresh releases from server
    const refreshResponse = await fetch(API_BASE);
    if (!refreshResponse.ok) throw new Error('Failed to refresh releases');
    const data = await refreshResponse.json();
    releases = data.data || data;
    
    closeDeleteModal();
    renderVersionList();
    alert('Release deleted successfully!');
  } catch (error) {
    console.error('Error deleting release:', error);
    alert('Failed to delete release: ' + error.message);
    closeDeleteModal();
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
  const notes = document.getElementById('releaseNotes').value.trim();
  const modalError = document.getElementById('modalError');

  console.log('Form submit:', { mode, version, start_date, notes });

  // Validation
  if (!version) {
    modalError.textContent = 'Version is required';
    modalError.style.display = 'block';
    return;
  }

  if (!start_date) {
    modalError.textContent = 'Start date is required';
    modalError.style.display = 'block';
    return;
  }

  // Year validation (allow current year ±5 years)
  const year = new Date(start_date).getFullYear();
  const currentYear = new Date().getFullYear();
  const minYear = currentYear - 1;
  const maxYear = currentYear + 5;
  if (isNaN(year) || year < minYear || year > maxYear) {
    modalError.textContent = `Start date year must be between ${minYear} and ${maxYear}`;
    modalError.style.display = 'block';
    return;
  }

  // Clear errors
  modalError.textContent = '';
  modalError.style.display = 'none';

  // Build the release data object - match your C# Release model exactly
  const releaseData = {
    version: version,
    start_date: start_date,  // Send as YYYY-MM-DD string, backend will convert to DateOnly
    rollout_date: null,
    complete_date: null,
    notes: notes || null,
    hotfix_notes: null
  };

  console.log('Sending data:', releaseData);

  try {
    let response;
    
    if (mode === 'add') {
      response = await fetch(API_BASE, {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify(releaseData)
      });
    } else if (mode === 'edit') {
      response = await fetch(`${API_BASE}/${original}`, {
        method: 'PUT',
        headers: { 
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify(releaseData)
      });
    }

    console.log('Response status:', response.status);

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Error response:', errorText);
      
      // Try to parse as JSON for better error messages
      try {
        const errorJson = JSON.parse(errorText);
        throw new Error(errorJson.error || errorJson.title || errorText);
      } catch {
        throw new Error(errorText || `Failed to ${mode} release`);
      }
    }

    // Refresh releases from server
    const refreshResponse = await fetch(API_BASE);
    if (!refreshResponse.ok) throw new Error('Failed to refresh releases');
    const data = await refreshResponse.json();
    releases = data.data || data;
    
    closeReleaseModal();
    renderVersionList();
    alert(`Release ${mode === 'add' ? 'added' : 'updated'} successfully!`);
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
    
    // Add Release button
    const addBtn = document.getElementById('addReleaseBtn');
    if (addBtn) {
      addBtn.addEventListener('click', () => openReleaseModal('add'));
    }
    
    // Modal controls
    const modalClose = document.getElementById('modalClose');
    const modalCancel = document.getElementById('modalCancel');
    const modalBackdrop = document.getElementById('modalBackdrop');
    const releaseForm = document.getElementById('releaseForm');
    
    if (modalClose) modalClose.addEventListener('click', closeReleaseModal);
    if (modalCancel) modalCancel.addEventListener('click', closeReleaseModal);
    if (modalBackdrop) modalBackdrop.addEventListener('click', closeReleaseModal);
    if (releaseForm) releaseForm.addEventListener('submit', _handleFormSubmit);
    
    // Delete modal wiring
    const deleteModalClose = document.getElementById('deleteModalClose');
    const cancelDeleteBtn = document.getElementById('cancelDeleteBtn');
    const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');
    const deleteBackdrop = document.getElementById('deleteBackdrop');
    
    if (deleteModalClose) deleteModalClose.addEventListener('click', cancelDelete);
    if (cancelDeleteBtn) cancelDeleteBtn.addEventListener('click', cancelDelete);
    if (confirmDeleteBtn) confirmDeleteBtn.addEventListener('click', confirmDelete);
    if (deleteBackdrop) deleteBackdrop.addEventListener('click', cancelDelete);
  } else {
    adminControls.setAttribute('aria-hidden', 'true');
    adminControls.style.display = 'none';
  }
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', setup);
} else {
  setup();
}