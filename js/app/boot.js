(function (g) {
  "use strict";
  function init() {
    g.AppViewport.initThree();
    g.AppPicking.initPicking();
    const dz = document.getElementById("drop");
    ["dragover", "drop"].forEach(function (ev) {
      dz.addEventListener(ev, function (e) {
        e.preventDefault();
      });
    });
    dz.addEventListener("drop", function (e) {
      if (e.dataTransfer && e.dataTransfer.files) {
        g.AppFile.addFiles(e.dataTransfer.files);
      }
    });
    g.AppToolbar.initToolbar();
    g.Panels.init("panels", "panelsmenu");
    g.AppPanel.registerPanels();
    g.Panels.build();
    g.AppViewport.setViewMode(
      g.AppState.opts.viewMode === "fps" ? "fps" : "orbit",
    );
  }
  window.addEventListener("load", init);
  window.RDXApp = {
    show: function (n, r) {
      return g.AppBuilder.show(n, r);
    },
    setViewMode: function (m) {
      return g.AppViewport.setViewMode(m);
    },
    gotoCamera: function (a, b) {
      return g.AppNavigation.gotoCamera(a, b);
    },
    gotoTable: function (t) {
      return g.AppNavigation.gotoTable(t);
    },
    gotoLight: function (i) {
      return g.AppNavigation.gotoLight(i);
    },
    gotoPlayer: function (i) {
      return g.AppNavigation.gotoPlayer(i);
    },
    gotoZone: function (kind, i) {
      return g.AppNavigation.gotoZone(kind, i);
    },
    gotoNested: function (t, b, n) {
      return g.AppNavigation.gotoNested(t, b, n);
    },
    openModelDetail: function (t) {
      return g.AppDetail.openModelDetail(t);
    },
    setNestedVisible: function (t, b, n, v) {
      return g.AppDetail.setNestedVisible(t, b, n, v);
    },
    setTableVisible: function (t, v) {
      return g.AppMenu.setTableVisible(t, v);
    },
    showViewContextMenu: function (x, y, ti) {
      return g.AppMenu.showViewContextMenu(x, y, ti);
    },
    openExportDialog: function (pre) {
      return g.AppExport.openExportDialog(pre);
    },
    get opts() {
      return g.AppState.opts;
    },
    selectTable: function (ti, ev) {
      if (ev && ev.shiftKey) {
        g.AppSelection.selectRangeTo(ti, !!(ev.ctrlKey || ev.metaKey));
      } else if (ev && (ev.ctrlKey || ev.metaKey)) {
        g.AppSelection.toggleSelected(ti, true);
      } else g.AppSelection.setSelected(ti);
    },
    selectTables: function (list) {
      g.AppSelection.setSelectedList(list);
    },
    toggleTable: function (ti) {
      g.AppSelection.toggleSelected(ti, true);
    },
    selectMark: function (key, ev) {
      if (ev && ev.shiftKey) {
        g.AppSelection.selectMarkRangeTo(key, !!(ev.ctrlKey || ev.metaKey));
      } else if (ev && (ev.ctrlKey || ev.metaKey)) {
        g.AppSelection.toggleSelectedMark(key, true);
      } else g.AppSelection.setSelectedMark(key);
    },
    toggleMark: function (key) {
      g.AppSelection.toggleSelectedMark(key, true);
    },
    getMarkSelection: function () {
      return g.AppSelection.getMarkSelection();
    },
    clearSelection: function () {
      g.AppSelection.clearSelection(true);
    },
    getSelected: function () {
      return g.AppState.selectedTis.length ? g.AppState.selectedTis[0] : null;
    },
    getSelection: function () {
      return g.AppSelection.getSelection();
    },
    getHovered: function () {
      return g.AppState.hoveredTi;
    },
    getExportItems: function (s) {
      return g.AppExport.getExportItems(s);
    },
    getExportInfo: function () {
      return g.AppExport.getExportInfo();
    },
    exportGltf: function (a, b, c, d) {
      return g.AppExport.doExportGltf(a, b, c, d);
    },
    getSupportedTextureFormats: function () {
      return g.AppTextureExport.getSupportedFormats();
    },
    exportTextures: function (f) {
      return g.AppTextureExport.exportTextures(f);
    },
  };
})(window);
