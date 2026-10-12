(function (g) {
  "use strict";
  function pickMeshes() {
    const S = g.AppState;
    const arr = [];
    for (let i = 0; i < S.modelEntries.length; i++) {
      if (S.modelEntries[i].mesh.visible) arr.push(S.modelEntries[i].mesh);
    }
    return arr;
  }
  function setPtr(ev) {
    const S = g.AppState;
    const rect = S.renderer.domElement.getBoundingClientRect();
    if (rect.width < 1 || rect.height < 1) return false;
    S.pickPtr.x = ((ev.clientX - rect.left) / rect.width) * 2 - 1;
    S.pickPtr.y = -((ev.clientY - rect.top) / rect.height) * 2 + 1;
    S.raycaster.setFromCamera(S.pickPtr, S.camera);
    return true;
  }
  function pickTi(ev) {
    const S = g.AppState;
    if (!S.root || !S.modelEntries.length || !S.raycaster) return null;
    if (!setPtr(ev)) return null;
    const hits = S.raycaster.intersectObjects(pickMeshes(), false);
    if (!hits.length) return null;
    const ud = hits[0].object.userData.entry;
    return ud && ud.ti != null ? ud.ti : null;
  }
  function chainVisible(o) {
    while (o) {
      if (!o.visible) return false;
      o = o.parent;
    }
    return true;
  }
  function markKey(ud) {
    if (!ud || !ud.pick || ud.pick.kind == null || ud.pick.a == null) {
      return null;
    }
    return ud.pick.b != null
      ? ud.pick.kind + ":" + ud.pick.a + ":" + ud.pick.b
      : ud.pick.kind + ":" + ud.pick.a;
  }
  function pickKeyFromObject(o) {
    while (o) {
      const k = markKey(o.userData);
      if (k) return k;
      o = o.parent;
    }
    return null;
  }
  function pickBoth(ev) {
    const S = g.AppState;
    const out = { ti: null, mark: null };
    if (!S.root || !S.raycaster) return out;
    if (!setPtr(ev)) return out;
    let modelDist = Infinity;
    if (S.modelEntries.length) {
      try {
        const mh = S.raycaster.intersectObjects(pickMeshes(), false);
        if (mh.length && mh[0].object.userData.entry) {
          const ti = mh[0].object.userData.entry.ti;
          if (ti != null) {
            out.ti = ti;
            modelDist = mh[0].distance;
          }
        }
      } catch {
        /* ignore */
      }
    }
    let markDist = Infinity,
      mkey = null;
    if (
      S.opts.cameras ||
      S.opts.lights ||
      S.opts.players ||
      S.opts.boundary ||
      S.opts.aot ||
      S.opts.trigger ||
      S.opts.route
    ) {
      try {
        const targets = [];
        S.root.traverse(function (o) {
          if (o.userData && o.userData.pick && o.visible && chainVisible(o)) {
            targets.push(o);
          }
        });
        const kh = S.raycaster.intersectObjects(targets, true);
        for (let i = 0; i < kh.length; i++) {
          const k = pickKeyFromObject(kh[i].object);
          if (k) {
            mkey = k;
            markDist = kh[i].distance;
            break;
          }
        }
      } catch {
        /* ignore */
      }
    }
    if (mkey != null && markDist <= modelDist) {
      out.ti = null;
      out.mark = mkey;
    }
    return out;
  }
  function openDetailForHit(hit) {
    if (!hit) return;
    if (hit.mark != null) {
      const parts = String(hit.mark).split(":");
      const kind = parts[0];
      const a = parts.length > 1 ? +parts[1] : NaN;
      const b = parts.length > 2 ? +parts[2] : NaN;
      try {
        if (kind === "camera") {
          if (!isNaN(a) && !isNaN(b)) g.AppCameraDetail.openCameraDetail(a, b);
        } else if (kind === "light") {
          if (!isNaN(a)) g.AppLightDetail.openLightDetail(a);
        } else if (kind === "player") {
          if (!isNaN(a)) g.AppPlayerDetail.openPlayerDetail(a);
        } else if (
          kind === "boundary" ||
          kind === "aot" ||
          kind === "trigger" ||
          kind === "route"
        ) {
          if (!isNaN(a)) g.AppZoneDetail.openZoneDetail(kind, a);
        }
      } catch {
        /* ignore */
      }
      return;
    }
    if (hit.ti != null) {
      try {
        g.AppDetail.openModelDetail(hit.ti);
      } catch {
        /* ignore */
      }
    }
  }
  function initPicking() {
    const S = g.AppState;
    S.raycaster = new THREE.Raycaster();
    S.pickPtr = new THREE.Vector2();
    S.selGroup = new THREE.Group();
    S.selGroup.name = "selection";
    S.hovHelper = new THREE.Box3Helper(new THREE.Box3(), 0xffe066);
    S.hovHelper.visible = false;
    try {
      S.hovHelper.material.depthTest = false;
      S.hovHelper.renderOrder = 998;
    } catch {
      /* ignore */
    }
    S.scene.add(S.selGroup);
    S.scene.add(S.hovHelper);
    const el = S.renderer.domElement;
    let rDownX = 0,
      rDownY = 0,
      rDown = false;
    el.addEventListener("pointerdown", function (e) {
      if (e.button === 0) {
        S.downX = e.clientX;
        S.downY = e.clientY;
      } else if (e.button === 2) {
        rDownX = e.clientX;
        rDownY = e.clientY;
        rDown = true;
      }
    });
    el.addEventListener("pointerup", function (e) {
      if (S.viewMode !== "orbit") return;
      if (e.button !== 0) return;
      const dx = e.clientX - S.downX,
        dy = e.clientY - S.downY;
      if (dx * dx + dy * dy > 36) return;
      const hit = pickBoth(e);
      const additive = !!(e.ctrlKey || e.metaKey);
      if (hit.mark != null) {
        if (additive) g.AppSelection.toggleSelectedMark(hit.mark, true);
        else g.AppSelection.setSelectedMark(hit.mark);
      } else if (hit.ti == null) {
        if (!additive) g.AppSelection.clearSelection(true);
      } else if (additive) g.AppSelection.toggleSelected(hit.ti, true);
      else g.AppSelection.setSelected(hit.ti);
    });
    el.addEventListener("dblclick", function (e) {
      if (S.viewMode !== "orbit") return;
      if (!S.R()) return;
      const hit = pickBoth(e);
      if (hit.mark != null) {
        if (S.selectedMarks.indexOf(hit.mark) < 0) {
          g.AppSelection.setSelectedMark(hit.mark);
        }
        openDetailForHit(hit);
      } else if (hit.ti != null) {
        if (S.selectedTis.indexOf(hit.ti) < 0) {
          g.AppSelection.setSelected(hit.ti);
        }
        openDetailForHit(hit);
      }
    });
    el.addEventListener("pointermove", function (e) {
      S.lastHoverEv = e;
      if (S.hoverQueued) return;
      S.hoverQueued = true;
      requestAnimationFrame(function () {
        S.hoverQueued = false;
        if (S.viewMode !== "orbit") {
          if (S.hoveredTi != null || S.hoveredMark != null) {
            g.AppHighlight.setHover(null, null);
          }
          return;
        }
        const ev = S.lastHoverEv;
        S.lastHoverEv = null;
        if (!ev) return;
        if (ev.buttons !== 0) {
          if (S.hoveredTi != null || S.hoveredMark != null) {
            g.AppHighlight.setHover(null, null);
          }
          return;
        }
        const r = pickBoth(ev);
        g.AppHighlight.setHover(r.ti, r.mark);
      });
    });
    el.addEventListener("pointerleave", function () {
      g.AppHighlight.setHover(null, null);
      rDown = false;
    });
    el.addEventListener("contextmenu", function (e) {
      if (e.preventDefault) {
        try {
          e.preventDefault();
        } catch {
          /* ignore */
        }
      }
      const wasDown = rDown;
      rDown = false;
      if (S.viewMode !== "orbit") return false;
      if (g.Panels.anyDialogOpen && g.Panels.anyDialogOpen()) return false;
      if (!wasDown) return false;
      const cx = e && e.clientX != null ? e.clientX : 0,
        cy = e && e.clientY != null ? e.clientY : 0;
      const dx = cx - rDownX,
        dy = cy - rDownY;
      if (dx * dx + dy * dy > 36) return false;
      if (!S.R()) return false;
      const ti = pickTi(e);
      if (ti != null && S.selectedTis.indexOf(ti) < 0) {
        g.AppSelection.setSelected(ti);
      }
      g.AppMenu.showViewContextMenu(cx, cy, ti);
      return false;
    });
    document.addEventListener("keydown", function (e) {
      if (e.key !== "Escape") return;
      if (g.Panels.anyDialogOpen && g.Panels.anyDialogOpen()) return;
      if (S.selectedTis.length) g.AppSelection.clearSelection(true);
    });
  }
  g.AppPicking = {
    initPicking: initPicking,
  };
})(window);
