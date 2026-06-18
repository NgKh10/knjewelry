// cart.js — Xử lý giỏ hàng + lưu localStorage qua AppContext
$(document).ready(function () {

    // ── Thêm vào giỏ từ trang DANH SÁCH ──────────────────────────
    $('.btn-add-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('id');
        var $btn = $(this);
        var originalText = $btn.html();

        // Hiệu ứng loading
        $btn.html('<span class="spinner-border spinner-border-sm"></span> Đang xử lý...');
        $btn.prop('disabled', true);

        $.ajax({
            url: '/GioHang/ThemVaoGio',
            type: 'POST',
            data: { sanPhamId: productId, soLuong: 1 },
            success: function (res) {
                // Phục hồi nút
                $btn.html(originalText);
                $btn.prop('disabled', false);

                if (res.redirect) {
                    showToast(' Vui lòng đăng nhập để thêm sản phẩm!', 'warning');
                    setTimeout(function () {
                        window.location.href = res.redirect;
                    }, 1500);
                    return;
                }

                if (res.success) {
                    AppContext.updateCartBadge(res.soLuongGioHang);
                    if (res.cartItems) AppContext.saveLocalCart(res.cartItems);
                    showToast(' Đã thêm vào giỏ hàng!', 'success');
                } else {
                    showToast(res.message || '❌ Có lỗi xảy ra!', 'error');
                }
            },
            error: function () {
                $btn.html(originalText);
                $btn.prop('disabled', false);
                showToast('❌ Có lỗi xảy ra!', 'error');
            }
        });
    });

    // ── Thêm vào giỏ từ trang CHI TIẾT ───────────────────────────
    $('#btn-add-to-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('id');
        var bienTheId = $(this).data('bien-the-id') || null;
        var quantity = parseInt($('#quantity').val()) || 1;
        var size = $('.btn-size.active').data('size') || null;
        var color = $('.btn-color.active').data('color') || null;
        var $btn = $(this);
        var originalText = $btn.html();

        // Kiểm tra nếu có size nhưng chưa chọn
        var hasSizeButtons = $('.btn-size').length > 0;
        if (hasSizeButtons && !size) {
            showToast(' Vui lòng chọn kích cỡ', 'warning');
            return;
        }

        // Kiểm tra nếu có màu nhưng chưa chọn
        var hasColorButtons = $('.btn-color').length > 0;
        if (hasColorButtons && !color) {
            showToast(' Vui lòng chọn màu sắc', 'warning');
            return;
        }

        // Hiệu ứng loading
        $btn.html('<span class="spinner-border spinner-border-sm"></span> Đang xử lý...');
        $btn.prop('disabled', true);

        $.ajax({
            url: '/GioHang/ThemVaoGio',
            type: 'POST',
            data: {
                sanPhamId: productId,
                bienTheId: bienTheId,
                soLuong: quantity,
                kichCo: size,
                mauSac: color
            },
            success: function (res) {
                // Phục hồi nút
                $btn.html(originalText);
                $btn.prop('disabled', false);

                if (res.redirect) {
                    showToast(' Vui lòng đăng nhập để thêm sản phẩm!', 'warning');
                    setTimeout(function () {
                        window.location.href = res.redirect;
                    }, 1500);
                    return;
                }

                if (res.success) {
                    AppContext.updateCartBadge(res.soLuongGioHang);
                    if (res.cartItems) AppContext.saveLocalCart(res.cartItems);
                    showToast(' Đã thêm vào giỏ hàng!', 'success');
                } else {
                    showToast(res.message || '❌ Có lỗi xảy ra!', 'error');
                }
            },
            error: function () {
                $btn.html(originalText);
                $btn.prop('disabled', false);
                showToast('❌ Có lỗi xảy ra!', 'error');
            }
        });
    });

    // ── Hàm showToast ────────────────────────────────────────────
    function showToast(message, type = 'success') {
        var colors = {
            success: '#28a745',
            error: '#dc3545',
            warning: '#ffc107',
            info: '#17a2b8'
        };

        var oldToast = document.querySelector('.custom-toast');
        if (oldToast) oldToast.remove();

        var toast = document.createElement('div');
        toast.className = 'custom-toast';
        toast.textContent = message;
        toast.style.cssText = `
            position: fixed;
            bottom: 30px;
            right: 30px;
            background: ${colors[type] || '#28a745'};
            color: ${type === 'warning' ? '#333' : 'white'};
            padding: 14px 24px;
            border-radius: 10px;
            font-size: 15px;
            font-weight: 500;
            box-shadow: 0 4px 20px rgba(0,0,0,0.2);
            z-index: 99999;
            max-width: 400px;
            transform: translateY(80px);
            opacity: 0;
            transition: all 0.4s ease;
        `;
        document.body.appendChild(toast);

        setTimeout(function () {
            toast.style.transform = 'translateY(0)';
            toast.style.opacity = '1';
        }, 50);

        setTimeout(function () {
            toast.style.transform = 'translateY(80px)';
            toast.style.opacity = '0';
            setTimeout(function () {
                toast.remove();
            }, 400);
        }, 3000);
    }

}); // ← ĐÓNG $(document).ready()

// ── AppContext (đồng bộ giỏ hàng giữa các tab) ─────────────
var AppContext = {
    updateCartBadge: function (count) {
        var badge = document.getElementById('cart-count');
        if (badge) {
            badge.textContent = count;
            badge.style.display = count > 0 ? 'flex' : 'none';
        }
        localStorage.setItem('cartCount', count);
    },
    saveLocalCart: function (cartItems) {
        localStorage.setItem('cart', JSON.stringify(cartItems));
    },
    getLocalCart: function () {
        try {
            return JSON.parse(localStorage.getItem('cart') || '[]');
        } catch {
            return [];
        }
    }
};

// ── Đồng bộ giỏ hàng khi mở tab mới ────────────────────────
window.addEventListener('storage', function (e) {
    if (e.key === 'cartCount') {
        var badge = document.getElementById('cart-count');
        if (badge) {
            badge.textContent = e.newValue || '0';
            badge.style.display = parseInt(e.newValue || '0') > 0 ? 'flex' : 'none';
        }
    }
});

// Khôi phục số lượng giỏ hàng từ localStorage khi load trang
document.addEventListener('DOMContentLoaded', function () {
    var savedCount = localStorage.getItem('cartCount');
    if (savedCount) {
        var badge = document.getElementById('cart-count');
        if (badge) {
            badge.textContent = savedCount;
            badge.style.display = parseInt(savedCount) > 0 ? 'flex' : 'none';
        }
    }
});