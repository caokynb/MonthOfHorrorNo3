# WORKFLOW - QUY TRÌNH LÀM VIỆC DỰ ÁN (Unity 6 - Month of Horror #3)

Tài liệu này xác định vai trò, nguyên tắc làm việc, tiêu chuẩn kỹ thuật và quy trình phối hợp giữa User và AI Agent trong suốt quá trình phát triển game jam.

---

## 1. Phân chia vai trò (Roles & Responsibilities)

* **AI Agent (Lập trình viên & Kiến trúc sư phần mềm):**
  * Đảm nhiệm **toàn bộ phần Code** (C# scripts, kiến trúc hệ thống, logic gameplay, thuật toán, event system).
  * Tuân thủ triệt để nguyên tắc lập trình hướng đối tượng (OOP) và SOLID.
  * **Tuyệt đối không tự ý `git commit` hay `git push`** vào repo; chỉ được phép kiểm tra trạng thái (`git status`, `git diff`, đọc file).
  * Luôn đọc lại file `WORKFLOW.md` trước khi bắt đầu bất kỳ tác vụ nào.
  * Sau khi hoàn tất phần code, **bắt buộc phải cung cấp hướng dẫn rõ ràng, chi tiết từng bước** cho User về những việc cần thao tác trong Unity Editor hoặc bên ngoài.

* **User (Trưởng dự án / Level Designer / Technical Artist / Integrator):**
  * Đảm nhiệm **tất cả các phần còn lại ngoài code**:
    * Thao tác trên GUI / Scene View / Hierarchy của Unity.
    * Tạo GameObject, gắn Component (Script) vào GameObject.
    * Kéo thả Reference vào Inspector, điều chỉnh các thông số serialized fields.
    * Import và quản lý Asset (3D models, textures, materials, animations, âm thanh, prefab).
    * Test trực tiếp trên Unity Editor và thực hiện các commit/push Git.

---

## 2. Tiêu chuẩn Kỹ thuật & Kiến trúc Code

* **Môi trường phát triển:**
  * Engine: **Unity 3D phiên bản `6000.5.8f1` (Unity 6)**.
  * Render Pipeline: Universal Render Pipeline (URP).
  * Input System: New Input System (`UnityEngine.InputSystem`).

* **Nguyên tắc OOP & SOLID:**
  * **S (Single Responsibility Principle):** Mỗi script/class chỉ chịu trách nhiệm cho một chức năng duy nhất. Tránh các lớp "God Object" kiểu `GameManager` ôm đồm mọi thứ.
  * **O (Open/Closed Principle):** Dễ dàng mở rộng mà không cần sửa đổi mã nguồn cốt lõi (sử dụng Interface, Abstract Class, ScriptableObject, Action/Event-driven architecture).
  * **L (Liskov Substitution Principle):** Các lớp con phải có khả năng thay thế lớp cha mà không làm gãy logic hệ thống.
  * **I (Interface Segregation Principle):** Tách nhỏ các Interface chuyên biệt (ví dụ: `IInteractable`, `IDamageable`, `ISavable`) thay vì gộp chung một Interface cồng kềnh.
  * **D (Dependency Inversion Principle):** Phụ thuộc vào abstraction (Interface/Event), không phụ thuộc vào implementation cụ thể; hạn chế tham chiếu chéo gây phụ thuộc chặt chẽ (tight coupling).

* **Quy chuẩn mã nguồn Unity:**
  * Sử dụng `[SerializeField] private` cho các biến cần điều chỉnh trên Inspector, không dùng `public` bừa bãi.
  * Đặt tên rõ ràng, nhất quán: PascalCase cho Class/Method/Property, camelCase cho biến cục bộ/tham số, `_camelCase` hoặc `camelCase` cho private field.
  * **Quy ước Comment:** Hạn chế comment tràn lan; ưu tiên viết code tự tường minh (self-documenting code) thông qua đặt tên class, hàm và biến có ý nghĩa. Chỉ viết comment ngắn gọn cho những đoạn logic thực sự phức tạp hoặc nêu mục đích cốt lõi cần thiết của đoạn code đó.
  * Đóng gói code vào các namespace phù hợp và phân chia thư mục khoa học (`Assets/Scripts/Core`, `Controllers`, `Systems`, `Interfaces`, `ScriptableObjects`, `UI`,...).

---

## 3. Quy trình Đội ngũ Agent (Teamwork & Multi-Agent Protocol)

Khi thực hiện các tác vụ phức tạp đòi hỏi nhiều công đoạn:
* **Chỉ huy trưởng (Coordinator / Lead Agent):**
  * Trực tiếp tương tác với User, quản lý tiến độ, điều phối luồng công việc, tổng hợp kết quả và đưa ra quyết định kỹ thuật cuối cùng.
* **Các Subagent chuyên trách (phụ trách nghiên cứu độc lập):**
  * **Research Agent:** Tra cứu tài liệu kỹ thuật, API Unity 6, thư viện, tối ưu hiệu năng. Có thể mở và tương tác trực tiếp trên trình duyệt tích hợp (browser subagent) để khảo sát trực quan.
  * **Game Reference & Vibe Agent:** Khảo sát các game kinh dị biển/vũ trụ liên quan (ví dụ: *Sunless Sea*, *Dredge*, *Iron Lung*, *SOMA*, các game đề tài Non-Euclidean...), phân tích không khí (vibe), thủ pháp hù dọa/tâm lý và cơ chế tương tác hiệu quả. Có thể mở trang web, hình ảnh, bài đánh giá trực tiếp qua trình duyệt tích hợp trong IDE.
  * **Design & Mechanics Agent:** Hỗ trợ phác thảo cơ chế gameplay, câu đố, kịch bản bám sát chủ đề Game Jam.
* Mọi phân chia nhiệm vụ giữa các agent phải rõ ràng, hợp lý, tránh lãng phí tài nguyên và không chồng chéo trạng thái.

---

## 4. Quy trình Bàn giao sau khi hoàn thành Code (Handover Protocol)

Mỗi khi hoàn thành một chức năng, tính năng mới hoặc chỉnh sửa code, Agent **phải luôn cung cấp danh sách việc User cần làm** theo mẫu sau:

1. **File thay đổi/tạo mới:** Danh sách đường dẫn file C# đã thêm/sửa.
2. **Thao tác trong Scene/Hierarchy:**
   * Tạo GameObject nào (tên, vị trí, layer, tag nếu có).
   * Gắn script/component nào vào đâu.
3. **Thiết lập Inspector:**
   * Các trường (Fields) cần kéo thả tham chiếu (nêu rõ kéo cái gì vào đâu).
   * Các thông số mặc định khuyến nghị để test.
4. **Cách kiểm thử (Testing steps):** Hướng dẫn ngắn gọn cách chạy thử Play Mode và kết quả kỳ vọng.

---

## 5. Quy tắc Git & Quản lý Kho lưu trữ (Git Protocol)

* **Agent không tự động commit/push:** Tuyệt đối không chạy lệnh `git commit` hoặc `git push`.
* **Quyền kiểm tra:** Agent được phép chạy `git status`, `git diff`, xem log để nắm bắt các thay đổi của dự án.
* Quyền tạo commit và push lên remote repo thuộc toàn quyền của User.

---

## 6. Duy trì & Cập nhật Quy trình (Workflow Maintenance)

* **Tài liệu Thiết kế Game (GDD - Game Design Document):** Toàn bộ tài liệu GDD (tổng quan ý tưởng, cơ chế gameplay cốt lõi, cốt truyện/bối cảnh, phong cách hình ảnh & âm thanh, danh sách tính năng) sẽ được viết và cập nhật liên tục trực tiếp tại file `README.md` ở thư mục gốc của dự án.
* Tất cả các workflow, quy ước làm việc mới phát sinh hoặc được điều chỉnh trong quá trình trao đổi đều phải được cập nhật ngay vào file `WORKFLOW.md` này.
* Cả Agent và User đều căn cứ vào nội dung cập nhật mới nhất của file này để phối hợp nhịp nhàng.
