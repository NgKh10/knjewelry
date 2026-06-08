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

    // ── Toast thông báo ───────────────────────────────────────────
    function showToast(message, type) {
        var bg = type === 'success' ? '#28a745' : '#dc3545';
        var t  = $('<div>').text(message).css({
            position: 'fixed', bottom: '20px', right: '20px', zIndex: 9999,
            background: bg, color: '#fff', padding: '12px 22px',
            borderRadius: '8px', fontSize: '14px',
            boxShadow: '0 2px 10px rgba(0,0,0,0.25)'
        });
        $('body').append(t);
        setTimeout(function () { t.fadeOut(400, function () { t.remove(); }); }, 3000);
    }
});
