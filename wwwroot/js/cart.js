// cart.js — Xử lý giỏ hàng + lưu localStorage qua AppContext
$(document).ready(function () {

    // ── Thêm vào giỏ từ trang DANH SÁCH ──────────────────────────
    $('.btn-add-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('id');

        $.ajax({
            url : '/GioHang/ThemVaoGio',
            type: 'POST',
            data: { sanPhamId: productId, soLuong: 1 },
            success: function (res) {
                if (res.success) {
                    AppContext.updateCartBadge(res.soLuongGioHang);
                    // Lưu cartItems trả về thẳng vào localStorage (không cần request phụ)
                    if (res.cartItems) AppContext.saveLocalCart(res.cartItems);
                    showToast('Đã thêm vào giỏ hàng!', 'success');
                } else {
                    showToast(res.message || 'Có lỗi xảy ra!', 'error');
                }
            },
            error: function () { showToast('Có lỗi xảy ra!', 'error'); }
        });
    });

    // ── Thêm vào giỏ từ trang CHI TIẾT ───────────────────────────
    $('#btn-add-to-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('id');
        var bienTheId = $(this).data('bien-the-id') || null;
        var quantity  = parseInt($('#quantity').val()) || 1;
        var size      = $('.btn-size.active').data('size')   || null;
        var color     = $('.btn-color.active').data('color') || null;

        $.ajax({
            url : '/GioHang/ThemVaoGio',
            type: 'POST',
            data: { sanPhamId: productId, bienTheId: bienTheId,
                    soLuong: quantity, kichCo: size, mauSac: color },
            success: function (res) {
                if (res.success) {
                    AppContext.updateCartBadge(res.soLuongGioHang);
                    if (res.cartItems) AppContext.saveLocalCart(res.cartItems);
                    showToast('Đã thêm vào giỏ hàng!', 'success');
                } else {
                    showToast(res.message || 'Có lỗi xảy ra!', 'error');
                }
            },
            error: function () { showToast('Có lỗi xảy ra!', 'error'); }
        });
    });

    // Hàm xử lý response từ server
    function handleAddToCartResponse(data, productName, btn) {
        var originalText = btn.innerHTML;

        if (data.redirect) {
            // Chưa đăng nhập -> hiển thị thông báo và chuyển hướng
            showToast(' Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!', 'warning');
            btn.innerHTML = 'Đang chuyển...';
            btn.disabled = true;
            setTimeout(function () {
                window.location.href = data.redirect;
            }, 1500);
            return;
        }

        // Phục hồi nút
        btn.innerHTML = originalText;
        btn.disabled = false;

        if (data.success) {
            var cartBadge = document.getElementById('cart-count');
            if (cartBadge) cartBadge.textContent = data.soLuongGioHang;
            showToast(' Đã thêm "' + productName + '" vào giỏ hàng!', 'success');
        } else {
            showToast('❌ ' + (data.message || 'Thêm vào giỏ hàng thất bại!'), 'error');
        }
    }

    // Hàm showToast
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
        transform: translateY(80px);
        opacity: 0;
        transition: all 0.4s ease;
        max-width: 400px;
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
