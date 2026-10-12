(function (g) {
  "use strict";
  function tiBox(ti) {
    const S = g.AppState;
    const box = new THREE.Box3();
    if (ti == null || !S.root) return box;
    try {
      S.root.updateMatrixWorld(true);
    } catch {
      /* ignore */
    }
    S.modelEntries.forEach(function (e) {
      if (e.ti !== ti || !e.mesh.visible) return;
      try {
        box.expandByObject(e.mesh);
      } catch {
        /* ignore */
      }
    });
    return box;
  }
  function tiVisible(ti) {
    const S = g.AppState;
    if (S.tableVis[ti] === false) return false;
    for (let i = 0; i < S.modelEntries.length; i++) {
      if (S.modelEntries[i].ti === ti && S.modelEntries[i].mesh.visible) {
        return true;
      }
    }
    return false;
  }
  function clearSelGroup() {
    const S = g.AppState;
    if (!S.selGroup) return;
    for (let i = S.selGroup.children.length - 1; i >= 0; i--) {
      const h = S.selGroup.children[i];
      S.selGroup.remove(h);
      try {
        if (h.geometry) h.geometry.dispose();
      } catch {
        /* ignore */
      }
      try {
        if (h.material) h.material.dispose();
      } catch {
        /* ignore */
      }
    }
  }
  function updatePickHelpers() {
    const S = g.AppState;
    if (!S.selGroup || !S.hovHelper) return;
    clearSelGroup();
    S.selectedTis.forEach(function (ti) {
      if (!tiVisible(ti)) return;
      const b = tiBox(ti);
      if (b.isEmpty()) return;
      let h = null;
      try {
        h = new THREE.Box3Helper(b.clone(), 0xff7e00);
        h.material.depthTest = false;
        h.renderOrder = 999;
      } catch {
        return;
      }
      S.selGroup.add(h);
    });
    S.selectedMarks.forEach(function (key) {
      if (!markVisible(key)) return;
      const mb2 = markBox(key);
      if (mb2.isEmpty()) return;
      let mh2 = null;
      try {
        mh2 = new THREE.Box3Helper(mb2.clone(), 0xff7e00);
        mh2.material.depthTest = false;
        mh2.renderOrder = 999;
      } catch {
        return;
      }
      S.selGroup.add(mh2);
    });
    if (
      S.hoveredTi != null &&
      S.selectedTis.indexOf(S.hoveredTi) < 0 &&
      tiVisible(S.hoveredTi)
    ) {
      const hb = tiBox(S.hoveredTi);
      if (!hb.isEmpty()) {
        S.hovHelper.box.copy(hb);
        S.hovHelper.visible = true;
        return;
      }
    }
    if (S.hoveredMark != null && markVisible(S.hoveredMark)) {
      const mb = markBox(S.hoveredMark);
      if (!mb.isEmpty()) {
        S.hovHelper.box.copy(mb);
        S.hovHelper.visible = true;
        return;
      }
    }
    S.hovHelper.visible = false;
  }
  function chainVisibleUp(o) {
    while (o) {
      if (!o.visible) return false;
      o = o.parent;
    }
    return true;
  }
  function markObject(key) {
    const S = g.AppState;
    if (!S.root || key == null) return null;
    let found = null;
    try {
      S.root.traverse(function (o) {
        if (found || !o.userData || !o.userData.pick) return;
        const ud = o.userData.pick;
        const k =
          ud.b != null
            ? ud.kind + ":" + ud.a + ":" + ud.b
            : ud.kind + ":" + ud.a;
        if (k === key && o.visible && chainVisibleUp(o)) found = o;
      });
    } catch {
      /* ignore */
    }
    return found;
  }
  function markVisible(key) {
    return !!markObject(key);
  }
  function markBox(key) {
    const S = g.AppState;
    const box = new THREE.Box3();
    const o = markObject(key);
    if (!o) return box;
    try {
      S.root.updateMatrixWorld(true);
    } catch {
      /* ignore */
    }
    try {
      box.expandByObject(o);
    } catch {
      /* ignore */
    }
    return box;
  }
  function setHovered(ti) {
    if (ti != null) ti = +ti;
    if (ti != null && isNaN(ti)) ti = null;
    setHover(ti, null);
  }
  function setHoveredMark(key) {
    if (key != null) key = String(key);
    setHover(null, key);
  }
  function setHover(ti, mark) {
    const S = g.AppState;
    if (ti != null) ti = +ti;
    if (ti != null && isNaN(ti)) ti = null;
    if (mark != null) mark = String(mark);
    if (S.hoveredTi === ti && S.hoveredMark === mark) return;
    S.hoveredTi = ti;
    S.hoveredMark = mark;
    updatePickHelpers();
    syncModelsPanelSelection(false);
    syncMarkerPanelSelection(false);
  }
  function syncModelsPanelSelection(scroll, focusTi) {
    const S = g.AppState;
    try {
      const rows = document.querySelectorAll("#panel-models .filerow[data-ti]");
      if (!rows.length) return;
      rows.forEach(function (r) {
        const ti = +r.getAttribute("data-ti");
        r.classList.toggle("selected", S.selectedTis.indexOf(ti) >= 0);
        r.classList.toggle("hovered", ti === S.hoveredTi);
      });
      const focus = focusTi != null ? focusTi : S.lastFocusTi;
      if (scroll && focus != null) {
        const sel = document.querySelector(
          '#panel-models .filerow[data-ti="' + focus + '"]',
        );
        if (sel) {
          const det = sel.closest ? sel.closest("details") : null;
          if ((!det || det.open) && sel.scrollIntoView) {
            sel.scrollIntoView({ block: "nearest" });
          }
        }
      }
    } catch {
      /* ignore */
    }
  }
  function syncMarkerPanelSelection(scroll, focusKey) {
    const S = g.AppState;
    try {
      const rows = document.querySelectorAll(".filerow[data-mark]");
      if (!rows.length) return;
      rows.forEach(function (r) {
        const key = r.getAttribute("data-mark");
        r.classList.toggle("selected", S.selectedMarks.indexOf(key) >= 0);
        r.classList.toggle("hovered", key === S.hoveredMark);
      });
      const focus = focusKey !== undefined ? focusKey : S.lastFocusMark;
      if (scroll && focus != null) {
        const sel = document.querySelector(
          '.filerow[data-mark="' + focus + '"]',
        );
        if (sel && sel.scrollIntoView) sel.scrollIntoView({ block: "nearest" });
      }
    } catch {
      /* ignore */
    }
  }
  g.AppHighlight = {
    clearSelGroup: clearSelGroup,
    updatePickHelpers: updatePickHelpers,
    setHovered: setHovered,
    setHoveredMark: setHoveredMark,
    setHover: setHover,
    syncModelsPanelSelection: syncModelsPanelSelection,
    syncMarkerPanelSelection: syncMarkerPanelSelection,
  };
})(window);
