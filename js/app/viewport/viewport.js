(function (g) {
  "use strict";
  function resize() {
    const S = g.AppState;
    if (!S.renderer || !S.camera) return;
    const v = document.getElementById("view");
    const w = (v && v.clientWidth) || 800,
      h = (v && v.clientHeight) || 600;
    S.renderer.setSize(w, h);
    S.camera.aspect = w / h;
    S.camera.updateProjectionMatrix();
  }
  function animate() {
    const S = g.AppState;
    requestAnimationFrame(animate);
    const dt = Math.min(0.1, S.clock.getDelta());
    if (S.viewMode === "fps") S.fps.update(dt);
    else S.controls.update();
    S.renderer.render(S.scene, S.camera);
  }
  function initThree() {
    const S = g.AppState;
    S.scene = new THREE.Scene();
    S.scene.background = new THREE.Color(0x202028);
    S.camera = new THREE.PerspectiveCamera(60, 1, 1, 100000);
    S.camera.position.set(2000, 2000, 2000);
    S.renderer = new THREE.WebGLRenderer({ antialias: true });
    if (THREE.sRGBEncoding !== undefined) {
      S.renderer.outputEncoding = THREE.sRGBEncoding;
    }
    document.getElementById("view").appendChild(S.renderer.domElement);
    S.controls = new THREE.OrbitControls(S.camera, S.renderer.domElement);
    S.controls.mouseButtons = {
      LEFT: -1,
      MIDDLE: THREE.MOUSE.ROTATE,
      RIGHT: THREE.MOUSE.PAN,
    };
    S.fps = new g.FPSControls(S.camera, S.renderer.domElement);
    S.clock = new THREE.Clock();
    S.simpleRig = new THREE.Group();
    S.simpleRig.name = "simple";
    S.simpleRig.add(new THREE.AmbientLight(0xffffff, 0.65));
    const d = new THREE.DirectionalLight(0xffffff, 0.75);
    d.position.set(1, 2, 1);
    S.simpleRig.add(d);
    S.scene.add(S.simpleRig);
    window.addEventListener("resize", resize);
    resize();
    animate();
  }
  function setViewMode(m) {
    const S = g.AppState;
    if (m !== "fps") m = "orbit";
    S.viewMode = m;
    S.opts.viewMode = m;
    try {
      if (g.AppSetting && g.AppSetting.saveSettings) {
        g.AppSetting.saveSettings();
      }
    } catch {
      /* ignore */
    }
    document.getElementById("menu-orbit").textContent =
      (m === "orbit" ? "✓ " : "○ ") + "Orbit";
    document.getElementById("menu-fps").textContent =
      (m === "fps" ? "✓ " : "○ ") + "FPV";
    S.controls.enabled = m === "orbit";
    if (m === "fps") S.fps.enable();
    else S.fps.disable();
  }
  function refit() {
    const S = g.AppState;
    if (!S.root) return;
    const box = new THREE.Box3().setFromObject(S.root);
    if (box.isEmpty()) return;
    const c = box.getCenter(new THREE.Vector3()),
      s = box.getSize(new THREE.Vector3()).length();
    S.controls.target.copy(c);
    S.camera.position.set(c.x + s * 0.5, c.y + s * 0.5, c.z + s * 0.5);
    S.camera.far = Math.max(200000, s * 10);
    S.camera.updateProjectionMatrix();
  }
  function frameBox(box, dir) {
    const S = g.AppState;
    const c = box.getCenter(new THREE.Vector3());
    const size = box.getSize(new THREE.Vector3());
    let radius = size.length() / 2;
    if (!(radius > 0)) radius = 50;
    const vFov = (S.camera.fov * Math.PI) / 180;
    const hFov =
      2 * Math.atan(Math.tan(vFov / 2) * Math.max(0.1, S.camera.aspect || 1));
    const fitFov = Math.min(vFov, hFov);
    let dist = (radius / Math.tan(fitFov / 2)) * 0.26;
    dist = Math.max(dist, radius * 0.35, 12);
    S.controls.target.copy(c);
    S.camera.position.copy(c.clone().add(dir.multiplyScalar(dist)));
    S.camera.near = Math.max(1, dist / 1000);
    S.camera.far = Math.max(S.camera.far, dist + radius * 10);
    S.camera.updateProjectionMatrix();
  }
  g.AppViewport = {
    initThree: initThree,
    setViewMode: setViewMode,
    refit: refit,
    frameBox: frameBox,
  };
})(window);
