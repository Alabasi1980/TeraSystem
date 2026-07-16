/*
 * ==========================================================================
 *  table-mapping-wizard.js  —  Oracle Source Mapping Wizard controller
 *  Exposes window.TableMappingWizard(opts).
 *
 *  opts = {
 *    listObjectsApiUrl, previewApiUrl, validateQueryApiUrl,
 *    editMode, initialData: { editId, oracleSource, sourceType, sqlTargetTable, syncMode, incrementalColumn }
 *  }
 *
 *  5-step wizard:
 *   Step 1 — Source Type (Table / View / Query)
 *   Step 2 — Oracle Source (searchable list or query editor)
 *   Step 3 — Preview & Validate (data grid, column metadata, schema diff)
 *   Step 4 — SQL Target (auto-suggested name, schema selection)
 *   Step 5 — Sync Settings (Full / Incremental mode, date column selection)
 *
 *  Submits to existing form handlers: ?handler=Add | ?handler=Edit
 * ==========================================================================
 */
(function (global) {
  'use strict';

  /* ─── tiny helpers ─── */
  function $(id) { return document.getElementById(id); }
  function show(el) { if (el) el.classList.remove('wm-hidden'); }
  function hide(el) { if (el) el.classList.add('wm-hidden'); }
  function isBlank(v) { return v === null || v === undefined || ('' + v).trim() === ''; }
  function escapeHtml(s) {
    return ('' + (s == null ? '' : s))
      .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
  }
  function debounce(fn, ms) {
    var t;
    return function () {
      var ctx = this, args = arguments;
      clearTimeout(t);
      t = setTimeout(function () { fn.apply(ctx, args); }, ms);
    };
  }

  /* ═══════════════════════════════════════════════════════════════════════ */
  function TableMappingWizard(opts) {
    this.opts = opts || {};
    this.opts.listObjectsApiUrl = this.opts.listObjectsApiUrl || '/api/oracle-browser/list-objects';
    this.opts.previewApiUrl = this.opts.previewApiUrl || '/api/oracle-browser/preview';
    this.opts.validateQueryApiUrl = this.opts.validateQueryApiUrl || '/api/oracle-browser/validate-query';

    this.state = {
      step: 1,
      sourceType: '',        // 'Table' | 'View' | 'Query'
      name: '',              // friendly mapping name
      oracleSource: '',      // e.g. 'NATEJSOFT.WAREHOUSE_STOCK'
      sqlTargetTable: '',    // target SQL Server table name
      editId: 0,
      isEditing: false,

      // Step 2 data
      oracleObjects: [],     // fetched from API
      selectedObject: null,  // { owner, objectName, objectType, columnCount }
      queryText: '',         // user query for Query mode
      queryValidated: false,
      queryColumns: [],

      // Step 3 data
      previewResult: null,   // DataPreviewResult
      schemaDiff: null,      // SchemaDiffResult
      activeTab: 'preview',  // 'preview' | 'columns' | 'schema',

      // Step 4 options
      autoCreateTable: false,
      applySchemaAfterSave: false,
      schema: 'dbo',

      // Step 5 options
      syncMode: 'Full',           // 'Full' | 'Incremental'
      incrementalColumn: '',      // Oracle column name for incremental watermark

      // UI state
      objectsLoading: false,
      previewLoading: false,
      queryValidating: false,
      searchQuery: '',
      wizardInProgress: false
    };

    this._searchTimer = null;

    this.init();
  }

  /* ─────────────────── INIT ─────────────────── */
  TableMappingWizard.prototype.init = function () {
    this.form = $('wm-form');
    if (!this.form) return;

    this.wireSourceTypeCards();
    this.wireSearch();
    this.wireQueryEditor();
    this.wireStep3Tabs();
    this.wireTargetName();
    this.wireSyncModeCards();
    this.wireNavButtons();
    this.wireKeyboard();

    // Bootstrap edit mode
    if (this.opts.editMode && this.opts.initialData) {
      this.bootstrapEditMode(this.opts.initialData);
    }
  };

  /* ─────────────────── EDIT MODE BOOTSTRAP ─────────────────── */
  TableMappingWizard.prototype.bootstrapEditMode = function (data) {
    this.state.editId = data.editId || 0;
    this.state.isEditing = data.editId > 0;

    // Step 1: highlight the correct card FIRST (selectSourceType resets
    // oracleSource, queryText, previewResult, etc., so we apply edit
    // data AFTER the reset).
    this.selectSourceType(data.sourceType || '', false);

    // Now apply the persisted edit data (after selectSourceType clears defaults)
    this.state.name = data.name || '';
    this.state.oracleSource = data.oracleSource || '';
    this.state.sqlTargetTable = data.sqlTargetTable || '';
    this.state.syncMode = data.syncMode || 'Full';
    this.state.incrementalColumn = data.incrementalColumn || '';

    // Step 2: prefill source
    if (this.state.sourceType === 'Query') {
      this.state.queryText = this.state.oracleSource;
      this.state.queryValidated = true;
      var editor = $('wm-query-editor');
      if (editor) editor.value = this.state.queryText;
      this.updateQueryStatus('valid', 'تم التحقق من الاستعلام مسبقاً');
    }

    // Step 4: prefill target
    var targetInput = $('wm-target-name');
    if (targetInput) targetInput.value = this.state.sqlTargetTable;

    // Preload data for step 3
    if (this.state.sourceType && this.state.oracleSource) {
      this.loadPreview(this.state.oracleSource, this.state.sourceType);
    }

    // Step indicator: jump to step 1 but show editing badge
    var titleEl = $('wm-modal-title');
    if (titleEl) titleEl.textContent = 'تعديل التعيين';
    var saveBtn = $('wm-btn-save');
    if (saveBtn) saveBtn.textContent = 'تحديث التعيين';
  };

  /* ─────────────────── STEP NAVIGATION ─────────────────── */
  TableMappingWizard.prototype.goToStep = function (n) {
    if (n < 1 || n > 5) return;
    this.state.step = n;
    this.state.wizardInProgress = true;
    this.updateStepUI();

    // Auto-load data when entering a step
    if (n === 2 && this.state.sourceType && this.state.sourceType !== 'Query') {
      this.loadOracleObjects(this.state.sourceType);
    }
    if (n === 3 && this.state.oracleSource) {
      this.loadPreview(this.state.oracleSource, this.state.sourceType);
    }
    if (n === 4) {
      this.suggestTargetName();
      this.loadSchemaDiff();
    }
    if (n === 5 && this.state.syncMode === 'Incremental') {
      this.loadDateColumns();
    }
  };

  TableMappingWizard.prototype.nextStep = function () {
    if (this.state.step >= 5) return;
    if (!this.validateCurrentStep()) return;
    this.goToStep(this.state.step + 1);
  };

  TableMappingWizard.prototype.prevStep = function () {
    if (this.state.step <= 1) return;
    this.goToStep(this.state.step - 1);
  };

  TableMappingWizard.prototype.updateStepUI = function () {
    var step = this.state.step;

    // Step indicators
    var steps = document.querySelectorAll('.wm-step-indicator');
    for (var i = 0; i < steps.length; i++) {
      var sn = parseInt(steps[i].getAttribute('data-step'), 10);
      steps[i].classList.remove('wm-step--active', 'wm-step--done');
      if (sn === step) steps[i].classList.add('wm-step--active');
      else if (sn < step) steps[i].classList.add('wm-step--done');
    }

    // Step panels
    var panels = document.querySelectorAll('.wm-step-panel');
    for (var j = 0; j < panels.length; j++) {
      var pn = parseInt(panels[j].getAttribute('data-step'), 10);
      if (pn === step) {
        show(panels[j]);
        panels[j].classList.add('wm-step-panel--active');
      } else {
        hide(panels[j]);
        panels[j].classList.remove('wm-step-panel--active');
      }
    }

    // Nav button states
    var prevBtn = $('wm-btn-prev');
    var nextBtn = $('wm-btn-next');
    if (prevBtn) prevBtn.disabled = (step === 1);
    if (nextBtn) {
      nextBtn.disabled = (step === 5);
      nextBtn.style.display = (step === 5) ? 'none' : '';
    }
  };

  TableMappingWizard.prototype.validateCurrentStep = function () {
    var s = this.state;
    if (s.step === 1 && !s.sourceType) {
      this.toast('اختر نوع المصدر أولاً.', 'error');
      return false;
    }
    if (s.step === 2) {
      if (s.sourceType === 'Query') {
        if (!s.queryText.trim()) {
          this.toast('أدخل استعلام SQL.', 'error');
          return false;
        }
        if (!s.queryValidated) {
          this.toast('قم بفحص الاستعلام أولاً.', 'error');
          return false;
        }
      } else {
        if (!s.oracleSource) {
          this.toast('اختر جدولاً أو عرضاً من القائمة.', 'error');
          return false;
        }
      }
    }
    if (s.step === 3) {
      if (!s.previewResult || s.previewResult.errorMessage) {
        this.toast('معاينة البيانات مطلوبة. تأكد من صحة المصدر.', 'error');
        return false;
      }
    }
    if (s.step === 4) {
      var targetName = ($('wm-target-name') || {}).value || '';
      if (!targetName.trim()) {
        this.toast('اسم الجدول الهدف مطلوب.', 'error');
        return false;
      }
      if (!/^[A-Za-z_][A-Za-z0-9_]*$/.test(targetName.trim())) {
        this.toast('اسم الجدول يجب أن يبدأ بحرف أو شرطة سفلية، ويحتوي فقط على أحرف وأرقام وشرطة سفلية.', 'error');
        return false;
      }
    }
    return true;
  };

  /* ─────────────────── STEP 1: SOURCE TYPE ─────────────────── */
  TableMappingWizard.prototype.wireSourceTypeCards = function () {
    var self = this;
    var cards = document.querySelectorAll('.wm-source-type-card');
    for (var i = 0; i < cards.length; i++) {
      cards[i].addEventListener('click', function () {
        self.selectSourceType(this.getAttribute('data-type'), true);
      });
    }
  };

  TableMappingWizard.prototype.selectSourceType = function (type, advance) {
    if (!type) return;
    this.state.sourceType = type;
    this.state.oracleSource = '';
    this.state.selectedObject = null;
    this.state.queryText = '';
    this.state.queryValidated = false;
    this.state.queryColumns = [];
    this.state.previewResult = null;

    // Update cards visual
    var cards = document.querySelectorAll('.wm-source-type-card');
    for (var i = 0; i < cards.length; i++) {
      var ct = cards[i].getAttribute('data-type');
      cards[i].classList.toggle('wm-card--selected', ct === type);
    }

    if (advance) {
      this.goToStep(2);
    }
  };

  /* ─────────────────── STEP 2: ORACLE SOURCE ─────────────────── */
  TableMappingWizard.prototype.loadOracleObjects = function (type) {
    var self = this;
    if (this.state.objectsLoading) return;
    this.state.objectsLoading = true;
    this.renderObjectListLoading();

    var url = this.opts.listObjectsApiUrl + '?type=' + encodeURIComponent(type.toLowerCase());
    fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
      .then(function (r) {
        if (!r.ok) throw new Error('HTTP ' + r.status);
        return r.json();
      })
      .then(function (data) {
        self.state.oracleObjects = Array.isArray(data) ? data : [];
        self.state.objectsLoading = false;
        self.renderObjectList();
      })
      .catch(function (err) {
        self.state.objectsLoading = false;
        self.renderObjectListError('تعذر تحميل قائمة ' + (type === 'Table' ? 'الجداول' : 'العروض') + ': ' + (err.message || err));
      });
  };

  TableMappingWizard.prototype.renderObjectList = function () {
    var container = $('wm-object-list');
    if (!container) return;

    var query = (this.state.searchQuery || '').toLowerCase();
    var filtered = this.state.oracleObjects;
    if (query) {
      filtered = filtered.filter(function (o) {
        return (o.objectName || '').toLowerCase().indexOf(query) >= 0 ||
               (o.owner || '').toLowerCase().indexOf(query) >= 0;
      });
    }

    if (filtered.length === 0) {
      container.innerHTML =
        '<div class="wm-empty-state">' +
          '<div class="wm-empty-icon">&#128269;</div>' +
          '<p>' + (query ? 'لا توجد نتائج مطابقة للبحث.' : 'لا توجد ' + (this.state.sourceType === 'Table' ? 'جداول' : 'عروض') + ' متاحة.') + '</p>' +
        '</div>';
      return;
    }

    var html = '';
    for (var i = 0; i < filtered.length; i++) {
      var obj = filtered[i];
      var selected = (this.state.oracleSource === (obj.owner + '.' + obj.objectName));
      var mappedBadge = obj.isAlreadyMapped
        ? '<span class="wm-badge wm-badge--warning">مستخدم مسبقاً</span>'
        : '';
      html +=
        '<div class="wm-object-item' + (selected ? ' wm-object-item--selected' : '') + '" ' +
             'data-owner="' + escapeHtml(obj.owner) + '" ' +
             'data-name="' + escapeHtml(obj.objectName) + '" ' +
             'data-column-count="' + obj.columnCount + '">' +
          '<div class="wm-object-item__owner"><span class="wm-badge wm-badge--owner">' + escapeHtml(obj.owner) + '</span></div>' +
          '<div class="wm-object-item__name">' + escapeHtml(obj.objectName) + '</div>' +
          '<div class="wm-object-item__meta">' +
            '<span class="wm-badge wm-badge--info">' + obj.columnCount + ' أعمدة</span>' +
            mappedBadge +
          '</div>' +
        '</div>';
    }
    container.innerHTML = html;

    // Wire click events
    var self = this;
    var items = container.querySelectorAll('.wm-object-item');
    for (var j = 0; j < items.length; j++) {
      items[j].addEventListener('click', function () {
        self.selectObject(
          this.getAttribute('data-owner'),
          this.getAttribute('data-name'),
          parseInt(this.getAttribute('data-column-count'), 10)
        );
      });
    }
  };

  TableMappingWizard.prototype.renderObjectListLoading = function () {
    var container = $('wm-object-list');
    if (!container) return;
    var html = '';
    for (var i = 0; i < 6; i++) {
      html += '<div class="wm-skeleton-row">' +
        '<div class="wm-skeleton wm-skeleton--badge"></div>' +
        '<div class="wm-skeleton wm-skeleton--text"></div>' +
        '<div class="wm-skeleton wm-skeleton--badge wm-skeleton--sm"></div>' +
      '</div>';
    }
    container.innerHTML = html;
  };

  TableMappingWizard.prototype.renderObjectListError = function (msg) {
    var container = $('wm-object-list');
    if (!container) return;
    container.innerHTML =
      '<div class="wm-empty-state wm-empty-state--error">' +
        '<div class="wm-empty-icon">&#9888;</div>' +
        '<p>' + escapeHtml(msg) + '</p>' +
        '<button type="button" class="wd-btn wd-btn--ghost wd-btn--sm wm-mt-2" onclick="window._wmWizard && window._wmWizard.loadOracleObjects(window._wmWizard.state.sourceType)">' +
          'إعادة المحاولة' +
        '</button>' +
      '</div>';
  };

  TableMappingWizard.prototype.selectObject = function (owner, name, columnCount) {
    this.state.oracleSource = owner + '.' + name;
    this.state.selectedObject = { owner: owner, objectName: name, columnCount: columnCount };

    // Update visual selection
    var items = document.querySelectorAll('.wm-object-item');
    for (var i = 0; i < items.length; i++) {
      var isSelected = items[i].getAttribute('data-owner') === owner && items[i].getAttribute('data-name') === name;
      items[i].classList.toggle('wm-object-item--selected', isSelected);
    }

    // Auto-advance to step 3
    this.goToStep(3);
  };

  /* ─────────────────── STEP 2: SEARCH ─────────────────── */
  TableMappingWizard.prototype.wireSearch = function () {
    var self = this;
    var searchInput = $('wm-search-input');
    if (!searchInput) return;
    searchInput.addEventListener('input', function () {
      self.state.searchQuery = searchInput.value;
      if (self._searchTimer) clearTimeout(self._searchTimer);
      self._searchTimer = setTimeout(function () {
        self.renderObjectList();
      }, 200);
    });
  };

  /* ─────────────────── STEP 2: QUERY EDITOR ─────────────────── */
  TableMappingWizard.prototype.wireQueryEditor = function () {
    var self = this;
    var editor = $('wm-query-editor');
    if (!editor) return;

    editor.addEventListener('input', function () {
      self.state.queryText = editor.value;
      self.state.queryValidated = false;
      self.updateQueryStatus('neutral', '');
    });

    var validateBtn = $('wm-btn-validate-query');
    if (validateBtn) {
      validateBtn.addEventListener('click', function () { self.validateQuery(); });
    }

    var previewBtn = $('wm-btn-preview-query');
    if (previewBtn) {
      previewBtn.addEventListener('click', function () {
        if (self.state.queryValidated) {
          self.state.oracleSource = self.state.queryText.trim();
          self.loadPreview(self.state.oracleSource, 'Query');
        }
      });
    }
  };

  TableMappingWizard.prototype.validateQuery = function () {
    var self = this;
    var query = (this.state.queryText || '').trim();
    if (!query) {
      this.updateQueryStatus('error', 'أدخل استعلام SQL أولاً.');
      return;
    }

    this.state.queryValidating = true;
    this.updateQueryStatus('loading', 'جاري فحص الاستعلام...');
    var statusEl = $('wm-query-validation-icon');

    fetch(this.opts.validateQueryApiUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'XMLHttpRequest' },
      body: JSON.stringify({ query: query })
    })
      .then(function (r) {
        if (!r.ok) throw new Error('HTTP ' + r.status);
        return r.json();
      })
      .then(function (result) {
        self.state.queryValidating = false;
        if (result.isValid) {
          self.state.queryValidated = true;
          self.state.queryColumns = result.columns || [];
          self.state.oracleSource = query;
          self.updateQueryStatus('valid', 'صالح — ' + self.state.queryColumns.length + ' أعمدة');
          self.goToStep(3);
        } else {
          self.state.queryValidated = false;
          self.updateQueryStatus('error', result.errorMessage || 'الاستعلام غير صالح.');
        }
      })
      .catch(function (err) {
        self.state.queryValidating = false;
        self.updateQueryStatus('error', 'تعذر فحص الاستعلام: ' + (err.message || err));
      });
  };

  TableMappingWizard.prototype.updateQueryStatus = function (status, msg) {
    var textEl = $('wm-query-status-text');
    var iconEl = $('wm-query-validation-icon');
    if (!textEl) return;

    textEl.textContent = msg;
    textEl.className = 'wm-query-status__text';

    if (iconEl) {
      iconEl.className = 'wm-query-validation-icon';
      if (status === 'valid') {
        textEl.classList.add('wm-text-success');
        iconEl.textContent = '\u2713';
        iconEl.classList.add('wm-text-success');
      } else if (status === 'error') {
        textEl.classList.add('wm-text-error');
        iconEl.textContent = '\u2717';
        iconEl.classList.add('wm-text-error');
      } else if (status === 'loading') {
        iconEl.textContent = '\u25CB';
        iconEl.classList.add('wm-spin');
      } else {
        iconEl.textContent = '';
      }
    }
  };

  /* ─────────────────── STEP 3: TABS ─────────────────── */
  TableMappingWizard.prototype.wireStep3Tabs = function () {
    var self = this;
    var tabs = document.querySelectorAll('.wm-tab-btn');
    for (var i = 0; i < tabs.length; i++) {
      tabs[i].addEventListener('click', function () {
        self.switchTab(this.getAttribute('data-tab'));
      });
    }
  };

  TableMappingWizard.prototype.switchTab = function (tab) {
    this.state.activeTab = tab;

    var tabs = document.querySelectorAll('.wm-tab-btn');
    for (var i = 0; i < tabs.length; i++) {
      tabs[i].classList.toggle('wm-tab-btn--active', tabs[i].getAttribute('data-tab') === tab);
    }

    var panels = document.querySelectorAll('.wm-tab-panel');
    for (var j = 0; j < panels.length; j++) {
      panels[j].classList.toggle('wm-tab-panel--active', panels[j].getAttribute('data-tab') === tab);
    }
  };

  /* ─────────────────── STEP 3: DATA PREVIEW ─────────────────── */
  TableMappingWizard.prototype.loadPreview = function (source, sourceType) {
    var self = this;
    if (!source) return;
    this.state.previewLoading = true;
    this.renderPreviewLoading();

    fetch(this.opts.previewApiUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'XMLHttpRequest' },
      body: JSON.stringify({ source: source, sourceType: sourceType, limit: 10 })
    })
      .then(function (r) {
        if (!r.ok) throw new Error('HTTP ' + r.status);
        return r.json();
      })
      .then(function (result) {
        self.state.previewLoading = false;
        self.state.previewResult = result;
        if (result.errorMessage) {
          self.renderPreviewError(result.errorMessage);
        } else {
          self.renderPreviewGrid(result);
          self.renderColumnMetadata(result);
        }
      })
      .catch(function (err) {
        self.state.previewLoading = false;
        self.renderPreviewError('تعذر تحميل المعاينة: ' + (err.message || err));
      });
  };

  TableMappingWizard.prototype.renderPreviewGrid = function (result) {
    var container = $('wm-preview-content');
    if (!container) return;

    if (!result.rows || result.rows.length === 0) {
      container.innerHTML =
        '<div class="wm-empty-state">' +
          '<div class="wm-empty-icon">&#128203;</div>' +
          '<p>لا توجد بيانات في هذا المصدر.</p>' +
        '</div>';
      return;
    }

    // Build table
    var html = '<div class="wm-preview-table-wrap"><table class="wm-preview-table"><thead><tr>';
    for (var i = 0; i < result.columns.length; i++) {
      html += '<th><span class="wm-col-name">' + escapeHtml(result.columns[i]) + '</span>' +
              '<span class="wm-col-type">' + escapeHtml(result.columnTypes[i] || '') + '</span></th>';
    }
    html += '</tr></thead><tbody>';

    for (var r = 0; r < result.rows.length; r++) {
      html += '<tr>';
      for (var c = 0; c < result.columns.length; c++) {
        var val = result.rows[r][result.columns[c]];
        var display = val === null ? '<span class="wm-null">NULL</span>' : escapeHtml(String(val));
        html += '<td>' + display + '</td>';
      }
      html += '</tr>';
    }

    html += '</tbody></table></div>';
    html += '<div class="wm-preview-meta">عرض ' + result.rows.length + ' من أصل ' + (result.totalRows || '?') + ' صف</div>';
    container.innerHTML = html;
  };

  TableMappingWizard.prototype.renderPreviewLoading = function () {
    var container = $('wm-preview-content');
    if (!container) return;
    var html = '<div class="wm-skeleton-grid">';
    for (var r = 0; r < 5; r++) {
      html += '<div class="wm-skeleton-row">';
      for (var c = 0; c < 4; c++) {
        html += '<div class="wm-skeleton wm-skeleton--cell"></div>';
      }
      html += '</div>';
    }
    html += '</div>';
    container.innerHTML = html;
  };

  TableMappingWizard.prototype.renderPreviewError = function (msg) {
    var container = $('wm-preview-content');
    if (!container) return;
    container.innerHTML =
      '<div class="wm-empty-state wm-empty-state--error">' +
        '<div class="wm-empty-icon">&#9888;</div>' +
        '<p>' + escapeHtml(msg) + '</p>' +
      '</div>';
  };

  /* ─────────────────── STEP 3: COLUMN METADATA ─────────────────── */
  TableMappingWizard.prototype.renderColumnMetadata = function (result) {
    var container = $('wm-columns-content');
    if (!container) return;

    if (!result.columns || result.columns.length === 0) {
      container.innerHTML = '<div class="wm-empty-state"><p>لا توجد أعمدة.</p></div>';
      return;
    }

    var html = '<table class="wm-meta-table"><thead><tr>' +
      '<th>العمود</th><th>نوع البيانات</th><th>النوع المقترح في SQL Server</th>' +
    '</tr></thead><tbody>';

    // Oracle-to-SQL type mapping (mirrors the backend logic for display)
    var typeMap = {
      'String': 'NVARCHAR(MAX)',
      'Decimal': 'DECIMAL(38,10)',
      'Int32': 'INT',
      'Int64': 'BIGINT',
      'DateTime': 'DATETIME2',
      'Double': 'FLOAT',
      'Boolean': 'BIT',
      'Byte[]': 'VARBINARY(MAX)'
    };

    for (var i = 0; i < result.columns.length; i++) {
      var oracleType = result.columnTypes[i] || 'UNKNOWN';
      var sqlType = typeMap[oracleType] || 'NVARCHAR(MAX)';
      html += '<tr>' +
        '<td class="wm-col-name-cell">' + escapeHtml(result.columns[i]) + '</td>' +
        '<td><span class="wm-badge wm-badge--oracle">' + escapeHtml(oracleType) + '</span></td>' +
        '<td><span class="wm-badge wm-badge--sql">' + escapeHtml(sqlType) + '</span></td>' +
      '</tr>';
    }

    html += '</tbody></table>';
    container.innerHTML = html;
  };

  /* ─────────────────── STEP 3: SCHEMA DIFF ─────────────────── */
  TableMappingWizard.prototype.loadSchemaDiff = function () {
    // Schema diff is loaded on-demand via the page model's PreviewSchema handler
    // For the wizard, we show the existing schema diff if available from page model
    var container = $('wm-schema-content');
    if (!container) return;

    // If we have a target name, show placeholder for diff
    var targetName = ($('wm-target-name') || {}).value || '';
    if (!targetName) {
      container.innerHTML =
        '<div class="wm-empty-state">' +
          '<div class="wm-empty-icon">&#128221;</div>' +
          '<p>أدخل اسم الجدول الهدف في الخطوة التالية لعرض مقارنة المخطط.</p>' +
        '</div>';
      return;
    }

    container.innerHTML =
      '<div class="wm-empty-state">' +
        '<p>سيتم عرض مقارنة المخطط بعد حفظ التعيين.</p>' +
      '</div>';
  };

  /* ─────────────────── STEP 4: TARGET NAME ─────────────────── */
  TableMappingWizard.prototype.wireTargetName = function () {
    var self = this;
    var input = $('wm-target-name');
    if (!input) return;
    input.addEventListener('input', function () {
      self.state.sqlTargetTable = input.value;
    });
  };

  TableMappingWizard.prototype.suggestTargetName = function () {
    var input = $('wm-target-name');
    if (!input) return;

    // Only suggest if empty (don't clobber user input)
    if (input.value.trim()) return;

    var suggestion = '';
    if (this.state.sourceType === 'Query') {
      suggestion = 'stg_QUERY_' + Date.now();
    } else if (this.state.oracleSource) {
      // NATEJSOFT.WAREHOUSE_STOCK → stg_WAREHOUSE_STOCK
      var parts = this.state.oracleSource.split('.');
      var name = parts.length > 1 ? parts[parts.length - 1] : parts[0];
      suggestion = 'stg_' + name;
    }

    input.value = suggestion;
    this.state.sqlTargetTable = suggestion;
  };

  /* ─────────────────── STEP 5: SYNC MODE ─────────────────── */
  TableMappingWizard.prototype.wireSyncModeCards = function () {
    var self = this;
    var cards = document.querySelectorAll('.wm-sync-mode-card');
    for (var i = 0; i < cards.length; i++) {
      cards[i].addEventListener('click', function () {
        var mode = this.getAttribute('data-sync-mode');
        self.setSyncMode(mode);
      });
    }

    // Wire incremental column dropdown
    var colSelect = $('wm-incremental-column');
    if (colSelect) {
      colSelect.addEventListener('change', function () {
        self.state.incrementalColumn = colSelect.value;
      });
    }
  };

  TableMappingWizard.prototype.setSyncMode = function (mode) {
    this.state.syncMode = mode || 'Full';
    if (mode !== 'Incremental') {
      this.state.incrementalColumn = '';
    }

    // Update card visuals
    var cards = document.querySelectorAll('.wm-sync-mode-card');
    for (var i = 0; i < cards.length; i++) {
      var m = cards[i].getAttribute('data-sync-mode');
      cards[i].classList.toggle('wm-card--selected', m === mode);
    }

    // Show/hide incremental options
    var incrOpts = $('wm-incremental-options');
    if (incrOpts) {
      if (mode === 'Incremental') {
        show(incrOpts);
        this.loadDateColumns();
      } else {
        hide(incrOpts);
      }
    }

    // Update summary
    var sumMode = $('wm-sum-sync-mode');
    var sumIncrRow = $('wm-sum-incr-row');
    var sumIncrCol = $('wm-sum-incr-col');
    if (sumMode) sumMode.textContent = mode === 'Incremental' ? 'مزامنة تزايدية' : 'مزامنة كاملة';
    if (sumIncrRow) sumIncrRow.style.display = mode === 'Incremental' ? '' : 'none';
    if (sumIncrCol) sumIncrCol.textContent = this.state.incrementalColumn || '--';
  };

  TableMappingWizard.prototype.loadDateColumns = function () {
    var self = this;
    var select = $('wm-incremental-column');
    if (!select) return;
    if (!this.state.oracleSource) return;

    // Clear existing options except the placeholder
    while (select.options.length > 1) {
      select.remove(1);
    }

    // Add loading option
    var loadingOpt = document.createElement('option');
    loadingOpt.value = '';
    loadingOpt.textContent = 'جاري تحميل الأعمدة...';
    loadingOpt.disabled = true;
    select.appendChild(loadingOpt);

    // Use preview API to get column metadata
    fetch(this.opts.previewApiUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'XMLHttpRequest' },
      body: JSON.stringify({ source: this.state.oracleSource, sourceType: this.state.sourceType, limit: 1 })
    })
      .then(function (r) {
        if (!r.ok) throw new Error('HTTP ' + r.status);
        return r.json();
      })
      .then(function (result) {
        // Remove loading option
        if (select.options.length > 1) select.remove(1);

        if (result.errorMessage || !result.columns) {
          var errOpt = document.createElement('option');
          errOpt.value = '';
          errOpt.textContent = 'تعذر تحميل الأعمدة';
          errOpt.disabled = true;
          select.appendChild(errOpt);
          return;
        }

        // Filter date/time columns
        var dateTypes = ['DateTime', 'DateTime2', 'Date', 'Timestamp'];
        var dateColumns = [];
        for (var i = 0; i < result.columns.length; i++) {
          var colType = (result.columnTypes[i] || '').toLowerCase();
          var isDate = dateTypes.some(function (dt) {
            return colType.indexOf(dt.toLowerCase()) >= 0;
          });
          if (isDate) {
            dateColumns.push(result.columns[i]);
          }
        }

        if (dateColumns.length === 0) {
          var noOpt = document.createElement('option');
          noOpt.value = '';
          noOpt.textContent = 'لا توجد أعمدة تاريخ في المصدر';
          noOpt.disabled = true;
          select.appendChild(noOpt);
          return;
        }

        for (var j = 0; j < dateColumns.length; j++) {
          var opt = document.createElement('option');
          opt.value = dateColumns[j];
          opt.textContent = dateColumns[j];
          select.appendChild(opt);
        }

        // Restore previous selection if valid
        if (self.state.incrementalColumn) {
          select.value = self.state.incrementalColumn;
          if (select.value !== self.state.incrementalColumn) {
            // Previous selection not found; reset
            self.state.incrementalColumn = '';
            select.value = '';
          }
        }
      })
      .catch(function () {
        // Remove loading option
        if (select.options.length > 1) select.remove(1);
        var errOpt = document.createElement('option');
        errOpt.value = '';
        errOpt.textContent = 'تعذر تحميل الأعمدة';
        errOpt.disabled = true;
        select.appendChild(errOpt);
      });
  };

  /* ─────────────────── NAV BUTTONS ─────────────────── */
  TableMappingWizard.prototype.wireNavButtons = function () {
    var self = this;
    var prevBtn = $('wm-btn-prev');
    var nextBtn = $('wm-btn-next');
    var saveBtn = $('wm-btn-save');
    var cancelBtn = $('wm-btn-cancel');

    if (prevBtn) prevBtn.addEventListener('click', function () { self.prevStep(); });
    if (nextBtn) nextBtn.addEventListener('click', function () { self.nextStep(); });
    if (saveBtn) saveBtn.addEventListener('click', function () { self.save(); });
    if (cancelBtn) cancelBtn.addEventListener('click', function () { self.close(); });
  };

  /* ─────────────────── KEYBOARD ─────────────────── */
  TableMappingWizard.prototype.wireKeyboard = function () {
    var self = this;
    document.addEventListener('keydown', function (e) {
      var overlay = $('wm-overlay');
      if (!overlay || !overlay.classList.contains('is-open')) return;

      if (e.key === 'Escape') {
        e.preventDefault();
        self.close();
      }
    });
  };

  /* ─────────────────── SAVE ─────────────────── */
  TableMappingWizard.prototype.save = function () {
    if (!this.validateCurrentStep()) return;

    // Sync hidden form fields
    var nameInput = $('wm-h-name');
    var oracleInput = $('wm-h-oracleSource');
    var typeInput = $('wm-h-sourceType');
    var targetInput = $('wm-h-sqlTargetTable');
    var editIdInput = $('wm-h-editId');
    var syncModeInput = $('wm-h-syncMode');
    var incrColInput = $('wm-h-incrementalColumn');

    if (nameInput) nameInput.value = this.state.name;
    if (oracleInput) oracleInput.value = this.state.oracleSource;
    if (typeInput) typeInput.value = this.state.sourceType;
    if (targetInput) targetInput.value = this.state.sqlTargetTable;
    if (editIdInput) editIdInput.value = this.state.editId || 0;
    if (syncModeInput) syncModeInput.value = this.state.syncMode || 'Full';
    if (incrColInput) incrColInput.value = this.state.incrementalColumn || '';

    // Set form action
    if (this.form) {
      this.form.action = this.state.isEditing ? '?handler=Edit' : '?handler=Add';
      this.form.submit();
    }
  };

  /* ─────────────────── OPEN / CLOSE ─────────────────── */
  TableMappingWizard.prototype.open = function (editData) {
    if (editData) {
      this.bootstrapEditMode(editData);
    } else {
      this.goToStep(1);
    }
    var overlay = $('wm-overlay');
    if (overlay) {
      overlay.classList.add('is-open');
      document.body.style.overflow = 'hidden';
    }
    this.state.wizardInProgress = false;
  };

  TableMappingWizard.prototype.close = function () {
    if (this.state.wizardInProgress) {
      if (!global.confirm('هل تريد إغلاق المعالج؟ سيتم فقدان غير المحفوظ.')) return;
    }
    var overlay = $('wm-overlay');
    if (overlay) {
      overlay.classList.remove('is-open');
      document.body.style.overflow = '';
    }
    this.resetState();
  };

  TableMappingWizard.prototype.resetState = function () {
    this.state.step = 1;
    this.state.sourceType = '';
    this.state.name = '';
    this.state.oracleSource = '';
    this.state.sqlTargetTable = '';
    this.state.selectedObject = null;
    this.state.queryText = '';
    this.state.queryValidated = false;
    this.state.queryColumns = [];
    this.state.previewResult = null;
    this.state.oracleObjects = [];
    this.state.searchQuery = '';
    this.state.wizardInProgress = false;
    this.state.activeTab = 'preview';
    this.state.syncMode = 'Full';
    this.state.incrementalColumn = '';

    // Reset UI
    var cards = document.querySelectorAll('.wm-source-type-card');
    for (var i = 0; i < cards.length; i++) {
      cards[i].classList.remove('wm-card--selected');
    }
    var editor = $('wm-query-editor');
    if (editor) editor.value = '';
    var searchInput = $('wm-search-input');
    if (searchInput) searchInput.value = '';
    var targetName = $('wm-target-name');
    if (targetName) targetName.value = '';

    // Reset sync mode cards
    var syncCards = document.querySelectorAll('.wm-sync-mode-card');
    for (var k = 0; k < syncCards.length; k++) {
      syncCards[k].classList.remove('wm-card--selected');
    }
    var incrOpts = $('wm-incremental-options');
    if (incrOpts) hide(incrOpts);
    var incrCol = $('wm-incremental-column');
    if (incrCol) incrCol.value = '';
    var sumMode = $('wm-sum-sync-mode');
    if (sumMode) sumMode.textContent = 'مزامنة كاملة';
    var sumIncrRow = $('wm-sum-incr-row');
    if (sumIncrRow) sumIncrRow.style.display = 'none';

    this.updateStepUI();
    this.updateQueryStatus('neutral', '');
    this.switchTab('preview');
  };

  /* ─────────────────── TOAST ─────────────────── */
  TableMappingWizard.prototype.toast = function (msg, type) {
    var host = $('wd-toast-container');
    if (!host) return;
    var t = document.createElement('div');
    t.className = 'wd-toast wd-toast--' + (type || 'success');
    t.innerHTML = '<span>' + (type === 'error' ? '&#9888;' : '&#10003;') + '</span><span>' + escapeHtml(msg) + '</span>';
    host.appendChild(t);
    setTimeout(function () { if (t.parentNode) t.parentNode.removeChild(t); }, 4000);
  };

  /* ═══════════════════════════════════════════════════════════════════════ */
  global.TableMappingWizard = TableMappingWizard;

})(window);
