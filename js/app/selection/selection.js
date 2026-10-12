(function (g) {
  "use strict";
  function normTi(ti) {
    if (ti == null) return null;
    ti = +ti;
    return isNaN(ti) ? null : ti;
  }
  function getSelection() {
    return g.AppState.selectedTis.slice();
  }
  function setSelected(ti) {
    const S = g.AppState;
    ti = normTi(ti);
    S.selectedTis = ti == null ? [] : [ti];
    S.selectedMarks = [];
    S.selectAnchor = ti;
    S.lastFocusTi = ti;
    S.lastFocusMark = null;
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(true, ti);
    g.AppHighlight.syncMarkerPanelSelection(false);
    g.AppExport.refreshExportPanel();
  }
  function setSelectedList(list, focusTi) {
    const S = g.AppState;
    const seen = {},
      out = [];
    (list || []).forEach(function (t) {
      t = normTi(t);
      if (t != null && !seen[t]) {
        seen[t] = 1;
        out.push(t);
      }
    });
    S.selectedTis = out;
    S.selectedMarks = [];
    if (focusTi !== undefined) S.lastFocusTi = normTi(focusTi);
    else S.lastFocusTi = out.length ? out[out.length - 1] : null;
    S.lastFocusMark = null;
    if (
      S.selectAnchor != null &&
      S.selectedTis.indexOf(S.selectAnchor) < 0 &&
      !focusTi
    ) {
      S.selectAnchor = S.lastFocusTi;
    }
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(true, S.lastFocusTi);
    g.AppHighlight.syncMarkerPanelSelection(false);
    g.AppExport.refreshExportPanel();
  }
  function toggleSelected(ti, scroll) {
    const S = g.AppState;
    ti = normTi(ti);
    if (ti == null) return;
    const ix = S.selectedTis.indexOf(ti);
    if (ix >= 0) S.selectedTis.splice(ix, 1);
    else S.selectedTis.push(ti);
    S.selectAnchor = ti;
    S.lastFocusTi = ti;
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(!!scroll, ti);
    g.AppHighlight.syncMarkerPanelSelection(false);
    g.AppExport.refreshExportPanel();
  }
  function clearSelection(scroll) {
    const S = g.AppState;
    S.selectedTis = [];
    S.selectedMarks = [];
    S.selectAnchor = null;
    S.lastFocusTi = null;
    S.lastFocusMark = null;
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(!!scroll, null);
    g.AppHighlight.syncMarkerPanelSelection(false);
    g.AppExport.refreshExportPanel();
  }
  function getPanelTiOrder() {
    try {
      const rows = document.querySelectorAll("#panel-models .filerow[data-ti]");
      if (rows && rows.length) {
        const out = [],
          seen = {};
        rows.forEach(function (r) {
          const ti = normTi(r.getAttribute("data-ti"));
          if (ti != null && !seen[ti]) {
            seen[ti] = 1;
            out.push(ti);
          }
        });
        if (out.length) return out;
      }
    } catch {
      /* ignore */
    }
    const S = g.AppState,
      s = {},
      o = [];
    S.modelEntries.forEach(function (en) {
      if (!s[en.ti]) {
        s[en.ti] = 1;
        o.push(en.ti);
      }
    });
    return o;
  }
  function selectRangeTo(ti, additive) {
    const S = g.AppState;
    ti = normTi(ti);
    if (ti == null) return;
    const order = getPanelTiOrder();
    const anchor =
      S.selectAnchor != null && order.indexOf(S.selectAnchor) >= 0
        ? S.selectAnchor
        : order.length
          ? order[0]
          : ti;
    const ia = order.indexOf(anchor),
      ib = order.indexOf(ti);
    if (ia < 0 || ib < 0) {
      toggleSelected(ti, true);
      return;
    }
    const lo = Math.min(ia, ib),
      hi = Math.max(ia, ib);
    const range = order.slice(lo, hi + 1);
    if (additive) {
      const seen = {};
      S.selectedTis.forEach(function (t) {
        seen[t] = 1;
      });
      range.forEach(function (t) {
        if (!seen[t]) {
          seen[t] = 1;
          S.selectedTis.push(t);
        }
      });
    } else {
      const rs = {},
        rout = [];
      range.forEach(function (t) {
        if (!rs[t]) {
          rs[t] = 1;
          rout.push(t);
        }
      });
      S.selectedTis = rout;
      S.selectedMarks = [];
      S.lastFocusMark = null;
    }
    S.lastFocusTi = ti;
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(true, ti);
    g.AppHighlight.syncMarkerPanelSelection(false);
    g.AppExport.refreshExportPanel();
  }
  function handlePanelSelect(ev, ti) {
    ti = normTi(ti);
    if (ti == null) return;
    ev = ev || {};
    const additive = !!(ev.ctrlKey || ev.metaKey);
    const ranged = !!ev.shiftKey;
    if (ranged) selectRangeTo(ti, additive);
    else if (additive) toggleSelected(ti, false);
    else setSelected(ti);
    if (ranged) {
      if (ev.preventDefault) {
        try {
          ev.preventDefault();
        } catch {
          /* ignore */
        }
      }
      try {
        const s = document.getSelection
          ? document.getSelection()
          : window.getSelection
            ? window.getSelection()
            : null;
        if (s && s.removeAllRanges) s.removeAllRanges();
      } catch {
        /* ignore */
      }
    }
  }
  function normMark(key) {
    if (key == null) return null;
    return String(key);
  }
  function getMarkSelection() {
    return g.AppState.selectedMarks.slice();
  }
  function setSelectedMark(key) {
    const S = g.AppState;
    key = normMark(key);
    S.selectedMarks = key == null ? [] : [key];
    S.selectedTis = [];
    S.selectAnchor = key;
    S.lastFocusTi = null;
    S.lastFocusMark = key;
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(false);
    g.AppHighlight.syncMarkerPanelSelection(true, key);
    g.AppExport.refreshExportPanel();
  }
  function toggleSelectedMark(key, scroll) {
    const S = g.AppState;
    key = normMark(key);
    if (key == null) return;
    const ix = S.selectedMarks.indexOf(key);
    if (ix >= 0) S.selectedMarks.splice(ix, 1);
    else S.selectedMarks.push(key);
    S.selectAnchor = key;
    S.lastFocusMark = key;
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(false);
    g.AppHighlight.syncMarkerPanelSelection(!!scroll, key);
    g.AppExport.refreshExportPanel();
  }
  function markPanelOrder(key) {
    try {
      const row = document.querySelector('.filerow[data-mark="' + key + '"]');
      const panel = row && row.closest ? row.closest(".panel") : null;
      const rows = (panel || document).querySelectorAll(".filerow[data-mark]");
      if (rows && rows.length) {
        const out = [];
        rows.forEach(function (r) {
          out.push(r.getAttribute("data-mark"));
        });
        if (out.length) return out;
      }
    } catch {
      /* ignore */
    }
    return [key];
  }
  function selectMarkRangeTo(key, additive) {
    const S = g.AppState;
    key = normMark(key);
    if (key == null) return;
    const order = markPanelOrder(key);
    const anchor =
      S.selectAnchor != null && order.indexOf(S.selectAnchor) >= 0
        ? S.selectAnchor
        : order.length
          ? order[0]
          : key;
    const ia = order.indexOf(anchor),
      ib = order.indexOf(key);
    if (ia < 0 || ib < 0) {
      toggleSelectedMark(key, true);
      return;
    }
    const range = order.slice(Math.min(ia, ib), Math.max(ia, ib) + 1);
    if (additive) {
      const seen = {};
      S.selectedMarks.forEach(function (k) {
        seen[k] = 1;
      });
      range.forEach(function (k) {
        if (!seen[k]) {
          seen[k] = 1;
          S.selectedMarks.push(k);
        }
      });
    } else {
      const rs = {},
        rout = [];
      range.forEach(function (k) {
        if (!rs[k]) {
          rs[k] = 1;
          rout.push(k);
        }
      });
      S.selectedMarks = rout;
      S.selectedTis = [];
    }
    S.selectAnchor = key;
    S.lastFocusMark = key;
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(false);
    g.AppHighlight.syncMarkerPanelSelection(true, key);
    g.AppExport.refreshExportPanel();
  }
  function handleMarkSelect(ev, key) {
    key = normMark(key);
    if (key == null) return;
    ev = ev || {};
    const additive = !!(ev.ctrlKey || ev.metaKey);
    const ranged = !!ev.shiftKey;
    if (ranged) selectMarkRangeTo(key, additive);
    else if (additive) toggleSelectedMark(key, false);
    else setSelectedMark(key);
    if (ranged) {
      if (ev.preventDefault) {
        try {
          ev.preventDefault();
        } catch {
          /* ignore */
        }
      }
      try {
        const s = document.getSelection
          ? document.getSelection()
          : window.getSelection
            ? window.getSelection()
            : null;
        if (s && s.removeAllRanges) s.removeAllRanges();
      } catch {
        /* ignore */
      }
    }
  }
  g.AppSelection = {
    normTi: normTi,
    getSelection: getSelection,
    setSelected: setSelected,
    setSelectedList: setSelectedList,
    toggleSelected: toggleSelected,
    clearSelection: clearSelection,
    selectRangeTo: selectRangeTo,
    handlePanelSelect: handlePanelSelect,
    getMarkSelection: getMarkSelection,
    setSelectedMark: setSelectedMark,
    toggleSelectedMark: toggleSelectedMark,
    selectMarkRangeTo: selectMarkRangeTo,
    handleMarkSelect: handleMarkSelect,
  };
})(window);
