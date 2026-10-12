(function (g) {
  "use strict";
  function register() {
    g.Panels.register(
      "help",
      "Help",
      function (body) {
        body.innerHTML =
          '<section class="help-section"><h3>Orbit</h3><dl><dt>Middle-drag</dt><dd>Orbit</dd><dt>Shift + middle-drag</dt><dd>Pan</dd><dt>Right-drag</dt><dd>Pan</dd><dt>Wheel</dt><dd>Zoom</dd><dt>Left-click</dt><dd>Select; Ctrl-click adds</dd><dt>Double-click</dt><dd>Open details</dd><dt>Right-click</dt><dd>Open menu</dd><dt>Hover / Esc</dt><dd>Highlight / clear selection</dd></dl></section>' +
          '<section class="help-section"><h3>FPV</h3><dl><dt>WASD</dt><dd>Move</dd><dt>Space / Ctrl or C</dt><dd>Up / down</dd><dt>Shift</dt><dd>Move faster</dd><dt>Wheel</dt><dd>Adjust speed</dd></dl></section>';
      },
      { x: 10, y: 10, visible: false, nomenu: true },
    );
  }
  g.AppHelpPanel = { register: register };
})(window);
