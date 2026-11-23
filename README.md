https://itch.io/jam/month-of-horror-3
# GAME DESIGN DOCUMENT (GDD)
## Dự án: Echoes of the Endless Deep (Vọng Âm Biển Đêm)

*Tài liệu thiết kế trò chơi tham gia game jam "Month of Horror #3: Endless Ocean" trên itch.io.*

---

## 1. Tổng quan Dự án (Project Overview)
* **Tên Game (Tạm thời):** Echoes of the Endless Deep
* **Engine & Môi trường:** Unity 3D (6000.5.8f1) URP
* **Nền tảng:** PC (Windows)
* **Thể loại:** 2.5D Top-Down Psychological Horror / Marine Exploration
* **Góc nhìn:** Top-Down 2.5D (Camera 3D nhìn nghiêng từ trên xuống, mặt nước/môi trường 3D, nhân vật/quái vật/vật thể là 2D Sprite Billboarding)
* **Chủ đề Game Jam:** Endless Ocean (Đại dương Vô Tận)
* **Thử thách áp dụng (Bonus Challenges):**
  - [x] **1. Non-Euclidean Space:** Các cánh cửa trên các hòn đảo dẫn đến những căn phòng có kích thước hoặc vị trí phi lý.
  - [x] **2. Truly Endless:** Biển lặp vô tận (Infinite Looping/Floating Origin), không có bờ cõi giới hạn.
  - [x] **3. Islands Out Of Time:** Các hòn đảo tí hon trôi dạt ngẫu nhiên, chứa các mảnh vỡ thời gian (bốt điện thoại công cộng, phòng khách cổ, trạm radio bỏ hoang).

---

## 2. Ý tưởng cốt lõi & Cảm xúc chủ đạo (Core Concept & Vibe)
* **Ý tưởng cốt lõi (Từ `myidea.txt`):** 
  * Đây là **một giấc mơ (A Dream)** — người chơi trôi dạt trên đại dương vô tận của tiềm thức.
  * **Những hòn đảo tượng trưng cho những sai lầm, những lời tự phán xét và vết thương tâm lý của chính mình (The islands represent your mistakes as a person).**
  * **Tiến trình tuyến tính (In order):** Người chơi tìm và ghé thăm từng hòn đảo một theo thứ tự từ 1 đến 7.
  * **Sự leo thang tuyệt vọng (Escalation):** Càng đi về sau, các sai lầm càng tồi tệ hơn, bầu không khí xung quanh càng trở nên đen tối, ngột ngạt. Cảm giác như có ai đó vô hình đang dõi theo bạn mọi lúc, và **chiếc thuyền càng lúc càng khó điều khiển hơn** (nước biển nặng nề, dòng chảy ghì chặt con thuyền).
* **Pitch (Ý tưởng 1 câu):** Một hành trình ác mộng xuyên qua đại dương tiềm thức, nơi mỗi hòn đảo là một bản án tâm lý phơi bày những sai lầm ngày một tồi tệ hơn, trong khi con thuyền dần mất lái giữa sự rình rập của bóng tối.
* **Tone & Mood:** Kinh dị tâm lý sâu sắc (Deep Psychological Horror), chấn thương tâm lý (Trauma/Depressive Dread), ngột ngạt, cô độc, hoang tưởng (Paranoia).
* **Triết lý Kể chuyện: Trừu tượng & Mập mờ (Ambiguous & Minimalist Storytelling):**
  * **Tuyệt đối không giải thích:** Trò chơi không đưa ra bất kỳ lời giải thích nào cho các thông điệp. Không có nhật ký dài dòng, không có người kể chuyện (narrator) phân trần.
  * **Sự lạnh lùng của ngôn từ:** Chỉ những câu chữ cộc lốc, sắc lẹm xuất hiện đơn độc giữa màn đêm (ví dụ: *"Why didn't you smile?"*, *"It's all your fault."*). Để người chơi tự gặm nhấm, tự soi chiếu và kết nối những mảnh vỡ cảm xúc theo trải nghiệm cá nhân của chính họ.
* **Sự kết hợp hoàn hảo với phong cách MS Paint:** Nét vẽ tay MS Paint thô ráp, méo mó, lạnh lùng thể hiện trọn vẹn những câu từ tự phán xét nghiệt ngã và những hình ảnh biểu tượng kỳ dị trong tiềm thức.

---

## 3. Vòng lặp chơi cốt lõi (Core Game Loop)
1. **Lắng nghe & Lái thuyền vượt sóng nặng (Sail & Listen):** Lái thuyền theo hướng âm thanh vọng lại từ hòn đảo tiếp theo. Khi qua từng đảo, mặt nước 3D càng trở nên đen đặc, nhớp nháp, chiếc thuyền điều khiển càng khó khăn, nặng nề hơn.
2. **Cập bến hòn đảo phán xét (Dock & Face the Prompt):** Neo thuyền vào hòn đảo nhỏ nổi lên giữa biển đêm. Bước lên đảo với ngọn đèn bão le lói.
3. **Phơi bày sai lầm (Reveal the Mistake):** Tương tác với khung cảnh và các vật thể biểu tượng mang phong cách MS Paint. Mỗi hòn đảo gắn liền với một câu hỏi/lời phán xét nhức nhối.
4. **Bóng tối & Sự rình rập (The Watcher & The Abyss):** Ở rìa tầm nhìn của đèn bão, hình bóng của một thực thể theo dõi (The Watcher) càng lúc càng tiến lại gần hơn.
5. **Kích hoạt âm thanh tiếp theo:** Hoàn thành tương tác trên đảo để mở ra tín hiệu âm thanh của hòn đảo kế tiếp.

---

## 4. Hành trình 7 Hòn Đảo Phán Xét (The 7 Islands of Guilt)

| STT | Tên Hòn Đảo & Lời Phán Xét | Ý nghĩa Tâm lý & Khung cảnh MS Paint | Biến đổi Môi trường & Thuyền |
| :--- | :--- | :--- | :--- |
| **Đảo 1** | *"Why didn't you smile?"* | Nụ cười gượng ép, sự kìm nén cảm xúc để làm hài lòng người khác. Khung cảnh: Một tấm gương nứt vẽ khuôn mặt cười méo mó bằng nét cọ vàng trên cát. | Biển đêm tĩnh lặng, sương mù mỏng, thuyền lái nhẹ nhàng. |
| **Đảo 2** | *"Did you deserve it?"* | Sự hoài nghi bản thân, cảm giác không xứng đáng, tự đổ lỗi khi bị đối xử tệ bạc. Khung cảnh: Một chiếc ghế gãy, một món quà bị bóc dở vứt dưới vũng nước. | Ánh sáng đèn pin bắt đầu chớp nháy nhẹ, tiếng gió lạnh rít qua mạn thuyền. |
| **Đảo 3** | *"You were just lucky."* | Hội chứng kẻ mạo danh (Imposter syndrome), tự phủ nhận mọi nỗ lực và giá trị của bản thân. Khung cảnh: Một con xúc xắc khổng lồ bị vỡ đôi, những bức vẽ chứng nhận bị gạch chéo đỏ. | Nước biển chuyển sang màu xám đục, thuyền bắt đầu có độ trễ khi bẻ lái. |
| **Đảo 4** | *"Why are you crying?"* | Sự xấu hổ khi yếu đuối, bị cấm đoán thể hiện nỗi đau, sự cô lập. Khung cảnh: Một chiếc ô đen rách nát, mặt đất ngập nước mắt, âm thanh tiếng nấc nghẹn ngào. | Sương mù dày đặc che khuất tầm nhìn, thỉnh thoảng thấy bóng đen thoáng qua ở góc màn hình. |
| **Đảo 5** | *"You caused all of this."* | Cảm giác tội lỗi khi làm tổn thương người khác, sự đổ vỡ không thể hàn gắn. Khung cảnh: Một khung cửa đứng giữa đảo (*Non-Euclidean*), bên trong là căn phòng đồ đạc vỡ nát. | Nước biển đen quánh như dầu, thuyền di chuyển chậm chạp, tiếng kim loại cào vào đáy thuyền. |
| **Đảo 6** | *"You never tried."* | Nỗi đau của sự buông xuôi, bất lực, cảm giác hèn nhát khi đã không cố gắng hết sức. Khung cảnh: Những bức thư chưa gửi, chiếc đồng hồ cát chảy ngược, một hình nhân gục đầu. | Tầm nhìn đèn bão co hẹp lại, kẻ theo dõi (The Watcher) đứng ngay mép đảo nhìn chằm chằm. |
| **Đảo 7** | *"It's all your fault."* | Bản án tối thượng: Sự sụp đổ hoàn toàn, tự kết án chính mình. Khung cảnh: Một bục đá đen vươn lên giữa xoáy nước, nơi người chơi đối mặt trực tiếp với bản ngã tội lỗi. | Biển động dữ dội, chiếc thuyền gần như bất động trước sức cản của dòng nước. |

* **Hồi Kết (The Choice / The Awakening):**
  * Tại đảo số 7, người chơi phải đưa ra lựa chọn cuối cùng: Tiếp tục tự hủy hoại bản thân trong vòng lặp tội lỗi vô tận (*Truly Endless Nightmare*), hay dũng cảm tha thứ và buông bỏ để thức tỉnh khỏi cơn mơ.

---

## 5. Các Hệ thống Cơ chế Kỹ thuật (Core Systems Architecture - OOP & SOLID)

### A. Hệ thống Di chuyển & Điều khiển (Controller System)
* `ShipController`: Điều khiển lái thuyền trên mặt nước 3D (quán tính nước, rẽ lái, đèn pha mũi thuyền).
* `PlayerController`: Điều khiển nhân vật đi bộ trên boong thuyền và trên các hòn đảo (di chuyển 8 hướng, xoay đèn bão theo chuột/hướng nhìn).
* `State Switcher`: Chuyển đổi mượt mà giữa chế độ Lái thuyền (Sailing Mode) và Đi bộ (On-Foot Mode).

### B. Hệ thống Âm thanh Định hướng (Audio Beacon & Spatial Sound System)
* `AudioBeacon`: Gắn trên mỗi đảo, phát ra âm thanh 3D lan tỏa trong không gian.
* `SonarCompass / AudioIndicator`: Chỉ báo trực quan nhẹ nhàng (gợn sóng âm thanh/kim chỉ hướng mờ ảo) giúp người chơi biết hướng âm thanh phát ra mà không phá vỡ tính kinh dị.

### C. Hệ thống Tương tác (Interaction System)
* Tuân thủ Interface Segregation (`IInteractable`):
  * `Inspectable`: Xem tài liệu, di vật.
  * `Triggerable`: Nhấc điện thoại, gạt cần, mở cửa.
  * `FuelSource`: Nhặt thêm dầu đèn.

### D. Hệ thống Đèn bão & Bóng tối (Light & Dread System)
* Quản lý lượng dầu đèn (Fuel / Sanity). Khi dầu cạn, tầm nhìn thu hẹp dần và âm thanh méo mó bắt đầu xuất hiện.

### E. Hệ thống Đảo Lạc Trôi (Island Spawner & Non-Euclidean Portals)
* Spawn các hòn đảo ngẫu nhiên theo tiến trình âm thanh.
* Cửa chuyển dịch không gian tức thời (Seamless Portal) sang căn phòng bên trong đảo.

---

## 6. Phong cách Nghe - Nhìn (Art & Audio Direction)
* **Hình ảnh (Art Style): MS Paint 2D Horror x 3D Atmospheric Void**
  * **Phong cách 2D:** Sử dụng các hình vẽ tay theo phong cách **MS Paint** (nét vẽ nguệch ngoạc, thô ráp, tối giản, mang hơi hướng liminal/analog horror kỳ dị giống các tựa game cult classic như *FAITH*, *World of Horror*, *Baldi's Basics*).
  * **Môi trường 3D:** Đại dương 3D đen ngòm, sương mù dày đặc (Volumetric Fog) và ánh sáng thể tích thời gian thực trong Unity 6 URP. Sự tương phản giữa nét vẽ MS Paint 2D thô sơ và không gian 3D u tối tạo nên cảm giác kinh dị siêu thực (uncanny/surreal horror) độc nhất vô nhị.
  * **Thiết lập Texture trong Unity:** Texture Type: `Sprite (2D and UI)`, Filter Mode: `Point (no filter)`, Compression: `None` để giữ nguyên độ sắc nét nguyên bản của nét vẽ Paint.
  * **Tuân thủ bản quyền:** 100% vẽ tay bởi người làm game, hoàn toàn không sử dụng Generative AI theo đúng quy định của Game Jam.
* **Âm thanh (Audio Direction - Trọng tâm tạo cảm giác rùng rợn):** 
  * **Hệ thống Vọng Âm Không Gian 3D (Haunting Spatial Echo Beacons):** Đây là "linh hồn" tạo nên nỗi sợ của game. Tiếng vọng âm lẻ loi, ma quái từ hòn đảo mục tiêu phát ra giữa đại dương đen đặc (tiếng cười cắt ngang, tiếng kim loại cào xé, tiếng tích tắc đồng hồ, tiếng thì thầm méo mó) sẽ kích hoạt phản xạ thính giác và cảm giác lạnh sống lưng cho người chơi.
  * **Âm thanh Biển & Tâm trí:** Tiếng nước biển 3D dập dềnh u tối, tiếng gỗ thuyền kêu răng rắc khi nước cản nặng dần, tiếng gió rít buốt giá, và nhịp tim đập dồn dập (heartbeat) khi Kẻ Theo Dõi (The Watcher) áp sát rìa ánh đèn.

---

## 7. Lộ trình Triển khai (Milestones & Roadmap)
* [x] **Giai đoạn 1:** Khởi tạo dự án, thống nhất Workflow & cấu trúc GDD.
* [x] **Giai đoạn 2:** Chốt ý tưởng chi tiết (Echoes of the Endless Deep), định hình cơ chế.
* [ ] **Giai đoạn 3:** Xây dựng Kiến trúc nền tảng (Event Bus, Input System, Player & Ship Controller 2.5D).
* [ ] **Giai đoạn 4:** Xây dựng Cơ chế Âm thanh Định vị (Audio Beacon) & Hệ thống Đèn bão (Lantern/Darkness).
* [ ] **Giai đoạn 5:** Xây dựng Hệ thống Tương tác (Interaction System) & Đảo dị thường (Islands Out of Time).
* [ ] **Giai đoạn 6:** Thêm hiệu ứng không gian phi Euclid (Non-Euclidean Doorways).
* [ ] **Giai đoạn 7:** Level Design, tích hợp âm thanh, sương mù URP & Đánh bóng (Polish).
* [ ] **Giai đoạn 8:** Build, test & nộp game lên itch.io.
