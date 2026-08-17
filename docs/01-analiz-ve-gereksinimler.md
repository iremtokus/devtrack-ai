# DevTrack AI — Analiz ve Gereksinimler Dokümanı

## Doküman Hakkında

Bu doküman, DevTrack AI projesinin geliştirme öncesi ihtiyaçlarını, MVP sınırlarını ve kabul koşullarını tanımlar. Amaç; geliştirilecek ürünün hangi problemi çözeceğini, kullanıcıya hangi işlevleri sunacağını ve hangi kurallara uyması gerektiğini açık biçimde ortaya koymaktır.

## 1. Proje Tanımı

DevTrack AI, yazılım geliştirme alanında kendi kendine öğrenen bireyler için hazırlanan kişisel bir öğrenme takip uygulamasıdır. Kullanıcı, öğrenmek istediği konuyu bir roadmap olarak tanımlar; bu roadmap'i haftalara, haftaları da öğrenme görevlerine ayırır. Uygulama, kullanıcının bugün ve içinde bulunulan hafta ne çalışması gerektiğini görmesini, tamamladığı işleri işaretlemesini ve ilerlemesini takip etmesini sağlar.

DevTrack AI yalnızca yapılacaklar listesi olmayı hedeflemez. Görevlerin roadmap ve hafta bağlamında tutulması, öğrenme ilerlemesinin hesaplanması, geciken çalışmaların gösterilmesi, uygulama içi hatırlatmalar ve mevcut görevlere dayalı öneriler sunulması projenin öğrenme odaklı yönünü oluşturur.

## 2. Problem Tanımı

Yazılım öğrenen kişiler çoğu zaman birden fazla eğitim, video, okuma kaynağı ve uygulama çalışmasını aynı anda takip eder. Bu içerikler farklı platformlarda bulunduğunda günlük görevler, haftalık hedefler ve teslim tarihleri dağınık hâle gelir. Kullanıcı hangi konuya öncelik vermesi gerektiğini, ne kadar ilerlediğini veya hangi görevlerin geciktiğini kolayca göremeyebilir.

Genel amaçlı yapılacaklar uygulamaları görev kaydı tutabilse de öğrenme roadmap'i, hafta planı, çalışma süresi, kaynak türü ve öğrenme ilerlemesi gibi bağlamları birlikte sunmayabilir. DevTrack AI bu bilgileri tek bir yapıda toplayarak “Bugün ne çalışmalıyım?” ve “Bu hafta neyi bitirmeliyim?” sorularına açık cevap vermeyi amaçlar.

## 3. Projenin Amacı

Projenin temel amacı, bireysel kullanıcının yazılım öğrenme planını düzenli ve ölçülebilir şekilde yönetmesini sağlamaktır. Kullanıcı bir roadmap oluşturabilmeli, roadmap'i haftalara ayırabilmeli ve her haftaya görevler ekleyebilmelidir. Günlük ve haftalık görünümler yaklaşan çalışmaları görünür hâle getirmeli; görevlerin tamamlanması dashboard ve progress değerlerine yansımalıdır.

Uygulama ayrıca bugün tamamlanması gereken veya gecikmiş görevler için uygulama içi bildirimler üretmelidir. AI Learning Assistant, kullanıcının mevcut görev verilerini değerlendirerek çalışma önerileri sunmalıdır. Bu öneriler tavsiye niteliğinde olmalı; kullanıcının açık onayı olmadan görev verilerini değiştirmemelidir.

## 4. Hedef Kullanıcı

Projenin hedef kullanıcısı, yazılım veya teknoloji alanında kendi öğrenme planını takip etmek isteyen bireysel kullanıcıdır. Bu kullanıcı bir öğrenci, yeni mezun, kariyer değiştiren bir kişi veya belirli bir teknoloji için kendi çalışma programını hazırlayan junior developer olabilir.

Kullanıcının proje yönetimi konusunda ileri düzey bilgiye sahip olduğu varsayılmaz. Bu nedenle ekranlar ve işlemler açık, anlaşılır ve temel teknik bilgiyle kullanılabilir olmalıdır.

## 5. Kullanıcı Senaryosu

Elif, backend geliştirme öğrenmek isteyen yeni mezun bir yazılımcıdır. DevTrack AI'ye kayıt olur ve hesabına giriş yapar. “Backend Developer Roadmap” adında bir roadmap oluşturur ve çalışma planını haftalara ayırır. İlk haftaya “C# Temelleri” başlığını ekler; bu haftanın altına video izleme, okuma ve egzersiz görevleri oluşturur.

Elif her gün **Bugün** ekranına girerek o gün planladığı görevleri görür. **Bu Hafta** ekranından haftanın tamamını kontrol eder. Bir egzersizi bitirdiğinde görevi tamamlandı olarak işaretler ve Dashboard üzerindeki tamamlanan görev sayısının ve ilerleme yüzdesinin değiştiğini görür.

Bir görev son tarihini geçtiğinde uygulama içi bildirim oluşturur ve bildirimi okundu olarak işaretler. Daha sonra AI Asistan'a bugünkü çalışma önerisini ve haftalık özetini sorar. Geciken görevleri için yeniden planlama önerisi aldığında önerilen tarihleri önce inceler. Uygun bulduğu görevleri seçip açıkça onayladıktan sonra yeni tarihlerin uygulanmasını sağlar.

## 6. Fonksiyonel Gereksinimler

### Authentication

- **FR-01 — Kayıt:** Sistem, geçerli e-posta adresi, en az sekiz karakterli parola ve görünen ad ile yeni kullanıcı kaydı oluşturabilmelidir.
- **FR-02 — Giriş:** Kayıtlı kullanıcı, e-posta adresi ve parolasıyla giriş yapabilmeli; başarılı giriş sonucunda API erişimi için JWT almalıdır.
- **FR-03 — Çıkış:** Kullanıcı çıkış yapabilmeli; frontend saklanan JWT ve kullanıcı oturumu bilgisini temizlemelidir.
- **FR-04 — Korumalı erişim:** Giriş yapmamış kullanıcı korumalı frontend sayfalarına veya korumalı API kaynaklarına erişememelidir.

### Roadmap ve hafta yönetimi

- **FR-05 — Roadmap oluşturma:** Kullanıcı başlık, isteğe bağlı açıklama, hafta süresi ve başlangıç tarihi ile roadmap oluşturabilmelidir.
- **FR-06 — Roadmap listeleme ve detay:** Kullanıcı kendi roadmap'lerini listeleyebilmeli ve seçtiği roadmap'in haftalarını, görevlerini ve ilerlemesini görebilmelidir.
- **FR-07 — Roadmap güncelleme:** Kullanıcı kendisine ait roadmap'in temel bilgilerini güncelleyebilmelidir.
- **FR-08 — Roadmap silme:** Kullanıcı kendisine ait roadmap'i, ilişkili haftaları ve görevleriyle birlikte onay sonrasında silebilmelidir.
- **FR-09 — Roadmap week yönetimi:** Kullanıcı kendisine ait roadmap'e hafta ekleyebilmeli, haftayı düzenleyebilmeli ve haftayı ilişkili görevleriyle birlikte silebilmelidir.

### Görev yönetimi

- **FR-10 — Görev oluşturma:** Kullanıcı kendisine ait bir roadmap haftasına başlık, açıklama, tür, durum, öncelik, planlanan tarih, son tarih, tahmini süre ve isteğe bağlı kaynak URL içeren görev ekleyebilmelidir.
- **FR-11 — Görev görüntüleme ve listeleme:** Kullanıcı kendi görevlerini görüntüleyebilmeli; genel listede durum, roadmap ve tarih aralığı filtrelerini kullanabilmelidir.
- **FR-12 — Görev güncelleme:** Kullanıcı kendisine ait görevin desteklenen alanlarını güncelleyebilmelidir.
- **FR-13 — Görev silme:** Kullanıcı kendisine ait görevi onay sonrasında silebilmelidir.
- **FR-14 — Görev tamamlama:** Kullanıcı görevi tamamlandı olarak işaretleyebilmeli veya tamamlanma durumunu geri alabilmelidir.

### Takip ve ilerleme

- **FR-15 — Bugün görünümü:** Sistem, kullanıcının planlanan tarihi bugün olan görevlerini ayrı bir ekranda göstermelidir.
- **FR-16 — Bu Hafta görünümü:** Sistem, pazartesi–pazar aralığında planlanan görevleri günlere göre gruplayarak göstermeli ve haftalık tamamlanma özetini sunmalıdır.
- **FR-17 — Dashboard:** Dashboard; bugünkü görevleri, bu haftaki görevleri, geciken görevleri, yakın zamanda tamamlanan görevleri, yaklaşan son tarihleri, tamamlanan/kalan görev sayılarını ve learning streak değerini göstermelidir.
- **FR-18 — Progress:** Sistem genel, roadmap ve hafta seviyesindeki tamamlanma yüzdelerini tamamlanan görev sayısına göre hesaplamalıdır.

### Bildirimler

- **FR-19 — Reminder yönetimi:** Kullanıcı bugün tamamlanacak ve geciken incomplete görevler için uygulama içi reminder üretebilmeli, tüm veya yalnızca okunmamış reminder'ları listeleyebilmeli ve kendisine ait reminder'ı okundu olarak işaretleyebilmelidir.

### AI Learning Assistant

- **FR-20 — AI günlük çalışma önerisi:** Sistem, kullanıcının mevcut incomplete görevleri, tarihleri, öncelikleri ve tahmini sürelerinden bugünkü çalışma önerisi oluşturmalıdır.
- **FR-21 — AI yeniden planlama:** Sistem, gecikmiş veya planlanan tarihi geçmiş incomplete görevler için yeni tarih önerileri oluşturabilmelidir; öneri üretimi görev tarihlerini otomatik değiştirmemelidir.
- **FR-22 — AI haftalık özet:** Sistem, içinde bulunulan haftada planlanan ve tamamlanan görevlere göre haftalık ilerleme özeti oluşturmalıdır.
- **FR-23 — AI sonraki görev önerisi:** Sistem, overdue durumu, tarih ve öncelik bilgilerini kullanarak sıradaki öğrenme görevini önermelidir.
- **FR-24 — AI geride kalma analizi:** Sistem, overdue ve planlanan tarihi geçmiş incomplete görevlerden kullanıcının geride kalma durumunu açıklayan bir öneri oluşturmalıdır.
- **FR-25 — AI önerisini uygulama:** Yeniden planlama önerisi, yalnızca kullanıcı öneri kalemlerini seçip açıkça uyguladığında görev tarihlerini değiştirmelidir. Backend öneri ve görev sahipliğini, görev durumunu ve tarihleri uygulama öncesinde yeniden doğrulamalıdır.

## 7. Fonksiyonel Olmayan Gereksinimler

- **NFR-01 — Parola güvenliği:** Parolalar düz metin olarak saklanmamalı; güvenli bir password hashing mekanizması kullanılmalıdır.
- **NFR-02 — Authentication:** Korumalı API işlemlerinde geçerli, imzalı ve süreli JWT doğrulaması yapılmalıdır.
- **NFR-03 — Authorization ve ownership:** Her kullanıcı yalnızca kendi roadmap, hafta, görev, reminder ve AI recommendation kaynaklarına erişebilmelidir. Ownership frontend'e bırakılmamalıdır.
- **NFR-04 — MVP performansı:** Yerel geliştirme ortamında bireysel kullanıcıya ait normal büyüklükteki roadmap ve görev listeleri kabul edilebilir bekleme süresinde açılmalı; veritabanı erişimleri uygun yerlerde asynchronous olmalıdır. MVP için dağıtık cache veya ayrı performans altyapısı gerekmemektedir.
- **NFR-05 — Responsive arayüz:** Login, register ve bütün korumalı uygulama sayfaları masaüstü ve dar mobil ekranlarda kullanılabilir olmalıdır.
- **NFR-06 — Hata yönetimi:** API iç stack trace bilgisini normal kullanıcıya göstermemeli; doğrulama, bulunamama, yetkisiz erişim ve AI provider hataları öngörülebilir yanıtlarla ele alınmalıdır. Frontend boş ekran yerine anlaşılır Türkçe hata mesajı göstermelidir.
- **NFR-07 — Maintainability:** Kod katmanların mevcut sorumluluklarına uygun, junior developer tarafından takip edilebilir ve gereksiz mimari karmaşıklıktan uzak olmalıdır.
- **NFR-08 — Secret ve configuration güvenliği:** JWT key, database credential, parola ve API key kaynak kodda tutulmamalı veya loglanmamalı; environment/configuration üzerinden sağlanmalıdır.
- **NFR-09 — Veri bütünlüğü:** Önemli tarih, aralık, ilişki ve ownership kuralları backend tarafından doğrulanmalı; ilişkiler ve silme davranışları veritabanı modeliyle tutarlı olmalıdır.

## 8. İş Kuralları

- **BR-01:** Kullanıcı yalnızca kendi kaynaklarını okuyabilir, oluşturabilir, güncelleyebilir veya silebilir.
- **BR-02:** `UserId` request body'dan güvenilir kabul edilmez. Kullanıcı kimliği JWT içindeki `NameIdentifier` claim'inden alınır.
- **BR-03:** Başka bir kullanıcıya ait resource ID kullanıldığında özel kaynak açığa çıkarılmamalı; erişim güvenli biçimde engellenmelidir.
- **BR-04:** Roadmap süresi 1–104 hafta aralığında olmalıdır.
- **BR-05:** Week number en az 1 olmalı ve bağlı roadmap'in duration değerini aşmamalıdır.
- **BR-06:** Aynı roadmap içinde aynı week number ikinci kez kullanılamaz.
- **BR-07:** Roadmap duration, mevcut bir week number değerinden daha kısa olacak şekilde güncellenemez.
- **BR-08:** Bir task yalnızca authenticated kullanıcının roadmap'ine bağlı week içinde oluşturulabilir.
- **BR-09:** Task estimated duration 1–1440 dakika aralığında olmalıdır.
- **BR-10:** Task deadline, planned date değerinden önce olamaz.
- **BR-11:** Optional resource URL verilirse mutlak HTTP veya HTTPS adresi olmalıdır.
- **BR-12:** Task ilk kez completed yapıldığında `CompletedAtUtc` atanmalıdır. Completed durumda kalan görev güncellendiğinde geçerli completion timestamp korunmalıdır.
- **BR-13:** Completed durumundan incomplete duruma dönen task için `CompletedAtUtc` temizlenmeli ve completion geri alma işlemi status'u `Todo` yapmalıdır.
- **BR-14:** Completed task, deadline geçmiş olsa bile overdue kabul edilmez.
- **BR-15:** Incomplete task yalnızca deadline bugünden önceyse overdue kabul edilir.
- **BR-16:** Reminder yalnızca kullanıcıya ait incomplete ve due-today/overdue görevler için üretilmelidir.
- **BR-17:** Aynı kullanıcı ve task/event için duplicate reminder oluşturulmamalıdır.
- **BR-18:** AI provider yalnızca authenticated kullanıcıya ait learning context ile çalışmalıdır.
- **BR-19:** AI provider'ın döndürdüğü task reference ve proposed dates backend tarafından doğrulanmalıdır.
- **BR-20:** Completed task AI reschedule işlemiyle yeniden planlanamaz.
- **BR-21:** AI önerisi kullanıcı açıkça onaylamadan `LearningTask` tarihlerini değiştiremez. Proposal ve proposal items izlenebilirlik amacıyla kaydedilebilir; görev değişikliği ayrı apply işlemi gerektirir.
- **BR-22:** Başka bir kullanıcıya ait AI recommendation uygulanamaz ve aynı applied recommendation tekrar uygulanamaz.

## 9. MVP Kapsamı

MVP aşağıdaki ürün parçalarından oluşur:

- JWT tabanlı bireysel kullanıcı hesabı
- Roadmap, week ve task yönetimi
- Learning task type, status ve priority alanları
- Daily/weekly task views
- Completion ve overdue takibi
- Dashboard progress ve streak bilgileri
- In-app reminder üretme ve okundu takibi
- Beş ayrı local AI suggestion action'ı
- Kullanıcı onaylı AI reschedule apply akışı
- AI recommendation için temel traceability kaydı
- PostgreSQL persistence ve EF Core migration'ları
- Responsive Türkçe web arayüzü
- Backend otomatik testleri ve frontend build/lint kontrolleri

## 10. Kapsam Dışı

Aşağıdaki özellikler bu MVP'nin parçası değildir:

- Native mobil veya masaüstü push notification
- Otomatik çalışan background scheduler
- Harici LLM, OpenAI veya başka bir AI API entegrasyonu
- Sosyal profil, arkadaş ekleme veya içerik paylaşma
- Multi-user collaboration, ekip veya organizasyon yönetimi
- Microservice architecture, message queue, Kafka veya Redis
- Gelişmiş analytics, raporlama paneli veya veri dışa aktarma
- Takvim ve e-posta entegrasyonu
- Recurring tasks
- Şifre sıfırlama ve e-posta doğrulama
- Offline kullanım ve native mobil uygulama
- Production deployment otomasyonu

## 11. Kabul Kriterleri

- **AC-01:** Geçerli bilgilerle kayıt olan kullanıcı JWT alabilmeli ve korumalı uygulama ekranına erişebilmelidir.
- **AC-02:** Hatalı parola ile giriş denemesi yetkisiz yanıt almalı; duplicate e-posta ile ikinci hesap oluşturulamamalıdır.
- **AC-03:** Authenticated kullanıcı bir roadmap oluşturabilmeli, detayını açabilmeli, güncelleyebilmeli ve silebilmelidir.
- **AC-04:** Kullanıcı roadmap duration içinde bir week oluşturabilmeli; duration değerini aşan week number backend tarafından reddedilmelidir.
- **AC-05:** Kullanıcı kendisine ait week içine task oluşturabilmeli, task'ı görüntüleyebilmeli, güncelleyebilmeli ve silebilmelidir.
- **AC-06:** Kullanıcı Today ekranında bugüne planlanan, This Week ekranında pazartesi–pazar aralığına planlanan görevleri görebilmelidir.
- **AC-07:** Task completed yapıldığında completion timestamp atanmalı, görev overdue listesinden çıkmalı ve ilgili progress yüzdesi güncellenmelidir.
- **AC-08:** Completion geri alındığında completion timestamp temizlenmeli ve deadline geçmişse task yeniden overdue olmalıdır.
- **AC-09:** İkinci kullanıcı, başka kullanıcıya ait roadmap, week, task, reminder veya AI recommendation ID'siyle özel veriye erişememeli veya onu değiştirememelidir.
- **AC-10:** Due-today veya overdue task için reminder üretilebilmeli; aynı task/event için ikinci üretim duplicate reminder oluşturmamalıdır.
- **AC-11:** Kullanıcı kendisine ait reminder'ı read olarak işaretleyebilmeli; başka kullanıcının reminder'ını değiştirememelidir.
- **AC-12:** AI Asistan beş action için mevcut kullanıcı görevlerinden boş olmayan ve action'a uygun bir öneri üretebilmelidir.
- **AC-13:** AI reschedule önerisi üretildiğinde task tarihi değişmemeli; tarih yalnızca kullanıcı seçip apply işlemini tamamladığında güncellenmelidir.
- **AC-14:** Completed task, invalid task reference veya başka kullanıcıya ait recommendation için AI reschedule apply işlemi reddedilmelidir.
- **AC-15:** Backend `dotnet build backend/DevTrack.sln` ve `dotnet test backend/DevTrack.sln` komutlarını; frontend ise `npm run lint --prefix frontend` ve `npm run build --prefix frontend` komutlarını hatasız tamamlayabilmelidir.

## 12. Varsayımlar ve Kısıtlar

- Uygulama bireysel kullanım için tasarlanır; aynı kaynak üzerinde ortak çalışma yapılmaz.
- Geliştirme ve demo ortamında yerel PostgreSQL kullanılır ve `devtrack` veritabanının önceden oluşturulduğu varsayılır.
- PostgreSQL connection string ve JWT key environment/configuration üzerinden sağlanır.
- Frontend ve backend yerel geliştirmede ayrı process olarak çalışır; frontend varsayılan olarak `http://localhost:5173`, API `http://localhost:5000` adresini kullanır.
- AI provider `local-fallback-v1` ile sınırlıdır. Bu provider harici servis çağırmaz ve mevcut görev verilerinden deterministik sonuç üretir.
- AI çıktısı uzman görüşü veya kesin karar değildir; öğrenme planı önerisidir.
- Reminder üretimi kullanıcı işlemiyle tetiklenir; arka planda kendiliğinden çalışan scheduler yoktur.
- Backend testleri EF Core InMemory provider kullanır; gerçek PostgreSQL'e karşı ayrı otomatik entegrasyon test ortamı bulunmaz.
- Frontend için otomatik component veya end-to-end test altyapısı bulunmaz; TypeScript build ve lint temel kalite kontrolüdür.
- Tarih tabanlı backend hesapları mevcut uygulamada UTC gününü temel alır.

## Gereksinim Özeti

DevTrack AI MVP; kullanıcının kendi öğrenme roadmap'ini haftalara ve görevlere ayırmasını, günlük/haftalık planını takip etmesini, ilerlemesini görmesini ve geciken çalışmalarını fark etmesini sağlamalıdır. Güvenli kullanıcı izolasyonu backend tarafından uygulanmalı, önemli veri kuralları frontend'e bırakılmamalıdır. AI Asistan yalnızca mevcut kullanıcı verilerinden öneri üretmeli; backend öneriyi doğrulamalı ve kullanıcı açıkça karar vermeden görev verilerini değiştirmemelidir.

Bu dokümanda **25 fonksiyonel gereksinim**, **9 fonksiyonel olmayan gereksinim**, **22 iş kuralı** ve **15 ölçülebilir kabul kriteri** tanımlanmıştır.
