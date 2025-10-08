// script.js - Release Notes interactivity
// Renders the versions list and details panel, handles the mobile dropdown,
// and provides in-memory admin actions (add/edit/delete) via modals.

// Sample release data (in-memory). Each release has: version, title, date, notes
let releases = [
  {
    version: '2.1.0',
    title: 'Improved Claims Processing',
    date: '2025-09-28',
    notes: `
- Improved performance for batch claim imports.
- Fixed race condition when assigning adjusters.
- Minor UI polish and accessibility fixes.
`
  },
  {
    version: '2.0.0',
    title: 'Major Update: Policy Engine',
    date: '2025-06-12',
    notes: `
- New policy validation engine.
- Added support for bundled policies.
- Migration tools for older policy records.
`
  },
  {
    version: '1.0.0',
    title: 'Initial release',
    date: '2024-10-01',
    notes: `
- Initial release with core features.
`
  }
];

// Toggle this value to true to show admin controls (in a real app, derive from auth)
// Set to true while testing to reveal Add/Edit/Delete buttons and functionality.
let isAdmin = true;

// Update the details panel with the given release object
function renderDetails(release) {
  const titleEl = document.getElementById('detailsTitle');
  const verEl = document.getElementById('detailsVersion');
  const dateEl = document.getElementById('detailsDate');
  const notesEl = document.getElementById('detailsNotes');

  // Title / version / date are simple text replacements
  titleEl.textContent = release.title;
  verEl.textContent = release.version;
  dateEl.textContent = release.date;

  // Notes are plain text. Lines starting with '-' are converted to list items.
  const lines = release.notes.trim().split('\n').map(l => l.trim()).filter(l => l.length > 0);
  const listItems = lines.filter(l => l.startsWith('-')).map(l => `<li>${l.replace(/^[-\s]+/, '')}</li>`).join('');
  const otherLines = lines.filter(l => !l.startsWith('-')).map(l => `<p>${l}</p>`).join('');

  if (listItems) {
    // Show a list followed by any non-list paragraphs
    notesEl.innerHTML = `<ul>${listItems}</ul>${otherLines}`;
  } else {
    // Fall back to paragraphs or a placeholder
    notesEl.innerHTML = otherLines || '<p>No notes available.</p>';
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
      // keep it hidden so non-admins don't see an empty panel
      saveRow.style.display = 'none';
    }
  }
}

// Render the versions list (left column) and the mobile select dropdown
// - clears prior content, renders cards and dropdown options, and attaches handlers
// - releases are sorted newest-first by date
function renderVersionList() {
  const list = document.getElementById('versionList');
  const select = document.getElementById('versionSelect');
  list.innerHTML = '';
  select.innerHTML = '';

  // sort releases newest first by date so UI is chronological descending
  releases.sort((a, b) => new Date(b.date) - new Date(a.date));

  // Note: the mobile dropdown (#versionSelect) and the recent list (#versionList)
  // currently render the same `releases` data. The dropdown is intended to
  // eventually list more items (e.g., an archive), so we populate both here.

  // render as a card list so items look like the mock
  releases.forEach(r => {
  // create the card list item for this release
    const li = document.createElement('li');
    li.className = 'card-item version-item';
    li.setAttribute('data-version', r.version);

  // footer will hold the details action and an actions area for edit/delete
    const footer = document.createElement('div');
    footer.className = 'card-footer';

    

  // edit icon (left) — visible only when isAdmin
    const editBtn = document.createElement('button');
    editBtn.className = 'action-btn edit';
    editBtn.innerHTML = '<span aria-hidden="true">✎</span><span class="label">Edit</span>';
    editBtn.addEventListener('click', (e) => { e.stopPropagation(); openReleaseModal('edit', r.version); });
    // Explicitly control visibility so styles don't override via CSS
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
      // clear other expanded details buttons
      list.querySelectorAll('.details-btn').forEach(b => b.setAttribute('aria-expanded', 'false'));
      detailsBtn.setAttribute('aria-expanded', 'true');
      renderDetails(r);
    });

  // delete icon (right) — visible only when isAdmin
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
  // create a right-aligned actions container and move edit/delete into it
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
    title.textContent = `${r.version} — ${r.title}`;
    const meta = document.createElement('div');
    meta.className = 'card-meta';
    meta.textContent = r.date;
    const excerpt = document.createElement('div');
    excerpt.className = 'card-excerpt muted';
    excerpt.textContent = r.notes.trim().split('\n').slice(0,2).join(' ').replace(/^-\s*/g, '');

    body.appendChild(title);
    body.appendChild(meta);
    //body.appendChild(excerpt);
    // insert body before any admin actions
    li.insertBefore(body, li.firstChild);

    list.appendChild(li);

    // populate the mobile select option with version and title for clarity
    const opt = document.createElement('option');
    opt.value = r.version;
    opt.textContent = `${r.version} — ${r.title}`;
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

// Admin functions and modal lifecycle (open/close, form submit)
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
    document.getElementById('releaseTitle').value = rel.title;
    document.getElementById('releaseDate').value = rel.date;
    document.getElementById('releaseNotes').value = rel.notes.trim();
    document.getElementById('originalVersion').value = rel.version;
  } else {
    titleEl.textContent = 'Add Release';
    form.reset();
    document.getElementById('releaseDate').value = new Date().toISOString().slice(0,10);
    document.getElementById('originalVersion').value = '';
  }

  // show modal
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

// Admin: delete release (asks for confirmation)
function deleteRelease(version) {
  // open delete confirmation modal instead of using confirm()
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

function confirmDelete() {
  if (!_pendingDeleteVersion) return closeDeleteModal();
  releases = releases.filter(r => r.version !== _pendingDeleteVersion);
  closeDeleteModal();
  renderVersionList();
}

function cancelDelete() {
  closeDeleteModal();
}

// Admin: add a new release using prompt-based inputs (simple demo)
// Form-based submit handler for add/edit
function _handleFormSubmit(e) {
  e.preventDefault();
  const mode = document.getElementById('formMode').value;
  const original = document.getElementById('originalVersion').value;
  const version = document.getElementById('releaseVersion').value.trim();
  const title = document.getElementById('releaseTitle').value.trim();
  const date = document.getElementById('releaseDate').value;
  let notes = document.getElementById('releaseNotes').value || '';
  const modalError = document.getElementById('modalError');

  if (!version || !title || !date) {
    modalError.textContent = 'Please fill in required fields: version, title, and date.';
    modalError.style.display = 'block';
    return;
  }

  // normalize notes: ensure each non-empty line starts with '- '
  notes = notes.split('\n').map(l => l.trim()).filter(l => l.length > 0).map(l => l.startsWith('-') ? l : `- ${l}`).join('\n');

  // Year validation: disallow years before 2024 or after 2026
  const year = (new Date(date)).getFullYear();
  if (isNaN(year) || year < 2024 || year > 2026) {
    modalError.textContent = 'Upload date year must be between 2024 and 2026.';
    modalError.style.display = 'block';
    return;
  }

  // clear modal errors if any
  modalError.textContent = '';
  modalError.style.display = 'none';

  if (mode === 'add') {
    if (releases.find(r => r.version === version)) {
      alert('Version already exists');
      return;
    }
    releases.push({ version, title, date, notes });
  } else if (mode === 'edit') {
    const idx = releases.findIndex(r => r.version === original);
    if (idx === -1) {
      alert('Original release not found');
      closeReleaseModal();
      return;
    }
    // if version changed and collides with another existing version, prevent it
    if (version !== original && releases.find(r => r.version === version)) {
      alert('Another release with that version already exists.');
      return;
    }
    releases[idx] = { version, title, date, notes };
  }

  closeReleaseModal();
  renderVersionList();
}

// Initialization: render the list and wire admin controls visibility/handler
function setup() {
  renderVersionList();

  const adminControls = document.getElementById('adminControls');
  if (isAdmin) {
    adminControls.setAttribute('aria-hidden', 'false');
    adminControls.style.display = 'block';
    document.getElementById('addReleaseBtn').addEventListener('click', () => openReleaseModal('add'));
    // modal controls
    document.getElementById('modalClose').addEventListener('click', closeReleaseModal);
    document.getElementById('modalCancel').addEventListener('click', closeReleaseModal);
    document.getElementById('modalBackdrop').addEventListener('click', closeReleaseModal);
    document.getElementById('releaseForm').addEventListener('submit', _handleFormSubmit);
    // delete modal wiring
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
