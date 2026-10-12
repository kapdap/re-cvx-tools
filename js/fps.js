(function (g) {
  "use strict";
  function FPS(camera, dom) {
    this.camera = camera;
    this.dom = dom;
    this.active = false;
    this.locked = false;
    this.yaw = 0;
    this.pitch = 0;
    this.speed = 100;
    try {
      let sv =
        g.AppState && g.AppState.opts
          ? parseFloat(g.AppState.opts.fpsSpeed)
          : NaN;
      if (isFinite(sv)) {
        if (sv < 10) sv = 10;
        if (sv > 20000) sv = 20000;
        this.speed = sv;
      }
    } catch {
      /* ignore */
    }
    this.keys = {};
    const self = this;
    document.addEventListener("pointerlockchange", function () {
      self.locked = document.pointerLockElement === dom;
      const h = document.getElementById("fpshint");
      if (h) h.style.display = self.active && !self.locked ? "block" : "none";
    });
    document.addEventListener("mousemove", function (e) {
      if (!self.active || !self.locked) return;
      self.yaw -= e.movementX * 0.0025;
      self.pitch -= e.movementY * 0.0025;
      if (self.pitch > 1.55) self.pitch = 1.55;
      if (self.pitch < -1.55) self.pitch = -1.55;
      self.applyLook();
    });
    document.addEventListener("keydown", function (e) {
      if (!self.active) return;
      self.keys[e.code] = true;
      if (e.code === "Space") e.preventDefault();
    });
    document.addEventListener("keyup", function (e) {
      self.keys[e.code] = false;
    });
    dom.addEventListener(
      "wheel",
      function (e) {
        if (!self.active) return;
        e.preventDefault();
        self.speed *= e.deltaY > 0 ? 0.9 : 1.12;
        if (self.speed < 10) self.speed = 10;
        if (self.speed > 20000) self.speed = 20000;
        try {
          if (g.AppState && g.AppState.opts) {
            g.AppState.opts.fpsSpeed = self.speed;
          }
          if (g.AppSetting && g.AppSetting.saveSettings) {
            g.AppSetting.saveSettings();
          }
        } catch {
          /* ignore */
        }
        self.updateHint();
      },
      { passive: false },
    );
    dom.addEventListener("click", function () {
      if (self.active && !self.locked) dom.requestPointerLock();
    });
  }
  FPS.prototype.enable = function () {
    this.active = true;
    try {
      let sv =
        g.AppState && g.AppState.opts
          ? parseFloat(g.AppState.opts.fpsSpeed)
          : NaN;
      if (isFinite(sv)) {
        if (sv < 10) sv = 10;
        if (sv > 20000) sv = 20000;
        this.speed = sv;
      }
    } catch {
      /* ignore */
    }
    const d = new THREE.Vector3();
    this.camera.getWorldDirection(d);
    this.yaw = Math.atan2(-d.x, -d.z);
    this.pitch = Math.asin(Math.max(-1, Math.min(1, d.y)));
    this.applyLook();
    const h = document.getElementById("fpshint");
    if (h) {
      h.style.display = "block";
      this.updateHint();
    }
  };
  FPS.prototype.disable = function () {
    this.active = false;
    this.keys = {};
    if (document.pointerLockElement) document.exitPointerLock();
    const h = document.getElementById("fpshint");
    if (h) h.style.display = "none";
  };
  FPS.prototype.applyLook = function () {
    const e = new THREE.Euler(this.pitch, this.yaw, 0, "YXZ");
    this.camera.quaternion.setFromEuler(e);
  };
  FPS.prototype.updateHint = function () {
    const h = document.getElementById("fpshint");
    if (h) {
      h.innerHTML =
        "FPV: click to capture mouse · WASD move · Space up · Ctrl/C down · Shift fast · wheel speed (" +
        Math.round(this.speed) +
        ")";
    }
  };
  FPS.prototype.update = function (dt) {
    if (!this.active) return;
    const k = this.keys,
      fast = k.ShiftLeft || k.ShiftRight ? 4 : 1;
    const v = this.speed * fast * dt;
    const fwd = new THREE.Vector3();
    this.camera.getWorldDirection(fwd);
    const right = new THREE.Vector3()
      .crossVectors(fwd, this.camera.up)
      .normalize();
    const mv = new THREE.Vector3();
    if (k.KeyW) mv.add(fwd);
    if (k.KeyS) mv.sub(fwd);
    if (k.KeyD) mv.add(right);
    if (k.KeyA) mv.sub(right);
    if (k.Space) mv.y += 1;
    if (k.ControlLeft || k.ControlRight || k.KeyC) mv.y -= 1;
    if (mv.lengthSq() > 0) {
      mv.normalize().multiplyScalar(v);
      this.camera.position.add(mv);
    }
  };
  g.FPSControls = FPS;
})(window);
