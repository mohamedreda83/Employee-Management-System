/* ==========================================================================
   SITE — general page behaviors
   ========================================================================== */

(function () {
  "use strict";

  /* ---------- Live clock (Attendance page) ---------- */
  function initLiveClock() {
    const clockEl = document.querySelector("[data-live-clock]");
    if (!clockEl) return;
    const tick = function () {
      const now = new Date();
      let h = now.getHours();
      const m = now.getMinutes().toString().padStart(2, "0");
      const s = now.getSeconds().toString().padStart(2, "0");
      const ampm = h >= 12 ? "PM" : "AM";
      h = h % 12;
      h = h === 0 ? 12 : h;
      clockEl.textContent = h + ":" + m + ":" + s + " " + ampm;
    };
    tick();
    setInterval(tick, 1000);
  }

  /* ---------- Employees grid/list view toggle ---------- */
  function initViewToggle() {
    const toggle = document.querySelector("[data-view-toggle]");
    if (!toggle) return;
    const gridView = document.querySelector("[data-view-grid]");
    const listView = document.querySelector("[data-view-list]");
    const buttons = toggle.querySelectorAll("button");

    const setView = function (view) {
      buttons.forEach(function (b) { b.classList.toggle("active", b.dataset.view === view); });
      if (gridView) gridView.classList.toggle("d-none", view !== "grid");
      if (listView) listView.classList.toggle("d-none", view !== "list");
      try { localStorage.setItem("employees-view", view); } catch (e) { /* noop */ }
    };

    buttons.forEach(function (b) {
      b.addEventListener("click", function () { setView(b.dataset.view); });
    });

    let saved = "grid";
    try { saved = localStorage.getItem("employees-view") || "grid"; } catch (e) { /* noop */ }
    setView(saved);
  }

  /* ---------- Auto-submit filter selects ---------- */
  function initFilterAutoSubmit() {
    document.querySelectorAll("[data-auto-submit]").forEach(function (el) {
      el.addEventListener("change", function () {
        el.closest("form").submit();
      });
    });
  }

  /* ---------- Search debounce within filter form (manual submit still works) ---------- */
  function initSearchDebounce() {
    document.querySelectorAll("[data-debounce-search]").forEach(function (input) {
      let timer = null;
      input.addEventListener("input", function () {
        clearTimeout(timer);
        timer = setTimeout(function () {
          const form = input.closest("form");
          if (form.requestSubmit) { form.requestSubmit(); } else { form.submit(); }
        }, 550);
      });
    });
  }

  /* ---------- Avatar preview on file input ---------- */
  function initAvatarPreview() {
    document.querySelectorAll("[data-avatar-input]").forEach(function (input) {
      input.addEventListener("change", function () {
        const previewId = input.getAttribute("data-preview-target");
        const preview = document.getElementById(previewId);
        if (!preview || !input.files || !input.files[0]) return;
        const reader = new FileReader();
        reader.onload = function (e) {
          preview.innerHTML = '<img src="' + e.target.result + '" alt="Preview">';
        };
        reader.readAsDataURL(input.files[0]);
      });
    });
  }

  /* ---------- Confirm dialogs for destructive actions ---------- */
  function initConfirmForms() {
    document.querySelectorAll("[data-confirm]").forEach(function (form) {
      form.addEventListener("submit", function (e) {
        const msg = form.getAttribute("data-confirm") || "Are you sure?";
        if (!window.confirm(msg)) {
          e.preventDefault();
        }
      });
    });
  }

  /* ---------- Auto-dismiss alerts ---------- */
  function initAutoDismissAlerts() {
    document.querySelectorAll("[data-auto-dismiss]").forEach(function (alert) {
      setTimeout(function () {
        if (typeof bootstrap !== "undefined") {
          const inst = bootstrap.Alert.getOrCreateInstance(alert);
          inst.close();
        } else {
          alert.remove();
        }
      }, 5000);
    });
  }

  /* ---------- Sync Approve/Reject modal hidden inputs ---------- */
  function initDecisionModals() {
    document.querySelectorAll("[data-decision-trigger]").forEach(function (btn) {
      btn.addEventListener("click", function () {
        const id = btn.getAttribute("data-id");
        const name = btn.getAttribute("data-name");
        const modalSelector = btn.getAttribute("data-bs-target");
        const modal = document.querySelector(modalSelector);
        if (!modal) return;
        const idInput = modal.querySelector("[data-decision-id]");
        const nameEl = modal.querySelector("[data-decision-name]");
        if (idInput) idInput.value = id;
        if (nameEl) nameEl.textContent = name || "";
      });
    });
  }

  document.addEventListener("DOMContentLoaded", function () {
    initLiveClock();
    initViewToggle();
    initFilterAutoSubmit();
    initSearchDebounce();
    initAvatarPreview();
    initConfirmForms();
    initAutoDismissAlerts();
    initDecisionModals();

    if (typeof bootstrap !== "undefined") {
      document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
        new bootstrap.Tooltip(el);
      });
    }
  });
})();
