# Ninja LegaCy

Phiên bản: Dự án Unity (phiên bản editor): 2022.3.62f2

Tổng quan
--------
Ninja LegaCy là một game hành động/phiêu lưu 2D với lối chơi điều khiển nhân vật ninja, chiến đấu với quái vật, thu thập vật phẩm và hoàn thành nhiệm vụ. Dự án được tổ chức trong thư mục `Assets` với nội dung chính trong `Assets/Resources/MyGame`.

Chơi như thế nào
-----------------
- Mục tiêu: Hoàn thành các nhiệm vụ trong từng màn, tiêu diệt quái vật và thu thập vật phẩm để nâng cấp nhân vật.
- Cơ chế: Di chuyển, tấn công, nhảy, sử dụng kỹ năng/tiêu hao năng lượng.

Điều khiển (mặc định)
---------------------
- Di chuyển: Phím mũi tên hoặc `A`/`D`
- Nhảy: `Space`
- Tấn công: `J` (hoặc chuột trái)
- Kỹ năng / Hành động phụ: `K` / `L` (tùy config)

Yêu cầu & Cài đặt
-----------------
- Unity Editor: 2022.3.62f2 (LTS) — mở bằng Unity Hub hoặc trực tiếp bằng phiên bản này.
- Hệ điều hành: Windows (phát triển), macOS và Linux có thể chạy nếu build tương thích.

Chạy dự án
---------
1. Mở Unity Hub → Add > chọn folder dự án `07_Ninja_LegaCy`.
2. Mở scene chính (tìm trong `Assets/Resources/MyGame/Scenes` nếu có). Nếu không rõ, mở scene có tên chứa "Main" hoặc kiểm tra thư mục `Resources/MyGame/Scenes`.
3. Nhấn Play trong Editor để thử nghiệm.

Xây dựng (Build)
----------------
1. File → Build Settings → thêm scene chính vào Scenes In Build.
2. Chọn nền tảng (PC, Mac & Linux Standalone) và nhấn Build.

Cấu trúc quan trọng
-------------------
- `Assets/Resources/MyGame/Scripts`: mã nguồn game (Enemy, Player, UI,...).
- `Assets/Resources/MyGame/Scenes`: các scene của game.
- `Assets/Game Resources`: tài nguyên (sprites, âm thanh, font...).

Ghi chú cho người phát triển
----------------------------
- Khi thêm assets lớn, theo dõi `Library` và `.meta` để tránh xung đột.
- Sử dụng Git (hoặc hệ thống quản lý phiên bản) và commit thường xuyên.

Credits
-------
- Tác giả / Nhóm phát triển: (Bạn có thể điền tên ở đây)
- Tài nguyên: sprites, âm thanh, fonts (khai báo license tương ứng nếu dùng tài nguyên bên thứ ba).

License
-------
Mặc định: Chưa khai báo. Nếu bạn muốn chia sẻ công khai, cân nhắc sử dụng MIT hoặc GPL và thêm file `LICENSE` tương ứng.

Liên hệ
-------
Vui lòng cập nhật phần liên hệ trong file này nếu cần — ví dụ email hoặc repo GitHub.

---
Tạo tự động bởi trợ lý; bạn có thể sửa nội dung này để thêm hướng dẫn đặc thù, controls chi tiết, hoặc thông tin build chính xác.
