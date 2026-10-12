(function (g) {
  "use strict";
  const LAYER_TIPS = {
    room: {
      title: "Room",
      body: "Static stage geometry.",
    },
    actor: {
      title: "Actors",
      body: "Enemies, bosses and NPCs.",
    },
    obj: {
      title: "Objects",
      body: "Interactive props and set pieces.",
    },
    item: {
      title: "Items",
      body: "Collectible pickups.",
    },
    cameras: {
      title: "Cameras",
      body: "Fixed cameras.",
    },
    lights: {
      title: "Lights",
      body: "Point and sun lights.",
    },
    players: {
      title: "Players",
      body: "Player spawn position and facing.",
    },
    boundary: {
      title: "Boundaries",
      body: "Collision boxes.",
    },
    aot: {
      title: "AOT",
      body: "Interactive zones: doors, stairs, items, messages.",
    },
    trigger: {
      title: "Triggers",
      body: "Event zones that fire on entry or sound.",
    },
    route: {
      title: "Routes",
      body: "Enemy pathfinding waypoints.",
    },
    routeLinks: {
      title: "Links",
      body: "Lines showing enemy routes.",
    },
  };
  const LAYER_PANEL = {
    room: "models",
    actor: "models",
    obj: "models",
    item: "models",
    cameras: "cameras",
    lights: "lights",
    players: "players",
    boundary: "boundaries",
    aot: "aot",
    trigger: "triggers",
    route: "routes",
  };
  function register() {
    g.Panels.register(
      "layers",
      "Layers",
      function (body) {
        const S = g.AppState;
        [
          ["room", "Room"],
          ["actor", "Actors"],
          ["obj", "Objects"],
          ["item", "Items"],
          ["cameras", "Cameras"],
          ["lights", "Lights"],
          ["players", "Players"],
          ["boundary", "Boundaries"],
          ["aot", "AOT"],
          ["trigger", "Triggers"],
          ["route", "Routes"],
          ["routeLinks", "Links"],
        ].forEach(function (p) {
          const key = p[0],
            label = p[1];
          const tip = LAYER_TIPS[key];
          const row = document.createElement("div");
          row.className = "filerow";
          try {
            if (g.AppTooltip && tip) {
              g.AppTooltip.attach(row, {
                title: tip.title || label,
                body: tip.body,
              });
            }
          } catch {
            /* ignore */
          }
          row.oncontextmenu = (function (k) {
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
              try {
                if (g.AppTooltip) g.AppTooltip.hide();
              } catch {
                /* ignore */
              }
              const cx = e && e.clientX != null ? e.clientX : 0,
                cy = e && e.clientY != null ? e.clientY : 0;
              g.AppMenu.showLayersContextMenu(cx, cy, k);
              return false;
            };
          })(key);
          row.ondblclick = (function (k) {
            return function () {
              const target = LAYER_PANEL[k];
              if (!target) return;
              try {
                g.Panels.setVisible(target, true);
              } catch {
                /* ignore */
              }
            };
          })(key);
          const lab = document.createElement("span");
          lab.className = "grow truncate";
          lab.textContent = label;
          const eye = document.createElement("button");
          eye.className = "icon-btn";
          eye.textContent = "👁";
          try {
            if (g.AppTooltip) {
              g.AppTooltip.attach(eye, "Toggle " + label + " layer visibility");
            }
          } catch {
            /* ignore */
          }
          if (!S.opts[key]) eye.classList.add("off");
          eye.onclick = (function (k) {
            return function () {
              try {
                if (g.AppTooltip) g.AppTooltip.hide();
              } catch {
                /* ignore */
              }
              g.AppMenu.setLayerVisible(k, !S.opts[k]);
            };
          })(key);
          eye.ondblclick = function (e) {
            e = e || window.event;
            if (e && e.stopPropagation) {
              try {
                e.stopPropagation();
              } catch {
                /* ignore */
              }
            }
          };
          row.appendChild(lab);
          row.appendChild(eye);
          body.appendChild(row);
        });
      },
      { x: 10, y: 10 },
    );
  }
  g.AppLayerPanel = { register: register };
})(window);
