// --- 1. LOGIC CHO SIDEBAR ---
function initSidebar() {
    const allSideMenu = document.querySelectorAll('#sidebar .side-menu.top li a');
    const sidebar = document.getElementById('sidebar');

    if (!sidebar) return; // Kiểm tra an toàn

    allSideMenu.forEach(item => {
        const li = item.parentElement;
        item.addEventListener('click', function () {
            allSideMenu.forEach(i => i.parentElement.classList.remove('active'));
            li.classList.add('active');
        });
    });

    // Tự động ẩn sidebar nếu màn hình nhỏ (Tablet/Phone)
    if (window.innerWidth < 768) {
        sidebar.classList.add('hide');
    }
}

// --- 2. LOGIC CHO NAVBAR & TÌM KIẾM ---
function initNavbar() {
    const menuBar = document.querySelector('#content nav .bx.bx-menu');
    const sidebar = document.getElementById('sidebar'); // Quan trọng: phải lấy lại sidebar ở đây
    const searchButton = document.querySelector('#content nav form .form-input button');
    const searchButtonIcon = document.querySelector('#content nav form .form-input button .bx');
    const searchForm = document.querySelector('#content nav form');

    // Logic Đóng/Mở Sidebar
    if (menuBar && sidebar) {
        menuBar.addEventListener('click', function () {
            sidebar.classList.toggle('hide');
            console.log("Đã click menu!"); // Kiểm tra trong Console (F12)
        });
    }

    // Logic nút tìm kiếm trên màn hình nhỏ
    if (searchButton && searchForm) {
        searchButton.addEventListener('click', function (e) {
            if (window.innerWidth < 576) {
                e.preventDefault();
                searchForm.classList.toggle('show');
                if (searchForm.classList.contains('show')) {
                    searchButtonIcon.classList.replace('bx-search', 'bx-x');
                } else {
                    searchButtonIcon.classList.replace('bx-x', 'bx-search');
                }
            }
        });
    }

    // Xử lý khi resize màn hình
    window.addEventListener('resize', function () {
        if (this.innerWidth > 576) {
            if (searchButtonIcon) searchButtonIcon.classList.replace('bx-x', 'bx-search');
            if (searchForm) searchForm.classList.remove('show');
        }
    });
}

// --- 3. LOGIC CHO FILTER DROPDOWN ---
function toggleFilter() {
    var dropdown = document.getElementById('filterDropdown');
    if (dropdown) {
        dropdown.style.display = dropdown.style.display === 'none' ? 'block' : 'none';
    }
}

document.addEventListener('click', function(e) {
    if (!e.target.closest('.filter-wrapper')) {
        var dropdown = document.getElementById('filterDropdown');
        if (dropdown) dropdown.style.display = 'none';
    }
});