(function (g) {
  "use strict";
  function register() {
    g.Panels.register(
      "cameras",
      "Cameras",
      function (body) {
        const S = g.AppState;
        const Rr = S.R();
        if (!Rr) {
          body.textContent = "No file loaded";
          return;
        }
        if (!Rr.cameras.length) {
          body.textContent = "No cameras";
          return;
        }
        Rr.cameras.forEach(function (c, i) {
          c.blocks.forEach(function (b, j) {
            const key = "camera:" + i + ":" + j;
            const row = document.createElement("div");
            row.className = "filerow";
            row.setAttribute("data-mark", key);
            if (S.selectedMarks.indexOf(key) >= 0) {
              row.classList.add("selected");
            }
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
            row.oncontextmenu = (function (k, ci, bi) {
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
                g.AppMenu.showCamerasContextMenu(cx, cy, ci, bi, k);
                return false;
              };
            })(key, i, j);
            const s = document.createElement("span");
            s.className = "grow truncate";
            s.textContent = "CAM_" + ("00" + i).slice(-3) + "_" + j;
            row.ondblclick = (function (ci, bi) {
              return function () {
                g.AppCameraDetail.openCameraDetail(ci, bi);
              };
            })(i, j);
            const go = document.createElement("button");
            go.className = "icon-btn";
            go.textContent = "◎";
            go.title = "jump camera here (framing " + j + ")";
            go.onclick = function (e) {
              if (e) e.stopPropagation();
              g.AppViewport.setViewMode(S.viewMode);
              g.AppNavigation.gotoCamera(i, j);
            };
            row.appendChild(s);
            g.AppRowInfo.addInfoButton(
              row,
              "open camera details",
              (function (ci, bi) {
                return function () {
                  g.AppCameraDetail.openCameraDetail(ci, bi, true);
                };
              })(i, j),
            );
            g.AppRowInfo.addMarkEye(row, key);
            row.appendChild(go);
            body.appendChild(row);
          });
        });
      },
      { x: 10, y: 10, visible: false, layerKey: "cameras" },
    );
  }
  g.AppCameraPanel = { register: register };
})(window);
