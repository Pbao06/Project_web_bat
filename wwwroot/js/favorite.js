/**
 * Chức năng yêu thích sản phẩm dùng chung cho Store Index và Details
 */
async function toggleWish(event, button, productId = null) {
    if (event) {
        event.stopPropagation();
    }

    // Nếu không truyền productId trực tiếp, lấy từ data-id của button hoặc .product-card cha
    if (!productId) {
        productId = button.getAttribute('data-id');
        if (!productId) {
            const card = button.closest('.product-card');
            if (card) {
                productId = card.getAttribute('data-id');
            }
        }
    }

    if (!productId) {
        console.error("Không tìm thấy ProductId");
        return;
    }

    const icon = button.querySelector('i');

    try {
        const response = await fetch(`/User/Store/Togglelike?productId=${productId}`, {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const result = await response.json();
        
        if (result.success) {
            // Cập nhật giao diện bằng cách thêm/xóa class 'wished'
            // CSS sẽ tự động xử lý màu sắc dựa trên class này
            if (result.statuslike) {
                button.classList.add('wished');
                if (icon) {
                    icon.classList.remove('bx-heart');
                    icon.classList.add('bxs-heart');
                }
            } else {
                button.classList.remove('wished');
                if (icon) {
                    icon.classList.remove('bxs-heart');
                    icon.classList.add('bx-heart');
                }
            }
        } else {
            // Xử lý khi chưa đăng nhập (dùng ShowGlobalModal)
            if (result.message && (result.message.toLowerCase().includes("đăng nhập") || result.message.toLowerCase().includes("login"))) {
                if (typeof showGlobalModal === 'function') {
                    showGlobalModal({
                        title: 'Yêu cầu đăng nhập',
                        content: 'Bạn cần đăng nhập để thêm sản phẩm vào danh sách yêu thích.',
                        confirmText: 'Đăng nhập ngay',
                        cancelText: 'Hủy',
                        onConfirm: function() {
                            window.location.href = "/User/Account/Login";
                        }
                    });
                } else {
                    alert("Bạn cần đăng nhập để thực hiện chức năng này.");
                    window.location.href = "/User/Account/Login";
                }
            } else {
                if (typeof showToast === 'function') {
                    showToast(result.message || "Có lỗi xảy ra, vui lòng thử lại.", 'error');
                } else {
                    alert(result.message || "Có lỗi xảy ra");
                }
            }
        }
    } catch (error) {
        console.error('Error:', error);
        if (typeof showToast === 'function') {
            showToast("Không thể kết nối đến máy chủ.", 'error');
        }
    }
}
