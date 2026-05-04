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

    list.innerHTML = items.map(i => `
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
                    <button class="qty-btn" onclick="changeQty(${i.id}, -1)">−</button>
                    <span class="qty-num">${i.qty}</span>
                    <button class="qty-btn" onclick="changeQty(${i.id}, 1)">+</button>
                    <small style="color:#aaa;font-size:11px;margin-left:4px;">= ${fmt(i.price * i.qty)}</small>
                </div>
            </div>
            <button class="cart-remove" onclick="removeFromCart(${i.id})" title="Xoá">✕</button>
        </div>
    `).join('');
}

function addToCart(id, name, price, image) {
    const product = { id, name, price, image};
    if (!cart[id]) cart[id] = { ...product, qty: 0 };
    cart[id].qty++;
    saveCart();
    refreshCart();
    showToast('✅ Đã thêm "' + name + '" vào giỏ hàng!');
}

function removeFromCart(id) {
    delete cart[id];
    saveCart(); 
    refreshCart();
}

function changeQty(id, delta) {
    if (!cart[id]) return;
    cart[id].qty += delta;
    if (cart[id].qty <= 0) delete cart[id];
    saveCart();
    refreshCart();
}

function checkout() {
    if (Object.keys(cart).length === 0) {
        showToast('⚠️ Giỏ hàng đang trống!');
        return;
    }

    window.location.href = "/Checkout";

    //showToast('🎉 Đặt hàng thành công! Cảm ơn bạn.');
    //cart = {};
    //saveCart();
    //refreshCart();
    //toggleCart();
}

/* ── SEARCH BAR ─────────────────────────────────────────── */
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

/* ── TOAST ──────────────────────────────────────────────── */
let toastTimer;
function showToast(msg) {
    const t = document.getElementById('toast');
    if (!t) return;
    t.textContent = msg;
    t.classList.add('show');
    clearTimeout(toastTimer);
    toastTimer = setTimeout(() => t.classList.remove('show'), 2500);
}

/* ── INIT (chạy khi DOM sẵn sàng) ──────────────────────── */
document.addEventListener('DOMContentLoaded', () => {
    refreshCart();

    // Active nav link theo URL hiện tại
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-links a').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (href && (currentPath === href || currentPath.startsWith(href + '/'))) {
            link.classList.add('active');
        }
    });
});