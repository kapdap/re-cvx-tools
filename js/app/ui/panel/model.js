(function (g) {
  "use strict";
  function register() {
    g.Panels.register(
      "models",
      "Models",
      function (body) {
        const S = g.AppState;
        const Rr = S.R();
        if (!Rr) {
          body.textContent = "No file loaded";
          return;
        }
        const groups = { room: [], actor: [], obj: [], item: [] };
        Object.keys(Rr.map || {}).forEach(function (ti) {
          const kind = Rr.map[ti][0];
          (groups[kind] = groups[kind] || []).push(+ti);
        });
        const collapsed = g.AppSetting.loadCollapsed();
        Object.keys(groups).forEach(function (kind) {
          if (!groups[kind].length) return;
          const isCol = collapsed.indexOf(kind) >= 0;
          const det = document.createElement("details");
          det.className = "treegroup";
          if (!isCol) det.open = true;
          det.ontoggle = function () {
            g.AppSetting.saveCollapsed(kind, !det.open);
          };
          const head = document.createElement("summary");
          head.className = "filerow treehead";
          const lab = document.createElement("span");
          lab.className = "grow truncate";
          lab.textContent =
            kind.toUpperCase() + " (" + groups[kind].length + ")";
          lab.style.fontWeight = "bold";
          head.appendChild(lab);
          if (S.opts[kind] !== undefined) {
            const secEye = document.createElement("button");
            secEye.className = "icon-btn";
            secEye.textContent = "👁";
            secEye.title = "toggle " + kind + " layer";
            if (!S.opts[kind]) secEye.classList.add("off");
            secEye.onclick = (function (k) {
              return function (e) {
                if (e) {
                  if (e.preventDefault) {
                    try {
                      e.preventDefault();
                    } catch {
                      /* ignore */
                    }
                  }
                  if (e.stopPropagation) {
                    try {
                      e.stopPropagation();
                    } catch {
                      /* ignore */
                    }
                  }
                }
                g.AppMenu.setLayerVisible(k, !S.opts[k]);
                try {
                  g.Panels.refresh("models");
                } catch {
                  /* ignore */
                }
                return false;
              };
            })(kind);
            head.appendChild(secEye);
          }
          det.appendChild(head);
          const kids = document.createElement("div");
          kids.className = "treekids";
          groups[kind].forEach(function (ti) {
            const row = document.createElement("div");
            row.className = "filerow treekid";
            row.setAttribute("data-ti", ti);
            if (S.selectedTis.indexOf(ti) >= 0) row.classList.add("selected");
            if (ti === S.hoveredTi) row.classList.add("hovered");
            const lab2 = document.createElement("span");
            lab2.className = "grow truncate";
            lab2.textContent = "MDL_" + ("00" + ti).slice(-3);
            lab2.title = "select model (ctrl+click add, shift+click range)";
            row.onclick = function (e) {
              g.AppSelection.handlePanelSelect(e || window.event, ti);
            };
            row.onmousedown = function (e) {
              e = e || window.event;
              if (e && e.shiftKey && e.preventDefault) {
                try {
                  e.preventDefault();
                } catch {
                  /* ignore */
                }
              }
            };
            row.onmouseenter = function () {
              g.AppHighlight.setHovered(ti);
            };
            row.onmouseleave = function () {
              g.AppHighlight.setHovered(null);
            };
            row.ondblclick = (function (t) {
              return function () {
                g.AppDetail.openModelDetail(t);
              };
            })(ti);
            row.oncontextmenu = function (e) {
              e = e || window.event;
              if (e.preventDefault) {
                try {
                  e.preventDefault();
                } catch {
                  /* ignore */
                }
              }
              if (e.stopPropagation) {
                try {
                  e.stopPropagation();
                } catch {
                  /* ignore */
                }
              }
              if (S.selectedTis.indexOf(ti) < 0) g.AppSelection.setSelected(ti);
              const cx = e && e.clientX != null ? e.clientX : 0,
                cy = e && e.clientY != null ? e.clientY : 0;
              g.AppMenu.showModelsContextMenu(cx, cy, ti);
              return false;
            };
            const eye = document.createElement("button");
            eye.className = "icon-btn";
            eye.textContent = "👁";
            eye.title = "toggle visibility";
            if (S.tableVis[ti] === false) {
              eye.classList.add("off");
            }
            eye.onclick = function (e) {
              if (e) e.stopPropagation();
              g.AppMenu.setTableVisible(ti, !(S.tableVis[ti] !== false));
            };
            const go = document.createElement("button");
            go.className = "icon-btn";
            go.textContent = "◎";
            go.title = "center camera on model";
            go.onclick = function (e) {
              if (e) e.stopPropagation();
              g.AppSelection.setSelected(ti);
              g.AppNavigation.gotoTable(ti);
            };
            const det2 = document.createElement("button");
            det2.className = "icon-btn";
            det2.textContent = "ⓘ";
            det2.title = "open model details";
            det2.onclick = function (e) {
              if (e) e.stopPropagation();
              g.AppDetail.openModelDetail(ti, true);
            };
            row.appendChild(lab2);
            row.appendChild(det2);
            row.appendChild(eye);
            row.appendChild(go);
            kids.appendChild(row);
          });
          det.appendChild(kids);
          body.appendChild(det);
        });
      },
      { x: 260, y: 10 },
    );
  }
  g.AppModelPanel = { register: register };
})(window);
