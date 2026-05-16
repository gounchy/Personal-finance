/**
 * SpendWise — vanilla auth overlay (login / register / forgot).
 * Depends on: #auth-overlay, [data-auth-open], [data-auth-switch], [data-auth-close]
 */
(function () {
  "use strict";

  var overlay = document.getElementById("auth-overlay");
  if (!overlay) return;

  var modal = overlay.querySelector(".auth-modal");
  var panels = overlay.querySelectorAll("[data-auth-panel]");
  var backdrop = overlay.querySelector(".auth-overlay__backdrop");

  function cleanQuery() {
    var u = new URL(window.location.href);
    if (!u.search) return;
    var q = u.searchParams;
    var changed = false;
    ["openLogin", "openRegister", "openForgot", "returnUrl"].forEach(function (k) {
      if (q.has(k)) {
        q.delete(k);
        changed = true;
      }
    });
    if (changed) {
      var next = u.pathname + (q.toString() ? "?" + q.toString() : "") + u.hash;
      history.replaceState({}, "", next);
    }
  }

  function setActive(view) {
    var v = view || "login";
    panels.forEach(function (p) {
      var id = p.getAttribute("data-auth-panel");
      var on = id === v;
      p.classList.toggle("is-active", on);
      p.setAttribute("aria-hidden", on ? "false" : "true");
    });
    if (modal) {
      modal.classList.toggle("auth-modal--wide", v === "register");
    }
    overlay.setAttribute("data-auth-view", v);
  }

  function open(view) {
    var v = view || "login";
    setActive(v);
    overlay.classList.add("is-open");
    overlay.setAttribute("aria-hidden", "false");
    document.body.classList.add("auth-scroll-lock");
    window.setTimeout(function () {
      var active = overlay.querySelector(".auth-form.is-active");
      if (active) {
        var fe = active.querySelector(
          'input:not([type="hidden"]):not([type="checkbox"]), textarea, select'
        );
        if (fe && typeof fe.focus === "function") fe.focus();
      }
    }, 320);
  }

  function close() {
    overlay.classList.remove("is-open");
    overlay.setAttribute("aria-hidden", "true");
    document.body.classList.remove("auth-scroll-lock");
  }

  document.querySelectorAll("[data-auth-open]").forEach(function (btn) {
    btn.addEventListener("click", function () {
      open(btn.getAttribute("data-auth-open") || "login");
    });
  });

  document.querySelectorAll("[data-auth-switch]").forEach(function (btn) {
    btn.addEventListener("click", function (e) {
      e.preventDefault();
      setActive(btn.getAttribute("data-auth-switch"));
    });
  });

  overlay.querySelectorAll("[data-auth-close]").forEach(function (el) {
    el.addEventListener("click", function (e) {
      e.preventDefault();
      close();
    });
  });

  if (modal) {
    modal.addEventListener("click", function (e) {
      e.stopPropagation();
    });
  }

  overlay.addEventListener("click", function (e) {
    if (e.target === overlay || e.target === backdrop) close();
  });

  document.addEventListener("keydown", function (e) {
    if (e.key === "Escape" && overlay.classList.contains("is-open")) {
      e.preventDefault();
      close();
    }
  });

  /* Deep link: /?openLogin=1 / openRegister / openForgot */
  (function initFromQuery() {
    var q = new URLSearchParams(window.location.search);
    if (q.get("openLogin")) {
      open("login");
      cleanQuery();
    } else if (q.get("openRegister")) {
      open("register");
      cleanQuery();
    } else if (q.get("openForgot")) {
      open("forgot");
      cleanQuery();
    }
  })();
})();
