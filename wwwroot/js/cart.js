// cart.js
$(document).ready(function () {
    // Thêm vào giỏ hàng từ trang danh sách sản phẩm
    $('.btn-add-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('id');
        var quantity = 1;

        $.ajax({
            uurl: '/GioHang/ThemVaoGio',  // Đường dẫn này
            type: 'POST',
            data: {
                sanPhamId: productId,
                bienTheId: null,
                soLuong: quantity
            },
            success: function (response) {
                if (response.success) {
                    $('#cart-count').text(response.soLuongGioHang);
                    showNotification('Đã thêm vào giỏ hàng!', 'success');
                } else {
                    showNotification(response.message || 'Có lỗi xảy ra!', 'error');
                }
            },
            error: function () {
                showNotification('Có lỗi xảy ra!', 'error');
            }
        });
    });

    // Thêm vào giỏ hàng từ trang chi tiết sản phẩm
    $('#btn-add-to-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('id');
        var quantity = $('#quantity').val();
        var size = $('.btn-size.active').data('size');
        var color = $('.btn-color.active').data('color');

        $.ajax({
            url: '/GioHang/ThemVaoGio',
            type: 'POST',
            data: {
                sanPhamId: productId,
                bienTheId: null,
                soLuong: quantity,
                kichCo: size,
                mauSac: color
            },
            success: function (response) {
                if (response.success) {
                    $('#cart-count').text(response.soLuongGioHang);
                    showNotification('Đã thêm vào giỏ hàng!', 'success');
                } else {
                    showNotification(response.message || 'Có lỗi xảy ra!', 'error');
                }
            },
            error: function () {
                showNotification('Có lỗi xảy ra!', 'error');
            }
        });
    });

    // Hiển thị thông báo
    function showNotification(message, type) {
        var notification = $('<div class="notification ' + type + '">' + message + '</div>');
        $('body').append(notification);
        notification.fadeIn(300);
        setTimeout(function () {
            notification.fadeOut(300, function () {
                $(this).remove();
            });
        }, 3000);
    }
});