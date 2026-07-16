/*
 * ==========================================================================
 *  card-builder.js  —  Card Builder Wizard controller
 *  Exposes window.CardBuilderWizard(opts).
 *
 *  opts = {
 *    previewApiUrl, tablesApiUrl, savedQueriesApiUrl,
 *    cloneId, initialData: { cardType, sourceType, sourceId, customSql, title, displayName, measurement }
 *  }
 *
 *  Reads/owns the existing DOM in Builder.cshtml and wires the wizard:
 *   - Step navigation + validation
 *   - Type picker (Step 1)
 *   - Source panels: Template / SqlTable / CustomSQL / SavedQuery (Step 2)
 *   - Template rendering + {TableName} substitution
 *   - Oracle tables fetch + dropdown
 *   - Live preview POST -> Syncfusion render (chart AND table)
 *   - Palette injection, filter add/remove, advanced options
 *   - Save / Save & Add / Cancel (native form submit after sync)
 *   - Clone / initialData bootstrap
 *
 *  NOTE: This file never edits HTML/CSS/CS — it only manipulates the DOM
 *  that Builder.cshtml already declares.
 * ==========================================================================
 */
(function (global) {
  'use strict';

  /* ---------------- tiny helpers ---------------- */
  function $(id) { return document.getElementById(id); }
  function show(el) { if (el) el.classList.remove('wd-hidden'); }
  function hide(el) { if (el) el.classList.add('wd-hidden'); }
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
  var CHART_ICON = {
    KPI: 'wb-icon-kpi', Bar: 'wb-icon-bar', Line: 'wb-icon-line',
    Pie: 'wb-icon-pie', Table: 'wb-icon-table', Gauge: 'wb-icon-gauge'
  };
  function chartIconSvg(type) {
    var id = CHART_ICON[type] || 'wb-icon-bar';
    return '<svg class="wb-icon" style="width:28px;height:28px" aria-hidden="true"><use href="#' + id + '"></use></svg>';
  }

  /* ===================================================================== */
  function CardBuilderWizard(opts) {
    this.opts = opts || {};
    this.opts.previewApiUrl = this.opts.previewApiUrl || '/api/dashboard/cardbuilder/preview?handler=Preview';
    this.opts.tablesApiUrl = this.opts.tablesApiUrl || '/api/tablemappings/active';
    this.opts.savedQueriesApiUrl = this.opts.savedQueriesApiUrl || '/api/savedqueries';

    this.state = {
      step: 1,
      cardType: 'KPI',
      sourceType: 'Template',
      sourceId: '',
      customSql: '',
      selectedTable: null,   // { id, oracleSource, sqlTargetTable }
      currentTemplate: null, // template object
      previewSql: '',
      measurement: '',
      gridWidth: 4,
      gridHeight: 2,
      gridX: '',
      gridY: '',
      palette: 'primary',
      refreshInterval: 300,
      title: '',
      displayName: ''
    };

    this.tables = [];
    this._previewComp = null;
    this._lastResult = null;
    this._lastChartType = null;
    this._previewTimer = null;
    this._lastRequest = null;
    this._savedNoteShown = false;

    this.init();
  }

  /* ----------------------- INIT ----------------------- */
  CardBuilderWizard.prototype.init = function () {
    var self = this;
    this.form = $('wb-form');
    if (!this.form) { return; }

    this.readInitialDom();
    this.renderTemplates();
    this.renderPalettes();
    this.wireTypePicker();
    this.wireSourceType();
    this.wireFields();
    this.wireVisual();
    this.wireFilters();
    this.wireNav();
    this.wireSave();
    this.initKpiModePicker();
    this.initDateFilterMode();

    // Avoid duplicate form-field names: the hidden inputs are the canonical
    // binders consumed by Builder.cshtml.cs. We strip conflicting names from
    // the display controls so only the hidden values are posted.
    this.cleanupDuplicateNames();

    this.loadTables();
    this.applyInitialUi();

    // Best-effort clone enrichment (falls back to DOM values already applied).
    this.tryClone();

    this.updateFooter();
    this.maybeInitialPreview();
  };

  CardBuilderWizard.prototype.readInitialDom = function () {
    var s = this.state;
    s.cardType = ($('wb-h-cardType').value) || 'KPI';
    s.sourceType = ($('wb-h-sourceType').value) || 'Template';
    s.sourceId = $('wb-h-sourceId').value || '';
    s.customSql = $('wb-h-customSql').value || '';
    s.gridWidth = parseInt($('wb-h-gridWidth').value, 10) || 4;
    s.gridHeight = parseInt($('wb-h-gridHeight').value, 10) || 2;
    s.gridX = $('wb-h-gridX').value || '';
    s.gridY = $('wb-h-gridY').value || '';
    s.palette = $('wb-h-colorPalette').value || 'primary';
    s.refreshInterval = parseInt($('wb-h-refreshInterval').value, 10) || 300;
    s.title = $('wb-title').value || '';
    s.displayName = $('wb-display-name').value || '';
    s.measurement = $('wb-measurement').value || '';

    // reflect into the visible (display) controls too
    if ($('wb-grid-width')) $('wb-grid-width').value = s.gridWidth;
    if ($('wb-grid-height')) $('wb-grid-height').value = s.gridHeight;
    if ($('wb-grid-x')) $('wb-grid-x').value = s.gridX;
    if ($('wb-grid-y')) $('wb-grid-y').value = s.gridY;
    if ($('wb-refresh-interval')) $('wb-refresh-interval').value = String(s.refreshInterval);
    if ($('wb-custom-sql')) $('wb-custom-sql').value = s.customSql;
    var st = $('wb-source-type'); if (st) st.value = s.sourceType;
    if (s.sourceType === 'CustomSQL') s.previewSql = s.customSql.trim();
  };

  CardBuilderWizard.prototype.applyInitialUi = function () {
    var self = this;
    // type cards
    this.typeCards.forEach(function (c) {
      var active = c.getAttribute('data-type') === self.state.cardType;
      c.setAttribute('aria-checked', active ? 'true' : 'false');
      c.setAttribute('tabindex', active ? '0' : '-1');
    });
    // source panel
    this.showSourcePanel(this.state.sourceType);
    this.resetTemplateSelection();
    this.updateStepUI();
  };

  /* ----------------------- STEP NAV ----------------------- */
  CardBuilderWizard.prototype.wireNav = function () {
    var self = this;
    var next = $('wb-btn-next'), prev = $('wb-btn-prev'), cancel = $('wb-btn-cancel'), retry = $('wb-preview-retry');
    if (next) next.addEventListener('click', function () { self.next(); });
    if (prev) prev.addEventListener('click', function () { self.prev(); });
    if (cancel) cancel.addEventListener('click', function () { self.cancel(); });
    if (retry) retry.addEventListener('click', function () { self.retry(); });
  };

  CardBuilderWizard.prototype.next = function () {
    var maxStep = this.isKpiStepVisible() ? 5 : 4;
    if (this.state.step >= maxStep) return;
    if (!this.validateStep(this.state.step)) return;
    this.goToStep(this.state.step + 1);
  };
  CardBuilderWizard.prototype.prev = function () {
    if (this.state.step <= 1) return;
    this.goToStep(this.state.step - 1);
  };
  CardBuilderWizard.prototype.goToStep = function (n) {
    // Skip step 4 (KPI) when chartType is not KPI
    if (n === 4 && !this.isKpiStepVisible()) {
      n = 5;
    }
    // If going backwards from step 5 and KPI not visible, go to step 3
    if (n === 4 && !this.isKpiStepVisible()) {
      n = 3;
    }
    this.state.step = n;
    this.updateStepUI();
    this.updateFooter();
    var err = $('wb-step' + n + '-error'); if (err) { err.textContent = ''; hide(err); }
  };

  CardBuilderWizard.prototype.updateStepUI = function () {
    var step = this.state.step;
    var kpiVisible = this.isKpiStepVisible();
    // Toggle KPI step indicator visibility
    var kpiStepLi = document.querySelector('.wb-step--kpi');
    if (kpiStepLi) {
      if (kpiVisible) show(kpiStepLi); else hide(kpiStepLi);
    }
    var steps = document.querySelectorAll('.wb-step');
    Array.prototype.forEach.call(steps, function (li) {
      var n = parseInt(li.getAttribute('data-step'), 10);
      li.classList.remove('wb-step--active', 'wb-step--done');
      if (n === step) li.classList.add('wb-step--active');
      else if (n < step) li.classList.add('wb-step--done');
    });
    var panels = document.querySelectorAll('.wb-step-panel');
    Array.prototype.forEach.call(panels, function (p) {
      var n = parseInt(p.getAttribute('data-step'), 10);
      if (n === step) show(p); else hide(p);
    });
    // Also toggle the KPI step panel visibility explicitly
    var kpiPanel = $('wb-step-kpi');
    if (kpiPanel) {
      if (step === 4 && kpiVisible) show(kpiPanel); else hide(kpiPanel);
    }
  };

  CardBuilderWizard.prototype.validateStep = function (step) {
    var errEl = $('wb-step' + step + '-error');
    var msg = '';
    if (step === 2) {
      if (!this.hasSource()) {
        if (this.state.sourceType === 'SavedQuery') msg = 'مصدر الاستعلامات المحفوظة غير متاح حالياً. اختر مصدراً آخر.';
        else if (this.state.sourceType === 'Template') msg = 'اختر قالباً من القائمة لإكمال هذه الخطوة.';
        else if (this.state.sourceType === 'SqlTable') msg = 'اختر جدولاً أو عرضاً من القائمة.';
        else if (this.state.sourceType === 'CustomSQL') msg = 'أدخل استعلام SQL في الخانة المخصصة.';
      }
    } else if (step === 3) {
      if (!this.state.title.trim()) msg = 'الرجاء إدخال العنوان.';
      else if (!this.state.displayName.trim()) msg = 'الرجاء إدخال اسم العرض.';
    } else if (step === 4 && this.isKpiStepVisible()) {
      var valCol = $('wb-kpi-value-column');
      var dateCol = $('wb-kpi-date-column');
      if (valCol && !valCol.value) msg = 'الرجاء اختيار عمود القيمة.';
      else if (dateCol && !dateCol.value) msg = 'الرجاء اختيار عمود التاريخ.';
    }
    if (errEl) {
      if (msg) { errEl.textContent = msg; show(errEl); }
      else { errEl.textContent = ''; hide(errEl); }
    }
    return !msg;
  };

  /* ----------------------- TYPE PICKER (Step 1) ----------------------- */
  CardBuilderWizard.prototype.wireTypePicker = function () {
    var self = this;
    this.typeCards = Array.prototype.slice.call(document.querySelectorAll('.wb-type-card'));
    this.typeCards.forEach(function (card) {
      card.addEventListener('click', function () { self.selectType(card.getAttribute('data-type')); });
      card.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); self.selectType(card.getAttribute('data-type')); }
      });
    });
  };

  CardBuilderWizard.prototype.selectType = function (type) {
    if (!type) return;
    this.state.cardType = type;
    var self = this;
    this.typeCards.forEach(function (c) {
      var active = c.getAttribute('data-type') === type;
      c.setAttribute('aria-checked', active ? 'true' : 'false');
      c.setAttribute('tabindex', active ? '0' : '-1');
    });
    if ($('wb-h-cardType')) $('wb-h-cardType').value = type;
    // a new card type invalidates a previously picked template
    this.state.currentTemplate = null;
    this.state.previewSql = (this.state.sourceType === 'CustomSQL') ? this.state.customSql.trim() : '';
    this.resetTemplateSelection();
    this.renderTemplates();
    this.setChartTypeName(type);
    this.updateStepUI();
    this.schedulePreview();
    this.updateFooter();
  };

  /* ----------------------- TEMPLATES (Step 2) ----------------------- */
  CardBuilderWizard.prototype.renderTemplates = function () {
    var grid = $('wb-template-grid'), empty = $('wb-template-empty');
    if (!grid) return;
    var self = this;
    grid.innerHTML = '';
    var list = (global.CardBuilderTemplates || []).filter(function (t) {
      return t.chartType === self.state.cardType;
    });
    if (!list.length) { show(empty); return; }
    hide(empty);
    list.forEach(function (t) {
      var card = document.createElement('div');
      card.className = 'wb-template-card';
      card.setAttribute('role', 'option');
      card.setAttribute('tabindex', '0');
      card.setAttribute('data-template-id', t.id);
      card.innerHTML =
        '<div class="wb-template-card__thumb">' + chartIconSvg(t.chartType) + '</div>' +
        '<div class="wb-template-card__name">' + escapeHtml(t.name) + '</div>' +
        '<div class="wb-template-card__desc">' + escapeHtml(t.description) + '</div>' +
        '<div class="wb-template-card__tags">' +
          '<span class="wb-template-tag">' + t.defaultGridWidth + '×' + t.defaultGridHeight + '</span>' +
          '<span class="wb-template-tag">' + escapeHtml(t.chartType) + '</span>' +
        '</div>';
      card.addEventListener('click', function () { self.selectTemplate(t); });
      card.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); self.selectTemplate(t); }
      });
      grid.appendChild(card);
    });
  };

  CardBuilderWizard.prototype.resetTemplateSelection = function () {
    var cards = document.querySelectorAll('.wb-template-card');
    Array.prototype.forEach.call(cards, function (c) {
      c.style.borderColor = ''; c.style.boxShadow = '';
    });
  };

  CardBuilderWizard.prototype.markTemplateSelected = function (id) {
    this.resetTemplateSelection();
    var card = document.querySelector('.wb-template-card[data-template-id="' + id + '"]');
    if (card) { card.style.borderColor = 'var(--c-primary)'; card.style.boxShadow = 'var(--shadow-md)'; }
  };

  CardBuilderWizard.prototype.findTableForSource = function (src) {
    if (!src) return null;
    for (var i = 0; i < this.tables.length; i++) {
      if ((this.tables[i].name && this.tables[i].name === src) || this.tables[i].oracleSource === src) return this.tables[i];
    }
    return null;
  };

  CardBuilderWizard.prototype.selectTemplate = function (t) {
    if (!t) return;
    this.state.currentTemplate = t;
    this.state.sourceType = 'Template';
    if ($('wb-h-sourceType')) $('wb-h-sourceType').value = 'Template';
    var sel = $('wb-source-type'); if (sel) sel.value = 'Template';
    this.showSourcePanel('Template');

    var sql = t.sqlQueryTemplate;

    // Try to find a matching table, but don't require it
    var matched = this.findTableForSource(t.requiredOracleSource);
    if (matched) {
      sql = sql.replace(/\{TableName\}/g, matched.sqlTargetTable);
      this.state.sourceId = matched.sqlTargetTable;
    } else if (this.tables.length) {
      // No exact match — use the first available table silently
      sql = sql.replace(/\{TableName\}/g, this.tables[0].sqlTargetTable);
      this.state.sourceId = this.tables[0].sqlTargetTable;
    } else {
      // No tables at all — leave {TableName} as placeholder
      this.state.sourceId = t.id;
    }
    if ($('wb-h-sourceId')) $('wb-h-sourceId').value = this.state.sourceId;
    this.state.previewSql = sql;

    // prefill Step 3 fields (only if empty so we don't clobber user input)
    if ($('wb-title') && !$('wb-title').value) { $('wb-title').value = t.name; this.state.title = t.name; }
    if ($('wb-display-name') && !$('wb-display-name').value) { $('wb-display-name').value = t.name; this.state.displayName = t.name; }

    // grid + refresh defaults from template
    this.setGrid(t.defaultGridWidth, t.defaultGridHeight);
    this.setRefresh(t.defaultRefreshInterval);

    this.markTemplateSelected(t.id);
    this.schedulePreview();
    this.updateFooter();
  };

  /* ----------------------- SOURCE TYPE (Step 2) ----------------------- */
  CardBuilderWizard.prototype.wireSourceType = function () {
    var self = this;
    var sel = $('wb-source-type');
    if (!sel) return;
    sel.addEventListener('change', function () { self.onSourceTypeChange(); });
  };

  CardBuilderWizard.prototype.onSourceTypeChange = function () {
    var sel = $('wb-source-type');
    if (!sel) return;
    var type = sel.value;
    this.state.sourceType = type;
    if ($('wb-h-sourceType')) $('wb-h-sourceType').value = type;
    this.showSourcePanel(type);

    if (type === 'Template') {
      this.state.previewSql = this.state.currentTemplate ? this.state.previewSql : '';
    } else if (type === 'SqlTable') {
      this.state.previewSql = this.state.selectedTable ? ('SELECT TOP 10 * FROM [' + this.state.selectedTable.sqlTargetTable + ']') : '';
    } else if (type === 'CustomSQL') {
      this.state.previewSql = this.state.customSql.trim();
    } else if (type === 'SavedQuery') {
      this.state.previewSql = '';
      this.showSavedQueryNote();
    }
    this.schedulePreview();
    this.updateFooter();
  };

  CardBuilderWizard.prototype.showSourcePanel = function (type) {
    var panels = document.querySelectorAll('.wb-source-panel');
    Array.prototype.forEach.call(panels, function (p) {
      if (p.getAttribute('data-source') === type) show(p); else hide(p);
    });
  };

  CardBuilderWizard.prototype.showSavedQueryNote = function () {
    if (this._savedNoteShown) return;
    var panel = $('wb-panel-savedquery');
    if (!panel) return;
    var note = document.createElement('p');
    note.style.cssText = 'color:var(--c-warning);font-size:12px;margin-top:var(--sp-2);';
    note.textContent = 'مصدر الاستعلامات المحفوظة غير متاح حالياً في هذه النسخة. اختر «قالب جاهز» أو «جدول Oracle» أو «SQL مخصص».';
    panel.appendChild(note);
    var hint = $('wb-saved-query-hint'); if (hint) hint.textContent = 'غير متاح حالياً';
    var sq = $('wb-saved-query'); if (sq) sq.disabled = true;
    this._savedNoteShown = true;
  };

  /* ----------------------- ORACLE TABLES (Step 2) ----------------------- */
  CardBuilderWizard.prototype.loadTables = function () {
    var self = this;
    var hint = $('wb-sql-table-hint');
    fetch(this.opts.tablesApiUrl, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
      .then(function (r) { if (!r.ok) throw new Error('HTTP ' + r.status); return r.json(); })
      .then(function (data) {
        self.tables = Array.isArray(data) ? data : [];
        self.populateSqlTableSelect();
        self.maybeInitialPreview();
      })
      .catch(function () {
        if (hint) hint.textContent = 'تعذر تحميل الجداول النشطة';
      });
  };

  CardBuilderWizard.prototype.populateSqlTableSelect = function () {
    var sel = $('wb-sql-table');
    if (!sel) return;
    var self = this;
    // keep the placeholder option
    sel.innerHTML = '<option value="">ابحث أو اختر جدولاً...</option>';
    this.tables.forEach(function (t) {
      var o = document.createElement('option');
      o.value = t.sqlTargetTable;
      o.textContent = (t.name || t.oracleSource || '') + ' (' + t.sqlTargetTable + ')';
      sel.appendChild(o);
    });
    // wire change once
    if (!sel._wbWired) {
      sel.addEventListener('change', function () { self.applySqlTable(sel.value); });
      sel._wbWired = true;
    }
    // preselect for clone (SqlTable)
    if (this.state.sourceType === 'SqlTable' && this.state.sourceId) {
      sel.value = this.state.sourceId;
      this.applySqlTable(this.state.sourceId);
    }
    var hint = $('wb-sql-table-hint'); if (hint) hint.textContent = 'اختر جدولاً لعرض أعمدة المصدر';
  };

  CardBuilderWizard.prototype.applySqlTable = function (val) {
    if (!val) {
      this.state.selectedTable = null;
      this.state.sourceId = '';
      if ($('wb-h-sourceId')) $('wb-h-sourceId').value = '';
      var mf = $('wb-measurement-field'); if (mf) { mf.disabled = true; mf.innerHTML = '<option value="">اختر جدولاً أولاً</option>'; }
      return;
    }
    var t = null;
    for (var i = 0; i < this.tables.length; i++) {
      if (this.tables[i].sqlTargetTable === val || String(this.tables[i].id) === String(val)) { t = this.tables[i]; break; }
    }
    this.state.selectedTable = t || { sqlTargetTable: val };
    this.state.sourceId = val;
    if ($('wb-h-sourceId')) $('wb-h-sourceId').value = val;
    var mf = $('wb-measurement-field'); if (mf) mf.disabled = false;
    this.state.previewSql = 'SELECT TOP 10 * FROM [' + val + ']';
    this.schedulePreview();
    this.updateFooter();
  };

  /* ----------------------- FIELDS (Step 3) ----------------------- */
  CardBuilderWizard.prototype.wireFields = function () {
    var self = this;
    var title = $('wb-title'), dn = $('wb-display-name'), meas = $('wb-measurement'), sqlMeas = $('wb-sql-measurement');
    if (title) title.addEventListener('input', function () { self.state.title = title.value; self.updateFooter(); });
    if (dn) dn.addEventListener('input', function () { self.state.displayName = dn.value; self.updateFooter(); });
    if (meas) meas.addEventListener('change', function () {
      self.state.measurement = meas.value;
      if (sqlMeas) sqlMeas.value = meas.value;
      self.schedulePreview();
    });
    if (sqlMeas) sqlMeas.addEventListener('input', function () {
      self.state.measurement = sqlMeas.value;
      if (meas) meas.value = sqlMeas.value;
      self.schedulePreview();
    });
  };

  /* ----------------------- VISUAL (Step 4) ----------------------- */
  CardBuilderWizard.prototype.wireVisual = function () {
    var self = this;
    var gw = $('wb-grid-width'), gh = $('wb-grid-height'), gx = $('wb-grid-x'), gy = $('wb-grid-y'), ri = $('wb-refresh-interval');
    if (gw) gw.addEventListener('input', function () { self.state.gridWidth = parseInt(gw.value, 10) || 1; if ($('wb-h-gridWidth')) $('wb-h-gridWidth').value = self.state.gridWidth; });
    if (gh) gh.addEventListener('input', function () { self.state.gridHeight = parseInt(gh.value, 10) || 1; if ($('wb-h-gridHeight')) $('wb-h-gridHeight').value = self.state.gridHeight; });
    if (gx) gx.addEventListener('input', function () { self.state.gridX = gx.value; if ($('wb-h-gridX')) $('wb-h-gridX').value = gx.value; });
    if (gy) gy.addEventListener('input', function () { self.state.gridY = gy.value; if ($('wb-h-gridY')) $('wb-h-gridY').value = gy.value; });
    if (ri) ri.addEventListener('change', function () { self.state.refreshInterval = parseInt(ri.value, 10) || 0; if ($('wb-h-refreshInterval')) $('wb-h-refreshInterval').value = self.state.refreshInterval; });
    this.setChartTypeName(this.state.cardType);
  };

  CardBuilderWizard.prototype.setGrid = function (w, h) {
    this.state.gridWidth = w || 4;
    this.state.gridHeight = h || 2;
    if ($('wb-grid-width')) $('wb-grid-width').value = this.state.gridWidth;
    if ($('wb-grid-height')) $('wb-grid-height').value = this.state.gridHeight;
    if ($('wb-h-gridWidth')) $('wb-h-gridWidth').value = this.state.gridWidth;
    if ($('wb-h-gridHeight')) $('wb-h-gridHeight').value = this.state.gridHeight;
  };

  CardBuilderWizard.prototype.setRefresh = function (v) {
    this.state.refreshInterval = (v == null ? 300 : v);
    if ($('wb-refresh-interval')) $('wb-refresh-interval').value = String(this.state.refreshInterval);
    if ($('wb-h-refreshInterval')) $('wb-h-refreshInterval').value = this.state.refreshInterval;
  };

  CardBuilderWizard.prototype.setChartTypeName = function (type) {
    var el = $('wb-chart-type-name'); if (el) el.textContent = type || 'الرسم';
  };

  CardBuilderWizard.prototype.renderPalettes = function () {
    var picker = $('wb-palette-picker');
    if (!picker) return;
    var self = this;
    picker.innerHTML = '';
    (global.CardBuilderPalettes || []).forEach(function (p) {
      var btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'wd-btn wd-btn--ghost wd-btn--sm';
      btn.setAttribute('data-palette', p.id);
      btn.setAttribute('aria-pressed', p.id === self.state.palette ? 'true' : 'false');
      if (p.id === self.state.palette) btn.style.borderColor = 'var(--c-primary)';
      var sw = '<span style="display:inline-flex;gap:4px;margin-inline-end:6px;vertical-align:middle;">';
      (p.colors || []).forEach(function (c) {
        sw += '<span style="width:12px;height:12px;border-radius:50%;background:' + c + ';display:inline-block;"></span>';
      });
      sw += '</span>';
      btn.innerHTML = sw + '<span>' + escapeHtml(p.name) + '</span>';
      btn.addEventListener('click', function () { self.selectPalette(p.id); });
      picker.appendChild(btn);
    });
  };

  CardBuilderWizard.prototype.selectPalette = function (id) {
    this.state.palette = id;
    if ($('wb-h-colorPalette')) $('wb-h-colorPalette').value = id;
    var btns = document.querySelectorAll('.wb-palette-picker [data-palette]');
    Array.prototype.forEach.call(btns, function (b) {
      var active = b.getAttribute('data-palette') === id;
      b.setAttribute('aria-pressed', active ? 'true' : 'false');
      b.style.borderColor = active ? 'var(--c-primary)' : '';
    });
    // re-render last preview with the new palette (no re-fetch needed)
    if (this._lastResult && this._lastResult.status === 'success') {
      this.renderSyncfusion(this._lastResult);
    }
  };

  CardBuilderWizard.prototype.getPaletteColors = function () {
    var pal = (global.CardBuilderPalettes || []).filter(function (p) { return p.id === this.state.palette; }, this);
    return (pal && pal[0] && pal[0].colors) ? pal[0].colors : ['#1F4E79', '#2E6DA4', '#8FBCDE'];
  };

  /* ----------------------- FILTERS (advanced) ----------------------- */
  CardBuilderWizard.prototype.wireFilters = function () {
    var self = this;
    var add = $('wb-add-filter'), container = $('wb-filters-container');
    if (add) add.addEventListener('click', function () { self.addFilterRow(); });
    if (container) container.addEventListener('click', function (e) {
      var rm = e.target.closest ? e.target.closest('.wb-filter-remove') : null;
      if (rm) {
        var row = rm.closest('.wb-filter-row');
        if (row && row.parentNode && row.parentNode.children.length > 1) row.parentNode.removeChild(row);
      }
    });
  };

  CardBuilderWizard.prototype.addFilterRow = function () {
    var container = $('wb-filters-container');
    if (!container) return;
    var first = container.querySelector('.wb-filter-row');
    if (!first) return;
    var clone = first.cloneNode(true);
    var inputs = clone.querySelectorAll('input');
    Array.prototype.forEach.call(inputs, function (i) { i.value = ''; });
    container.appendChild(clone);
  };

  CardBuilderWizard.prototype.collectFilters = function () {
    var container = $('wb-filters-container');
    var out = [];
    if (!container) return out;
    var rows = container.querySelectorAll('.wb-filter-row');
    Array.prototype.forEach.call(rows, function (r) {
      var k = r.querySelector('.wb-filter-key');
      var v = r.querySelector('.wb-filter-value');
      if (k && v && k.value.trim() !== '') out.push({ key: k.value.trim(), value: v.value.trim() });
    });
    return out;
  };

  /* ----------------------- LIVE PREVIEW (core) ----------------------- */
  CardBuilderWizard.prototype.getPreviewSql = function () {
    switch (this.state.sourceType) {
      case 'CustomSQL': return (this.state.customSql || '').trim();
      case 'SqlTable': return this.state.selectedTable ? ('SELECT TOP 10 * FROM [' + this.state.selectedTable.sqlTargetTable + ']') : '';
      case 'Template': return (this.state.previewSql || '').trim();
      case 'SavedQuery': return '';
      default: return '';
    }
  };

  CardBuilderWizard.prototype.buildPreviewRequest = function () {
    return {
      chartType: this.state.cardType,
      dataSourceType: 'SQL Query',
      sqlQuery: this.getPreviewSql(),
      sqlSource: this.state.selectedTable ? this.state.selectedTable.sqlTargetTable : null,
      previewRowLimit: 10
    };
  };

  CardBuilderWizard.prototype.schedulePreview = function () {
    var self = this;
    if (this._previewTimer) clearTimeout(this._previewTimer);
    this._previewTimer = setTimeout(function () { self.executePreview(); }, 400);
  };

  CardBuilderWizard.prototype.maybeInitialPreview = function () {
    if (this.getPreviewSql()) this.schedulePreview();
  };

  CardBuilderWizard.prototype.retry = function () {
    if (this._lastRequest) this.executePreview(this._lastRequest);
    else this.executePreview();
  };

  CardBuilderWizard.prototype.executePreview = function (reqOverride) {
    var self = this;
    var req = reqOverride || this.buildPreviewRequest();
    this._lastRequest = req;
    if (!req || !req.sqlQuery) {
      this.setPreviewState('idle');
      return;
    }
    this.setPreviewState('loading');
    this.setConnection('online');

    fetch(this.opts.previewApiUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'XMLHttpRequest' },
      body: JSON.stringify(req)
    })
      .then(function (r) { if (!r.ok) throw new Error('HTTP ' + r.status); return r.json(); })
      .then(function (result) { self.handlePreviewResult(result || {}); })
      .catch(function (err) {
        self.setConnection('offline');
        self.setPreviewState('error', 'تعذر الاتصال بخدمة المعاينة: ' + (err && err.message ? err.message : err));
      });
  };

  CardBuilderWizard.prototype.handlePreviewResult = function (result) {
    this._lastResult = result;
    this._lastChartType = result.chartType;
    if (result.status === 'success') {
      this.populateMeasurementColumns(result.columns, result.sampleData);
      this.autoFillMeasurement(result.columns, result.sampleData);
      this.setPreviewState('success');
      this.renderSyncfusion(result);
    } else if (result.status === 'empty') {
      this.populateMeasurementColumns(result.columns, result.sampleData);
      this.setPreviewState('empty', 'الاستعلام نُفّذ بنجاح لكنه لم يرجع أي صفوف.');
    } else {
      this.setConnection('offline');
      this.setPreviewState('error', result.errorMessage || 'حدث خطأ أثناء توليد المعاينة.');
    }
  };

  CardBuilderWizard.prototype.detectNumeric = function (columns, sampleData) {
    var out = [];
    if (!sampleData || !sampleData.length) return out;
    var row = sampleData[0];
    (columns || []).forEach(function (c) {
      var v = row[c];
      if (typeof v === 'number') out.push(c);
    });
    return out;
  };

  CardBuilderWizard.prototype.populateMeasurementColumns = function (columns, sampleData) {
    if (!columns || !columns.length) return;
    var numeric = this.detectNumeric(columns, sampleData);
    var self = this;
    ['wb-measurement', 'wb-measurement-field'].forEach(function (id) {
      var sel = $(id);
      if (!sel) return;
      var cur = sel.value;
      sel.innerHTML = '<option value="">اختر قياساً...</option>';
      columns.forEach(function (c) {
        var o = document.createElement('option');
        o.value = c;
        o.textContent = c + (numeric.indexOf(c) >= 0 ? ' (رقمي)' : '');
        sel.appendChild(o);
      });
      if (cur) sel.value = cur;
    });

    // Also populate KPI column dropdowns (TASK-KPI-006)
    var kpiValueCol = $('wb-kpi-value-column');
    var kpiDateCol = $('wb-kpi-date-column');
    var kpiCategoryCol = $('wb-kpi-category-column');

    if (kpiValueCol) {
      var curVal = kpiValueCol.value;
      kpiValueCol.innerHTML = '<option value="">اختر عموداً...</option>';
      columns.forEach(function (col) {
        kpiValueCol.innerHTML += '<option value="' + escapeHtml(col) + '">' + escapeHtml(col) + (numeric.indexOf(col) >= 0 ? ' (رقمي)' : '') + '</option>';
      });
      if (curVal) kpiValueCol.value = curVal;
    }
    if (kpiDateCol) {
      var curDate = kpiDateCol.value;
      kpiDateCol.innerHTML = '<option value="">اختر عموداً...</option>';
      columns.forEach(function (col) {
        kpiDateCol.innerHTML += '<option value="' + escapeHtml(col) + '">' + escapeHtml(col) + '</option>';
      });
      if (curDate) kpiDateCol.value = curDate;
    }
    if (kpiCategoryCol) {
      var curCat = kpiCategoryCol.value;
      kpiCategoryCol.innerHTML = '<option value="">بدون تصنيف</option>';
      columns.forEach(function (col) {
        kpiCategoryCol.innerHTML += '<option value="' + escapeHtml(col) + '">' + escapeHtml(col) + '</option>';
      });
      if (curCat) kpiCategoryCol.value = curCat;
    }
  };

  CardBuilderWizard.prototype.autoFillMeasurement = function (columns, sampleData) {
    var numeric = this.detectNumeric(columns, sampleData);
    if (!this.state.measurement && numeric.length) {
      this.state.measurement = numeric[0];
      var m = $('wb-measurement'); if (m) m.value = numeric[0];
      var mf = $('wb-measurement-field'); if (mf) mf.value = numeric[0];
      var sm = $('wb-sql-measurement'); if (sm) sm.value = numeric[0];
    }
  };

  CardBuilderWizard.prototype.renderSyncfusion = function (result) {
    var self = this;
    var content = $('wb-preview-content');
    if (!content) return;
    content.innerHTML = '';
    if (this._previewComp && typeof this._previewComp.destroy === 'function') {
      try { this._previewComp.destroy(); } catch (e) { /* ignore */ }
    }
    this._previewComp = null;

    var host = document.createElement('div');
    host.style.width = '100%';
    var isTable = (result.chartType === 'Table');
    host.style.height = isTable ? '360px' : '340px';
    content.appendChild(host);

    try {
      if (isTable) {
        var gridCfg = {};
        if (result.chartConfig) {
          for (var k in result.chartConfig) { if (result.chartConfig.hasOwnProperty(k)) gridCfg[k] = result.chartConfig[k]; }
        }
        gridCfg.dataSource = result.sampleData || [];
        if (!gridCfg.columns) {
          gridCfg.columns = (result.columns || []).map(function (c) { return { field: c, headerText: c, width: '120' }; });
        }
        gridCfg.allowSorting = true;
        gridCfg.allowPaging = false;
        gridCfg.height = '100%';
        this._previewComp = new global.ej.grids.Grid(gridCfg, host);
      } else {
        var cfg = {};
        if (result.chartConfig) {
          for (var k2 in result.chartConfig) { if (result.chartConfig.hasOwnProperty(k2)) cfg[k2] = result.chartConfig[k2]; }
        }
        cfg.width = '100%';
        cfg.height = '100%';
        var colors = self.getPaletteColors();
        if (colors && colors.length) cfg.palettes = [colors];
        // Gauge produces a "LinearGauge" series which is not a Chart series type — fall back to Column.
        if (cfg.series && cfg.series[0] && cfg.series[0].type === 'LinearGauge') cfg.series[0].type = 'Column';
        this._previewComp = new global.ej.charts.Chart(cfg, host);
      }
    } catch (e) {
      this.setConnection('offline');
      this.setPreviewState('error', 'تعذر عرض العنصر: ' + (e && e.message ? e.message : e));
    }
  };

  /* ----------------------- PREVIEW STATES ----------------------- */
  CardBuilderWizard.prototype.setPreviewState = function (s, msg) {
    var skeleton = $('wb-preview-skeleton'), content = $('wb-preview-content'),
        empty = $('wb-preview-empty'), error = $('wb-preview-error');
    hide(skeleton); hide(content); hide(empty); hide(error);
    if (s === 'loading') show(skeleton);
    else if (s === 'success') show(content);
    else if (s === 'empty') {
      show(empty);
      if (msg) { var p = empty.querySelector('p'); if (p) p.textContent = msg; }
    } else if (s === 'error') {
      show(error);
      if (msg) { var m = $('wb-preview-error-msg'); if (m) m.textContent = msg; }
    } else { // idle / no-sql-yet
      show(empty);
    }
  };

  CardBuilderWizard.prototype.setConnection = function (status) {
    var el = $('wb-connection-status');
    if (!el) return;
    var dot = el.querySelector('.wb-connection-status__dot');
    var txt = el.querySelector('.wb-connection-status__text');
    if (status === 'online') {
      if (txt) txt.textContent = 'متصل';
      if (dot) dot.style.background = 'var(--c-success)';
      el.classList.remove('wb-offline'); el.classList.add('wb-online');
    } else {
      if (txt) txt.textContent = 'غير متصل';
      if (dot) dot.style.background = 'var(--c-error)';
      el.classList.remove('wb-online'); el.classList.add('wb-offline');
    }
  };

  /* ----------------------- SAVE / CANCEL ----------------------- */
  CardBuilderWizard.prototype.wireSave = function () {
    var self = this;
    var save = $('wb-btn-save'), saveAdd = $('wb-btn-save-add');
    if (save) save.addEventListener('click', function () { self.submitForm('save'); });
    if (saveAdd) saveAdd.addEventListener('click', function () { self.submitForm('saveAndAddAnother'); });
  };

  CardBuilderWizard.prototype.canSave = function () {
    return !!this.state.cardType && this.hasSource() && !!this.state.title.trim() && !!this.state.displayName.trim();
  };

  CardBuilderWizard.prototype.hasSource = function () {
    switch (this.state.sourceType) {
      case 'Template': return !!this.state.currentTemplate;
      case 'SqlTable': return !!this.state.selectedTable;
      case 'CustomSQL': return (this.state.customSql || '').trim().length > 0;
      case 'SavedQuery': return false;
      default: return false;
    }
  };

  CardBuilderWizard.prototype.updateFooter = function () {
    var prev = $('wb-btn-prev'), next = $('wb-btn-next'), save = $('wb-btn-save'), saveAdd = $('wb-btn-save-add');
    var step = this.state.step;
    var maxStep = this.isKpiStepVisible() ? 5 : 4;
    if (prev) { prev.disabled = (step === 1); prev.setAttribute('aria-disabled', step === 1 ? 'true' : 'false'); }
    if (next) {
      var canNext = this.validateStepSilent(step) && step < maxStep;
      next.disabled = !canNext;
      next.setAttribute('aria-disabled', canNext ? 'false' : 'true');
    }
    var saveOk = this.canSave();
    if (save) { save.disabled = !saveOk; save.setAttribute('aria-disabled', saveOk ? 'false' : 'true'); }
    if (saveAdd) { saveAdd.disabled = !saveOk; saveAdd.setAttribute('aria-disabled', saveOk ? 'false' : 'true'); }
  };

  // non-UI validation used by footer (does not write error text)
  CardBuilderWizard.prototype.validateStepSilent = function (step) {
    if (step === 1) return true;
    if (step === 2) return this.hasSource();
    if (step === 3) return !!this.state.title.trim() && !!this.state.displayName.trim();
    if (step === 4 && this.isKpiStepVisible()) {
      var valCol = $('wb-kpi-value-column');
      var dateCol = $('wb-kpi-date-column');
      return (valCol && !!valCol.value) && (dateCol && !!dateCol.value);
    }
    return true;
  };

  /* ----------------------- KPI STEP VISIBILITY ----------------------- */
  CardBuilderWizard.prototype.isKpiStepVisible = function () {
    return this.state.cardType === 'KPI';
  };

  /* ----------------------- KPI MODE PICKER (Step 4) ----------------------- */
  CardBuilderWizard.prototype.initKpiModePicker = function () {
    var modeCards = document.querySelectorAll('.wb-kpi-mode-card');
    var hiddenInput = $('wb-h-kpiMode');
    var changeSection = $('wb-kpi-change-section');
    var sparklineSection = $('wb-kpi-sparkline-section');
    var totalSection = $('wb-kpi-total-section');
    var dateSection = $('wb-kpi-date-section');

    Array.prototype.forEach.call(modeCards, function (card) {
      card.addEventListener('click', function () {
        // Update selection
        Array.prototype.forEach.call(modeCards, function (c) {
          c.setAttribute('aria-checked', 'false');
          c.setAttribute('tabindex', '-1');
        });
        card.setAttribute('aria-checked', 'true');
        card.setAttribute('tabindex', '0');

        var mode = card.getAttribute('data-mode');
        if (hiddenInput) hiddenInput.value = mode;

        // Show/hide sections based on mode
        if (mode === 'simple') {
          if (changeSection) changeSection.style.display = 'none';
          if (sparklineSection) sparklineSection.style.display = 'none';
          if (totalSection) totalSection.style.display = 'none';
          if (dateSection) dateSection.style.display = 'none';
        } else if (mode === 'withChange') {
          if (changeSection) changeSection.style.display = '';
          if (sparklineSection) sparklineSection.style.display = 'none';
          if (totalSection) totalSection.style.display = 'none';
          if (dateSection) dateSection.style.display = '';
        } else if (mode === 'composite') {
          if (changeSection) changeSection.style.display = '';
          if (sparklineSection) sparklineSection.style.display = '';
          if (totalSection) totalSection.style.display = '';
          if (dateSection) dateSection.style.display = '';
        }
      });
      card.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); card.click(); }
      });
    });
  };

  /* ----------------------- DATE FILTER MODE TOGGLE (Step 4) ----------------------- */
  CardBuilderWizard.prototype.initDateFilterMode = function () {
    var dateFilterMode = $('wb-kpi-date-filter-mode');
    if (dateFilterMode) {
      dateFilterMode.addEventListener('change', function () {
        var mode = this.value;
        var fixedStart = $('wb-kpi-fixed-start-field');
        var fixedEnd = $('wb-kpi-fixed-end-field');
        var relativeDays = $('wb-kpi-relative-days-field');
        if (fixedStart) fixedStart.style.display = mode === 'fixed' ? '' : 'none';
        if (fixedEnd) fixedEnd.style.display = mode === 'fixed' ? '' : 'none';
        if (relativeDays) relativeDays.style.display = mode === 'relative' ? '' : 'none';
      });
    }
  };

  CardBuilderWizard.prototype.cleanupDuplicateNames = function () {
    // hidden inputs are the canonical posted values; strip conflicting names
    ['wb-saved-query', 'wb-sql-table', 'wb-grid-width', 'wb-grid-height', 'wb-grid-x', 'wb-grid-y', 'wb-refresh-interval', 'wb-custom-sql',
     'wb-kpi-value-column', 'wb-kpi-date-column', 'wb-kpi-category-column', 'wb-kpi-change-source', 'wb-kpi-sparkline-months',
     'wb-kpi-grand-total-source', 'wb-kpi-date-filter-mode', 'wb-kpi-fixed-start', 'wb-kpi-fixed-end', 'wb-kpi-relative-days'
    ].forEach(function (id) {
      var el = $(id); if (el) el.removeAttribute('name');
    });
    // measurement names are resolved in syncHiddenInputs (CustomSQL vs others)
  };

  CardBuilderWizard.prototype.syncHiddenInputs = function () {
    var s = this.state;
    if ($('wb-h-cardType')) $('wb-h-cardType').value = s.cardType;
    if ($('wb-h-sourceType')) $('wb-h-sourceType').value = s.sourceType;
    if ($('wb-h-sourceId')) $('wb-h-sourceId').value = s.sourceId;
    if ($('wb-h-customSql')) $('wb-h-customSql').value = s.customSql;
    if ($('wb-h-gridWidth')) $('wb-h-gridWidth').value = s.gridWidth;
    if ($('wb-h-gridHeight')) $('wb-h-gridHeight').value = s.gridHeight;
    if ($('wb-h-gridX')) $('wb-h-gridX').value = s.gridX;
    if ($('wb-h-gridY')) $('wb-h-gridY').value = s.gridY;
    if ($('wb-h-colorPalette')) $('wb-h-colorPalette').value = s.palette;
    if ($('wb-h-refreshInterval')) $('wb-h-refreshInterval').value = s.refreshInterval;
    if ($('wb-h-chartOptionsJson')) $('wb-h-chartOptionsJson').value = JSON.stringify({ palette: s.palette, chartType: s.cardType });

    if ($('wb-h-filtersJson')) $('wb-h-filtersJson').value = JSON.stringify(this.collectFilters());
    if ($('wb-h-drillDownConfigJson')) {
      var lvl = $('wb-drill-level') ? $('wb-drill-level').value : '';
      var fld = $('wb-drill-field') ? $('wb-drill-field').value : '';
      $('wb-h-drillDownConfigJson').value = JSON.stringify({ level: lvl, field: fld });
    }
    if ($('wb-h-customLabelsJson')) {
      var yl = $('wb-custom-label') ? $('wb-custom-label').value : '';
      var tf = $('wb-tooltip-format') ? $('wb-tooltip-format').value : '';
      $('wb-h-customLabelsJson').value = JSON.stringify({ yAxis: yl, tooltip: tf });
    }

    // measurement canonical field resolution
    if (s.sourceType === 'CustomSQL') {
      var sm = $('wb-sql-measurement'); if (sm) { sm.setAttribute('name', 'measurement'); sm.value = s.measurement; }
      var m = $('wb-measurement'); if (m) m.removeAttribute('name');
    } else {
      var m2 = $('wb-measurement'); if (m2) { m2.setAttribute('name', 'measurement'); m2.value = s.measurement; }
      var sm2 = $('wb-sql-measurement'); if (sm2) sm2.removeAttribute('name');
    }

    // Sync KPI hidden fields (TASK-KPI-006)
    this.syncKpiHiddenFields();
  };

  CardBuilderWizard.prototype.syncKpiHiddenFields = function () {
    var kpiMode = $('wb-h-kpiMode') ? $('wb-h-kpiMode').value : 'simple';
    var isChange = kpiMode === 'withChange' || kpiMode === 'composite';
    var isComposite = kpiMode === 'composite';

    if ($('wb-h-valueColumn')) $('wb-h-valueColumn').value = $('wb-kpi-value-column') ? $('wb-kpi-value-column').value : '';
    if ($('wb-h-dateColumn')) $('wb-h-dateColumn').value = $('wb-kpi-date-column') ? $('wb-kpi-date-column').value : '';
    if ($('wb-h-categoryColumn')) $('wb-h-categoryColumn').value = $('wb-kpi-category-column') ? $('wb-kpi-category-column').value : '';
    if ($('wb-h-showChange')) $('wb-h-showChange').value = isChange ? 'true' : 'false';
    if ($('wb-h-changeSource')) $('wb-h-changeSource').value = $('wb-kpi-change-source') ? $('wb-kpi-change-source').value : 'previousPeriod';
    if ($('wb-h-showSparkline')) $('wb-h-showSparkline').value = isComposite ? 'true' : 'false';
    if ($('wb-h-sparklineMonths')) $('wb-h-sparklineMonths').value = $('wb-kpi-sparkline-months') ? $('wb-kpi-sparkline-months').value : '6';
    if ($('wb-h-showGrandTotal')) $('wb-h-showGrandTotal').value = isComposite ? 'true' : 'false';
    if ($('wb-h-grandTotalSource')) $('wb-h-grandTotalSource').value = $('wb-kpi-grand-total-source') ? $('wb-kpi-grand-total-source').value : 'sameTable';
    if ($('wb-h-dateFilterMode')) $('wb-h-dateFilterMode').value = $('wb-kpi-date-filter-mode') ? $('wb-kpi-date-filter-mode').value : 'dashboard';
    if ($('wb-h-fixedStartDate')) $('wb-h-fixedStartDate').value = $('wb-kpi-fixed-start') ? $('wb-kpi-fixed-start').value : '';
    if ($('wb-h-fixedEndDate')) $('wb-h-fixedEndDate').value = $('wb-kpi-fixed-end') ? $('wb-kpi-fixed-end').value : '';
    if ($('wb-h-relativeDays')) $('wb-h-relativeDays').value = $('wb-kpi-relative-days') ? $('wb-kpi-relative-days').value : '30';
  };

  CardBuilderWizard.prototype.submitForm = function (action) {
    if (!this.form) return;
    if (!this.canSave()) {
      // surface the offending step error and jump there
      var target = !this.hasSource() ? 2 : 3;
      this.goToStep(target);
      this.validateStep(target);
      this.toast('يرجى إكمال الحقول المطلوبة قبل الحفظ.', 'error');
      return;
    }
    this.syncHiddenInputs();
    var act = $('wb-h-action');
    if (!act) {
      act = document.createElement('input');
      act.type = 'hidden';
      act.id = 'wb-h-action';
      act.name = 'action';
      this.form.appendChild(act);
    }
    act.value = action;
    // native POST to Builder page (OnPostAsync). Server simulates success + redirect.
    this.form.submit();
  };

  CardBuilderWizard.prototype.cancel = function () {
    if (global.confirm) {
      if (!global.confirm('هل تريد إلغاء إنشاء البطاقة والعودة للقائمة؟')) return;
    }
    global.location.href = '/admin-secure-panel/Cards/Index';
  };

  /* ----------------------- CLONE (best-effort) ----------------------- */
  CardBuilderWizard.prototype.tryClone = function () {
    var id = this.opts.cloneId;
    if (!id) return;
    var self = this;
    fetch('/api/dashboard/cardbuilder/clone/' + encodeURIComponent(id), { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
      .then(function (r) { if (!r.ok) throw new Error('notfound'); return r.json(); })
      .then(function (data) {
        if (!data) return;
        if (data.chartType && data.chartType !== self.state.cardType) self.selectType(data.chartType);
        if (data.title && !self.state.title) { self.state.title = data.title; if ($('wb-title')) $('wb-title').value = data.title; }
        if (data.displayName && !self.state.displayName) { self.state.displayName = data.displayName; if ($('wb-display-name')) $('wb-display-name').value = data.displayName; }
        if (data.sqlQuery && !self.state.customSql) {
          self.state.customSql = data.sqlQuery;
          self.state.previewSql = data.sqlQuery;
          if ($('wb-h-customSql')) $('wb-h-customSql').value = data.sqlQuery;
          if ($('wb-custom-sql')) $('wb-custom-sql').value = data.sqlQuery;
        }
        if (data.sourceType) {
          self.state.sourceType = data.sourceType;
          if ($('wb-h-sourceType')) $('wb-h-sourceType').value = data.sourceType;
          if ($('wb-source-type')) $('wb-source-type').value = data.sourceType;
          self.showSourcePanel(data.sourceType);
        }
        if (data.gridWidth) self.setGrid(data.gridWidth, data.gridHeight || self.state.gridHeight);
        if (data.refreshInterval != null) self.setRefresh(data.refreshInterval);
        self.updateFooter();
        self.maybeInitialPreview();
      })
      .catch(function () { /* fall back to DOM values already applied in readInitialDom */ });
  };

  /* ----------------------- TOAST (Vitality) ----------------------- */
  CardBuilderWizard.prototype.toast = function (msg, type) {
    var host = $('wd-toast-host');
    if (!host) return;
    var t = document.createElement('div');
    t.className = 'wd-toast wd-toast--' + (type || 'info');
    t.innerHTML = '<span class="wd-toast__icon">' + (type === 'error' ? '!' : '✓') + '</span><span class="wd-toast__msg"></span>';
    t.querySelector('.wd-toast__msg').textContent = msg;
    host.appendChild(t);
    setTimeout(function () { if (t.parentNode) t.parentNode.removeChild(t); }, 4200);
  };

  /* ===================================================================== */
  global.CardBuilderWizard = CardBuilderWizard;

})(window);
