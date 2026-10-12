(function (g) {
  "use strict";
  const SETKEY = "rdxweb.settings";
  const TREEKEY = "rdxweb.modelsTree";
  const EXPKEY = "rdxweb.export";
  function loadSettings() {
    try {
      const s = JSON.parse(localStorage.getItem(SETKEY) || "{}");
      Object.keys(g.AppState.opts).forEach(function (k) {
        if (k === "fpsSpeed" || k === "viewMode") return;
        if (s[k] !== undefined) g.AppState.opts[k] = !!s[k];
      });
      if (s.fpsSpeed !== undefined) {
        let v = parseFloat(s.fpsSpeed);
        if (isFinite(v)) {
          if (v < 10) v = 10;
          if (v > 20000) v = 20000;
          g.AppState.opts.fpsSpeed = v;
        }
      }
      if (s.route === undefined && s.event !== undefined) {
        g.AppState.opts.route = !!s.event;
      }
      if (s.viewMode === "orbit" || s.viewMode === "fps") {
        g.AppState.opts.viewMode = s.viewMode;
        g.AppState.viewMode = s.viewMode;
      }
    } catch {
      /* ignore */
    }
  }
  function saveSettings() {
    try {
      localStorage.setItem(SETKEY, JSON.stringify(g.AppState.opts));
    } catch {
      /* ignore */
    }
  }
  function loadCollapsed() {
    try {
      return JSON.parse(localStorage.getItem(TREEKEY) || "[]");
    } catch {
      return [];
    }
  }
  function saveCollapsed(kind, isCol) {
    try {
      let c = loadCollapsed();
      if (isCol) {
        if (c.indexOf(kind) < 0) c.push(kind);
      } else {
        c = c.filter(function (k) {
          return k !== kind;
        });
      }
      localStorage.setItem(TREEKEY, JSON.stringify(c));
    } catch {
      /* ignore */
    }
  }
  function loadExportOpts() {
    const d = { selectedOnly: false, textures: true };
    try {
      const s = JSON.parse(localStorage.getItem(EXPKEY) || "{}");
      if (s.selectedOnly !== undefined) d.selectedOnly = !!s.selectedOnly;
      if (s.textures !== undefined) d.textures = !!s.textures;
    } catch {
      /* ignore */
    }
    return d;
  }
  function saveExportOpts(o) {
    try {
      localStorage.setItem(
        EXPKEY,
        JSON.stringify({
          selectedOnly: !!o.selectedOnly,
          textures: !!o.textures,
        }),
      );
    } catch {
      /* ignore */
    }
  }
  g.AppSetting = {
    saveSettings: saveSettings,
    loadCollapsed: loadCollapsed,
    saveCollapsed: saveCollapsed,
    loadExportOpts: loadExportOpts,
    saveExportOpts: saveExportOpts,
  };
  loadSettings();
})(window);
