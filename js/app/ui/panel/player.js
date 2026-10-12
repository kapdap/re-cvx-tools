(function (g) {
  "use strict";
  function register() {
    g.Panels.register(
      "players",
      "Players",
      function (body) {
        const S = g.AppState;
        const Rr = S.R();
        if (!Rr) {
          body.textContent = "No file loaded";
          return;
        }
        const list = Rr.players || Rr.player || [];
        if (!list.length) {
          body.textContent = "No players";
          return;
        }
        list.forEach(function (P, i) {
          const key = "player:" + i;
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
              g.AppMenu.showPlayersContextMenu(cx, cy, idx, k);
              return false;
            };
          })(key, i);
          const s = document.createElement("span");
          s.className = "grow truncate";
          s.textContent = "PLY_" + ("00" + i).slice(-3);
          row.ondblclick = (function (idx) {
            return function () {
              g.AppPlayerDetail.openPlayerDetail(idx);
            };
          })(i);
          const go = document.createElement("button");
          go.className = "icon-btn";
          go.textContent = "◎";
          go.title = "center camera on player spawn";
          go.onclick = function (e) {
            if (e) e.stopPropagation();
            g.AppNavigation.gotoPlayer(i);
          };
          row.appendChild(s);
          g.AppRowInfo.addInfoButton(
            row,
            "open player details",
            (function (idx) {
              return function () {
                g.AppPlayerDetail.openPlayerDetail(idx, true);
              };
            })(i),
          );
          g.AppRowInfo.addMarkEye(row, key);
          row.appendChild(go);
          body.appendChild(row);
        });
      },
      { x: 10, y: 10, visible: false, layerKey: "players" },
    );
  }
  g.AppPlayerPanel = { register: register };
})(window);
