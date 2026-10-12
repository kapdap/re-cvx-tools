(function (g) {
  "use strict";
  function register() {
    g.Panels.register(
      "lights",
      "Lights",
      function (body) {
        const S = g.AppState;
        const Rr = S.R();
        if (!Rr) {
          body.textContent = "No file loaded";
          return;
        }
        if (!Rr.lights.length) {
          body.textContent = "No lights";
          return;
        }
        Rr.lights.forEach(function (L, i) {
          const key = "light:" + i;
          const row = document.createElement("div");
          row.className = "filerow";
          row.setAttribute("data-mark", key);
          if (S.selectedMarks.indexOf(key) >= 0) row.classList.add("selected");
          if (S.hoveredMark === key) row.classList.add("hovered");
          row.onclick = (function (k) {
            return function (e) {
              g.AppSelection.handleMarkSelect(e || window.event, k);
            };
          })(key);
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
          row.onmouseenter = (function (k) {
            return function () {
              g.AppHighlight.setHoveredMark(k);
            };
          })(key);
          row.onmouseleave = function () {
            g.AppHighlight.setHoveredMark(null);
          };
          row.oncontextmenu = (function (k, idx) {
            return function (e) {
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
              if (S.selectedMarks.indexOf(k) < 0) {
                g.AppSelection.setSelectedMark(k);
              }
              const cx = e && e.clientX != null ? e.clientX : 0,
                cy = e && e.clientY != null ? e.clientY : 0;
              g.AppMenu.showLightsContextMenu(cx, cy, idx, k);
              return false;
            };
          })(key, i);
          const s = document.createElement("span");
          s.className = "grow truncate";
          s.textContent =
            "LGT_" +
            ("00" + i).slice(-3) +
            " · " +
            (L.lsrc === 2 ? "SUN" : "POINT");
          row.ondblclick = (function (idx) {
            return function () {
              g.AppLightDetail.openLightDetail(idx);
            };
          })(i);
          const eye = document.createElement("button");
          eye.className = "icon-btn";
          eye.textContent = "👁";
          eye.title = "toggle visibility";
          if (S.lightVis[i] === false) {
            eye.classList.add("off");
          }
          eye.onclick = function (e) {
            if (e) e.stopPropagation();
            const v = !(S.lightVis[i] !== false);
            S.lightVis[i] = v;
            eye.classList.toggle("off", !v);
            S.lightEntries.forEach(function (e) {
              if (e.i === i) e.obj.visible = v;
            });
          };
          const go = document.createElement("button");
          go.className = "icon-btn";
          go.textContent = "◎";
          go.title = "center camera on light";
          go.onclick = function (e) {
            if (e) e.stopPropagation();
            g.AppNavigation.gotoLight(i);
          };
          row.appendChild(s);
          g.AppRowInfo.addInfoButton(
            row,
            "open light details",
            (function (idx) {
              return function () {
                g.AppLightDetail.openLightDetail(idx, true);
              };
            })(i),
          );
          row.appendChild(eye);
          row.appendChild(go);
          body.appendChild(row);
        });
      },
      { x: 10, y: 10, visible: false, layerKey: "lights" },
    );
  }
  g.AppLightPanel = { register: register };
})(window);
