        (function () {
            var PALETTE = window.WD_PALETTE || ['#1F4E79', '#163A5A', '#2E6DA4', '#0A2540', '#8FBCDE', '#5B7A99'];

            // Per-card color palette definitions (TASK-CARD-UX-002)
            // These mirror the ColorPalettes dictionary in Builder.cshtml.cs.
            var COLOR_PALETTES = {
                primary:   ['#1F4E79', '#2E6DA4', '#8FBCDE'],
                secondary: ['#2E6DA4', '#1F4E79', '#8FBCDE'],
                accent:    ['#0A2540', '#1F4E79', '#2E6DA4'],
                success:   ['#1E9E6A', '#28A745', '#4CD97B'],
                warning:   ['#E0A106', '#FFC107', '#FFD54F'],
                info:      ['#2E6DA4', '#17A2B8', '#4FC3F7'],
                custom:    ['#1F4E79', '#E0A106', '#1E9E6A', '#D64545', '#2E6DA4', '#8FBCDE']
            };

            /** Return the colour array for a card's palette.
             *  Priority: 1) DOM data-color-palette attribute
             *            2) window.WD_CARDS[i].colorPalette
             *            3) 'primary' default
             *  Fallback: PALETTE if paletteId not found in COLOR_PALETTES. */
            function wdGetPalette(cardId) {
                if (cardId == null) return COLOR_PALETTES.primary;
                var paletteId = 'primary';
                var el = document.getElementById('card-' + cardId);
                if (el) {
                    paletteId = el.getAttribute('data-color-palette') || 'primary';
                } else {
                    var cards = window.WD_CARDS || [];
                    for (var i = 0; i < cards.length; i++) {
                        if (cards[i].id === cardId) {
                            paletteId = cards[i].colorPalette || 'primary';
                            break;
                        }
                    }
                }
                return COLOR_PALETTES[paletteId] || PALETTE;
            }
            var CHARTS = {}; // id -> control instance (for resize refresh)
            window.CHARTS_INSTANCES = CHARTS; // Expose for resize handler (TASK-DASH-005)

            // Shared utilities (toNum, isNum, formatNum, escapeHtml, showToast,
            // wdEmptyHtml, wdErrorHtml) are provided by ~/js/dashboard-utils.js.

            function animateCountUp(el, target, duration, formatType, unit) {
                if (!el) return;
                var start = 0;
                var startTime = null;
                var fmtType = formatType || 'Currency';
                var fmtUnit = unit || '';
                function step(timestamp) {
                    if (!startTime) startTime = timestamp;
                    var progress = Math.min((timestamp - startTime) / duration, 1);
                    // Ease out cubic
                    var eased = 1 - Math.pow(1 - progress, 3);
                    var current = Math.round(start + (target - start) * eased);
                    el.textContent = formatNum ? formatNum(current) : current.toLocaleString();
                    if (progress < 1) {
                        requestAnimationFrame(step);
                    } else {
                        // Final value: format depends on kpi size class (S vs M/L) and value format type
                        var kpiEl = el.closest('.wd-kpi');
                        var isSmall = kpiEl && kpiEl.classList.contains('wd-kpi--size-small');
                        var finalText = window.formatKpiValue
                            ? window.formatKpiValue(target, fmtType, fmtUnit, isSmall)
                            : (isSmall
                                ? (window.formatNum ? window.formatNum(target) : target)
                                : (window.formatMoney ? window.formatMoney(target) : target));
                        el.innerHTML = wdKpiMoneyHtml(finalText);
                    }
                }
                requestAnimationFrame(step);
            }

            function wdKpiMoneyHtml(value) {
                var text = value === null || value === undefined ? '' : String(value);
                var currencyMatch = text.match(/\s*ط¯\.ط£\s*$/);
                if (!currencyMatch) {
                    /* Non-currency: split number from trailing unit (e.g. "23 ط¨ط¬ظ‡ط©", "14.7%", "14,700") */
                    var parts = text.match(/^([\d,.\-+]+)\s*(.*)$/);
                    var numPart  = parts ? parts[1] : text;
                    var unitPart = parts ? parts[2] : '';
                    return '<span dir="rtl" style="display:inline-flex;flex-direction:row;align-items:baseline;gap:0.25em;unicode-bidi:isolate;white-space:nowrap;">'
                        + '<span dir="ltr" style="unicode-bidi:isolate;font-variant-numeric:tabular-nums;">' + escapeHtml(numPart) + '</span>'
                        + (unitPart ? '<span dir="rtl" style="unicode-bidi:isolate;">' + escapeHtml(unitPart) + '</span>' : '')
                        + '</span>';
                }
                var numberText = text.slice(0, text.length - currencyMatch[0].length);
                return '<span class="wd-kpi-money" dir="rtl">'
                    + '<span class="wd-kpi-money__number" dir="ltr">' + escapeHtml(numberText) + '</span>'
                    + '<span class="wd-kpi-money__currency" dir="rtl">ط¯.ط£</span>'
                    + '</span>';
            }

            function wdKpiComparisonText(changeSource) {
                switch (changeSource) {
                    case 'previousMonth': return 'ظ…ظ‚ط§ط±ظ†ط© ط¨ط§ظ„ط´ظ‡ط± ط§ظ„ط³ط§ط¨ظ‚';
                    case 'previousYear': return 'ظ…ظ‚ط§ط±ظ†ط© ط¨ط§ظ„ط³ظ†ط© ط§ظ„ط³ط§ط¨ظ‚ط©';
                    case 'lastMonth': return 'ظ…ظ‚ط§ط±ظ†ط© ط¨ط§ظ„ط´ظ‡ط± ط§ظ„ظ…ط§ط¶ظٹ';
                    case 'customQuery': return 'ظ…ظ‚ط§ط±ظ†ط© ظ…ط®طµطµط©';
                    case 'previousPeriod':
                    default:
                        return 'ظ…ظ‚ط§ط±ظ†ط© ط¨ط§ظ„ظپطھط±ط© ط§ظ„ط³ط§ط¨ظ‚ط©';
                }
            }

            function wdKpiChangeIconSvg(direction) {
                var path = direction === 'up'
                    ? '<path d="m5 12 7-7 7 7"></path><path d="M12 19V5"></path>'
                    : direction === 'down'
                        ? '<path d="M12 5v14"></path><path d="m19 12-7 7-7-7"></path>'
                        : '<path d="M5 12h14"></path>';
                return '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false">' + path + '</svg>';
            }

            function wdNormalizeKpiDirection(direction) {
                return direction === 'up' || direction === 'down' || direction === 'flat' ? direction : 'flat';
            }

            /** Map grid width â†’ KPI size class hint (container queries still drive density). */
            function wdKpiSizeClassFromWidth(w) {
                w = parseInt(w, 10) || 4;
                if (w <= 3) return 'wd-kpi--size-small';
                if (w <= 7) return 'wd-kpi--size-medium';
                return 'wd-kpi--size-large';
            }

            /**
             * Sync KPI density after S/M/L resize or re-render:
             * - update wd-kpi--size-* from card grid width
             * - resize sparkline ApexCharts instance if present
             * Exposed on window for layout-mode IIFE (resize buttons).
             */
            function wdSyncKpiDensity(cardEl) {
                if (!cardEl) return;
                var kpiRoot = cardEl.querySelector('.wd-kpi');
                if (!kpiRoot) return;

                var w = parseInt(cardEl.getAttribute('data-grid-w'), 10)
                    || parseInt(cardEl.dataset.gridW, 10)
                    || 4;
                var sizeClass = wdKpiSizeClassFromWidth(w);
                kpiRoot.classList.remove('wd-kpi--size-small', 'wd-kpi--size-medium', 'wd-kpi--size-large');
                kpiRoot.classList.add(sizeClass);

                var cardId = cardEl.getAttribute('data-card-id')
                    || (cardEl.id && String(cardEl.id).replace(/^card-/, ''));
                if (cardId != null && cardId !== '') {
                    var sparkKey = 'spark-' + cardId;
                    var doResize = function () {
                        var sparkEntry = CHARTS[sparkKey];
                        if (!sparkEntry || !sparkEntry.control) return;
                        try {
                            if (typeof sparkEntry.control.resize === 'function') {
                                sparkEntry.control.resize();
                            } else if (typeof sparkEntry.control.windowResizeSync === 'function') {
                                sparkEntry.control.windowResizeSync();
                            }
                        } catch (_) { /* non-blocking */ }
                    };
                    // Wait a frame so size class / layout settle before chart measures
                    if (typeof requestAnimationFrame === 'function') {
                        requestAnimationFrame(function () { requestAnimationFrame(doResize); });
                    } else {
                        setTimeout(doResize, 32);
                    }
                }
            }
            window.wdSyncKpiDensity = wdSyncKpiDensity;

            function wdRenderKpiCard(card) {
                var kpiMode = card.kpiMode || 'simple';
                var hasChangeContext = kpiMode === 'withChange' || kpiMode === 'composite';
                var modeClass = 'wd-kpi--' + kpiMode;
                var sizeClass = 'wd-kpi--size-medium';
                var cardEl = document.getElementById('card-' + card.cardId);
                if (cardEl) {
                    var w = parseInt(cardEl.getAttribute('data-grid-w'), 10)
                        || parseInt(cardEl.dataset.gridW, 10)
                        || 4;
                    sizeClass = wdKpiSizeClassFromWidth(w);
                }

                var html = '<div class="wd-kpi ' + modeClass + ' ' + sizeClass + '">';
                html += '<div class="wd-kpi__cluster">';

                // Row: main + categories
                html += '<div class="wd-kpi__row">';

                html += '<div class="wd-kpi__main">';
                // Prefer kpiMainValue for count-up consistency with wdRenderCard
                var rawValue = (card.kpiMainValue !== null && card.kpiMainValue !== undefined)
                    ? card.kpiMainValue
                    : card.kpiValue;
                var target = card.kpiTarget || 0;
                var display = card.kpiFormatted !== null && card.kpiFormatted !== undefined
                    ? card.kpiFormatted
                    : (rawValue !== null && rawValue !== undefined
                        ? (sizeClass === 'wd-kpi--size-small'
                            ? (window.formatKpiValue ? window.formatKpiValue(rawValue, card.valueFormatType || 'Currency', card.valueUnit || '', true) : rawValue)
                            : (window.formatKpiValue ? window.formatKpiValue(rawValue, card.valueFormatType || 'Currency', card.valueUnit || '', false) : rawValue))
                        : 'â€”');
                html += '<div class="wd-kpi__hero">';
                html += '<div class="wd-kpi__value" data-kpi-target="' + target + '">' + wdKpiMoneyHtml(display) + '</div>';
                if (hasChangeContext && card.kpiChangePercent !== null && card.kpiChangePercent !== undefined) {
                    var changePercent = card.kpiChangePercent;
                    var direction = window.wdNormalizeKpiDirection ? window.wdNormalizeKpiDirection(card.kpiChangeDirection || 'flat') : 'flat';
                    var sign = direction === 'up' ? '+' : direction === 'down' ? '-' : '';
                    var changeClass = 'wd-kpi__change--' + direction;
                    html += '<div class="wd-kpi__change ' + changeClass + '">';
                    if (window.wdKpiChangeIconSvg) html += window.wdKpiChangeIconSvg(direction);
                    html += '<span>' + sign + Math.abs(changePercent) + '%</span></div>';
                }
                html += '</div>'; // hero

                if (kpiMode === 'composite') {
                    html += '<div class="wd-kpi__grandtotal wd-kpi__details" data-card-id="' + card.cardId + '"></div>';
                }
                html += '</div>'; // main

                html += '<div class="wd-kpi__categories is-empty">';
                html += '<div class="wd-kpi-breakdown" data-card-id="' + card.cardId + '"></div>';
                html += '</div>';

                html += '</div>'; // row

                // Sparkline attached directly under content (inside cluster)
                html += '<div class="wd-kpi__sparkline-section">';
                html += '<div class="wd-kpi__sparkline" data-card-id="' + card.cardId + '"></div>';
                html += '</div>';

                if (!hasChangeContext) {
                    html += '<div class="wd-kpi__label">ظ‚ظٹظ…ط© ط§ظ„ظ…ط¤ط´ط±</div>';
                }

                html += '</div>'; // cluster
                html += '</div>'; // kpi
                return html;
            }

            function wdDestroySparkline(cardId) {
                var key = 'spark-' + cardId;
                var entry = CHARTS[key];
                if (entry && entry.control && typeof entry.control.destroy === 'function') {
                    try { entry.control.destroy(); } catch (_) { /* Safe cleanup only; never block card refresh. */ }
                }
                delete CHARTS[key];
            }

            function wdRenderSparkline(container, data, cardId, formatType, unit) {
                wdDestroySparkline(cardId);

                var fmtType = formatType || 'Currency';
                var fmtUnit = unit || '';

                var rows = Array.isArray(data) ? data : [];
                var points = rows.map(function(r, index) {
                    r = r || {};
                    var rawValue = r.MonthlyValue;
                    if (rawValue === null || rawValue === undefined) rawValue = r.monthlyValue;
                    if (rawValue === null || rawValue === undefined) rawValue = r.Value;
                    if (rawValue === null || rawValue === undefined) rawValue = r.value;
                    if (rawValue === null || rawValue === undefined) rawValue = 0;
                    var value = toNum(rawValue);
                    var month = r.Month || r.month || r.MonthLabel || r.monthLabel || r.Label || r.label || ('ط§ظ„ط´ظ‡ط± ' + (index + 1));
                    return { month: String(month), value: value };
                }).filter(function(point) {
                    return isFinite(point.value);
                });

                if (points.length < 2) {
                    container.classList.add('wd-kpi__sparkline-empty');
                    container.setAttribute('role', 'status');
                    container.textContent = 'ظ„ط§ طھظˆط¬ط¯ ط¨ظٹط§ظ†ط§طھ ط§طھط¬ط§ظ‡ ظƒط§ظپظٹط©';
                    return;
                }

                container.classList.remove('wd-kpi__sparkline-empty');
                container.innerHTML = '';
                container.setAttribute('aria-label', 'ط§طھط¬ط§ظ‡ ط§ظ„ظ…ط¤ط´ط± ط­ط³ط¨ ط§ظ„ط´ظ‡ط±');

                var values = points.map(function(point) { return point.value; });
                var months = points.map(function(point) { return point.month; });
                var pal = wdGetPalette(cardId);
                /* KPI mockup gold; allow palette override only when clearly non-default */
                var KPI_SPARK_GOLD = '#E8A317';
                var sparkColor = KPI_SPARK_GOLD;
                if (pal && pal[0] && typeof pal[0] === 'string') {
                    var p0 = pal[0].trim().toLowerCase();
                    /* Keep gold unless card has an explicit custom accent distinct from primary blues */
                    if (p0 && p0 !== 'var(--c-primary)' && p0.indexOf('1f4e79') === -1 && p0.indexOf('#1f4e79') === -1) {
                        /* still prefer gold for KPI mockup fidelity â€” palette only if it's already gold/orange family */
                        if (/#e[0-9a-f]{2}a1|#e0a106|#f5a|#e8a|#ffa|#ff8|#f59e0b|#d97706|orange|gold|warning/i.test(p0)) {
                            sparkColor = pal[0];
                        }
                    }
                }
                var isDark = document.documentElement.getAttribute('data-theme') === 'midnight';
                var lastIndex = values.length - 1;
                var markerRing = isDark ? 'rgba(255,255,255,0.92)' : '#ffffff';

                function formatSparkValue(val) {
                    return window.formatKpiValue
                        ? window.formatKpiValue(toNum(val), fmtType, fmtUnit, false)
                        : (formatMoney ? formatMoney(toNum(val)) : toNum(val).toLocaleString('ar-SA'));
                }

                function deltaHtml(index) {
                    if (index < 1) {
                        return '<div class="wd-spark-tooltip__delta wd-spark-tooltip__delta--flat">ط£ظˆظ„ ط´ظ‡ط± ظپظٹ ط§ظ„ط§طھط¬ط§ظ‡</div>';
                    }
                    var delta = values[index] - values[index - 1];
                    var direction = delta > 0 ? 'up' : delta < 0 ? 'down' : 'flat';
                    if (direction === 'flat') {
                        return '<div class="wd-spark-tooltip__delta wd-spark-tooltip__delta--flat">ط¨ط¯ظˆظ† طھط؛ظٹط± ط¹ظ† ط§ظ„ط´ظ‡ط± ط§ظ„ط³ط§ط¨ظ‚</div>';
                    }
                    /* Keep number LTR-isolated from Arabic label to avoid bi-di garble (e.g. 6.8+â€¦K) */
                    var sign = delta > 0 ? '+' : 'âˆ’';
                    var num = sign + formatSparkValue(Math.abs(delta));
                    return '<div class="wd-spark-tooltip__delta wd-spark-tooltip__delta--' + direction + '">'
                        + '<span class="wd-spark-tooltip__delta-num">' + escapeHtml(num) + '</span>'
                        + '<span class="wd-spark-tooltip__delta-label">ط¹ظ† ط§ظ„ط´ظ‡ط± ط§ظ„ط³ط§ط¨ظ‚</span>'
                        + '</div>';
                }

                /* Small markers on all points; last point larger with ring via discrete */
                var discreteMarkers = values.map(function(_, idx) {
                    var isLast = idx === lastIndex;
                    return {
                        seriesIndex: 0,
                        dataPointIndex: idx,
                        fillColor: sparkColor,
                        strokeColor: isLast ? markerRing : sparkColor,
                        size: isLast ? 7 : 3.5,
                        strokeWidth: isLast ? 2.5 : 0
                    };
                });

                var chartConfig = {
                    chart: {
                        type: 'area',
                        height: '100%',
                        sparkline: { enabled: true },
                        background: 'transparent',
                        /* LTR time series â€” rtl:true garbles mixed Arabic/number tooltips */
                        rtl: false,
                        toolbar: { show: false },
                        zoom: { enabled: false },
                        parentHeightOffset: 0,
                        fontFamily: 'Cairo, Tajawal, Tahoma, sans-serif',
                        animations: { enabled: true, easing: 'easeinout', speed: 550 }
                    },
                    series: [{ data: values }],
                    stroke: { curve: 'smooth', width: 2.75, lineCap: 'round' },
                    colors: [sparkColor],
                    fill: {
                        type: 'gradient',
                        gradient: {
                            shade: isDark ? 'dark' : 'light',
                            type: 'vertical',
                            shadeIntensity: 0.35,
                            gradientToColors: ['#F5C56B'],
                            opacityFrom: isDark ? 0.42 : 0.36,
                            opacityTo: 0.03,
                            stops: [0, 80, 100]
                        }
                    },
                    dataLabels: { enabled: false },
                    markers: {
                        size: 3.5,
                        colors: [sparkColor],
                        strokeColors: sparkColor,
                        strokeWidth: 0,
                        hover: { size: 6, sizeOffset: 1.5 },
                        discrete: discreteMarkers
                    },
                    tooltip: {
                        enabled: true,
                        shared: false,
                        intersect: true,
                        followCursor: true,
                        theme: isDark ? 'dark' : 'light',
                        marker: { show: false },
                        custom: function(opts) {
                            var i = opts && opts.dataPointIndex != null ? opts.dataPointIndex : 0;
                            var value = opts.series[opts.seriesIndex][i];
                            return '<div class="wd-spark-tooltip">'
                                + '<div class="wd-spark-tooltip__month">' + escapeHtml(months[i] || ('ط§ظ„ط´ظ‡ط± ' + (i + 1))) + '</div>'
                                + '<div class="wd-spark-tooltip__value" dir="ltr">' + escapeHtml(formatSparkValue(value)) + '</div>'
                                + deltaHtml(i)
                                + '</div>';
                        }
                    },
                    xaxis: {
                        categories: months,
                        tooltip: { enabled: false },
                        axisBorder: { show: false },
                        axisTicks: { show: false },
                        labels: { show: false }
                    },
                    yaxis: { show: false, labels: { show: false } },
                    grid: { show: false, padding: { top: 8, right: 8, bottom: 6, left: 8 } }
                };

                var chart = new ApexCharts(container, chartConfig);
                chart.render();
                CHARTS['spark-' + cardId] = { control: chart, kind: 'sparkline' };
            }

            function wdRenderCategoryBreakdown(container, data, cardId, formatType, unit) {
                var fmtType = formatType || 'Currency';
                var fmtUnit = unit || '';
                var rows = Array.isArray(data) ? data : [];
                var categoriesWrap = container && container.closest
                    ? container.closest('.wd-kpi__categories')
                    : null;
                if (rows.length === 0) {
                    container.style.display = 'none';
                    if (categoriesWrap) {
                        categoriesWrap.classList.add('is-empty');
                        categoriesWrap.setAttribute('hidden', '');
                    }
                    return;
                }
                /* Max 5 rows; CSS density hides extras for M/S */
                var visibleRows = rows.slice(0, 5);
                var html = '<div class="wd-kpi-breakdown">';
                html += '<div class="wd-kpi-breakdown__title">ط£ط¹ظ„ظ‰ ط§ظ„طھطµظ†ظٹظپط§طھ</div>';
                html += '<table class="wd-kpi-breakdown__table">';
                visibleRows.forEach(function(row) {
                    row = row || {};
                    var value = row.CategoryValue || row.categoryValue || row.Value || row.value || 0;
                    var pctRaw = row.Percentage != null ? row.Percentage
                        : (row.percentage != null ? row.percentage
                            : (row.Pct != null ? row.Pct : 0));
                    var pctNum = toNum(pctRaw);
                    var pctLabel = (Math.round(pctNum * 10) / 10) + '%';
                    var code = row.Code || row.CategoryCode || row.ItemCode || row.Id
                        || row.code || row.categoryCode || row.itemCode || row.id
                        || row.Category || row.category || row.CategoryName || 'â€”';
                    html += '<tr>';
                    /* Visual LTR (table direction:ltr): % | value | code(gold) */
                    html += '<td class="wd-kpi-breakdown__pct">' + escapeHtml(String(pctLabel)) + '</td>';
                    html += '<td class="wd-kpi-breakdown__val">' + wdKpiMoneyHtml(window.formatKpiValue ? window.formatKpiValue(toNum(value), fmtType, fmtUnit, false) : formatMoney(toNum(value))) + '</td>';
                    html += '<td class="wd-kpi-breakdown__code">' + escapeHtml(String(code)) + '</td>';
                    html += '</tr>';
                });
                html += '</table></div>';
                container.innerHTML = html;
                container.style.display = '';
                if (categoriesWrap) {
                    categoriesWrap.classList.remove('is-empty');
                    categoriesWrap.removeAttribute('hidden');
                }
            }

            function wdRenderGrandTotal(container, card) {
                var source = card.grandTotalSource || 'both';
                var fmtType = card.valueFormatType || 'Currency';
                var fmtUnit = card.valueUnit || '';
                var html = '';

                if (source === 'allTime' || source === 'both') {
                    if (card.kpiGrandTotal !== null && card.kpiGrandTotal !== undefined) {
                        var val = window.formatKpiValue ? window.formatKpiValue(toNum(card.kpiGrandTotal), fmtType, fmtUnit, false) : formatMoney(toNum(card.kpiGrandTotal));
                        html += '<div class="wd-kpi-grandtotal__row">';
                        html += '<span class="wd-kpi-grandtotal__label">ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ ط§ظ„ظƒظ„ظٹ:</span>';
                        html += '<span class="wd-kpi-grandtotal__value">' + wdKpiMoneyHtml(val) + '</span>';
                        html += '</div>';
                    }
                }

                if (source === 'yearToDate' || source === 'both') {
                    if (card.kpiYearToDateTotal !== null && card.kpiYearToDateTotal !== undefined) {
                        var valYtd = window.formatKpiValue ? window.formatKpiValue(toNum(card.kpiYearToDateTotal), fmtType, fmtUnit, false) : formatMoney(toNum(card.kpiYearToDateTotal));
                        var year = new Date().getFullYear();
                        html += '<div class="wd-kpi-grandtotal__row">';
                        html += '<span class="wd-kpi-grandtotal__label">ط¥ط¬ظ…ط§ظ„ظٹ ' + year + ':</span>';
                        html += '<span class="wd-kpi-grandtotal__value">' + wdKpiMoneyHtml(valYtd) + '</span>';
                        html += '</div>';
                    }
                }

                if (!html) {
                    container.innerHTML = '<div class="wd-kpi-grandtotal"><div class="wd-kpi-grandtotal__row"><span class="wd-kpi-grandtotal__label">ط§ظ„ظ…ط¬ط§ظ…ظٹط¹:</span><span class="wd-kpi-grandtotal__value">' + wdKpiMoneyHtml('â€”') + '</span></div></div>';
                    container.style.display = '';
                    return;
                }

                container.innerHTML = '<div class="wd-kpi-grandtotal">' + html + '</div>';
                container.style.display = '';
            }

            function clearBody(el) {
                var body = el.querySelector('.wd-card__body');
                body.innerHTML = '';
                return body;
            }

            function wdRenderCard(card) {
                var el = document.getElementById('card-' + card.cardId);
                if (!el) return;
                wdDestroySparkline(card.cardId);
                var body = clearBody(el);

                // Apply palette accent top border for Chart/Gauge/Table cards
                el.classList.remove('wd-card--accent-top');
                if (card.chartType === 'Chart' || card.chartType === 'Bar' || card.chartType === 'Line' || card.chartType === 'Pie' || card.chartType === 'Gauge' || card.chartType === 'Table') {
                    el.classList.add('wd-card--accent-top');
                    var pal = typeof wdGetPalette === 'function' ? wdGetPalette(card.cardId) : null;
                    if (pal && pal[0]) {
                        el.style.borderTopColor = pal[0];
                    }
                }

                if (card.status === 'empty') { body.innerHTML = wdEmptyHtml(); return; }
                if (card.status === 'error') { body.innerHTML = wdErrorHtml(card.errorMessage, card.cardId); return; }

                try {
                    if (card.chartType === 'KPI') {
                        body.innerHTML = wdRenderKpiCard(card);
                        var kpiVal = body.querySelector('.wd-kpi__value');
                        if (kpiVal) animateCountUp(kpiVal, toNum(card.kpiMainValue || card.kpiValue), 1000, card.valueFormatType || 'Currency', card.valueUnit || '');

                        // Render sparkline (always present in adaptive shell)
                        var sparkContainer = body.querySelector('.wd-kpi__sparkline');
                        if (sparkContainer && card.kpiSparklineData) {
                            wdRenderSparkline(sparkContainer, card.kpiSparklineData, card.cardId, card.valueFormatType || 'Currency', card.valueUnit || '');
                        }

                        // Category breakdown â€” hide parent column when empty
                        var breakdownContainer = body.querySelector('.wd-kpi-breakdown');
                        if (breakdownContainer) {
                            if (card.kpiCategoryBreakdown && card.kpiCategoryBreakdown.length > 0) {
                                wdRenderCategoryBreakdown(breakdownContainer, card.kpiCategoryBreakdown, card.cardId, card.valueFormatType || 'Currency', card.valueUnit || '');
                            } else {
                                wdRenderCategoryBreakdown(breakdownContainer, [], card.cardId, card.valueFormatType || 'Currency', card.valueUnit || '');
                            }
                        }

                        // Grand total (all-time / year-to-date / both)
                        if (card.kpiMode === 'composite') {
                            var grandTotalContainer = body.querySelector('.wd-kpi__grandtotal');
                            if (grandTotalContainer) {
                                wdRenderGrandTotal(grandTotalContainer, card);
                            }
                        }

                        wdSyncKpiDensity(el);
                        return;
                    }
                    if (!card.columns || card.columns.length < 1) { body.innerHTML = wdEmptyHtml(); return; }

                    var viz = document.createElement('div');
                    viz.style.width = '100%';
                    viz.style.height = '100%';
                    viz.style.minHeight = '180px';
                    body.appendChild(viz);

                    if (card.chartType === 'Table') { wdRenderGrid(viz, card); return; }
                    if (card.chartType === 'Gauge') { wdRenderGauge(viz, card); return; }
                    wdRenderChart(viz, card);
                } catch (e) {
                    body.innerHTML = wdErrorHtml('طھط¹ط°ط± ط¹ط±ط¶ ط§ظ„ط¹ظ†طµط±: ' + (e && e.message ? e.message : e), card.cardId);
                }
            }

            function wdRenderChart(viz, card) {
                var cols = card.columns;
                var data = (card.rows || []).map(function (r) {
                    return { label: r[cols[0]], value: toNum(r[cols[1]]) };
                });
                var typeMap = { Bar: 'bar', Line: 'line', Pie: 'pie' };
                var type = typeMap[card.chartType] || 'bar';
                var isPie = card.chartType === 'Pie';
                var isLine = card.chartType === 'Line';
                var isBar = card.chartType === 'Bar';

                var seriesData = data.map(function(r) { return r.value; });
                var labels = data.map(function(r) { return r.label; });

                var series;
                if (isPie) {
                    series = seriesData;
                } else {
                    series = [{ name: card.title, data: seriesData }];
                }

                var cardPalette = wdGetPalette(card.cardId);
                var isDark = document.documentElement.getAttribute('data-theme') === 'midnight';
                var gridColor = isDark ? '#3a4555' : '#D4E2F0';
                var textColor = isDark ? '#8a95a3' : '#5B7A99';
                var tooltipBg = isDark ? '#232a34' : '#0A2540';

                var chartConfig = {
                    chart: {
                        type: type,
                        height: '100%',
                        toolbar: { show: false },
                        zoom: { enabled: isBar || isLine },
                        rtl: true,
                        fontFamily: 'Cairo, Tajawal, Tahoma, sans-serif',
                        background: 'transparent',
                        animations: { enabled: true, easing: 'easeinout', speed: 600 }
                    },
                    colors: isPie ? cardPalette : [cardPalette[0] || '#1F4E79'],
                    series: series,
                    labels: isPie ? labels : undefined,
                    plotOptions: isPie ? {} : {
                        bar: {
                            borderRadius: 4,
                            columnWidth: '60%',
                            distributed: false
                        }
                    },
                    xaxis: isPie ? undefined : {
                        categories: labels,
                        labels: { style: { colors: textColor, fontSize: '12px' } },
                        axisBorder: { show: false },
                        axisTicks: { show: false }
                    },
                    yaxis: isPie ? undefined : {
                        labels: { style: { colors: textColor, fontSize: '12px' } }
                    },
                    grid: isPie ? {} : {
                        borderColor: gridColor,
                        strokeDashArray: 0,
                        xaxis: { lines: { show: false } },
                        yaxis: { lines: { show: true } }
                    },
                    stroke: isPie ? {} : {
                        show: true,
                        width: isLine ? 2.5 : 0,
                        curve: isLine ? 'smooth' : 'straight'
                    },
                    fill: isPie ? {} : {
                        type: isLine ? 'gradient' : 'solid',
                        gradient: isLine ? {
                            shadeIntensity: 1,
                            opacityFrom: 0.35,
                            opacityTo: 0.05,
                            stops: [50, 100]
                        } : undefined
                    },
                    dataLabels: isPie ? {
                        enabled: true,
                        style: { fontSize: '12px', fontWeight: 600 }
                    } : { enabled: false },
                    legend: isPie ? {
                        position: 'bottom',
                        fontSize: '12px',
                        labels: { colors: textColor }
                    } : { show: false },
                    tooltip: {
                        enabled: true,
                        theme: isDark ? 'dark' : 'light',
                        style: { fontSize: '13px' },
                        y: { formatter: function(val) { return formatNum ? formatNum(val) : val; } }
                    }
                };

                var chart = new ApexCharts(viz, chartConfig);
                chart.render();
                CHARTS[card.cardId] = { control: chart, kind: 'chart' };
            }

            function wdRenderGauge(viz, card) {
                var val = toNum(card.kpiValue);
                var max = val <= 0 ? 100 : (val <= 100 ? 100 : Math.ceil(val * 1.2));
                var pct = Math.min(Math.round((val / max) * 100), 100);
                var isDark = document.documentElement.getAttribute('data-theme') === 'midnight';

                var pal = wdGetPalette(card.cardId);
                var gaugeColor = pal[0] || '#2E6DA4';
                var trackColor = isDark ? '#2c3541' : '#D4E2F0';
                var textColor = isDark ? '#e4e8ed' : '#102A43';
                var subColor = isDark ? '#8a95a3' : '#5B7A99';

                var chartConfig = {
                    chart: {
                        type: 'radialBar',
                        height: '100%',
                        toolbar: { show: false },
                        rtl: true,
                        fontFamily: 'Cairo, Tajawal, Tahoma, sans-serif',
                        background: 'transparent'
                    },
                    series: [pct],
                    labels: [card.title],
                    plotOptions: {
                        radialBar: {
                            startAngle: -135,
                            endAngle: 135,
                            hollow: {
                                size: '70%',
                                background: 'transparent'
                            },
                            track: {
                                background: trackColor,
                                strokeWidth: '100%',
                                margin: 0
                            },
                            dataLabels: {
                                name: {
                                    show: true,
                                    fontSize: '13px',
                                    fontWeight: 500,
                                    color: subColor,
                                    offsetY: -10
                                },
                                value: {
                                    show: true,
                                    fontSize: '24px',
                                    fontWeight: 700,
                                    color: textColor,
                                    formatter: function() { return formatNum ? formatNum(val) : val; }
                                }
                            }
                        }
                    },
                    colors: [gaugeColor],
                    stroke: { lineCap: 'round' },
                    fill: {
                        type: 'gradient',
                        gradient: {
                            shade: 'dark',
                            type: 'horizontal',
                            shadeIntensity: 0.3,
                            gradientToColors: [pal[0] || '#1F4E79'],
                            stops: [0, 100]
                        }
                    }
                };

                var chart = new ApexCharts(viz, chartConfig);
                chart.render();
                CHARTS[card.cardId] = { control: chart, kind: 'gauge' };
            }

            // â”€â”€ Smart Table (TASK-DRILL-SMARTTABLE-001) â”€â”€

            function wdRenderGrid(viz, card) {
                renderSmartGrid(viz, card);
            }

            /**
             * renderSmartGrid â€” Renders a full smart table with sort, search, pagination,
             * column summaries, and type-aware cell formatting.
             *
             * viz  â€” The container element (HTMLElement)
             * data â€” API response with .columns[], .rows[], .cardId
             */
            function renderSmartGrid(viz, data) {
                var cols = data.columns || [];
                var allRows = data.rows || [];
                var st = window.__drillState;

                // Empty state â€” no columns or no rows
                if (cols.length === 0 || allRows.length === 0) {
                    viz.innerHTML = '<div class="wd-table__empty" style="padding:60px 20px;text-align:center;">'
                        + '<div class="wd-table__empty-icon" aria-hidden="true" style="margin-bottom:12px;">'
                        + '<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">'
                        + '<rect x="3" y="3" width="18" height="18" rx="2"></rect>'
                        + '<path d="M3 9h18"></path><path d="M9 21V9"></path>'
                        + '</svg></div>'
                        + '<h4 style="margin:0 0 6px;font-size:16px;">ظ„ط§ طھظˆط¬ط¯ ط¨ظٹط§ظ†ط§طھ</h4>'
                        + '<p style="margin:0;color:var(--c-text-muted);font-size:13px;">ظ„ظ… ظٹطھظ… ط§ظ„ط¹ط«ظˆط± ط¹ظ„ظ‰ ط³ط¬ظ„ط§طھ ظ„ط¹ط±ط¶ظ‡ط§ ظپظٹ ظ‡ط°ط§ ط§ظ„ط¬ط¯ظˆظ„.</p>'
                        + '</div>';
                    return;
                }

                // Use drill state if available; otherwise standalone defaults
                var sortCol     = (st && st.sortColumn)     || null;
                var sortAsc     = (st && st.sortAsc !== false);
                var searchQuery = (st && st.searchQuery)    || '';
                var pageSize    = (st && st.pageSize)       || 50;
                var currentPage = (st && st.currentPage)    || 1;

                // Step 1: Filter
                var filteredRows = searchQuery
                    ? applySearch(allRows, cols, searchQuery)
                    : allRows.slice();

                // Store filtered rows on state
                if (st) st.filteredRows = filteredRows;

                // Step 2: Sort
                if (sortCol) {
                    filteredRows = sortRows(filteredRows, sortCol, sortAsc, cols);
                }

                // Step 3: Paginate
                var totalRows = filteredRows.length;
                var totalPages = Math.max(1, Math.ceil(totalRows / pageSize));
                if (currentPage > totalPages) currentPage = totalPages;
                if (currentPage < 1) currentPage = 1;
                if (st) st.currentPage = currentPage;

                var startIdx = (currentPage - 1) * pageSize;
                var pageRows = filteredRows.slice(startIdx, startIdx + pageSize);

                // Determine numeric columns for summaries
                var numericCols = [];
                cols.forEach(function(c) { if (isNumericColumn(allRows, c)) numericCols.push(c); });

                // Compute summaries from ALL filtered rows (not just page)
                var summaries = calcSummaries(cols, filteredRows, numericCols);

                // Update toolbar info
                var infoEl = document.getElementById('wd-drill-info');
                if (infoEl) {
                    infoEl.textContent = totalRows + ' طµظپ'
                        + (searchQuery ? ' (ظ…ظ† ط£طµظ„ ' + allRows.length + ')' : '');
                }

                // â”€â”€ Build HTML â”€â”€
                var html = '<div class="wd-table-wrap">';
                html += '<div class="wd-table-scroll" style="max-height:55vh;overflow-y:auto;">';
                html += '<table class="wd-table wd-table--smart">';

                // â”€â”€ Header with sort â”€â”€
                html += '<thead><tr>';
                // Row number column
                html += '<th class="wd-col--rownum" scope="col">#</th>';
                cols.forEach(function(c) {
                    var isSorted = (sortCol === c);
                    var sortIcon = '';
                    if (isSorted) {
                        sortIcon = sortAsc ? 'â–²' : 'â–¼';
                    } else {
                        sortIcon = 'â‡…';
                    }
                    html += '<th scope="col" onclick="wdDrillSort(\'' + escapeHtml(c).replace(/'/g, "\\'") + '\')">'
                        + escapeHtml(c)
                        + ' <span class="wd-sort-icon' + (isSorted ? ' wd-sort-icon--active' : '') + '">' + sortIcon + '</span>'
                        + '</th>';
                });
                html += '</tr></thead>';

                // â”€â”€ Body â”€â”€
                html += '<tbody>';
                pageRows.forEach(function(r, idx) {
                    var globalIdx = startIdx + idx;
                    html += '<tr style="animation-delay:' + (idx * 20) + 'ms;" data-row-index="' + globalIdx + '">';
                    // Row number
                    html += '<td class="wd-col--rownum">' + (globalIdx + 1) + '</td>';
                    cols.forEach(function(c) {
                        var val = r[c];
                        var formatted = formatDrillCell(val);
                        html += '<td>' + formatted + '</td>';
                    });
                    html += '</tr>';
                });
                html += '</tbody></table>';
                html += '</div>'; // .wd-table-scroll

                // â”€â”€ Summaries row â”€â”€
                if (summaries.length > 0) {
                    html += '<div class="wd-table__summaries">';
                    summaries.forEach(function(s) {
                        html += '<span class="wd-table__summaries-item">'
                            + '<span class="wd-table__summaries-label">' + escapeHtml(s.label) + ':</span>'
                            + ' <span class="wd-table__summaries-value">' + s.value + '</span>'
                            + '</span>';
                    });
                    html += '</div>';
                }

                html += '</div>'; // .wd-table-wrap

                viz.innerHTML = html;

                // â”€â”€ Update pagination â”€â”€
                renderPagination(currentPage, totalPages);
            }

            /**
             * renderPagination â€” Updates the pagination controls at the bottom of the modal.
             */
            function renderPagination(currentPage, totalPages) {
                var infoEl = document.getElementById('wd-page-info');
                var prevBtn = document.getElementById('wd-page-prev');
                var nextBtn = document.getElementById('wd-page-next');
                var btnsContainer = document.getElementById('wd-page-btns');
                if (!infoEl || !prevBtn || !nextBtn || !btnsContainer) return;

                infoEl.textContent = 'طµظپط­ط© ' + currentPage + ' ظ…ظ† ' + totalPages;

                prevBtn.disabled = (currentPage <= 1);
                nextBtn.disabled = (currentPage >= totalPages);

                // Build page number buttons â€” show max 5 pages around current
                btnsContainer.innerHTML = '';
                var startPage = Math.max(1, currentPage - 2);
                var endPage = Math.min(totalPages, startPage + 4);
                if (endPage - startPage < 4) {
                    startPage = Math.max(1, endPage - 4);
                }

                for (var p = startPage; p <= endPage; p++) {
                    var btn = document.createElement('button');
                    btn.type = 'button';
                    btn.className = 'wd-drill-pagination__btn wd-drill-pagination__btn--page'
                        + (p === currentPage ? ' wd-drill-pagination__btn--active' : '');
                    btn.textContent = p;
                    btn.setAttribute('aria-label', 'ط§ظ„طµظپط­ط© ' + p);
                    (function(page) {
                        btn.addEventListener('click', function() {
                            goToPage(page);
                        });
                    })(p);
                    btnsContainer.appendChild(btn);
                }
            }

            /**
             * goToPage â€” Navigate to a specific page and re-render.
             */
            function goToPage(page) {
                var st = window.__drillState;
                if (!st) return;
                if (page < 1) page = 1;
                st.currentPage = page;
                // Re-run renderSmartGrid on the current data
                var bodyEl = document.getElementById('wd-drill-modal-body');
                if (!bodyEl) return;
                var tableWrap = bodyEl.querySelector('.wd-table-wrap');
                if (!tableWrap) return;
                var data = st.currentData;
                if (!data) return;
                // Re-render using existing columns/rows
                renderSmartGrid(bodyEl, data);
            }

            /**
             * applySearch â€” Filters rows where any column contains the query (case-insensitive).
             */
            function applySearch(rows, cols, query) {
                if (!query || !query.trim()) return rows.slice();
                var q = query.trim().toLocaleLowerCase();
                return rows.filter(function(r) {
                    for (var i = 0; i < cols.length; i++) {
                        var val = r[cols[i]];
                        if (val != null && String(val).toLocaleLowerCase().indexOf(q) !== -1) {
                            return true;
                        }
                    }
                    return false;
                });
            }

            /**
             * sortRows â€” Sorts rows by a given column using natural comparison.
             */
            function sortRows(rows, colName, asc, cols) {
                var sorted = rows.slice();
                sorted.sort(function(a, b) {
                    var va = a[colName];
                    var vb = b[colName];
                    // Handle nulls: sort to end
                    if (va == null && vb == null) return 0;
                    if (va == null) return 1;
                    if (vb == null) return -1;
                    // Try numeric comparison
                    var na = parseFloat(va);
                    var nb = parseFloat(vb);
                    if (!isNaN(na) && !isNaN(nb)) {
                        return asc ? na - nb : nb - na;
                    }
                    // String comparison
                    var sa = String(va).toLocaleLowerCase();
                    var sb = String(vb).toLocaleLowerCase();
                    if (sa < sb) return asc ? -1 : 1;
                    if (sa > sb) return asc ? 1 : -1;
                    return 0;
                });
                return sorted;
            }

            /**
             * isNumericColumn â€” Heuristic: checks if >60% of non-null values are numeric.
             */
            function isNumericColumn(rows, colName) {
                var count = 0;
                var numeric = 0;
                for (var i = 0; i < rows.length; i++) {
                    var v = rows[i][colName];
                    if (v == null || v === '') continue;
                    count++;
                    if (!isNaN(parseFloat(v)) && isFinite(v)) numeric++;
                }
                if (count === 0) return false;
                return (numeric / count) > 0.6;
            }

            /**
             * calcSummaries â€” Computes SUM/AVG/COUNT for each numeric column.
             */
            function calcSummaries(cols, rows, numericCols) {
                var items = [];
                if (numericCols.length === 0) return items;
                numericCols.forEach(function(col) {
                    var sum = 0;
                    var count = 0;
                    for (var i = 0; i < rows.length; i++) {
                        var v = parseFloat(rows[i][col]);
                        if (!isNaN(v) && isFinite(v)) {
                            sum += v;
                            count++;
                        }
                    }
                    if (count > 0) {
                        items.push({ label: 'âˆ‘ ' + col, value: formatDrillNumber(sum) });
                        items.push({ label: 'âˆ… ' + col, value: formatDrillNumber(sum / count) });
                        items.push({ label: '#' + col, value: count.toLocaleString('ar-SA') });
                    }
                });
                return items;
            }

            /**
             * formatDrillCell â€” Formats cell values with type-aware display.
             * Numbers â†’ locale-formatted with LTR direction
             * Dates   â†’ formatted as YYYY-MM-DD
             * Null    â†’ â€” (em dash)
             * Strings â†’ truncated with ellipsis
             */
            function formatDrillCell(val) {
                if (val == null || val === '') {
                    return '<span class="wd-table__null">â€”</span>';
                }
                // Check if it's a number
                if (typeof val === 'number' || (typeof val === 'string' && !isNaN(parseFloat(val)) && isFinite(val) && val.trim() !== '')) {
                    var num = typeof val === 'number' ? val : parseFloat(val);
                    // Heuristic: if it looks like a date (e.g. "2024-01-15" or "2024/01/15"), format as date
                    var s = String(val).trim();
                    if (/^\d{4}[-/]\d{1,2}[-/]\d{1,2}$/.test(s)) {
                        return '<span class="wd-table__date">' + escapeHtml(s) + '</span>';
                    }
                    // Check for time component (e.g. "2024-01-15T10:30:00" or "2024-01-15 10:30")
                    if (/^\d{4}[-/]\d{1,2}[-/]\d{1,2}[T ]\d{1,2}:\d{2}/.test(s)) {
                        return '<span class="wd-table__date">' + escapeHtml(s) + '</span>';
                    }
                    return '<span class="wd-table__number">' + formatDrillNumber(num) + '</span>';
                }
                // Date objects
                if (val instanceof Date) {
                    var y = val.getFullYear();
                    var m = String(val.getMonth() + 1).padStart(2, '0');
                    var d = String(val.getDate()).padStart(2, '0');
                    return '<span class="wd-table__date">' + y + '-' + m + '-' + d + '</span>';
                }
                // String: escape and truncate
                var text = String(val);
                if (text.length > 120) {
                    text = text.substring(0, 120) + 'â€¦';
                }
                return '<span class="wd-table__text">' + escapeHtml(text) + '</span>';
            }

            /**
             * formatDrillNumber â€” Formats a number using Arabic locale with 0-2 decimal places.
             */
            function formatDrillNumber(val) {
                if (val == null || isNaN(val)) return 'â€”';
                // For very large numbers, show with 0 decimals; for small numbers with up to 2
                var decimals = (Math.abs(val) >= 1000) ? 0 : ((Math.abs(val) < 1) ? 2 : 1);
                try {
                    return val.toLocaleString('ar-SA', { minimumFractionDigits: decimals, maximumFractionDigits: decimals });
                } catch (e) {
                    return val.toLocaleString('ar-SA');
                }
            }

            /**
             * wdDrillSort â€” Toggles sort on a column (called from header onClick).
             */
            function wdDrillSort(colName) {
                var st = window.__drillState;
                if (!st) return;
                if (st.sortColumn === colName) {
                    st.sortAsc = !st.sortAsc;
                } else {
                    st.sortColumn = colName;
                    st.sortAsc = true;
                }
                st.currentPage = 1; // Reset to first page on sort change
                var bodyEl = document.getElementById('wd-drill-modal-body');
                if (!bodyEl) return;
                var data = st.currentData;
                if (!data) return;
                renderSmartGrid(bodyEl, data);
            }
            window.wdDrillSort = wdDrillSort;

            function wdLoadCard(id, showSkeleton) {
                var el = document.getElementById('card-' + id);
                if (!el) return;
                if (showSkeleton) {
                    var body = el.querySelector('.wd-card__body');
                    body.innerHTML = '<div class="wd-skeleton-wrap"><div class="wd-skel wd-skel--tall"></div></div>';
                }
                var preset = window.WD_DATE_PRESET || 'today';
                var url = '/api/dashboard/card/' + id + '?preset=' + encodeURIComponent(preset);
                if (preset === 'custom') {
                    var dateFrom = document.getElementById('wd-date-from');
                    var dateTo = document.getElementById('wd-date-to');
                    if (dateFrom && dateTo && dateFrom.value && dateTo.value) {
                        url += '&dateFrom=' + encodeURIComponent(dateFrom.value) + '&dateTo=' + encodeURIComponent(dateTo.value);
                    }
                }
                fetch(url, { headers: { 'Accept': 'application/json' } })
                    .then(function (r) { return r.json(); })
                    .then(function (data) { wdRenderCard(data); })
                    .catch(function (err) {
                        var b = el.querySelector('.wd-card__body');
                        if (b) b.innerHTML = wdErrorHtml('طھط¹ط°ط± ط§ظ„ط§طھطµط§ظ„ ط¨ط§ظ„ط®ط§ط¯ظ…: ' + (err && err.message ? err.message : ''), id);
                        showToast('error', 'طھط¹ط°ط± طھط­ظ…ظٹظ„ ط¨ط·ط§ظ‚ط© #' + id);
                    });
            }

            window.wdRetry = function (id) { wdLoadCard(id, true); };

            function updateTimestamp() {
                var el = document.getElementById('wd-last-updated');
                if (el) el.textContent = 'ط¢ط®ط± طھط­ط¯ظٹط«: ' + new Date().toLocaleTimeString('ar-SA');
            }

            window.wdRefreshAll = function () {
                showToast('success', 'ط¬ط§ط±ظچ طھط­ط¯ظٹط« ط§ظ„ط¨ط·ط§ظ‚ط§طھâ€¦');
                updateTimestamp();
                (window.WD_CARDS || []).forEach(function (c) { wdLoadCard(c.id, true); });
            };

            // â”€â”€ Drill-down State Machine â”€â”€
            window.__drillState = null;

            function wdDestroyDrillCharts() {
                if (window.__drillChart) {
                    try { window.__drillChart.destroy(); } catch (e) {}
                    window.__drillChart = null;
                }
            }

            function wdShowDrillSkeleton() {
                var bodyEl = document.getElementById('wd-drill-modal-body');
                if (!bodyEl) return;
                bodyEl.innerHTML = '<div class="wd-drill-skeleton">'
                    + '<div class="wd-drill-skeleton__text-center">'
                    + '<div class="wd-drill-skeleton__spinner"></div>'
                    + '<div class="wd-drill-skeleton__loading-text">ط¬ط§ط±ظچ طھط­ظ…ظٹظ„ ط§ظ„ط¨ظٹط§ظ†ط§طھ...</div>'
                    + '</div>'
                    + '<div class="wd-drill-skeleton__table">'
                    + '<div class="wd-drill-skeleton__table-row">'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '</div>'
                    + '<div class="wd-drill-skeleton__table-row">'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '</div>'
                    + '<div class="wd-drill-skeleton__table-row">'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '</div>'
                    + '<div class="wd-drill-skeleton__table-row">'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '<div class="wd-drill-skeleton__cell"></div>'
                    + '</div>'
                    + '</div></div>';
            }

            function wdOpenDrill(cardId, cardTitle) {
                var modal = document.getElementById('wd-drill-modal');
                if (!modal) { window.location.href = '/Dashboard/Drill/' + cardId; return; }
                modal.hidden = false;
                document.body.style.overflow = 'hidden';

                var titleTextEl = document.getElementById('wd-drill-modal-title-text');
                var cardEl = document.getElementById('card-' + cardId);
                var title = cardTitle || (cardEl ? (cardEl.getAttribute('data-title') || 'طھظپط§طµظٹظ„') : 'طھظپط§طµظٹظ„');
                if (titleTextEl) titleTextEl.textContent = title;

                window.__drillState = {
                    cardId: cardId,
                    cardTitle: title,
                    currentLevel: 1,
                    trail: [{ level: 1, displayName: title, labelValue: null, parentValueForCurrentLevel: null }],
                    parentValueForCurrentLevel: null,
                    selectedParameterValue: null,
                    parameterColumn: null,
                    labelColumn: null,
                    nextRequiresParentValue: false,
                    hasNextLevel: false,
                    // Smart Table state (TASK-DRILL-SMARTTABLE-001)
                    sortColumn: null,
                    sortAsc: true,
                    searchQuery: '',
                    pageSize: 50,
                    currentPage: 1,
                    filteredRows: null
                };

                wdDestroyDrillCharts();
                wdShowDrillSkeleton();
                var footerEl = document.getElementById('wd-drill-modal-footer');
                if (footerEl) footerEl.innerHTML = '';
                var crumbEl = document.getElementById('wd-drill-modal-breadcrumb');
                if (crumbEl) crumbEl.innerHTML = '';
                // Reset toolbar
                var searchInput = document.getElementById('wd-drill-search');
                if (searchInput) searchInput.value = '';
                // Show toolbar/pagination (hidden by default; wdRenderGrid keeps them visible for table)
                var toolbar = document.getElementById('wd-drill-toolbar');
                if (toolbar) toolbar.hidden = false;
                var pagination = document.getElementById('wd-drill-pagination');
                if (pagination) pagination.hidden = false;
                wdLoadLevel();
            }
            window.wdOpenDrill = wdOpenDrill;

            function wdLoadLevel() {
                var st = window.__drillState;
                if (!st) return;
                var bodyEl = document.getElementById('wd-drill-modal-body');
                if (!bodyEl) return;
                wdDestroyDrillCharts();
                wdShowDrillSkeleton();
                st.currentData = null;
                var url = '/api/dashboard/drill/' + st.cardId + '/' + st.currentLevel
                    + '?parentValue=' + encodeURIComponent(st.parentValueForCurrentLevel || '');
                fetch(url, { headers: { 'Accept': 'application/json' } })
                    .then(function (r) { return r.json(); })
                    .then(function (data) {
                        if (!window.__drillState) return;
                        st.hasNextLevel = !!data.hasNextLevel;
                        st.nextRequiresParentValue = !!data.nextRequiresParentValue;
                        st.parameterColumn = data.parameterColumn || null;
                        st.labelColumn = data.labelColumn || null;
                        st.currentData = data;
                        wdRenderBreadcrumb();
                        wdRenderLevel(data);
                        wdRenderFooter();
                    })
                    .catch(function (err) {
                        if (!window.__drillState) return;
                        if (bodyEl) {
                            bodyEl.innerHTML = wdErrorHtml('طھط¹ط°ط± طھط­ظ…ظٹظ„ ط¨ظٹط§ظ†ط§طھ ط§ظ„طھط¹ظ…ظ‘ظ‚: ' + (err && err.message ? err.message : ''), st.cardId);
                            wdAppendDrillRetry(bodyEl);
                        }
                        wdRenderFooter();
                    });
            }

            function wdAppendDrillRetry(container) {
                if (!container) return;
                var retryWrap = document.createElement('div');
                retryWrap.style.cssText = 'margin-top:20px;text-align:center;';
                var retryBtn = document.createElement('button');
                retryBtn.type = 'button';
                retryBtn.className = 'wd-btn wd-btn--primary';
                retryBtn.innerHTML = '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M23 4v6h-6"/><path d="M1 20v-6h6"/><path d="M3.51 9a9 9 0 0114.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0020.49 15"/></svg> ط¥ط¹ط§ط¯ط© ط§ظ„ظ…ط­ط§ظˆظ„ط©';
                retryBtn.addEventListener('click', wdLoadLevel);
                retryWrap.appendChild(retryBtn);
                container.appendChild(retryWrap);
            }

            function wdRenderLevel(data) {
                var bodyEl = document.getElementById('wd-drill-modal-body');
                if (!bodyEl) return;
                var st = window.__drillState;
                if (!st) return;

                if (data.status === 'success') {
                    var div = document.createElement('div');
                    div.style.width = '100%';
                    if (data.chartType === 'Table') {
                        div.style.minHeight = '300px';
                        bodyEl.innerHTML = '';
                        bodyEl.appendChild(div);
                        wdRenderGrid(div, data);
                        // Add click selection to rows (uses data-row-index for pagination compatibility)
                        var table = div.querySelector('.wd-table');
                        if (table && st.parameterColumn) {
                            var rows = table.querySelectorAll('tbody tr');
                            rows.forEach(function (tr) {
                                tr.style.cursor = 'pointer';
                                tr.addEventListener('click', function () {
                                    // Remove previous selection
                                    rows.forEach(function(r) { r.classList.remove('wd-drill-row--selected'); });
                                    // Add selection to clicked row
                                    tr.classList.add('wd-drill-row--selected');
                                    var rowIdx = parseInt(tr.getAttribute('data-row-index'), 10);
                                    var rowData = (data.rows || [])[rowIdx];
                                    if (!rowData) return;
                                    var selVal = rowData[st.parameterColumn] != null ? rowData[st.parameterColumn] : rowData[Object.keys(rowData)[0]];
                                    var selLabel = st.labelColumn && rowData[st.labelColumn] != null ? rowData[st.labelColumn] : selVal;
                                    wdSelectRow(selVal, selLabel);
                                });
                            });
                        }
                    } else if (data.chartType === 'Gauge') {
                        div.style.height = '400px';
                        bodyEl.innerHTML = '';
                        bodyEl.appendChild(div);
                        wdRenderGauge(div, data);
                    } else if (data.chartType === 'KPI') {
                        var val = data.kpiValue != null ? data.kpiValue : (data.rows && data.rows[0] ? data.rows[0][data.columns && data.columns[0] || 'value'] : null);
                        var displayVal = val != null ? (typeof formatMoney === 'function' ? formatMoney(val) : val) : 'â€”';
                        bodyEl.innerHTML = '';
                        var kpiWrap = document.createElement('div');
                        kpiWrap.className = 'wd-drill-kpi';
                        var kpiVal = document.createElement('div');
                        kpiVal.className = 'wd-drill-kpi__value';
                        kpiVal.textContent = displayVal;
                        kpiWrap.appendChild(kpiVal);
                        if (data.title) {
                            var kpiTitle = document.createElement('div');
                            kpiTitle.className = 'wd-drill-kpi__label';
                            kpiTitle.textContent = data.title;
                            kpiWrap.appendChild(kpiTitle);
                        }
                        bodyEl.appendChild(kpiWrap);
                        if (st.hasNextLevel) {
                            var nextWrap = document.createElement('div');
                            nextWrap.className = 'wd-drill-kpi__next-btn';
                            var nextBtn = document.createElement('button');
                            nextBtn.type = 'button';
                            nextBtn.className = 'wd-btn wd-btn--primary';
                            nextBtn.innerHTML = 'ط§ظ„ظ…ط³طھظˆظ‰ ط§ظ„طھط§ظ„ظٹ <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>';
                            nextBtn.addEventListener('click', wdNextLevel);
                            nextWrap.appendChild(nextBtn);
                            kpiWrap.appendChild(nextWrap);
                        }
                    } else {
                        div.style.height = '400px';
                        bodyEl.innerHTML = '';
                        bodyEl.appendChild(div);
                        wdRenderChart(div, data);
                        // Add selection table below chart if hasNextLevel and parameterColumn exists
                        if (st.hasNextLevel && st.parameterColumn && data.rows && data.rows.length > 0) {
                            var selTableDiv = document.createElement('div');
                            selTableDiv.className = 'wd-drill-selection-table';
                            var selLabel = document.createElement('div');
                            selLabel.className = 'wd-drill-selection-table__header';
                            selLabel.textContent = 'ط§ط®طھط± ط¹ظ†طµط±ط§ظ‹ ظ„ظ„ط§ظ†طھظ‚ط§ظ„ ظ„ظ„ظ…ط³طھظˆظ‰ ط§ظ„طھط§ظ„ظٹ';
                            selTableDiv.appendChild(selLabel);
                            var selTable = document.createElement('table');
                            var cols = data.columns || [];
                            var tbody = document.createElement('tbody');
                            (data.rows || []).forEach(function (r, ri) {
                                var tr = document.createElement('tr');
                                var displayVal = r[st.labelColumn || st.parameterColumn || cols[0]] != null ? r[st.labelColumn || st.parameterColumn || cols[0]] : r[cols[0]];
                                var paramVal = r[st.parameterColumn || cols[0]] != null ? r[st.parameterColumn || cols[0]] : r[cols[0]];
                                var td = document.createElement('td');
                                td.textContent = displayVal != null ? String(displayVal) : 'â€”';
                                tr.appendChild(td);
                                tr.addEventListener('click', function () {
                                    // Remove previous selection
                                    tbody.querySelectorAll('tr').forEach(function(r) { r.classList.remove('wd-drill-row--selected'); });
                                    // Add selection to clicked row
                                    tr.classList.add('wd-drill-row--selected');
                                    wdSelectRow(paramVal, displayVal);
                                });
                                tbody.appendChild(tr);
                            });
                            selTable.appendChild(tbody);
                            selTableDiv.appendChild(selTable);
                            bodyEl.appendChild(selTableDiv);
                        }
                    }
                } else if (data.status === 'empty') {
                    bodyEl.innerHTML = '<div class="wd-drill-empty">'
                        + '<div class="wd-drill-empty__icon" aria-hidden="true">'
                        + '<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">'
                        + '<rect x="3" y="3" width="18" height="18" rx="2"></rect>'
                        + '<path d="M3 9h18"></path><path d="M9 21V9"></path>'
                        + '</svg></div>'
                        + '<h3>ظ„ط§ طھظˆط¬ط¯ ط¨ظٹط§ظ†ط§طھ ظ„ظ‡ط°ط§ ط§ظ„ظ…ط³طھظˆظ‰</h3>'
                        + '<p>ظ„ظ… ظٹطھظ… ط§ظ„ط¹ط«ظˆط± ط¹ظ„ظ‰ ط³ط¬ظ„ط§طھ ظپظٹ ظ‡ط°ط§ ط§ظ„ظ…ط³طھظˆظ‰. ط¬ط±ظ‘ط¨ ط§ظ„ط¹ظˆط¯ط© ظ„ظ„ظ…ط³طھظˆظ‰ ط§ظ„ط³ط§ط¨ظ‚ ط£ظˆ ط§ط®طھط± ط¹ظ†طµط±ط§ظ‹ ظ…ط®طھظ„ظپط§ظ‹.</p>'
                        + '</div>';
                } else if (data.status === 'error') {
                    bodyEl.innerHTML = '<div class="wd-drill-error">'
                        + '<div class="wd-drill-error__icon" aria-hidden="true">'
                        + '<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">'
                        + '<circle cx="12" cy="12" r="10"></circle>'
                        + '<path d="M12 8v4"></path><path d="M12 16h.01"></path>'
                        + '</svg></div>'
                        + '<h3>طھط¹ط°ط± طھط­ظ…ظٹظ„ ط§ظ„ط¨ظٹط§ظ†ط§طھ</h3>'
                        + '<p>' + escapeHtml(data.errorMessage || 'ط­ط¯ط« ط®ط·ط£ ط؛ظٹط± ظ…طھظˆظ‚ط¹ ط£ط«ظ†ط§ط، طھط­ظ…ظٹظ„ ط¨ظٹط§ظ†ط§طھ ط§ظ„طھط¹ظ…ظ‘ظ‚.') + '</p>'
                        + '</div>';
                    wdAppendDrillRetry(bodyEl);
                } else if (data.status === 'none') {
                    bodyEl.innerHTML = '<div class="wd-drill-empty">'
                        + '<div class="wd-drill-empty__icon" aria-hidden="true">'
                        + '<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">'
                        + '<circle cx="12" cy="12" r="10"></circle>'
                        + '<path d="M12 16v-4"></path><path d="M12 8h.01"></path>'
                        + '</svg></div>'
                        + '<h3>ظ…ط¹ظ„ظˆظ…ط§طھ</h3>'
                        + '<p>' + escapeHtml(data.errorMessage || 'ظ„ط§ طھظˆط¬ط¯ ظ…ط¹ظ„ظˆظ…ط§طھ ط¥ط¶ط§ظپظٹط©.') + '</p>'
                        + '</div>';
                } else {
                    bodyEl.innerHTML = '<div class="wd-drill-empty">'
                        + '<div class="wd-drill-empty__icon" aria-hidden="true">'
                        + '<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">'
                        + '<rect x="3" y="3" width="18" height="18" rx="2"></rect>'
                        + '<path d="M3 9h18"></path><path d="M9 21V9"></path>'
                        + '</svg></div>'
                        + '<h3>ظ„ط§ طھظˆط¬ط¯ ط¨ظٹط§ظ†ط§طھ</h3>'
                        + '<p>ظ„ظ… ظٹطھظ… ط§ظ„ط¹ط«ظˆط± ط¹ظ„ظ‰ ط¨ظٹط§ظ†ط§طھ ظ„ط¹ط±ط¶ظ‡ط§.</p>'
                        + '</div>';
                }
            }

            function wdSelectRow(value, label) {
                var st = window.__drillState;
                if (!st) return;
                st.selectedParameterValue = value;
                if (st.trail.length > 0) {
                    st.trail[st.trail.length - 1].labelValue = label;
                }
                wdRenderBreadcrumb();
                wdRenderFooter();
                // Show toast feedback
                if (typeof showToast === 'function') {
                    showToast('success', 'طھظ… ط§ط®طھظٹط§ط±: ' + (label || value));
                }
            }

            function wdNavigateToLevel(level) {
                var st = window.__drillState;
                if (!st) return;
                if (level < 1 || level > st.currentLevel) return;
                st.trail = st.trail.slice(0, level);
                st.currentLevel = level;
                st.parentValueForCurrentLevel = st.trail[level - 1] ? st.trail[level - 1].parentValueForCurrentLevel : null;
                st.selectedParameterValue = null;
                wdDestroyDrillCharts();
                wdLoadLevel();
            }

            function wdNextLevel() {
                var st = window.__drillState;
                if (!st) return;
                if (!st.hasNextLevel) return;
                if (st.nextRequiresParentValue && (st.selectedParameterValue == null || st.selectedParameterValue === '')) return;
                var parentValueForNextLevel = st.nextRequiresParentValue ? st.selectedParameterValue : null;
                st.currentLevel++;
                st.parentValueForCurrentLevel = parentValueForNextLevel;
                st.selectedParameterValue = null;
                st.trail.push({ level: st.currentLevel, displayName: '...', labelValue: null, parentValueForCurrentLevel: parentValueForNextLevel });
                wdDestroyDrillCharts();
                wdLoadLevel();
            }

            function wdRenderBreadcrumb() {
                var crumbEl = document.getElementById('wd-drill-modal-breadcrumb');
                if (!crumbEl) return;
                var st = window.__drillState;
                if (!st || !st.trail) { crumbEl.innerHTML = ''; return; }
                crumbEl.innerHTML = '';
                st.trail.forEach(function (t, i) {
                    if (i > 0) {
                        var sep = document.createElement('span');
                        sep.className = 'wd-modal__crumb-sep';
                        sep.setAttribute('aria-hidden', 'true');
                        sep.innerHTML = '<svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>';
                        crumbEl.appendChild(sep);
                    }
                    var label = t.labelValue != null ? t.displayName + ' (' + String(t.labelValue) + ')' : t.displayName;
                    if (i === st.trail.length - 1) {
                        var active = document.createElement('span');
                        active.className = 'wd-modal__crumb wd-modal__crumb--active';
                        active.textContent = label;
                        crumbEl.appendChild(active);
                    } else {
                        var btn = document.createElement('button');
                        btn.type = 'button';
                        btn.className = 'wd-modal__crumb wd-modal__crumb--link';
                        btn.textContent = label;
                        btn.addEventListener('click', function () { wdNavigateToLevel(i + 1); });
                        crumbEl.appendChild(btn);
                    }
                });
            }

            function wdRenderFooter() {
                var footerEl = document.getElementById('wd-drill-modal-footer');
                if (!footerEl) return;
                var st = window.__drillState;
                if (!st) { footerEl.innerHTML = ''; return; }
                footerEl.innerHTML = '';
                if (st.currentLevel > 1) {
                    var previousBtn = document.createElement('button');
                    previousBtn.type = 'button';
                    previousBtn.className = 'wd-btn wd-btn--ghost';
                    previousBtn.innerHTML = '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m15 18-6-6 6-6"/></svg> ط§ظ„ظ…ط³طھظˆظ‰ ط§ظ„ط³ط§ط¨ظ‚';
                    previousBtn.addEventListener('click', function () { wdNavigateToLevel(st.currentLevel - 1); });
                    footerEl.appendChild(previousBtn);
                }
                if (st.hasNextLevel) {
                    var disabled = st.nextRequiresParentValue && (st.selectedParameterValue == null || st.selectedParameterValue === '');
                    var nextBtn = document.createElement('button');
                    nextBtn.type = 'button';
                    nextBtn.className = 'wd-btn wd-btn--primary';
                    nextBtn.innerHTML = 'ط§ظ„ظ…ط³طھظˆظ‰ ط§ظ„طھط§ظ„ظٹ <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>';
                    nextBtn.disabled = disabled;
                    nextBtn.addEventListener('click', wdNextLevel);
                    footerEl.appendChild(nextBtn);
                    if (st.nextRequiresParentValue) {
                        var hint = document.createElement('span');
                        hint.className = 'wd-modal__footer-hint';
                        hint.textContent = 'ط§ط®طھط± ط¹ظ†طµط±ط§ظ‹ ظ…ظ† ط§ظ„ط¬ط¯ظˆظ„ ط£ط¹ظ„ط§ظ‡ ظ„ظ„ط§ظ†طھظ‚ط§ظ„';
                        footerEl.appendChild(hint);
                    }
                } else {
                    var badge = document.createElement('span');
                    badge.className = 'wd-modal__footer-badge';
                    badge.textContent = 'ط¢ط®ط± ظ…ط³طھظˆظ‰';
                    footerEl.appendChild(badge);
                }
                // Export CSV button â€” visible for Table/Chart levels with data
                if (st.currentData && st.currentData.rows && st.currentData.rows.length > 0) {
                    var chartType = st.currentData.chartType || '';
                    var exportableTypes = ['Table', 'Bar', 'Line', 'Pie'];
                    if (exportableTypes.indexOf(chartType) !== -1) {
                        // Excel export button (TASK-DRILL-EXCEL-001)
                        var excelBtn = document.createElement('button');
                        excelBtn.type = 'button';
                        excelBtn.className = 'wd-btn wd-btn--ghost';
                        excelBtn.innerHTML = '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="8" y1="16" x2="16" y2="16"/><line x1="8" y1="12" x2="16" y2="12"/></svg> Excel';
                        excelBtn.addEventListener('click', function() { wdExportExcel(); });
                        footerEl.appendChild(excelBtn);

                        var exportBtn = document.createElement('button');
                        exportBtn.type = 'button';
                        exportBtn.className = 'wd-btn wd-btn--ghost';
                        exportBtn.innerHTML = '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg> طھطµط¯ظٹط± CSV';
                        exportBtn.addEventListener('click', function () { wdExportCsv(); });
                        footerEl.appendChild(exportBtn);
                    }
                }
            }

            function wdExportCsv() {
                var st = window.__drillState;
                if (!st || !st.currentData || !st.currentData.rows || st.currentData.rows.length === 0) return;

                var data = st.currentData;
                var cols = data.columns || [];
                var rows = data.rows || [];

                // BOM for UTF-8
                var bom = '\uFEFF';

                // Helper: escape a CSV field (wrap in quotes if needed)
                function csvField(val) {
                    var s = val != null ? String(val) : '';
                    return '"' + s.replace(/"/g, '""') + '"';
                }

                // Header row
                var header = cols.map(csvField).join(',');

                // Data rows
                var dataRows = rows.map(function(r) {
                    return cols.map(function(c) {
                        return csvField(r[c]);
                    }).join(',');
                });

                // Combine
                var csv = bom + header + '\n' + dataRows.join('\n');

                // Create blob and download
                var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
                var url = URL.createObjectURL(blob);
                var link = document.createElement('a');
                link.href = url;

                // Filename: drill-{cardId}-level-{level}-{yyyyMMdd-HHmm}.csv
                var now = new Date();
                var pad = function(n) { return String(n).padStart(2, '0'); };
                var timestamp = now.getFullYear()
                    + pad(now.getMonth() + 1)
                    + pad(now.getDate()) + '-'
                    + pad(now.getHours())
                    + pad(now.getMinutes());
                link.download = 'drill-' + st.cardId + '-level-' + st.currentLevel + '-' + timestamp + '.csv';

                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                URL.revokeObjectURL(url);
            }
            window.wdExportCsv = wdExportCsv;

            function wdExportExcel() {
              var st = window.__drillState;
              if (!st) return;
              var parentVal = st.parentValueForCurrentLevel || '';
              var url = '/api/dashboard/drill/' + st.cardId + '/' + st.currentLevel + '/export?parentValue=' + encodeURIComponent(parentVal);
              window.open(url, '_blank');
            }
            window.wdExportExcel = wdExportExcel;

            function wdCloseDrillModal() {
                wdDestroyDrillCharts();
                window.__drillState = null;
                var modal = document.getElementById('wd-drill-modal');
                if (modal) {
                    modal.hidden = true;
                    document.body.style.overflow = '';
                }
                var body = document.getElementById('wd-drill-modal-body');
                if (body) body.innerHTML = '';
                var footerEl = document.getElementById('wd-drill-modal-footer');
                if (footerEl) footerEl.innerHTML = '';
                var crumbEl = document.getElementById('wd-drill-modal-breadcrumb');
                if (crumbEl) crumbEl.innerHTML = '';
                // Hide toolbar and pagination (TASK-DRILL-SMARTTABLE-001)
                var toolbar = document.getElementById('wd-drill-toolbar');
                if (toolbar) toolbar.hidden = true;
                var pagination = document.getElementById('wd-drill-pagination');
                if (pagination) pagination.hidden = true;
                var searchInput = document.getElementById('wd-drill-search');
                if (searchInput) searchInput.value = '';
            }

            function wdCheckConnection() {
                var badge = document.getElementById('wd-conn');
                var txt = document.getElementById('wd-conn-text');
                fetch('/api/sync/status', { headers: { 'Accept': 'application/json' } })
                    .then(function (r) { return r.json(); })
                    .then(function (d) {
                        if (!badge || !txt) return;
                        if (d && d.connected) {
                            badge.className = 'wd-conn wd-conn--online';
                            txt.textContent = 'ظ…طھطµظ„';
                        } else {
                            badge.className = 'wd-conn wd-conn--offline';
                            txt.textContent = 'ط؛ظٹط± ظ…طھطµظ„';
                        }
                    })
                    .catch(function () {
                        if (!badge || !txt) return;
                        badge.className = 'wd-conn wd-conn--offline';
                        txt.textContent = 'ط؛ظٹط± ظ…طھطµظ„';
                    });
            }

            document.addEventListener('DOMContentLoaded', function () {
                (window.WD_CARDS || []).forEach(function (c) { wdLoadCard(c.id, false); });

                // â”€â”€ Per-card auto-refresh with visual indicator (TASK-CARD-BEH-002) â”€â”€
                window._autoRefreshTimers = {};
                (window.WD_CARDS || []).forEach(function (c) {
                    if (c.refreshInterval > 0) {
                        var fn = (function (id) {
                            return function () {
                                var el = document.getElementById('card-' + id);
                                if (el) el.classList.add('wd-card--refreshing');
                                wdLoadCard(id, false);
                                setTimeout(function () {
                                    var el2 = document.getElementById('card-' + id);
                                    if (el2) el2.classList.remove('wd-card--refreshing');
                                }, 1500);
                            };
                        })(c.id);
                        window._autoRefreshTimers[c.id] = setInterval(fn, c.refreshInterval * 1000);
                    }
                });
                window.addEventListener('beforeunload', function () {
                    Object.keys(window._autoRefreshTimers || {}).forEach(function (k) {
                        clearInterval(window._autoRefreshTimers[k]);
                    });
                });

                wdCheckConnection();

                // Focus Mode toggle
                (function() {
                    var toggle = document.getElementById('wd-focus-toggle');
                    if (!toggle) return;
                    toggle.addEventListener('click', function() {
                        document.body.classList.toggle('wd-dashboard--focus');
                        toggle.classList.toggle('wd-btn--primary');
                        toggle.querySelector('svg').innerHTML = document.body.classList.contains('wd-dashboard--focus')
                            ? '<path d="M8 3v3a2 2 0 01-2 2H3"/><path d="M21 8h-3a2 2 0 01-2-2V3"/><path d="M16 21v-3a2 2 0 012-2h3"/><path d="M3 16h3a2 2 0 012 2v3"/>'
                            : '<path d="M8 3H5a2 2 0 00-2 2v3"/><path d="M21 8V5a2 2 0 00-2-2h-3"/><path d="M16 21h3a2 2 0 002-2v-3"/><path d="M3 16v3a2 2 0 002 2h3"/>';
                    });
                })();

                // === Layout Edit Mode Toggle (TASK-CARD-LAYOUT-EDIT-001) ===
                (function() {
                    var toggle = document.getElementById('wd-layout-edit-toggle');
                    if (!toggle) return;

                    var STORAGE_KEY = 'wd-layout-edit';

                    // Restore persisted state
                    if (localStorage.getItem(STORAGE_KEY) === 'true') {
                        document.body.classList.add('wd-layout-edit-active');
                        toggle.classList.add('active');
                    }

                    // Apply hidden attribute removal when edit mode is active on load
                    function syncResizeVisibility(isActive) {
                        document.querySelectorAll('.wd-card__resize').forEach(function(el) {
                            if (isActive) {
                                el.removeAttribute('hidden');
                                el.removeAttribute('aria-hidden');
                            } else {
                                el.setAttribute('hidden', '');
                                el.setAttribute('aria-hidden', 'true');
                            }
                        });
                    }
                    // Initial sync (in case state was restored from localStorage)
                    syncResizeVisibility(document.body.classList.contains('wd-layout-edit-active'));

                    toggle.addEventListener('click', function() {
                        var isActive = document.body.classList.toggle('wd-layout-edit-active');
                        toggle.classList.toggle('active', isActive);
                        localStorage.setItem(STORAGE_KEY, isActive);

                        syncResizeVisibility(isActive);

                        if (typeof showToast === 'function') {
                            showToast(isActive ? 'success' : 'info', isActive ? 'طھظ… طھظپط¹ظٹظ„ ظˆط¶ط¹ طھط¹ط¯ظٹظ„ ط§ظ„طھط®ط·ظٹط·' : 'طھظ… ط¥ظٹظ‚ط§ظپ ظˆط¶ط¹ طھط¹ط¯ظٹظ„ ط§ظ„طھط®ط·ظٹط·');
                        }
                    });
                })();

                // === Drill-down Modal Close Handlers ===
                (function() {
                    var modal = document.getElementById('wd-drill-modal');
                    if (!modal) return;
                    var closeBtn = modal.querySelector('.wd-modal__close');
                    var overlay = modal.querySelector('.wd-modal__overlay');

                    if (closeBtn) closeBtn.addEventListener('click', wdCloseDrillModal);
                    if (overlay) overlay.addEventListener('click', wdCloseDrillModal);

                    document.addEventListener('keydown', function(e) {
                        if (e.key === 'Escape' && !modal.hidden) {
                            wdCloseDrillModal();
                        }
                    });
                })();

                // === Smart Drill Table Event Listeners (TASK-DRILL-SMARTTABLE-001) ===
                (function() {
                    var searchInput = document.getElementById('wd-drill-search');
                    var pageSizeSelect = document.getElementById('wd-drill-page-size');
                    var prevBtn = document.getElementById('wd-page-prev');
                    var nextBtn = document.getElementById('wd-page-next');

                    if (searchInput) {
                        searchInput.addEventListener('input', function() {
                            var st = window.__drillState;
                            if (!st) return;
                            st.searchQuery = this.value;
                            st.currentPage = 1;
                            var bodyEl = document.getElementById('wd-drill-modal-body');
                            if (!bodyEl) return;
                            var data = st.currentData;
                            if (!data) return;
                            renderSmartGrid(bodyEl, data);
                        });
                    }

                    if (pageSizeSelect) {
                        pageSizeSelect.addEventListener('change', function() {
                            var st = window.__drillState;
                            if (!st) return;
                            st.pageSize = parseInt(this.value, 10);
                            st.currentPage = 1;
                            var bodyEl = document.getElementById('wd-drill-modal-body');
                            if (!bodyEl) return;
                            var data = st.currentData;
                            if (!data) return;
                            renderSmartGrid(bodyEl, data);
                        });
                    }

                    if (prevBtn) {
                        prevBtn.addEventListener('click', function() {
                            var st = window.__drillState;
                            if (!st) return;
                            if (st.currentPage > 1) {
                                goToPage(st.currentPage - 1);
                            }
                        });
                    }

                    if (nextBtn) {
                        nextBtn.addEventListener('click', function() {
                            var st = window.__drillState;
                            if (!st) return;
                            if (st.currentPage < Math.ceil((st.filteredRows || st.currentData?.rows || []).length / st.pageSize)) {
                                goToPage(st.currentPage + 1);
                            }
                        });
                    }
                })();

                // === Quick Date Presets ===
                (function() {
                    var presetBtns = document.querySelectorAll('.wd-preset-btn');
                    var customDates = document.getElementById('wd-custom-dates');
                    var dateFrom = document.getElementById('wd-date-from');
                    var dateTo = document.getElementById('wd-date-to');
                    if (!presetBtns.length) return;

                    window.WD_DATE_PRESET = 'today';

                    function getPresetDates(preset) {
                        var now = new Date();
                        var start, end;
                        switch (preset) {
                            case 'today':
                                start = end = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                                break;
                            case 'yesterday':
                                start = end = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 1);
                                break;
                            case '7days':
                                start = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 6);
                                end = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                                break;
                            case '30days':
                                start = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 29);
                                end = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                                break;
                            case 'month':
                                start = new Date(now.getFullYear(), now.getMonth(), 1);
                                end = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                                break;
                            case 'lastMonth':
                                start = new Date(now.getFullYear(), now.getMonth() - 1, 1);
                                end = new Date(now.getFullYear(), now.getMonth(), 0);
                                break;
                            case 'custom':
                                if (dateFrom && dateTo && dateFrom.value && dateTo.value) {
                                    start = new Date(dateFrom.value);
                                    end = new Date(dateTo.value);
                                } else {
                                    return null;
                                }
                                break;
                            default:
                                return null;
                        }
                        return {
                            from: start.toISOString().split('T')[0],
                            to: end.toISOString().split('T')[0]
                        };
                    }

                    presetBtns.forEach(function(btn) {
                        btn.addEventListener('click', function() {
                            var preset = this.getAttribute('data-preset');
                            presetBtns.forEach(function(b) { b.classList.remove('active'); });
                            this.classList.add('active');

                            window.WD_DATE_PRESET = preset;
                            if (customDates) {
                                customDates.classList.toggle('wd-hidden', preset !== 'custom');
                            }
                            // Reload all cards with the new preset
                            (window.WD_CARDS || []).forEach(function(c) { wdLoadCard(c.id, true); });
                        });
                    });

                    if (dateFrom && dateTo) {
                        var onCustomChange = function() {
                            if (window.WD_DATE_PRESET === 'custom' && dateFrom.value && dateTo.value) {
                                (window.WD_CARDS || []).forEach(function(c) { wdLoadCard(c.id, true); });
                            }
                        };
                        dateFrom.addEventListener('change', onCustomChange);
                        dateTo.addEventListener('change', onCustomChange);
                    }
                })();
            });

            var rt;
            window.addEventListener('resize', function () {
                clearTimeout(rt);
                rt = setTimeout(function () {
                    // ApexCharts handles responsive resize automatically.
                    // Force sync for any chart that might be out of viewport.
                    Object.keys(CHARTS).forEach(function(id) {
                        if (CHARTS[id] && CHARTS[id].control && CHARTS[id].control.windowResizeSync) {
                            CHARTS[id].control.windowResizeSync();
                        }
                    });
                }, 200);
            });
        })();

        /* ===== TASK-COD-014: client-side search + ChartType filtering ===== */
        (function () {
            var searchInput = document.getElementById('wd-search');
            var typeSelect = document.getElementById('wd-type-filter');
            var emptyState = document.getElementById('wd-filter-empty');
            var countLabel = document.getElementById('wd-filter-count');
            if (!searchInput || !typeSelect) return;

            var grid = document.querySelector('.wd-dashboard-grid');
            var cards = grid
                ? Array.prototype.slice.call(grid.querySelectorAll('.wd-dashboard-grid > .wd-card'))
                : [];

            // Smoothly toggle a card's hidden state. A per-element sequence guards
            // against rapid keystrokes cancelling an in-flight transition.
            function setCardHidden(el, hide) {
                var seq = (el._wdFilterSeq = (el._wdFilterSeq || 0) + 1);
                if (hide) {
                    el.classList.add('wd-card--fade');
                    setTimeout(function () {
                        if (el._wdFilterSeq === seq) {
                            el.classList.add('wd-hidden');
                            el.classList.remove('wd-card--fade');
                        }
                    }, 240);
                } else {
                    el.classList.remove('wd-hidden');
                    el.classList.add('wd-card--fade');
                    requestAnimationFrame(function () {
                        requestAnimationFrame(function () {
                            if (el._wdFilterSeq === seq) el.classList.remove('wd-card--fade');
                        });
                    });
                }
            }

            var wasEmpty = false;
            function applyFilter() {
                var q = (searchInput.value || '').trim().toLocaleLowerCase();
                var type = typeSelect.value;
                var visible = 0;

                cards.forEach(function (el) {
                    var title = (el.getAttribute('data-title') || '').toLocaleLowerCase();
                    var chartType = el.getAttribute('data-chart-type') || '';
                    var match = (q === '' || title.indexOf(q) !== -1) &&
                                (type === 'All' || chartType === type);
                    setCardHidden(el, !match);
                    if (match) visible++;
                });

                if (emptyState) emptyState.classList.toggle('wd-hidden', visible !== 0);
                if (countLabel) {
                    countLabel.textContent = (visible === cards.length)
                        ? (cards.length + ' ط¨ط·ط§ظ‚ط©')
                        : (' ط¸ط§ظ‡ط± ' + visible + ' ظ…ظ† ' + cards.length);
                }

                var isEmpty = visible === 0;
                if (isEmpty && !wasEmpty && window.wdShowToast) {
                    window.wdShowToast('info', 'ظ„ط§ طھظˆط¬ط¯ ط¨ط·ط§ظ‚ط§طھ ظ…ط·ط§ط¨ظ‚ط© ظ„ط¨ط­ط«ظƒ');
                }
                wasEmpty = isEmpty;
            }

            // Debounce typing so we never thrash layout on every keystroke.
            var debounceTimer;
            searchInput.addEventListener('input', function () {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(applyFilter, 120);
            });
            typeSelect.addEventListener('change', applyFilter);

            applyFilter(); // sync count label on first render
        })();

        /* ===== TASK-DASH-005: Drag & Drop, Resize, Per-card Refresh, Layout Persistence ===== */
        (function () {
            var grid = document.getElementById('wd-dashboard-grid');
            if (!grid || typeof Sortable === 'undefined') return;

            // â”€â”€ Size presets: CSS class â†’ { w, h, heightPx } â”€â”€
            var SIZE_PRESETS = {
                small:  { w: 3, h: 2, heightPx: 200 },
                medium: { w: 6, h: 3, heightPx: 360 },
                large:  { w: 9, h: 4, heightPx: 450 }
            };

            // â”€â”€ 1. SortableJS initialization â”€â”€
            var sortable = new Sortable(grid, {
                animation: 200,
                ghostClass: 'sortable-ghost',
                chosenClass: 'sortable-chosen',
                dragClass: 'sortable-drag',
                handle: '.wd-card__header',
                forceFallback: false,
                // Touch support for mobile
                delay: 150,
                delayOnTouchOnly: true,
                touchStartThreshold: 5,
                onEnd: function (evt) {
                    // After drag reorder, recalculate Y positions (row order) and save
                    recalcAndSave();
                }
            });

            // â”€â”€ 2. Resize button handling â”€â”€
            grid.addEventListener('click', function (e) {
                var btn = e.target.closest('.wd-resize-btn');
                if (!btn) return;

                var card = btn.closest('.wd-card');
                if (!card) return;

                var size = btn.getAttribute('data-size');
                var preset = SIZE_PRESETS[size];
                if (!preset) return;

                // Prevent click from triggering card drill-down
                e.stopPropagation();

                // Update active state among sibling buttons
                var siblings = btn.parentElement.querySelectorAll('.wd-resize-btn');
                siblings.forEach(function (b) { b.classList.remove('active'); });
                btn.classList.add('active');

                // Remove old size classes
                for (var s in SIZE_PRESETS) {
                    card.classList.remove('wd-span-' + SIZE_PRESETS[s].w);
                    card.classList.remove('wd-row-' + SIZE_PRESETS[s].h);
                }

                // Apply new size classes
                card.classList.add('wd-span-' + preset.w);
                card.classList.add('wd-row-' + preset.h);

                // Update inline styles
                card.style.height = preset.heightPx + 'px';

                // Update data attributes
                card.setAttribute('data-grid-w', preset.w);
                card.setAttribute('data-grid-h', preset.h);

                // Resize ApexCharts inside this card (main chart + spark via density sync)
                var cardId = card.getAttribute('data-card-id');
                var chartEntry = window.CHARTS_INSTANCES && window.CHARTS_INSTANCES[cardId];
                if (chartEntry) {
                    var ctrl = chartEntry.control || chartEntry;
                    try {
                        if (ctrl && typeof ctrl.windowResizeSync === 'function') ctrl.windowResizeSync();
                        else if (ctrl && typeof ctrl.resize === 'function') ctrl.resize();
                    } catch (ex) { /* ignore */ }
                }

                // KPI adaptive shell: update size class + sparkline resize
                if (typeof window.wdSyncKpiDensity === 'function') {
                    window.wdSyncKpiDensity(card);
                }

                // Save layout
                recalcAndSave();
            });

            // â”€â”€ 3. Per-card refresh button â”€â”€
            grid.addEventListener('click', function (e) {
                var btn = e.target.closest('.wd-card__refresh');
                if (!btn) return;

                var card = btn.closest('.wd-card');
                if (!card) return;

                var cardId = parseInt(card.getAttribute('data-card-id'), 10);
                if (!cardId) return;

                // Prevent click from triggering card drill-down
                e.stopPropagation();

                // Spin animation
                btn.classList.add('spinning');

                // Reload card data with skeleton
                if (typeof wdLoadCard === 'function') {
                    wdLoadCard(cardId, true);
                }

                // Stop spinning after data loads (approx 2s)
                setTimeout(function () {
                    btn.classList.remove('spinning');
                }, 2000);
            });

            // â”€â”€ 4. Layout persistence â”€â”€
            var saveTimer = null;
            function recalcAndSave() {
                clearTimeout(saveTimer);
                saveTimer = setTimeout(function () {
                    var cards = grid.querySelectorAll('.wd-card');
                    var layoutItems = [];
                    var y = 1;

                    cards.forEach(function (card) {
                        var cardId = parseInt(card.getAttribute('data-card-id'), 10);
                        var w = parseInt(card.getAttribute('data-grid-w'), 10) || 4;
                        var h = parseInt(card.getAttribute('data-grid-h'), 10) || 2;

                        layoutItems.push({
                            cardId: cardId,
                            gridPositionX: 1,
                            gridPositionY: y,
                            gridWidth: w,
                            gridHeight: h
                        });

                        y++;
                    });

                    // POST layout to server
                    fetch('?handler=SaveLayout', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                        },
                        body: JSON.stringify({ dashboardId: 0, cards: layoutItems })
                    })
                    .then(function (r) { return r.json(); })
                    .then(function (data) {
                        if (data && data.success) {
                            showLayoutSavedToast();
                        } else {
                            showToast('error', 'طھط¹ط°ط± ط­ظپط¸ ط§ظ„طھط®ط·ظٹط·');
                        }
                    })
                    .catch(function () {
                        showToast('error', 'طھط¹ط°ط± ط§ظ„ط§طھطµط§ظ„ ط¨ط§ظ„ط®ط§ط¯ظ… ظ„ط­ظپط¸ ط§ظ„طھط®ط·ظٹط·');
                    });
                }, 400);
            }

            function showLayoutSavedToast() {
                var existing = document.querySelector('.wd-layout-saved');
                if (existing) existing.remove();

                var toast = document.createElement('div');
                toast.className = 'wd-layout-saved';
                toast.textContent = 'âœ“ طھظ… ط­ظپط¸ ط§ظ„طھط®ط·ظٹط·';
                toast.setAttribute('role', 'status');
                document.body.appendChild(toast);
                setTimeout(function () {
                    toast.style.opacity = '0';
                    toast.style.transition = 'opacity 0.3s ease';
                    setTimeout(function () { toast.remove(); }, 400);
                }, 2000);
            }

            // â”€â”€ 5. Mark correct resize active state on load â”€â”€
            grid.querySelectorAll('.wd-card').forEach(function (card) {
                var w = parseInt(card.getAttribute('data-grid-w'), 10) || 4;
                var btns = card.querySelectorAll('.wd-resize-btn');
                btns.forEach(function (b) {
                    b.classList.remove('active');
                    var size = b.getAttribute('data-size');
                    if (size === 'small' && w <= 3) b.classList.add('active');
                    else if (size === 'medium' && w > 3 && w <= 7) b.classList.add('active');
                    else if (size === 'large' && w > 7) b.classList.add('active');
                });
            });
        })();
