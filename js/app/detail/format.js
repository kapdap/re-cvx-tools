(function (g) {
  "use strict";
  function fmtFloat(a) {
    return (
      "[" +
      a
        .map(function (v) {
          return Math.round(v * 10) / 10;
        })
        .join(", ") +
      "]"
    );
  }
  function fmtHex(v) {
    try {
      return "0x" + (v >>> 0).toString(16) + " (" + v + ")";
    } catch {
      return String(v);
    }
  }
  function ninjaDeg(raw) {
    try {
      const u = (raw | 0) & 0xffff;
      return Math.round((u * 3600) / 65536) / 10;
    } catch {
      return 0;
    }
  }
  function fmtAng(a) {
    return (
      "[" +
      a
        .map(function (v) {
          return v + " (~" + ninjaDeg(v) + "°)";
        })
        .join(", ") +
      "]"
    );
  }
  function kv(parent, k, v) {
    const r = document.createElement("div");
    r.className = "mdetail-kv";
    const kk = document.createElement("span");
    kk.className = "label";
    kk.textContent = k;
    const vv = document.createElement("span");
    vv.className = "mdetail-v";
    vv.textContent = v;
    vv.title = v;
    r.appendChild(kk);
    r.appendChild(vv);
    parent.appendChild(r);
    return r;
  }
  function section(parent, label) {
    const h = document.createElement("div");
    h.className = "label";
    h.textContent = label;
    parent.appendChild(h);
    return h;
  }
  function top(parent) {
    const t = document.createElement("div");
    t.className = "mdetail-top";
    parent.appendChild(t);
    return t;
  }
  function backgroundNote(parent, fileName, active) {
    if (fileName === active) return null;
    const note = document.createElement("div");
    note.className = "muted";
    note.textContent =
      "Showing " + fileName + " (not the active file: scene actions disabled).";
    parent.appendChild(note);
    return note;
  }
  function notFound(parent, label) {
    parent.textContent = label + " not found";
  }
  function collapsible(parent, label, open) {
    const det = document.createElement("details");
    det.className = "mdetail-collapsed";
    if (open) det.open = true;
    const head = document.createElement("summary");
    head.className = "label";
    head.textContent = label;
    head.style.cursor = "pointer";
    det.appendChild(head);
    const box = document.createElement("div");
    box.className = "mdetail-top";
    det.appendChild(box);
    parent.appendChild(det);
    return box;
  }
  function collapsedArray(parent, label, arr, fmtFn, noPreview) {
    const det = document.createElement("details");
    det.className = "mdetail-collapsed";
    const head = document.createElement("summary");
    head.className = "label";
    let preview = "";
    if (!noPreview) {
      try {
        preview =
          " (" +
          arr.length +
          ") " +
          arr
            .slice(0, 3)
            .map(function (v) {
              return fmtFn ? fmtFn(v) : String(Math.round(v * 100) / 100);
            })
            .join(", ") +
          (arr.length > 3 ? ", …" : "");
      } catch {
        preview = " (" + (arr ? arr.length : 0) + ")";
      }
    }
    head.textContent = label + preview;
    head.style.cursor = "pointer";
    det.appendChild(head);
    const box = document.createElement("div");
    box.className = "mdetail-top";
    (arr || []).forEach(function (v, i) {
      kv(box, "[" + i + "]", fmtFn ? fmtFn(v) : String(v));
    });
    det.appendChild(box);
    parent.appendChild(det);
    return det;
  }
  g.AppDetailFormat = {
    fmtFloat: fmtFloat,
    fmtHex: fmtHex,
    ninjaDeg: ninjaDeg,
    fmtAng: fmtAng,
    kv: kv,
    section: section,
    collapsible: collapsible,
    top: top,
    backgroundNote: backgroundNote,
    notFound: notFound,
    collapsedArray: collapsedArray,
  };
})(window);
