// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let cart = JSON.parse(localStorage.getItem('cart') || '{}');

function saveCart() {
    localStorage.setItem('cart', JSON.stringify(cart));
}

const fmt = n => n.toLocaleString('vi-VN') + 'đ';

/* ── CART PANEL ─────────────────────────────────────────── */
function toggleCart() {
    document.getElementById('cart-panel').classList.toggle('open');
    document.getElementById('cart-overlay').classList.toggle('open');
}

function refreshCart() {
    const items = Object.values(cart);
    const total = items.reduce((s, i) => s + i.price * i.qty, 0);
    const count = items.reduce((s, i) => s + i.qty, 0);

    const countEl = document.getElementById('cart-count');
    const totalEl = document.getElementById('cart-total-price');
    if (countEl) countEl.textContent = count;
    if (totalEl) totalEl.textContent = fmt(total);

    const list = document.getElementById('cart-items-list');
    if (!list) return;

    if (items.length === 0) {
        list.innerHTML = '<div class="empty-cart">🛒<br><br>Giỏ hàng đang trống<br><small>Hãy thêm sản phẩm vào giỏ!</small></div>';
        return;
    }

    list.innerHTML = items.map(i => {
        // Dùng data-* attribute thay vì nhét id vào onclick
        // → tránh hoàn toàn vấn đề quote conflict
        const idAttr = JSON.stringify(i.id); // dùng trong data-id
        return `
        <div class="cart-item">
            <div class="cart-item-emoji">
                ${i.image
                ? `<img src="${i.image}" alt="${i.name}" style="width:56px;height:56px;object-fit:cover;border-radius:6px;" />`
                : '🛍️'}
            </div>
            <div class="cart-item-info">
                <div class="cart-item-name">${i.name}</div>
                <div class="cart-item-price">${fmt(i.price)}</div>
                <div class="qty-row">
                    <button class="qty-btn" data-id='${idAttr}' data-delta="-1">−</button>
                    <span class="qty-num">${i.qty}</span>
                    <button class="qty-btn" data-id='${idAttr}' data-delta="1">+</button>
                    <small style="color:#aaa;font-size:11px;margin-left:4px;">= ${fmt(i.price * i.qty)}</small>
                </div>
            </div>
            <button class="cart-remove" data-id='${idAttr}' title="Xoá">✕</button>
        </div>`;
    }).join('');

    // Gắn event listener sau khi render xong
    list.querySelectorAll('.qty-btn[data-delta]').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = JSON.parse(this.dataset.id);
            const delta = parseInt(this.dataset.delta);
            changeQty(id, delta);
        });
    });

    list.querySelectorAll('.cart-remove[data-id]').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = JSON.parse(this.dataset.id);
            removeFromCart(id);
        });
    });
}

function addToCart(id, name, price, image) {
    const product = { id, name, price, image};
    if (!cart[id]) cart[id] = { ...product, qty: 0 };
    cart[id].qty++;
    saveCart();
    refreshCart();
    showToast('success', 'Thêm vào giỏ hàng', `Đã thêm "<b>${name}</b>" vào giỏ hàng!`);
}

function removeFromCart(id) {
    console.log("removeFromCart called with id:", id);
    delete cart[id];
    
    saveCart(); 
    refreshCart();
}

function changeQty(id, delta) {
    console.log("changeQty called with id:", id, "delta:", delta);
    if (!cart[id]) return;
    cart[id].qty += delta;
    if (cart[id].qty <= 0) delete cart[id];
    saveCart();
    refreshCart();
}

function checkout() {
    if (Object.keys(cart).length === 0) {
        /*showToast('⚠️ Giỏ hàng đang trống!');*/
        showToast('warning', 'Giỏ hàng trống', 'Vui lòng thêm sản phẩm trước khi đặt hàng.');
        return;
    }

    window.location.href = "/Checkout";
}

function toggleSearch() {
    const sb = document.getElementById('search-bar');
    if (!sb) return;
    sb.classList.toggle('open');
    if (sb.classList.contains('open')) {
        setTimeout(() => {
            const input = document.getElementById('search-input');
            if (input) input.focus();
        }, 200);
    }
}

function doSearch() {
    const input = document.getElementById('search-input');
    const keyword = input?.value?.trim();
    if (!keyword) return;
    window.location.href = `/Products?search=${encodeURIComponent(keyword)}`;
}

document.addEventListener('DOMContentLoaded', () => {
    refreshCart();

    // Search: nhấn Enter để tìm
    const searchInput = document.getElementById('search-input');
    if (searchInput) {
        searchInput.addEventListener('keydown', e => {
            if (e.key === 'Enter') doSearch();
        });
    }

    // Active nav link
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-links a').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (href && (currentPath === href || currentPath.startsWith(href + '/'))) {
            link.classList.add('active');
        }
    });
});

// ════════════════════════════════
// TOAST SYSTEM
// ════════════════════════════════

(function () {
    function getContainer() {
        let el = document.getElementById('toast-container');
        if (!el) {
            el = document.createElement('div');
            el.id = 'toast-container';
            el.className = 'toast-container';
            document.body.appendChild(el);
        }
        return el;
    }

    const TOAST_CONFIG = {
        success: { icon: '✓', defaultTitle: 'Thành công' },
        error: { icon: '✕', defaultTitle: 'Lỗi' },
        warning: { icon: '!', defaultTitle: 'Cảnh báo' },
        info: { icon: 'i', defaultTitle: 'Thông báo' },
    };

    /**
     * @param {'success'|'error'|'warning'|'info'} type
     * @param {string} title    - Tiêu đề (rỗng = dùng mặc định)
     * @param {string} msg      - Nội dung, hỗ trợ HTML
     * @param {number} duration - ms tự đóng, 0 = không tự đóng
     */
    window.showToast = function (type = 'info', title = '', msg = '', duration = 4000) {
        const cfg = TOAST_CONFIG[type] ?? TOAST_CONFIG.info;
        const container = getContainer();

        const toast = document.createElement('div');
        toast.className = `toast ${type}`;
        toast.setAttribute('role', 'alert');
        toast.innerHTML = `
            <span class="toast-icon">${cfg.icon}</span>
            <div class="toast-content">
                <div class="toast-title">${title || cfg.defaultTitle}</div>
                ${msg ? `<div class="toast-msg">${msg}</div>` : ''}
            </div>
            <button class="toast-close" aria-label="Đóng">✕</button>`;

        toast.querySelector('.toast-close')
            .addEventListener('click', () => dismiss(toast));

        container.appendChild(toast);

        let timer;
        if (duration > 0) {
            timer = setTimeout(() => dismiss(toast), duration);
        }

        // Hover → pause countdown
        toast.addEventListener('mouseenter', () => clearTimeout(timer));
        toast.addEventListener('mouseleave', () => {
            if (duration > 0) timer = setTimeout(() => dismiss(toast), 1500);
        });

        return toast;
    };

    function dismiss(toast) {
        if (!toast || toast.classList.contains('hide')) return;
        toast.classList.add('hide');
        toast.addEventListener('animationend', () => toast.remove(), { once: true });
    }

})();