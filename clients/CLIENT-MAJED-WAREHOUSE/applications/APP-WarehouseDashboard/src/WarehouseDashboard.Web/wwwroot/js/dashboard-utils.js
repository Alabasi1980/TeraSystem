/**
 * dashboard-utils.js — Shared JavaScript utilities for the Warehouse Dashboard.
 *
 * Provides common helpers used by both Index.cshtml (main dashboard) and
 * Drill.cshtml (drill-down page). Loaded once via _DashboardLayout.cshtml.
 *
 * Functions:
 *   toNum(v)          — parse any value to a number (0 on failure)
 *   isNum(v)          — check if a value is numeric
 *   formatNum(v)      — human-readable number with K/M/B suffixes
 *   formatMoney(v)    — full-precision money with commas, 3 decimals, and د.أ
 *   escapeHtml(s)     — HTML-escape a string
 *   showToast(type, msg) — show a transient toast notification
 *   wdEmptyHtml()     — HTML for an empty-data state
 *   wdErrorHtml(msg, id) — HTML for an error state with retry button
 */
(function () {
    'use strict';

    function toNum(v) {
        if (v === null || v === undefined || v === '') return 0;
        if (typeof v === 'number') return v;
        var n = parseFloat(String(v).replace(/,/g, ''));
        return isNaN(n) ? 0 : n;
    }

    function isNum(v) {
        if (typeof v === 'number') return true;
        if (typeof v === 'string' && v !== '' && !isNaN(parseFloat(v)) && isFinite(v)) return true;
        return false;
    }

    function formatNum(v) {
        var n = toNum(v);
        if (n === 0) return '0';
        var abs = Math.abs(n);
        if (abs >= 1e9) return (n / 1e9).toFixed(1).replace(/\.0$/, '') + 'B';
        if (abs >= 1e6) return (n / 1e6).toFixed(1).replace(/\.0$/, '') + 'M';
        if (abs >= 1e3) return (n / 1e3).toFixed(1).replace(/\.0$/, '') + 'K';
        return String(n);
    }

    function formatMoney(v) {
        var n = toNum(v);
        if (n === 0) return '0.000 د.أ';
        var parts = n.toFixed(3).split('.');
        var intPart = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return intPart + '.' + parts[1] + ' د.أ';
    }

    function escapeHtml(s) {
        if (!s) return '';
        return String(s).replace(/[&<>"']/g, function (ch) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[ch];
        });
    }

    function showToast(type, msg) {
        var host = document.getElementById('wd-toast-host');
        if (!host) return;
        var cls = type === 'error' ? 'error' : (type === 'info' ? 'info' : 'success');
        var icon = type === 'error' ? '\u2715' : (type === 'info' ? '\u2139' : '\u2713');
        var t = document.createElement('div');
        t.className = 'wd-toast wd-toast--' + cls;
        t.setAttribute('role', 'status');
        t.innerHTML = '<span class="wd-toast__icon">' + icon + '</span>' +
            '<span class="wd-toast__msg">' + escapeHtml(msg) + '</span>';
        host.appendChild(t);
        setTimeout(function () {
            t.style.opacity = '0';
            t.style.transform = 'translateY(-12px)';
            t.style.transition = 'opacity 240ms, transform 240ms';
        }, 4200);
        setTimeout(function () { if (t.parentNode) t.parentNode.removeChild(t); }, 4600);
    }

    function wdEmptyHtml(title, msg) {
        return '<div class="wd-empty"><div class="wd-empty__icon">\u25A6</div>' +
            '<h3>' + escapeHtml(title || '\u0644\u0627 \u062A\u0648\u062C\u062F \u0628\u064A\u0627\u0646\u0627\u062A') + '</h3>' +
            '<p>' + escapeHtml(msg || '\u0644\u0627 \u062A\u0648\u062C\u062F \u0633\u062C\u0644\u0627\u062A \u0644\u0647\u0630\u0647 \u0627\u0644\u0628\u0637\u0627\u0642\u0629 \u062D\u0627\u0644\u064A\u0627\u064B.') + '</p></div>';
    }

    function wdErrorHtml(msg, id) {
        var retryBtn = typeof id !== 'undefined'
            ? '<button type="button" class="wd-btn wd-btn--ghost wd-btn--sm" onclick="wdRetry(' + id + ')">\u0625\u0639\u0627\u062F\u0629 \u0627\u0644\u0645\u062D\u0627\u0648\u0644\u0629</button>'
            : '';
        return '<div class="wd-empty wd-empty--error"><div class="wd-empty__icon">\u26A0</div>' +
            '<h3>\u062A\u0639\u0630\u0631 \u062A\u062D\u0645\u064A\u0644 \u0627\u0644\u0628\u0637\u0627\u0642\u0629</h3>' +
            '<p>' + escapeHtml(msg || '\u062D\u062F\u062B \u062E\u0637\u0623 \u063A\u064A\u0631 \u0645\u062A\u0648\u0642\u0639.') + '</p>' +
            retryBtn + '</div>';
    }

    // Expose as globals (window.*) for backward compatibility with inline scripts.
    window.toNum = toNum;
    window.isNum = isNum;
    window.formatNum = formatNum;
    window.formatMoney = formatMoney;
    window.escapeHtml = escapeHtml;
    window.showToast = showToast;
    window.wdShowToast = showToast;
    window.wdEmptyHtml = wdEmptyHtml;
    window.wdErrorHtml = wdErrorHtml;
})();
