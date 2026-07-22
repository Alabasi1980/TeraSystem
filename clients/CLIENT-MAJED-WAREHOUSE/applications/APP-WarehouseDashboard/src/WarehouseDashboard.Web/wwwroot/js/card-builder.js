/*
 * ==========================================================================
 *  card-builder.js  —  Card Builder Wizard controller
 *  Exposes window.CardBuilderWizard(opts).
 *
 *  opts = {
 *    previewApiUrl, tablesApiUrl, savedQueriesApiUrl,
 *    cloneId, initialData: { cardType, sourceType, sourceId, customSql, title, displayName }
 *  }
 *
 *  Reads/owns the existing DOM in Builder.cshtml and wires the wizard:
 *   - Step navigation + validation
 *   - Type picker (Step 1)
 *   - Source panels: Template / SqlTable / CustomSQL / SavedQuery (Step 2)
 *   - Template rendering + {TableName} substitution
 *   - Oracle tables fetch + dropdown
 *   - Live preview POST -> Chart render (chart AND table)
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
    this.opts.previewApiUrl = this.opts.previewApiUrl || '/api/dashboard/cardbuilder?handler=Preview';
    this.opts.tablesApiUrl = this.opts.tablesApiUrl || '/api/tablemappings/active';
    this.opts.savedQueriesApiUrl = this.opts.savedQueriesApiUrl || '/api/savedqueries';
    this.opts.editId = this.opts.editId || '';
    this.opts.initialData = this.opts.initialData || null;
    this.editMode = !!this.opts.editId;

    this.state = {
      step: 1,
      cardType: 'KPI',
      sourceType: 'Template',
      sourceId: '',
      customSql: '',
      selectedTable: null,   // { id, oracleSource, sqlTargetTable }
      currentTemplate: null, // template object
      previewSql: '',
      gridWidth: 4,
      gridHeight: 2,
      gridX: '',
      gridY: '',
      palette: 'primary',
      refreshInterval: 300,
      title: '',
      displayName: '',
      aggregationType: 'Sum',
      kpiMode: 'simple',
      valueColumn: '',
      dateColumn: '',
      categoryColumn: '',
      showChange: false,
      changeSource: 'previousPeriod',
      showSparkline: false,
      sparklineMonths: 6,
      showGrandTotal: false,
      grandTotalSource: 'sameTable',
      dateFilterMode: 'dashboard',
      fixedStartDate: '',
      fixedEndDate: '',
      relativeDays: 30,
      valueFormatType: 'Currency',
      valueUnit: ''
    };

    this.tables = [];
    this._previewChart = null;
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
    // Show saved SQL query in preview textarea immediately (edit mode fix)
    this.updateSqlDisplay();
    this.renderTemplates();
    this.renderPalettes();
    this.wireTypePicker();
    this.wireSourceType();
    this.wireFields();
    this.wireVisual();
    this.wireNav();
    this.wireSave();
    this.initKpiModePicker();
    this.initKpiColumnMappings();
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
    var id = this.opts.initialData || {};

    s.cardType = id.cardType || ($('wb-h-cardType').value) || 'KPI';
    s.sourceType = id.sourceType || ($('wb-h-sourceType').value) || 'Template';
    s.sourceId = id.sourceId != null ? id.sourceId : ($('wb-h-sourceId').value || '');
    s.customSql = id.customSql != null ? id.customSql : ($('wb-h-customSql').value || '');
    s.gridWidth = parseInt(id.gridWidth != null ? id.gridWidth : $('wb-h-gridWidth').value, 10) || 4;
    s.gridHeight = parseInt(id.gridHeight != null ? id.gridHeight : $('wb-h-gridHeight').value, 10) || 2;
    s.gridX = id.gridX != null ? id.gridX : ($('wb-h-gridX').value || '');
    s.gridY = id.gridY != null ? id.gridY : ($('wb-h-gridY').value || '');
    s.palette = id.colorPalette != null ? id.colorPalette : ($('wb-h-colorPalette').value || 'primary');
    s.refreshInterval = parseInt(id.refreshInterval != null ? id.refreshInterval : $('wb-h-refreshInterval').value, 10) || 300;
    s.title = id.title != null ? id.title : ($('wb-title').value || '');
    s.description = id.description != null ? id.description : ($('wb-h-description').value || '');
    s.aggregationType = id.aggregationType != null ? id.aggregationType : ($('wb-h-aggregationType').value || 'Sum');
    s.originalSourceType = id.originalSourceType != null ? id.originalSourceType : ($('wb-h-originalSourceType').value || 'SqlTable');
    s.originalSourceId = id.originalSourceId != null ? id.originalSourceId : ($('wb-h-originalSourceId').value || '');
    s.kpiMode = id.kpiMode != null ? id.kpiMode : ($('wb-h-kpiMode').value || 'simple');
    s.valueColumn = id.valueColumn != null ? id.valueColumn : ($('wb-h-valueColumn').value || '');
    s.dateColumn = id.dateColumn != null ? id.dateColumn : ($('wb-h-dateColumn').value || '');
    s.categoryColumn = id.categoryColumn != null ? id.categoryColumn : ($('wb-h-categoryColumn').value || '');
    s.showChange = this.parseBool(id.showChange != null ? id.showChange : $('wb-h-showChange').value);
    s.changeSource = id.changeSource != null ? id.changeSource : ($('wb-h-changeSource').value || 'previousPeriod');
    s.showSparkline = this.parseBool(id.showSparkline != null ? id.showSparkline : $('wb-h-showSparkline').value);
    s.sparklineMonths = parseInt(id.sparklineMonths != null ? id.sparklineMonths : $('wb-h-sparklineMonths').value, 10) || 6;
    s.showGrandTotal = this.parseBool(id.showGrandTotal != null ? id.showGrandTotal : $('wb-h-showGrandTotal').value);
    s.grandTotalSource = id.grandTotalSource != null ? id.grandTotalSource : ($('wb-h-grandTotalSource').value || 'sameTable');
    s.dateFilterMode = id.dateFilterMode != null ? id.dateFilterMode : ($('wb-h-dateFilterMode').value || 'dashboard');
    s.fixedStartDate = id.fixedStartDate != null ? id.fixedStartDate : ($('wb-h-fixedStartDate').value || '');
    s.fixedEndDate = id.fixedEndDate != null ? id.fixedEndDate : ($('wb-h-fixedEndDate').value || '');
    s.relativeDays = parseInt(id.relativeDays != null ? id.relativeDays : $('wb-h-relativeDays').value, 10) || 30;
    s.valueFormatType = id.valueFormatType || ($('wb-h-valueFormatType') ? $('wb-h-valueFormatType').value : 'Currency');
    s.valueUnit = id.valueUnit || ($('wb-h-valueUnit') ? $('wb-h-valueUnit').value : '');
    s.previewSql = id.sqlQuery != null ? id.sqlQuery : ($('wb-h-sqlQuery').value || '');

    // Edit-mode safety: older/server-rendered cards may carry the SQL in sqlQuery
    // while the CustomSQL textarea value is empty. For CustomSQL, the textarea,
    // preview SQL and posted customSql must all start from the persisted query.
    if (s.sourceType === 'CustomSQL') {
      if (!s.customSql && s.previewSql) s.customSql = s.previewSql;
      if (!s.previewSql && s.customSql) s.previewSql = s.customSql;
    }

    // reflect into the visible (display) controls too
    if ($('wb-grid-width')) $('wb-grid-width').value = s.gridWidth;
    if ($('wb-grid-height')) $('wb-grid-height').value = s.gridHeight;
    if ($('wb-grid-x')) $('wb-grid-x').value = s.gridX;
    if ($('wb-grid-y')) $('wb-grid-y').value = s.gridY;
    if ($('wb-refresh-interval')) $('wb-refresh-interval').value = String(s.refreshInterval);
    if ($('wb-custom-sql')) $('wb-custom-sql').value = s.customSql;
    if ($('wb-title')) $('wb-title').value = s.title;
    if ($('wb-description')) $('wb-description').value = s.description;
    if ($('wb-h-description')) $('wb-h-description').value = s.description;
    if ($('wb-aggregation-type')) $('wb-aggregation-type').value = s.aggregationType;
    var st = $('wb-source-type'); if (st) st.value = s.sourceType;
    if ($('wb-h-sourceType')) $('wb-h-sourceType').value = s.sourceType;
    if ($('wb-h-customSql')) $('wb-h-customSql').value = s.customSql;
    if ($('wb-h-sqlQuery')) $('wb-h-sqlQuery').value = s.previewSql;
  };

  CardBuilderWizard.prototype.parseBool = function (v) {
    if (typeof v === 'boolean') return v;
    if (v === null || v === undefined) return false;
    return String(v).toLowerCase() === 'true';
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
    this.applyInitialPalette();
    this.applyInitialKpiSettings();
    this.updateStepUI();
  };

  CardBuilderWizard.prototype.applyInitialPalette = function () {
    var self = this;
    var btns = document.querySelectorAll('.wb-palette-picker [data-palette]');
    Array.prototype.forEach.call(btns, function (b) {
      var active = b.getAttribute('data-palette') === self.state.palette;
      b.setAttribute('aria-pressed', active ? 'true' : 'false');
      b.style.borderColor = active ? 'var(--c-primary)' : '';
    });
  };

  CardBuilderWizard.prototype.applyInitialKpiSettings = function () {
    if (this.state.cardType !== 'KPI') return;

    var kpiMode = this.state.kpiMode || 'simple';
    var hiddenInput = $('wb-h-kpiMode');
    if (hiddenInput) hiddenInput.value = kpiMode;

    // select the correct mode card
    var modeCards = document.querySelectorAll('.wb-kpi-mode-card');
    Array.prototype.forEach.call(modeCards, function (c) {
      var active = c.getAttribute('data-mode') === kpiMode;
      c.setAttribute('aria-checked', active ? 'true' : 'false');
      c.setAttribute('tabindex', active ? '0' : '-1');
    });

    // trigger click on the selected mode card to apply section visibility
    var selected = document.querySelector('.wb-kpi-mode-card[data-mode="' + kpiMode + '"]');
    if (selected) selected.click();

    // Pre-populate column dropdowns with saved values immediately (edit mode fix)
    // This ensures saved values are visible before the preview API completes
    var valueCol = $('wb-kpi-value-column');
    var dateCol = $('wb-kpi-date-column');
    var categoryCol = $('wb-kpi-category-column');
    this.addSavedKpiOption(valueCol, this.state.valueColumn);
    this.addSavedKpiOption(dateCol, this.state.dateColumn);
    this.addSavedKpiOption(categoryCol, this.state.categoryColumn);
    if (valueCol && this.state.valueColumn) valueCol.value = this.state.valueColumn;
    if (dateCol && this.state.dateColumn) dateCol.value = this.state.dateColumn;
    if (categoryCol && this.state.categoryColumn) categoryCol.value = this.state.categoryColumn;

    var changeSource = $('wb-kpi-change-source');
    if (changeSource && this.state.changeSource) changeSource.value = this.state.changeSource;

    var sparklineMonths = $('wb-kpi-sparkline-months');
    if (sparklineMonths) sparklineMonths.value = String(this.state.sparklineMonths);

    var grandTotalSource = $('wb-kpi-grand-total-source');
    if (grandTotalSource && this.state.grandTotalSource) grandTotalSource.value = this.state.grandTotalSource;

    var dateFilterMode = $('wb-kpi-date-filter-mode');
    if (dateFilterMode) {
      dateFilterMode.value = this.state.dateFilterMode;
      this.applyDateFilterMode(this.state.dateFilterMode);
    }

    var fixedStart = $('wb-kpi-fixed-start');
    var fixedEnd = $('wb-kpi-fixed-end');
    var relativeDays = $('wb-kpi-relative-days');
    if (fixedStart && this.state.fixedStartDate) fixedStart.value = this.state.fixedStartDate;
    if (fixedEnd && this.state.fixedEndDate) fixedEnd.value = this.state.fixedEndDate;
    if (relativeDays) relativeDays.value = String(this.state.relativeDays);

    this.syncKpiHiddenFields();
  };

  CardBuilderWizard.prototype.applyDateFilterMode = function (mode) {
    var fixedStart = $('wb-kpi-fixed-start-field');
    var fixedEnd = $('wb-kpi-fixed-end-field');
    var relativeDays = $('wb-kpi-relative-days-field');
    if (fixedStart) fixedStart.style.display = mode === 'fixed' ? '' : 'none';
    if (fixedEnd) fixedEnd.style.display = mode === 'fixed' ? '' : 'none';
    if (relativeDays) relativeDays.style.display = mode === 'relative' ? '' : 'none';
  };

  /* ---------- Helper: ensure saved KPI column value is an option (edit mode fix) ---------- */
  CardBuilderWizard.prototype.addSavedKpiOption = function (sel, value) {
    if (!sel || !value) return;
    // Check if value already exists as an option
    for (var i = 0; i < sel.options.length; i++) {
      if (sel.options[i].value === value) return; // already present
    }
    // Add it as a temporary option so it shows in the UI immediately
    var opt = document.createElement('option');
    opt.value = value;
    opt.textContent = value + ' (محفوظ)';
    sel.appendChild(opt);
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
    var maxStep = 5;
    if (this.state.step >= maxStep) return;
    if (!this.validateStep(this.state.step)) return;
    this.goToStep(this.state.step + 1);
  };
  CardBuilderWizard.prototype.prev = function () {
    if (this.state.step <= 1) return;
    this.goToStep(this.state.step - 1);
  };
  CardBuilderWizard.prototype.goToStep = function (n) {
    var currentStep = this.state.step;
    // Skip step 4 (KPI) when chartType is not KPI
    if (n === 4 && !this.isKpiStepVisible()) {
      n = currentStep > 4 ? 3 : 5;
    }
    if (n < 1) n = 1;
    if (n > 5) n = 5;
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
      if (!this.state.title.trim()) msg = 'الرجاء إدخال اسم البطاقة.';
    } else if (step === 4 && this.isKpiStepVisible()) {
      var valCol = $('wb-kpi-value-column');
      var dateCol = $('wb-kpi-date-column');
      var kpiModeInput = $('wb-h-kpiMode');
      var kpiMode = kpiModeInput ? kpiModeInput.value : 'simple';
      if (valCol && !valCol.value) msg = 'الرجاء اختيار عمود القيمة.';
      else if (kpiMode !== 'simple' && dateCol && !dateCol.value) msg = 'الرجاء اختيار عمود التاريخ.';
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
    this.updateSqlDisplay();

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
    this.updateSqlDisplay();
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
      return;
    }
    var t = null;
    for (var i = 0; i < this.tables.length; i++) {
      if (this.tables[i].sqlTargetTable === val || String(this.tables[i].id) === String(val)) { t = this.tables[i]; break; }
    }
    this.state.selectedTable = t || { sqlTargetTable: val };
    this.state.sourceId = val;
    if ($('wb-h-sourceId')) $('wb-h-sourceId').value = val;
    this.state.previewSql = 'SELECT TOP 10 * FROM [' + val + ']';
    this.updateSqlDisplay();
    this.schedulePreview();
    this.updateFooter();
  };

  /* ----------------------- FIELDS (Step 3) ----------------------- */
  CardBuilderWizard.prototype.wireFields = function () {
    var self = this;
    var title = $('wb-title'), desc = $('wb-description'), cs = $('wb-custom-sql');
    if (title) title.addEventListener('input', function () { self.state.title = title.value; self.updateFooter(); });
    if (desc) desc.addEventListener('input', function () { self.state.description = desc.value; if ($('wb-h-description')) $('wb-h-description').value = desc.value; });
    if (cs) cs.addEventListener('input', function () {
        self.state.customSql = cs.value;
        self.state.previewSql = cs.value;
        if ($('wb-h-customSql')) $('wb-h-customSql').value = cs.value;
        self.schedulePreview();
        self.updateFooter();
    });
  };

  /* ----------------------- VISUAL (Step 5) ----------------------- */
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
    var el = $('wb-chart-type-badge'); if (el) el.textContent = type || 'الرسم';
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
      this.renderChart(this._lastResult);
    }
  };

  CardBuilderWizard.prototype.getPaletteColors = function () {
    var pal = (global.CardBuilderPalettes || []).filter(function (p) { return p.id === this.state.palette; }, this);
    return (pal && pal[0] && pal[0].colors) ? pal[0].colors : ['#1F4E79', '#2E6DA4', '#8FBCDE'];
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
      this.populateColumnMappings(result.columns, result.sampleData);
      this.updateSqlDisplay();
      this.setPreviewState('success');
      this.renderChart(result);
    } else if (result.status === 'empty') {
      this.populateColumnMappings(result.columns, result.sampleData);
      this.updateSqlDisplay();
      this.setPreviewState('empty', 'الاستعلام نُفّذ بنجاح لكنه لم يرجع أي صفوف.');
    } else {
      this.updateSqlDisplay();
      this.setConnection('offline');
      this.setPreviewState('error', result.errorMessage || 'حدث خطأ أثناء توليد المعاينة.');
    }
  };

  CardBuilderWizard.prototype.detectNumeric = function (columns, sampleData) {
    var out = [];
    if (!sampleData || !sampleData.length) return out;
    (columns || []).forEach(function (c) {
      var values = sampleData.map(function (row) {
        return row && Object.prototype.hasOwnProperty.call(row, c) ? row[c] : null;
      }).filter(function (value) {
        return !isEmptySampleValue(value);
      });
      var hasNumericValue = values.some(function (value) {
        return isSafelyNumeric(value);
      });
      var hasUnsafeValue = values.some(function (value) {
        return !isSafelyNumeric(value);
      });
      if (hasNumericValue && !hasUnsafeValue) out.push(c);
    });
    return out;
  };

  function isEmptySampleValue(value) {
    return value === null || value === undefined || (typeof value === 'string' && value.trim() === '');
  }

  function isSafelyNumeric(value) {
    if (typeof value === 'number') return isFinite(value);
    if (typeof value !== 'string') return false;
    var text = value.trim();
    if (!text) return false;
    if (/^\d{4}[-\/]\d{1,2}[-\/]\d{1,2}/.test(text)) return false;
    var numericValue = Number(text);
    return isFinite(numericValue);
  }

  CardBuilderWizard.prototype.detectDateColumns = function (columns, sampleData) {
    var out = [];
    if (!sampleData || !sampleData.length) return out;
    (columns || []).forEach(function (c) {
      var hasDateValue = sampleData.some(function (row) {
        var v = row ? row[c] : null;
        return v instanceof Date || (typeof v === 'string' && /^\d{4}[-\/]\d{1,2}[-\/]\d{1,2}/.test(v));
      });
      // Check if any sampled value looks like a date, or use safe column-name clues.
      if (hasDateValue) {
        out.push(c);
      }
      else if (/date|time|dt\b/i.test(c)) {
        out.push(c);
      }
    });
    return out;
  };

  CardBuilderWizard.prototype.populateColumnMappings = function (columns, sampleData) {
    if (!columns || !columns.length) {
      this.updateKpiValueColumnOptions([]);
      return;
    }
    var numeric = this.detectNumeric(columns, sampleData);
    var dateCols = this.detectDateColumns(columns, sampleData);
    // Restore saved column values after repopulation (edit mode fix)
    this.updateKpiValueColumnOptions(numeric, this.state.valueColumn);

    var kpiDateCol = $('wb-kpi-date-column');
    if (kpiDateCol) {
      var curDate = kpiDateCol.value || this.state.dateColumn;
      kpiDateCol.innerHTML = '<option value="">اختر عموداً...</option>';
      (dateCols || []).forEach(function (col) {
        kpiDateCol.innerHTML += '<option value="' + escapeHtml(col) + '">' + escapeHtml(col) + '</option>';
      });
      if (curDate && dateCols.indexOf(curDate) >= 0) kpiDateCol.value = curDate;
    }

    var kpiCategoryCol = $('wb-kpi-category-column');
    if (kpiCategoryCol) {
      var curCat = kpiCategoryCol.value || this.state.categoryColumn;
      kpiCategoryCol.innerHTML = '<option value="">بدون تصنيف</option>';
      (columns || []).forEach(function (col) {
        kpiCategoryCol.innerHTML += '<option value="' + escapeHtml(col) + '">' + escapeHtml(col) + '</option>';
      });
      if (curCat && columns.indexOf(curCat) >= 0) kpiCategoryCol.value = curCat;
    }
    this.updateFooter();
  };

  CardBuilderWizard.prototype.updateKpiValueColumnOptions = function (numericColumns, savedValue) {
    var sel = $('wb-kpi-value-column');
    var msg = $('wb-kpi-value-column-message');
    if (!sel) return;

    var previousValue = savedValue || sel.value;
    sel.innerHTML = '';

    if (!numericColumns || !numericColumns.length) {
      sel.disabled = true;
      sel.innerHTML = '<option value="">لا توجد أعمدة رقمية متاحة</option>';
      if ($('wb-h-valueColumn')) $('wb-h-valueColumn').value = '';
      if (msg) {
        msg.textContent = 'لم يتم العثور على أعمدة رقمية في معاينة المصدر. اختر مصدراً يحتوي على قيمة رقمية.';
        show(msg);
      }
      this.updateFooter();
      return;
    }

    sel.disabled = false;
    sel.innerHTML = '<option value="">اختر عموداً...</option>';
    numericColumns.forEach(function (col) {
      var o = document.createElement('option');
      o.value = col;
      o.textContent = col;
      sel.appendChild(o);
    });

    if (previousValue && numericColumns.indexOf(previousValue) >= 0) {
      sel.value = previousValue;
    } else {
      sel.value = numericColumns[0];
    }
    if ($('wb-h-valueColumn')) $('wb-h-valueColumn').value = sel.value;

    if (msg) {
      msg.textContent = '';
      hide(msg);
    }
  };

  CardBuilderWizard.prototype.updateSqlDisplay = function () {
    var el = $('wb-preview-sql');
    if (el) el.value = this.state.previewSql || '';
  };

  CardBuilderWizard.prototype.renderChart = function (result) {
    var self = this;
    var content = $('wb-preview-content');
    if (!content) return;
    content.innerHTML = '';
    // Destroy previous chart
    if (this._previewChart) {
      try { this._previewChart.destroy(); } catch (e) { /* ignore */ }
    }
    this._previewChart = null;

    var host = document.createElement('div');
    host.style.width = '100%';
    var isTable = (result.chartType === 'Table');
    var isGauge = (result.chartType === 'Gauge');
    host.style.height = isTable ? '360px' : '340px';
    content.appendChild(host);

    try {
      var colors = self.getPaletteColors();
      
      if (isTable) {
        // HTML Table rendering (matching dashboard style)
        self.renderPreviewTable(host, result, colors);
      } else if (isGauge) {
        // ApexCharts radialBar for Gauge
        var val = result.kpiValue || 50;
        var max = val <= 0 ? 100 : (val <= 100 ? 100 : Math.ceil(val * 1.2));
        var pct = Math.min(Math.round((val / max) * 100), 100);
        
        self._previewChart = new ApexCharts(host, {
          chart: { type: 'radialBar', height: '100%', toolbar: { show: false }, rtl: true, fontFamily: 'Cairo, sans-serif', background: 'transparent' },
          series: [pct],
          labels: [result.title || 'Gauge'],
          plotOptions: {
            radialBar: {
              startAngle: -135, endAngle: 135,
              hollow: { size: '70%', background: 'transparent' },
              track: { background: '#D4E2F0', strokeWidth: '100%', margin: 0 },
              dataLabels: {
                name: { show: true, fontSize: '13px', fontWeight: 500, color: '#5B7A99', offsetY: -10 },
                value: { show: true, fontSize: '24px', fontWeight: 700, color: '#102A43', formatter: function() { return val; } }
              }
            }
          },
          colors: [(colors && colors[0]) || '#2E6DA4'],
          stroke: { lineCap: 'round' },
          fill: { type: 'gradient', gradient: { shade: 'dark', type: 'horizontal', shadeIntensity: 0.3, gradientToColors: [(colors && colors[0]) || '#1F4E79'], stops: [0, 100] } }
        });
        self._previewChart.render();
      } else {
        // ApexCharts for Bar/Line/Pie
        var typeMap = { Bar: 'bar', Line: 'line', Pie: 'pie' };
        var type = typeMap[result.chartType] || 'bar';
        var isPie = result.chartType === 'Pie';
        var isLine = result.chartType === 'Line';
        
        var seriesData = (result.sampleData || []).map(function(r) { return r[result.columns && result.columns[1]] || 0; });
        var labels = (result.sampleData || []).map(function(r) { return r[result.columns && result.columns[0]] || ''; });
        
        var series = isPie ? seriesData : [{ name: result.title || 'Chart', data: seriesData }];
        
        self._previewChart = new ApexCharts(host, {
          chart: { type: type, height: '100%', toolbar: { show: false }, rtl: true, fontFamily: 'Cairo, sans-serif', background: 'transparent', animations: { enabled: true, speed: 600 } },
          colors: isPie ? (colors || ['#1F4E79']) : [(colors && colors[0]) || '#1F4E79'],
          series: series,
          labels: isPie ? labels : undefined,
          plotOptions: isPie ? {} : { bar: { borderRadius: 4, columnWidth: '60%' } },
          xaxis: isPie ? undefined : { categories: labels, labels: { style: { colors: '#5B7A99', fontSize: '12px' } }, axisBorder: { show: false }, axisTicks: { show: false } },
          yaxis: isPie ? undefined : { labels: { style: { colors: '#5B7A99', fontSize: '12px' } } },
          grid: isPie ? {} : { borderColor: '#D4E2F0', strokeDashArray: 0, xaxis: { lines: { show: false } }, yaxis: { lines: { show: true } } },
          stroke: isPie ? {} : { show: true, width: isLine ? 2.5 : 0, curve: isLine ? 'smooth' : 'straight' },
          fill: isPie ? {} : { type: isLine ? 'gradient' : 'solid', gradient: isLine ? { shadeIntensity: 1, opacityFrom: 0.35, opacityTo: 0.05, stops: [50, 100] } : undefined },
          dataLabels: isPie ? { enabled: true, style: { fontSize: '12px', fontWeight: 600 } } : { enabled: false },
          legend: isPie ? { position: 'bottom', fontSize: '12px' } : { show: false },
          tooltip: { enabled: true, style: { fontSize: '13px' } }
        });
        self._previewChart.render();
      }
    } catch (e) {
      self.setConnection('offline');
      self.setPreviewState('error', 'تعذر عرض العنصر: ' + (e && e.message ? e.message : e));
    }
  };

  /* ----------------------- TABLE PREVIEW (HTML — matching dashboard) ----------------------- */
  CardBuilderWizard.prototype.renderPreviewTable = function (host, result, colors) {
    var cols = result.columns || [];
    var rows = result.sampleData || [];
    var accentColor = (colors && colors[0]) || '#1F4E79';
    
    var html = '<div style="width:100%;height:100%;display:flex;flex-direction:column;overflow:hidden;border-radius:8px;">';
    html += '<div style="flex:1;1;auto;overflow-y:auto;">';
    html += '<table style="width:100%;border-collapse:collapse;font-size:13px;font-family:Cairo,sans-serif;direction:rtl;">';
    
    // Header
    html += '<thead><tr>';
    cols.forEach(function(c) {
      html += '<th style="padding:10px 14px;font-weight:700;font-size:12px;text-transform:uppercase;letter-spacing:0.03em;color:#fff;text-align:right;white-space:nowrap;position:sticky;top:0;z-index:1;border-bottom:2px solid rgba(255,255,255,0.15);background:' + accentColor + ';">' + escapeHtml(c) + '</th>';
    });
    html += '</tr></thead>';
    
    // Body
    html += '<tbody>';
    rows.forEach(function(r, idx) {
      var bg = idx % 2 === 0 ? 'transparent' : 'rgba(212,226,240,0.3)';
      html += '<tr style="border-bottom:1px solid #e2e8f0;background:' + bg + ';">';
      cols.forEach(function(c) {
        html += '<td style="padding:10px 14px;text-align:right;color:#102A43;">' + escapeHtml(String(r[c] != null ? r[c] : '')) + '</td>';
      });
      html += '</tr>';
    });
    html += '</tbody></table>';
    html += '</div>';
    
    // Footer
    html += '<div style="display:flex;align-items:center;padding:8px 14px;background:#f0f4f8;border-top:1px solid #e2e8f0;font-size:12px;font-weight:600;color:#5B7A99;">';
    html += '<span>' + rows.length + ' صف</span>';
    html += '</div>';
    html += '</div>';
    
    host.innerHTML = html;
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
    var hasBasic = !!this.state.cardType && this.hasSource() && !!this.state.title.trim();
    if (!hasBasic) return false;

    if (this.isKpiStepVisible()) {
      var valCol = $('wb-kpi-value-column');
      if (!valCol || !valCol.value) return false;

      var kpiModeInput = $('wb-h-kpiMode');
      var kpiMode = kpiModeInput ? kpiModeInput.value : 'simple';
      // Date column is NOT required for simple KPI mode
      if (kpiMode !== 'simple') {
        var dateCol = $('wb-kpi-date-column');
        if (!dateCol || !dateCol.value) return false;
      }
    }
    return true;
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
    var maxStep = 5;
    if (prev) { prev.disabled = (step === 1); prev.setAttribute('aria-disabled', step === 1 ? 'true' : 'false'); }
    if (next) {
      if (step >= maxStep) {
        hide(next);
      } else {
        show(next);
        var canNext = this.validateStepSilent(step);
        next.disabled = !canNext;
        next.setAttribute('aria-disabled', canNext ? 'false' : 'true');
      }
    }
    var saveOk = this.canSave();
    if (save) { save.disabled = !saveOk; save.setAttribute('aria-disabled', saveOk ? 'false' : 'true'); }
    if (saveAdd) { saveAdd.disabled = !saveOk; saveAdd.setAttribute('aria-disabled', saveOk ? 'false' : 'true'); }
  };

  // non-UI validation used by footer (does not write error text)
  CardBuilderWizard.prototype.validateStepSilent = function (step) {
    if (step === 1) return true;
    if (step === 2) return this.hasSource();
    if (step === 3) return !!this.state.title.trim();
    if (step === 4 && this.isKpiStepVisible()) {
      var valCol = $('wb-kpi-value-column');
      var dateCol = $('wb-kpi-date-column');
      var kpiModeInput = $('wb-h-kpiMode');
      var kpiMode = kpiModeInput ? kpiModeInput.value : 'simple';
      if (!valCol || !valCol.value) return false;
      // Date column NOT required for simple mode
      if (kpiMode !== 'simple' && (!dateCol || !dateCol.value)) return false;
      return true;
    }
    return true;
  };

  /* ----------------------- KPI STEP VISIBILITY ----------------------- */
  CardBuilderWizard.prototype.isKpiStepVisible = function () {
    return this.state.cardType === 'KPI';
  };

  /* ----------------------- KPI MODE PICKER (Step 4) ----------------------- */
  CardBuilderWizard.prototype.initKpiModePicker = function () {
    var self = this;
    var modeCards = document.querySelectorAll('.wb-kpi-mode-card');
    var hiddenInput = $('wb-h-kpiMode');
    var changeSection = $('wb-kpi-change-section');
    var sparklineSection = $('wb-kpi-sparkline-section');
    var totalSection = $('wb-kpi-total-section');
    var dateSection = $('wb-kpi-date-section');
    var dateCol = $('wb-kpi-date-column');

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
        var dateStar = $('wb-date-required-star');
        if (mode === 'simple') {
          if (changeSection) changeSection.style.display = 'none';
          if (sparklineSection) sparklineSection.style.display = 'none';
          if (totalSection) totalSection.style.display = 'none';
          if (dateSection) dateSection.style.display = 'none';
          // Date column NOT required for simple mode
          if (dateStar) dateStar.style.display = 'none';
          if (dateCol) dateCol.removeAttribute('required');
        } else if (mode === 'withChange') {
          if (changeSection) changeSection.style.display = '';
          if (sparklineSection) sparklineSection.style.display = 'none';
          if (totalSection) totalSection.style.display = 'none';
          if (dateSection) dateSection.style.display = '';
          // Date column required for withChange and composite
          if (dateStar) dateStar.style.display = '';
          if (dateCol) dateCol.setAttribute('required', 'required');
        } else if (mode === 'composite') {
          if (changeSection) changeSection.style.display = '';
          if (sparklineSection) sparklineSection.style.display = '';
          if (totalSection) totalSection.style.display = '';
          if (dateSection) dateSection.style.display = '';
          if (dateStar) dateStar.style.display = '';
          if (dateCol) dateCol.setAttribute('required', 'required');
        }
        self.updateFooter();
      });
      card.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); card.click(); }
      });
    });
  };

  CardBuilderWizard.prototype.initKpiColumnMappings = function () {
    var self = this;
    ['wb-kpi-value-column', 'wb-kpi-date-column', 'wb-kpi-category-column'].forEach(function (id) {
      var el = $(id);
      if (el) {
        el.addEventListener('change', function () {
          self.syncKpiHiddenFields();
          self.updateFooter();
        });
      }
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
    ['wb-source-type', 'wb-saved-query', 'wb-sql-table', 'wb-grid-width', 'wb-grid-height', 'wb-grid-x', 'wb-grid-y', 'wb-refresh-interval', 'wb-custom-sql',
     'wb-kpi-value-column', 'wb-kpi-date-column', 'wb-kpi-category-column', 'wb-kpi-change-source', 'wb-kpi-sparkline-months',
     'wb-kpi-grand-total-source', 'wb-kpi-date-filter-mode', 'wb-kpi-fixed-start', 'wb-kpi-fixed-end', 'wb-kpi-relative-days'
    ].forEach(function (id) {
      var el = $(id); if (el) el.removeAttribute('name');
    });
  };

  CardBuilderWizard.prototype.syncHiddenInputs = function () {
    var s = this.state;
    if ($('wb-h-cardType')) $('wb-h-cardType').value = s.cardType;
    if ($('wb-h-sourceType')) $('wb-h-sourceType').value = s.sourceType;
    if ($('wb-h-sourceId')) $('wb-h-sourceId').value = s.sourceId;
    if ($('wb-h-customSql')) $('wb-h-customSql').value = s.customSql;
    if ($('wb-h-description')) $('wb-h-description').value = s.description || '';
    if ($('wb-h-gridWidth')) $('wb-h-gridWidth').value = s.gridWidth;
    if ($('wb-h-gridHeight')) $('wb-h-gridHeight').value = s.gridHeight;
    if ($('wb-h-gridX')) $('wb-h-gridX').value = s.gridX;
    if ($('wb-h-gridY')) $('wb-h-gridY').value = s.gridY;
    if ($('wb-h-colorPalette')) $('wb-h-colorPalette').value = s.palette;
    if ($('wb-h-refreshInterval')) $('wb-h-refreshInterval').value = s.refreshInterval;

    // Sync original source type (TASK-COD-028)
    if ($('wb-h-originalSourceType')) $('wb-h-originalSourceType').value = s.sourceType;
    if ($('wb-h-originalSourceId')) $('wb-h-originalSourceId').value = s.sourceId;

    // Sync SqlQuery — build proper SQL based on source type
    var sqlQuery = '';
    if (s.sourceType === 'Template' || s.sourceType === 'SavedQuery') {
      sqlQuery = s.previewSql || '';
    } else if (s.sourceType === 'SqlTable') {
      sqlQuery = this.buildSqlTableQueryForSave();
    } else if (s.sourceType === 'CustomSQL') {
      sqlQuery = s.customSql || '';
    }
    if ($('wb-h-sqlQuery')) $('wb-h-sqlQuery').value = sqlQuery;

    // Value format type (TASK-BUILDER-BEH-002)
    if ($('wb-h-valueFormatType')) $('wb-h-valueFormatType').value = s.valueFormatType;
    if ($('wb-h-valueUnit')) $('wb-h-valueUnit').value = s.valueUnit;

    // Sync KPI hidden fields (TASK-KPI-006)
    this.syncKpiHiddenFields();
  };

  CardBuilderWizard.prototype.buildSqlTableQueryForSave = function () {
    var table = this.state.selectedTable;
    if (!table || !table.sqlTargetTable) return '';
    // KPI cards: store table name only — BuildSql handles aggregation + date filter
    if (this.state.cardType === 'KPI') {
      return '[' + table.sqlTargetTable + ']';
    }
    // Other card types: SELECT * is fine
    return 'SELECT * FROM [' + table.sqlTargetTable + ']';
  };

  CardBuilderWizard.prototype.buildNumericExpression = function (table, columnName) {
    var safeColumn = '[' + String(columnName || '').replace(/[\[\];]/g, '').trim() + ']';
    var numericTextColumns = table && Array.isArray(table.numericTextColumns) ? table.numericTextColumns : [];
    var isNumericText = numericTextColumns.some(function (c) {
      return String(c || '').toLowerCase() === String(columnName || '').toLowerCase();
    });

    return isNumericText ? 'TRY_CAST(' + safeColumn + ' AS DECIMAL(28,6))' : safeColumn;
  };

  CardBuilderWizard.prototype.syncKpiHiddenFields = function () {
    var kpiMode = $('wb-h-kpiMode') ? $('wb-h-kpiMode').value : 'simple';
    var isChange = kpiMode === 'withChange' || kpiMode === 'composite';
    var isComposite = kpiMode === 'composite';

    // Use safe pattern (element && element.value) || fallback to avoid
    // empty string from hidden-but-present DOM elements overwriting defaults.
    if ($('wb-h-valueColumn')) {
      var vcEl = $('wb-kpi-value-column');
      $('wb-h-valueColumn').value = (vcEl && vcEl.value) || '';
    }
    if ($('wb-h-dateColumn')) {
      var dcEl = $('wb-kpi-date-column');
      $('wb-h-dateColumn').value = (dcEl && dcEl.value) || '';
    }
    if ($('wb-h-categoryColumn')) {
      var ccEl = $('wb-kpi-category-column');
      $('wb-h-categoryColumn').value = (ccEl && ccEl.value) || '';
    }
    if ($('wb-h-showChange')) $('wb-h-showChange').value = isChange ? 'true' : 'false';
    if ($('wb-h-changeSource')) {
      var csEl = $('wb-kpi-change-source');
      $('wb-h-changeSource').value = (csEl && csEl.value) || 'previousPeriod';
    }
    if ($('wb-h-showSparkline')) $('wb-h-showSparkline').value = isComposite ? 'true' : 'false';
    if ($('wb-h-sparklineMonths')) {
      var smEl = $('wb-kpi-sparkline-months');
      $('wb-h-sparklineMonths').value = (smEl && smEl.value) || '6';
    }
    if ($('wb-h-showGrandTotal')) $('wb-h-showGrandTotal').value = isComposite ? 'true' : 'false';
    if ($('wb-h-grandTotalSource')) {
      var gtsEl = $('wb-kpi-grand-total-source');
      $('wb-h-grandTotalSource').value = (gtsEl && gtsEl.value) || 'sameTable';
    }
    if ($('wb-h-dateFilterMode')) {
      var dfmEl = $('wb-kpi-date-filter-mode');
      $('wb-h-dateFilterMode').value = (dfmEl && dfmEl.value) || 'dashboard';
    }
    if ($('wb-h-fixedStartDate')) {
      var fsdEl = $('wb-kpi-fixed-start');
      $('wb-h-fixedStartDate').value = (fsdEl && fsdEl.value) || '';
    }
    if ($('wb-h-fixedEndDate')) {
      var fedEl = $('wb-kpi-fixed-end');
      $('wb-h-fixedEndDate').value = (fedEl && fedEl.value) || '';
    }
    if ($('wb-h-relativeDays')) {
      var rdEl = $('wb-kpi-relative-days');
      $('wb-h-relativeDays').value = (rdEl && rdEl.value) || '30';
    }

    // Value format type (TASK-BUILDER-BEH-002)
    if ($('wb-h-valueFormatType')) {
      var vftEl = $('wb-value-format-type');
      $('wb-h-valueFormatType').value = (vftEl && vftEl.value) || 'Currency';
    }
    if ($('wb-h-valueUnit')) {
      var vuEl = $('wb-value-unit');
      $('wb-h-valueUnit').value = (vuEl && vuEl.value) || '';
    }
  };

  CardBuilderWizard.prototype.submitForm = function (action) {
    if (!this.form) return;
    if (!this.canSave()) {
      // surface the offending step error and jump there
      var target = !this.hasSource() ? 2 : (!this.state.title.trim() ? 3 : (this.isKpiStepVisible() ? 4 : 3));
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
      act.name = 'saveAction';
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
