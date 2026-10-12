(function (g) {
  "use strict";
  function clearRoot() {
    const S = g.AppState;
    if (S.root) {
      S.scene.remove(S.root);
      S.root.traverse(function (o) {
        if (o.geometry) o.geometry.dispose();
      });
      S.root = null;
    }
    S.modelEntries = [];
    S.lightEntries = [];
    if (S.selGroup) g.AppHighlight.clearSelGroup();
    if (S.hovHelper) S.hovHelper.visible = false;
  }
  function show(name, refitCamera) {
    const S = g.AppState;
    if (refitCamera === undefined) refitCamera = true;
    g.AppSetting.saveSettings();
    const prevSels = S.selectedTis.slice();
    clearRoot();
    if (!name || !S.files[name]) {
      S.selectedTis = [];
      S.selectAnchor = null;
      S.lastFocusTi = null;
      S.hoveredTi = null;
      S.hoveredMark = null;
      if (S.simpleRig) S.simpleRig.visible = !S.opts.gameLights;
      g.AppHighlight.updatePickHelpers();
      return;
    }
    const Rr = S.files[name];
    S.root = new THREE.Group();
    S.root.name = name;
    S.scene.add(S.root);
    if (S.simpleRig) S.simpleRig.visible = !S.opts.gameLights;
    const built = g.RDXScene.buildGeometries(Rr),
      matCache = {};
    g.AppModelBuilder.build(S.root, Rr, built, matCache);
    g.AppMarkerBuilder.buildCamera(S.root, Rr);
    g.AppLightBuilder.build(S.root, Rr);
    g.AppMarkerBuilder.buildPlayer(S.root, Rr);
    g.AppMarkerBuilder.buildZone(S.root, Rr);
    if (refitCamera) g.AppViewport.refit();
    if (prevSels.length) {
      const alive = {};
      S.modelEntries.forEach(function (e) {
        alive[e.ti] = true;
      });
      S.selectedTis = prevSels.filter(function (ti) {
        return alive[ti];
      });
      if (S.selectAnchor != null && !alive[S.selectAnchor]) {
        S.selectAnchor = S.selectedTis.length
          ? S.selectedTis[S.selectedTis.length - 1]
          : null;
      }
      if (S.lastFocusTi != null && !alive[S.lastFocusTi]) {
        S.lastFocusTi = S.selectedTis.length
          ? S.selectedTis[S.selectedTis.length - 1]
          : null;
      }
    }
    if (S.hoveredTi != null) {
      const stillH = S.modelEntries.some(function (e) {
        return e.ti === S.hoveredTi;
      });
      if (!stillH) S.hoveredTi = null;
    }
    g.AppHighlight.updatePickHelpers();
    g.AppHighlight.syncModelsPanelSelection(false);
    g.AppHighlight.syncMarkerPanelSelection(false);
    g.AppDetail.refreshDetailPanels();
  }
  g.AppBuilder = { clearRoot: clearRoot, show: show };
})(window);
