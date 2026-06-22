/* ==========================================================================
   MOTION — GSAP choreography for the Employee Management System
   Re-initialized per page load via initPageMotion()
   ========================================================================== */

(function() {
    "use strict";

    function initPageMotion() {
        if (typeof gsap === "undefined") return;

        /* ---------- a) Page entrance ---------- */
        gsap.set(".app-sidebar", { x: -24, opacity: 0 });
        gsap.set(".app-topbar", { y: -16, opacity: 0 });
        gsap.set("[data-stagger-item]", { y: 22, opacity: 0 });

        const tl = gsap.timeline({ defaults: { ease: "power3.out", duration: 0.6 } });
        tl.to(".app-sidebar", { x: 0, opacity: 1 })
            .to(".app-topbar", { y: 0, opacity: 1, duration: 0.5 }, "-=0.35")
            .to("[data-stagger-item]", { y: 0, opacity: 1, stagger: 0.07, duration: 0.5 }, "-=0.3");

        /* ---------- d) Ambient background blobs ---------- */
        if (document.querySelector(".blob-1")) {
            gsap.to(".blob-1", { x: 50, y: -35, duration: 16, repeat: -1, yoyo: true, ease: "sine.inOut" });
        }
        if (document.querySelector(".blob-2")) {
            gsap.to(".blob-2", { x: -45, y: 40, duration: 20, repeat: -1, yoyo: true, ease: "sine.inOut" });
        }
        if (document.querySelector(".blob-3")) {
            gsap.to(".blob-3", { x: 30, y: 30, duration: 13, repeat: -1, yoyo: true, ease: "sine.inOut" });
        }

        /* ---------- e) KPI count-up ---------- */
        document.querySelectorAll("[data-count-to]").forEach(function(el) {
            const target = parseFloat(el.dataset.countTo) || 0;
            const decimals = el.dataset.countDecimals ? parseInt(el.dataset.countDecimals, 10) : 0;
            gsap.fromTo(
                el, { textContent: 0 }, {
                    textContent: target,
                    duration: 1.4,
                    ease: "power2.out",
                    delay: 0.3,
                    snap: { textContent: decimals > 0 ? 1 / Math.pow(10, decimals) : 1 },
                    onUpdate: function() {
                        const val = parseFloat(el.textContent);
                        el.textContent = decimals > 0 ?
                            val.toFixed(decimals) :
                            Math.floor(val).toLocaleString();
                    }
                }
            );
        });

        /* ---------- c) Sidebar sliding indicator ---------- */
        const sidebar = document.querySelector(".app-sidebar");
        const indicator = document.querySelector(".sidebar-indicator");
        if (sidebar && indicator) {
            const placeOnActive = function() {
                const active = sidebar.querySelector(".sidebar-link.active");
                if (active) {
                    gsap.set(indicator, { y: active.offsetTop, height: active.offsetHeight, opacity: 1 });
                } else {
                    gsap.set(indicator, { opacity: 0 });
                }
            };
            placeOnActive();

            sidebar.querySelectorAll(".sidebar-link").forEach(function(link) {
                link.addEventListener("mouseenter", function() {
                    gsap.to(indicator, { y: link.offsetTop, height: link.offsetHeight, opacity: 1, duration: 0.35, ease: "power3.out" });
                    sidebar.classList.add("sidebar-has-hover");
                });
            });
            sidebar.addEventListener("mouseleave", function() {
                sidebar.classList.remove("sidebar-has-hover");
                placeOnActive();
            });
            window.addEventListener("resize", placeOnActive);
        }

        /* ---------- Glass tab sliding indicator ---------- */
        document.querySelectorAll(".glass-tabs").forEach(function(tabs) {
            const tabIndicator = tabs.querySelector(".tab-indicator");
            const links = tabs.querySelectorAll(".tab-link");
            if (!tabIndicator || !links.length) return;

            const place = function(el, animate) {
                const r = el.getBoundingClientRect();
                const parentR = tabs.getBoundingClientRect();
                const props = { left: r.left - parentR.left, width: r.width };
                if (animate) {
                    gsap.to(tabIndicator, Object.assign({}, props, { duration: 0.4, ease: "power3.out" }));
                } else {
                    gsap.set(tabIndicator, props);
                }
            };

            const activeLink = tabs.querySelector(".tab-link.active") || links[0];
            place(activeLink, false);
            window.addEventListener("resize", function() {
                const current = tabs.querySelector(".tab-link.active") || links[0];
                place(current, false);
            });
        });
    }

    /* ---------- b) 3D tilt cards (disabled on touch) ---------- */
    function initTiltCards() {
        const supportsHover = window.matchMedia("(hover: hover)").matches && window.matchMedia("(pointer: fine)").matches;
        if (!supportsHover) return;

        document.querySelectorAll("[data-tilt]").forEach(function(card) {
            if (card.dataset.tiltBound) return;
            card.dataset.tiltBound = "true";
            const strength = 7;
            card.addEventListener("mousemove", function(e) {
                const r = card.getBoundingClientRect();
                const x = (e.clientX - r.left) / r.width;
                const y = (e.clientY - r.top) / r.height;
                const rotX = (y - 0.5) * -strength;
                const rotY = (x - 0.5) * strength;
                card.style.transform = "rotateX(" + rotX + "deg) rotateY(" + rotY + "deg) scale3d(1.015,1.015,1.015)";
                card.style.setProperty("--mx", x * 100 + "%");
                card.style.setProperty("--my", y * 100 + "%");
            });
            card.addEventListener("mouseleave", function() {
                card.style.transform = "rotateX(0) rotateY(0) scale3d(1,1,1)";
            });
        });
    }

    /* ---------- Sidebar mobile offcanvas toggle ---------- */
    function initSidebarToggle() {
        const toggleBtns = document.querySelectorAll("[data-sidebar-toggle]");
        const sidebar = document.querySelector(".app-sidebar");
        const backdrop = document.querySelector(".sidebar-backdrop");
        if (!sidebar) return;

        const closeSidebar = function() {
            sidebar.classList.remove("show");
            if (backdrop) backdrop.classList.remove("show");
        };

        toggleBtns.forEach(function(btn) {
            btn.addEventListener("click", function() {
                sidebar.classList.toggle("show");
                if (backdrop) backdrop.classList.toggle("show");
            });
        });

        if (backdrop) backdrop.addEventListener("click", closeSidebar);
    }

    /* ---------- Animated row removal (vacation approve/reject) ---------- */
    window.animateRowOut = function(row, callback) {
        if (typeof gsap === "undefined") {
            if (callback) callback();
            return;
        }
        gsap.to(row, {
            opacity: 0,
            x: 24,
            height: 0,
            paddingTop: 0,
            paddingBottom: 0,
            marginBottom: 0,
            duration: 0.4,
            ease: "power2.in",
            onComplete: callback
        });
    };

    /* ---------- Password visibility toggle ---------- */
    function initPasswordToggle() {
        document.querySelectorAll(".toggle-password").forEach(function(btn) {
            btn.addEventListener("click", function() {
                const targetId = btn.getAttribute("data-target");
                const input = document.getElementById(targetId);
                if (!input) return;
                const icon = btn.querySelector("i");
                if (input.type === "password") {
                    input.type = "text";
                    if (icon) { icon.classList.remove("bi-eye");
                        icon.classList.add("bi-eye-slash"); }
                } else {
                    input.type = "password";
                    if (icon) { icon.classList.remove("bi-eye-slash");
                        icon.classList.add("bi-eye"); }
                }
            });
        });
    }

    document.addEventListener("DOMContentLoaded", function() {
        initPageMotion();
        initTiltCards();
        initSidebarToggle();
        initPasswordToggle();
    });

    window.initPageMotion = initPageMotion;
    window.initTiltCards = initTiltCards;
})();