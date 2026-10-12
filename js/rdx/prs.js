(function (g) {
  "use strict";
  function decompress(src) {
    src = src instanceof Uint8Array ? src : new Uint8Array(src);
    const n = src.length,
      out = [];
    let pos = 0,
      cmd = 0,
      bits = 0;
    function getByte() {
      if (pos >= n) throw new Error("truncated PRS at " + pos);
      return src[pos++];
    }
    function getBit() {
      if (bits === 0) {
        cmd = getByte();
        bits = 8;
      }
      const b = cmd & 1;
      cmd >>= 1;
      bits--;
      return b;
    }
    while (pos < n) {
      if (getBit()) {
        out.push(getByte());
        continue;
      }
      let amount, start;
      if (getBit()) {
        const a = getByte(),
          b = getByte();
        const offset = ((b << 8) | a) >> 3;
        amount = a & 7;
        if (pos < n) {
          if (amount === 0) amount = getByte() + 1;
          else amount += 2;
        }
        start = out.length - 0x2000 + offset;
      } else {
        amount = ((getBit() << 1) | getBit()) + 2;
        start = out.length - 0x100 + getByte();
      }
      for (let i = 0; i < amount; i++, start++) {
        out.push(start < 0 ? 0 : start < out.length ? out[start] : 0);
      }
    }
    return new Uint8Array(out);
  }
  const MAGICS = [0x41200000, 0x40051eb8, 0x40028f5c];
  function tryDecompress(data) {
    const raw = data instanceof Uint8Array ? data : new Uint8Array(data);
    let out;
    try {
      out = decompress(raw);
    } catch {
      return raw;
    }
    if (out.length < 4) return raw;
    const m = (out[0] | (out[1] << 8) | (out[2] << 16) | (out[3] << 24)) >>> 0;
    for (let i = 0; i < MAGICS.length; i++) {
      if (m === MAGICS[i] >>> 0) return out;
    }
    return raw;
  }
  g.RDXPRS = {
    tryDecompress: tryDecompress,
  };
})(window);
