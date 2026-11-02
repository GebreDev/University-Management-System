// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ============================================================
// Responsive off-canvas sidebar
// Breakpoint matches site.css: >= 1024px -> sidebar pinned, hamburger hidden.
// < 1024px -> sidebar slides in from the left over a dimming overlay.
// ============================================================
var SIDEBAR_BREAKPOINT = 1024; // px
var SIDEBAR_SCROLL_STORAGE_KEY = 'ums-sidebar-scroll';

function saveSidebarScrollPosition() {
    var sidebar = document.getElementById('sidebar');
    if (!sidebar) return;

    try {
        sessionStorage.setItem(SIDEBAR_SCROLL_STORAGE_KEY, String(sidebar.scrollTop));
    } catch (error) {
        console.error('Unable to save the sidebar scroll position.', error);
    }
}

window.addEventListener('pagehide', saveSidebarScrollPosition);

function toggleSidebar(force) {
    var wrapper = document.getElementById('wrapper');
    if (!wrapper) return;

    // On large screens the sidebar is always visible - nothing to toggle.
    if (window.innerWidth >= SIDEBAR_BREAKPOINT && force === undefined) return;

    var open = (typeof force === 'boolean') ? force : !wrapper.classList.contains('toggled');
    wrapper.classList.toggle('toggled', open);

    var btn = document.getElementById('menu-toggle');
    if (btn) btn.setAttribute('aria-expanded', open ? 'true' : 'false');

    var sidebar = document.getElementById('sidebar');
    if (sidebar) sidebar.setAttribute('aria-hidden', open ? 'false' : 'true');

    // Keep the page from scrolling behind the open drawer (mobile only)
    document.body.style.overflow = (open && window.innerWidth < SIDEBAR_BREAKPOINT) ? 'hidden' : '';
}

// Close the drawer with the Escape key
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') toggleSidebar(false);
});

// If the viewport grows to desktop size, reset the drawer state
window.addEventListener('resize', function () {
    if (window.innerWidth >= SIDEBAR_BREAKPOINT) {
        var wrapper = document.getElementById('wrapper');
        if (wrapper) wrapper.classList.remove('toggled');
        document.body.style.overflow = '';
        var btn = document.getElementById('menu-toggle');
        if (btn) btn.setAttribute('aria-expanded', 'false');
    }
});

// Site-wide JavaScript
$(document).ready(function () {
    // Initialize SignalR hubs if the user is authenticated
    const isAuthenticated = $("#currentUserId").length > 0;
    if (isAuthenticated) {
        // SignalR scripts must be loaded before this
        if (typeof signalR !== 'undefined') {
            if (typeof initializeChatHub === 'function') {
                initializeChatHub();
            }
            if (typeof initializeNotificationHub === 'function') {
                initializeNotificationHub();
            }
        }
    }

    // Auto-hide alerts after 5 seconds
    $(".alert").delay(5000).fadeOut("slow");

    // Highlight the sidebar link for the current page (longest URL match wins)
    var currentPath = window.location.pathname.toLowerCase();
    var bestHref = '';
    $('#sidebar .sidebar-link').each(function () {
        var href = ($(this).attr('href') || '').toLowerCase();
        if (href && href !== '/' && currentPath.indexOf(href) === 0 && href.length > bestHref.length) {
            bestHref = href;
        }
    });
    if (bestHref) {
        $('#sidebar .sidebar-link').filter(function () {
            return ($(this).attr('href') || '').toLowerCase() === bestHref;
        }).addClass('active');
    }

    // If the active link lives inside a collapsible submenu, expand it (and all parent dropdowns) on load
    $('#sidebar .sidebar-link.active').parents('.sidebar-dropdown').each(function () {
        $(this).addClass('open');
        $(this).children('.sidebar-dropdown-toggle').attr('aria-expanded', 'true');
    });

    // Restore the sidebar position after expanding the active submenu.
    var sidebar = document.getElementById('sidebar');
    if (sidebar) {
        try {
            var savedScrollPosition = sessionStorage.getItem(SIDEBAR_SCROLL_STORAGE_KEY);
            if (savedScrollPosition !== null) {
                var scrollTop = Number(savedScrollPosition);
                if (Number.isFinite(scrollTop) && scrollTop >= 0) {
                    sidebar.scrollTop = scrollTop;
                }
            }
        } catch (error) {
            console.error('Unable to restore the sidebar scroll position.', error);
        }
    }

    // Collapsible sidebar submenu toggle (Settings menu)
    $(document).on('click', '.sidebar-dropdown-toggle', function () {
        var $li = $(this).closest('.sidebar-dropdown');
        var open = $li.hasClass('open');
        $li.toggleClass('open', !open);
        $(this).attr('aria-expanded', open ? 'false' : 'true');
    });

    // ---- Notifications: click an unread item -> mark as read + navigate ----
    $(document).on('click', '.notification-item[data-read="false"]', function (e) {
        var $item = $(this);
        var id = $item.data('notification-id');
        if (!id) return;

        e.preventDefault();
        var href = $item.attr('href');
        var token = $('input[name="__RequestVerificationToken"]').first().val() || '';

        $.ajax({
            url: '/Notification/MarkAsRead',
            type: 'POST',
            data: { id: id, __RequestVerificationToken: token },
            success: function () {
                $item.attr('data-read', 'true').removeClass('unread');
                $item.find('.notif-dot').addClass('read');
                if (typeof updateNotificationBadge === 'function') updateNotificationBadge();
            },
            complete: function () {
                if (href) window.location.href = href;
            }
        });
    });

    // Replace browser-native delete prompts with the shared, accessible confirmation modal.
    document.addEventListener('submit', function (event) {
        var form = event.target;
        if (!(form instanceof HTMLFormElement)) return;

        if (form.dataset.confirmed === 'true') {
            delete form.dataset.confirmed;
            return;
        }

        if (!form.dataset.confirmMessage) return;

        event.preventDefault();

        var modalElement = document.getElementById('siteConfirmModal');
        var titleElement = document.getElementById('siteConfirmTitle');
        var messageElement = document.getElementById('siteConfirmMessage');
        var acceptButton = document.getElementById('siteConfirmAccept');
        if (!modalElement || !titleElement || !messageElement || !acceptButton || typeof bootstrap === 'undefined') {
            console.error('The confirmation modal is unavailable; the requested action was not submitted.');
            return;
        }

        var submitter = event.submitter;
        titleElement.textContent = form.dataset.confirmTitle || 'Confirm deletion';
        messageElement.textContent = form.dataset.confirmMessage;
        acceptButton.querySelector('span').textContent = form.dataset.confirmAction || 'Delete';

        var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        acceptButton.onclick = function () {
            acceptButton.onclick = null;
            modal.hide();
            form.dataset.confirmed = 'true';
            if (submitter && submitter.form === form) {
                form.requestSubmit(submitter);
            } else {
                form.requestSubmit();
            }
        };

        modal.show();
    }, true);

    // ---- Notifications: Mark all as read via AJAX (keeps dropdown open) ----
    $(document).on('submit', 'form[data-ajax-markallread]', function (e) {
        e.preventDefault();
        var $form = $(this);
        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function () {
                $('.notification-item').attr('data-read', 'true').removeClass('unread');
                $('.notif-dot').addClass('read');
                $form.closest('.notification-dropdown-header').find('[data-markallread]').addClass('d-none');
                $('.notif-badge').remove();
                if (typeof updateNotificationBadge === 'function') updateNotificationBadge();
            }
        });
    });
});

// ============================================================
// Sidebar live search — filters the menu as you type.
// ============================================================
function filterSidebar(query) {
    var nav = document.getElementById('sidebarNav');
    if (!nav) return;
    var q = (query || '').trim().toLowerCase();
    var sections = nav.querySelectorAll(':scope > .sidebar-section');
    var items = nav.querySelectorAll(':scope > .sidebar-item');
    var noResults = document.getElementById('sidebarNoResults');
    var visibleCount = 0;

    if (!q) {
        // Reset to the default state: everything visible, submenus collapsed
        items.forEach(function (item) {
            item.classList.remove('d-none');
            if (item.classList.contains('sidebar-dropdown')) {
                item.classList.remove('open');
                item.querySelectorAll(':scope > .sidebar-submenu > .sidebar-item')
                    .forEach(function (child) { child.classList.remove('d-none'); });
            }
        });
        sections.forEach(function (section) { section.classList.remove('d-none'); });
        if (noResults) noResults.classList.add('d-none');
        return;
    }

    // 1) Filter top-level items (submenus are hidden inside their parent li)
    items.forEach(function (item) {
        if (item.id === 'sidebarNoResults') return;
        var text = itemText(item);
        var ownMatch = text.indexOf(q) !== -1;
        var childMatch = false;

        // Inside a dropdown, filter its children too
        if (item.classList.contains('sidebar-dropdown')) {
            var children = item.querySelectorAll(':scope > .sidebar-submenu > .sidebar-item');
            children.forEach(function (child) {
                var m = itemText(child).indexOf(q) !== -1;
                child.classList.toggle('d-none', !m);
                if (m) childMatch = true;
            });
            if (childMatch) item.classList.add('open');
        }

        var show = ownMatch || childMatch;
        item.classList.toggle('d-none', !show);
        if (show) visibleCount++;
    });

    // 2) Hide section headers whose items are all hidden
    sections.forEach(function (section) {
        var hide = true;
        var node = section.nextElementSibling;
        while (node && !node.classList.contains('sidebar-section')) {
            if (node.classList.contains('sidebar-item') && !node.classList.contains('d-none')) {
                hide = false;
                break;
            }
            node = node.nextElementSibling;
        }
        section.classList.toggle('d-none', hide);
    });

    if (noResults) noResults.classList.toggle('d-none', visibleCount > 0);
}

// Returns the visible label of a sidebar item (skips icon-only nodes)
function itemText(item) {
    var link = item.querySelector('.sidebar-link');
    return link ? link.textContent.trim().toLowerCase() : '';
}

var sidebarSearchInput = document.getElementById('sidebarSearch');
if (sidebarSearchInput) {
    sidebarSearchInput.addEventListener('input', function () {
        filterSidebar(this.value);
    });
}

// ============================================================
// Dark mode toggle
// ============================================================
function applyTheme(theme) {
    var root = document.documentElement;
    if (theme === 'dark') root.setAttribute('data-theme', 'dark');
    else root.removeAttribute('data-theme');
    try { localStorage.setItem('ums-theme', theme); } catch (e) { }
    updateThemeIcon(theme);
}

function updateThemeIcon(theme) {
    var btn = document.getElementById('themeToggle');
    if (!btn) return;
    var isDark = theme === 'dark' || (theme === undefined && document.documentElement.getAttribute('data-theme') === 'dark');
    btn.innerHTML = isDark ? '<i class="fas fa-sun"></i>' : '<i class="fas fa-moon"></i>';
    btn.title = isDark ? 'Switch to light mode' : 'Switch to dark mode';
    btn.setAttribute('aria-label', btn.title);
}

var themeToggle = document.getElementById('themeToggle');
if (themeToggle) {
    themeToggle.addEventListener('click', function () {
        var isDark = document.documentElement.getAttribute('data-theme') === 'dark';
        applyTheme(isDark ? 'light' : 'dark');
    });
    updateThemeIcon();
}

// ============================================================
// Global header search: filters sidebar + any [data-search]
// elements on the page; Enter navigates to the first sidebar hit.
// ============================================================
var globalSearchInput = document.getElementById('globalSearch');
if (globalSearchInput) {
    globalSearchInput.addEventListener('input', function () {
        var q = this.value.trim().toLowerCase();
        filterSidebar(q);
        document.querySelectorAll('[data-search]').forEach(function (el) {
            var haystack = (el.getAttribute('data-search') || '').toLowerCase();
            el.style.display = (!q || haystack.indexOf(q) !== -1) ? '' : 'none';
        });
    });
    globalSearchInput.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            var first = document.querySelector('#sidebarNav .sidebar-item:not(.d-none) .sidebar-link');
            if (first && first.href) window.location.href = first.href;
        }
    });
}

// ============================================================
// Smart back buttons: every .btn-back-pill returns to the page the
// user actually came from. If there is in-site navigation history we
// go back one step (preserving filters, tabs and scroll context);
// otherwise we fall back to the link's own href (a sensible default
// destination such as a list page or the dashboard).
// ============================================================
document.addEventListener('click', function (e) {
    var link = e.target.closest ? e.target.closest('a.btn-back-pill') : null;
    if (!link) return;

    // Same tab, keep scroll position when the browser restores it.
    e.preventDefault();

    var canGoBack = window.history.length > 1;
    var referrer = document.referrer;
    var sameOriginReferrer = false;
    try {
        sameOriginReferrer = !!referrer && new URL(referrer).origin === window.location.origin;
    } catch (err) {
        sameOriginReferrer = false;
    }

    if (canGoBack && sameOriginReferrer) {
        // Arrived from another page on this site: return to it.
        window.history.back();
    } else {
        // Opened directly (new tab, bookmark, refresh after POST):
        // use the server-rendered fallback destination.
        var fallback = link.getAttribute('href');
        if (fallback && fallback !== '#') {
            window.location.href = fallback;
        }
    }
});