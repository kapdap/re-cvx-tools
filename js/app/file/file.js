(function (g) {
  "use strict";
  function loadFile(f) {
    const S = g.AppState;
    const r = new FileReader();
    r.onload = function () {
      if (S.order.indexOf(f.name) < 0) return;
      try {
        const Rr = g.RDX.parseRdx(new Uint8Array(r.result));
        Rr._uid = S.uidSeq++;
        Rr._name = f.name;
        S.files[f.name] = Rr;
        setActive(f.name);
      } catch {
        /* ignore */
      }
    };
    r.readAsArrayBuffer(f);
  }
  function addFiles(list) {
    const S = g.AppState;
    if (!list.length) return;
    if (!S.handles) S.handles = {};
    for (let i = 0; i < list.length; i++) {
      S.handles[list[i].name] = list[i];
      if (S.order.indexOf(list[i].name) < 0) S.order.push(list[i].name);
    }
    loadFile(S.handles[list[0].name]);
  }
  function removeFile(n) {
    const S = g.AppState;
    const idx = S.order.indexOf(n);
    delete S.files[n];
    if (S.handles) delete S.handles[n];
    S.order = S.order.filter(function (a) {
      return a !== n;
    });
    if (S.active === n) {
      S.active = null;
      S.selectedTis = [];
      S.selectedMarks = [];
      S.selectAnchor = null;
      S.lastFocusTi = null;
      S.lastFocusMark = null;
      S.hoveredTi = null;
      S.hoveredMark = null;
      g.AppDetail.closeAllDetails();
      g.AppBuilder.clearRoot();
      g.AppHighlight.updatePickHelpers();
      if (S.order.length) {
        setActive(S.order[idx] || S.order[idx - 1] || S.order[0]);
        return;
      }
      g.Panels.refresh();
    } else g.Panels.refresh("files");
  }
  function setActive(n) {
    const S = g.AppState;
    if (!S.files[n] && S.handles && S.handles[n]) {
      loadFile(S.handles[n]);
      return;
    }
    S.active = n;
    S.tableVis = {};
    S.detailVis = {};
    S.lightVis = {};
    S.markVis = {};
    S.selectedTis = [];
    S.selectedMarks = [];
    S.selectAnchor = null;
    S.lastFocusTi = null;
    S.lastFocusMark = null;
    S.hoveredTi = null;
    S.hoveredMark = null;
    g.AppDetail.closeAllDetails();
    g.AppBuilder.show(n, true);
    g.Panels.refresh();
  }
  g.AppFile = {
    addFiles: addFiles,
    removeFile: removeFile,
    setActive: setActive,
  };
})(window);
