# DevTrack AI — AI Çalışma Günlüğü

## Günlük Hakkında

Bu belge, DevTrack AI'yi geliştirirken AI coding assistant ile nasıl çalıştığımı anlatıyor. Buradaki prompt örnekleri uzun konuşmaların birebir dökümü değildir. Gerçek çalışma adımlarında verdiğim talimatların kısa ve okunabilir temsilidir. Yapılmayan bir geliştirme, test veya hata süreci eklenmemiştir.

## 1. AI'ı Projede Nasıl Kullandım?

AI coding assistant'ı projeyi benim yerime tamamen yapan bir araç olarak değil, geliştirme sürecini hızlandıran bir yardımcı olarak kullandım. Projenin klasör yapısını oluşturma, backend ve frontend kodlarını geliştirme, hata nedenlerini araştırma, test yazma, küçük refactoring işleri ve dokümantasyon hazırlama konularında destek aldım.

AI'ın oluşturduğu dosyaları veya “çalışıyor” sonucunu doğrudan doğru kabul etmedim. Backend ve frontend build komutlarını çalıştırdım, test sonuçlarını kontrol ettim ve uygulamayı gerçek PostgreSQL ile açtım. Özellikle kayıt, roadmap, görev, bildirim ve AI ekranlarını tarayıcıdan kullandım. Tarayıcıda gördüğüm CORS ve dil tutarlılığı gibi sorunları yeni talimatlarla tekrar AI'a ilettim.

Ürün kapsamı, Türkçe arayüz, kullanıcı deneyimi ve AI önerilerinin otomatik uygulanmaması gibi kararları ben verdim. AI daha çok bu kararları teknik olarak uygulama ve doğrulama konusunda yardımcı oldu.

## 2. Kullanılan Context Dosyaları

AI çalışma sırasında yalnızca o anki promptu değil, repository içindeki gerçek dosyaları da inceleyerek ilerledi:

- **`PROJECT.md`:** Projenin mimari sınırlarını, ownership kurallarını, AI güvenlik ilkelerini, change rules ve Definition of Done koşullarını içeriyor.
- **`README.md`:** Kurulum, çalıştırma, teknoloji ve mevcut MVP sınırları için insan odaklı proje özeti sağlıyor.
- **Kaynak kod:** Backend controller'ları, domain entity'leri, EF Core DbContext, fallback AI service ve React sayfaları gerçek davranışı anlamak için kullanıldı.
- **Test dosyaları:** `BackendFoundationTests.cs`, hangi davranışların otomatik test edildiğini ve hangi alanlarda ek kanıt gerektiğini gösterdi.
- **Configuration dosyaları:** `appsettings`, `.env.example`, `frontend/.env.example`, `.gitignore`, launch settings ve local EF tool manifest'i çalışma ortamını anlamak için incelendi.

`PROJECT.md`, sonraki AI çalışmalarında kapsamın kendiliğinden büyümemesi için kalıcı bağlam olarak hazırlandı. Özellikle single-application yapısının korunması, `UserId` değerinin JWT'den alınması, secret'ların kaynak kodda tutulmaması ve “AI suggests, backend validates, user decides” ilkesi bu dosyada açıkça yer alıyor.

## 3. AI Çalışma Günlüğü

### Aşama 1 — MVP'nin belirlenmesi

**Amaç**

Yazılım öğrenme sürecini takip edecek uygulamanın ilk sürümünde hangi özelliklerin bulunacağını belirlemek.

**AI'a verdiğim prompt örneği**

> Yazılım öğrenme sürecimi takip edebileceğim bir uygulama yapmak istiyorum. Roadmap, haftalık görevler, günlük takip ve hatırlatmalar olsun. Önce MVP olarak neler olması gerektiğini çıkaralım.

**AI'ın yaptığı**

Roadmap, week, learning task, Today, This Week, Dashboard, progress, reminder ve AI learning suggestion parçalarını tek uygulama kapsamında düzenledi. Authentication, ownership, validation, test ve dokümantasyon ihtiyaçlarını da teknik kapsamın içine yerleştirdi.

**Benim kontrolüm / müdahalem**

Uygulamanın generic todo list olmamasına, öğrenme görevlerini roadmap ve hafta bağlamında tutmasına karar verdim. Microservice veya gereksiz enterprise altyapı istemediğimi açıkça belirttim.

**Sonuç**

Tek kullanıcıya yönelik, öğrenme takibi odaklı ve aşamalara ayrılmış bir MVP kapsamı oluştu.

### Aşama 2 — Backend ve veritabanı

**Amaç**

.NET 8 API, PostgreSQL persistence, JWT authentication ve temel veri modelini kurmak.

**AI'a verdiğim prompt örneği**

> Backend'i .NET 8 ve PostgreSQL ile yapalım. Kullanıcı kayıt ve giriş yapabilsin, JWT kullanalım. Roadmap, hafta ve görev yapısını da buna göre oluşturalım.

**AI'ın yaptığı**

ASP.NET Core solution katmanlarını, entity'leri, DTO'ları, DbContext ilişkilerini, password hashing ve JWT akışını oluşturdu. EF Core/Npgsql package uyumsuzluğu giderildi, repository-local EF CLI hizalandı ve migration'lar hazırlandı.

**Benim kontrolüm / müdahalem**

Package sürümlerinin .NET 8 ile uyumlu olmasını, connection string ve JWT key'in kaynak koddan çıkarılmasını istedim. Yerel PostgreSQL'in kullanılmasını ve mevcut verilerin silinmemesini şart koştum.

**Sonuç**

PostgreSQL kullanan, JWT ile korunan ve migration uygulanabilen backend temeli çalışır hâle geldi.

### Aşama 3 — Frontend

**Amaç**

Mevcut backend endpoint'leriyle çalışan Türkçe ve responsive web arayüzü hazırlamak.

**AI'a verdiğim prompt örneği**

> React, TypeScript, Vite ve Tailwind ile frontend'i oluşturalım. Arayüz Türkçe olsun ve mevcut backend endpoint'lerini kullansın.

**AI'ın yaptığı**

Login, register, dashboard, Today, This Week, Roadmaps, Roadmap Detail, AI Assistant ve Notifications sayfalarını oluşturdu. Ortak API client, JWT handling, protected routes, responsive navigation, loading, error ve empty state bileşenlerini ekledi.

**Benim kontrolüm / müdahalem**

Arayüzün sade, öğrenci projesine uygun ve aşırı tasarlanmış olmamasını istedim. Frontend'in backend'de olmayan endpoint veya enum üretmemesini özellikle belirttim.

**Sonuç**

Backend'i source of truth olarak kullanan, build ve lint kontrollerini geçen React arayüzü hazırlandı.

### Aşama 4 — Roadmap ve görev akışı

**Amaç**

Roadmap → week → task hiyerarşisini ve CRUD işlemlerini güvenli şekilde tamamlamak.

**AI'a verdiğim prompt örneği**

> Roadmap'e hafta, haftaya görev ekleme akışını tamamlayalım. Kullanıcı sadece kendi verisini görebilsin ve ID değiştirerek başkasının kaynağına erişemesin.

**AI'ın yaptığı**

Roadmap, week ve task endpoint'lerini tamamladı. Week number/duration doğrulaması, task date kontrolleri, update/delete davranışları ve JWT kullanıcısına göre ownership filtreleri eklendi.

**Benim kontrolüm / müdahalem**

İkinci kullanıcıyla farklı resource ID'leri kullanarak read, update ve delete denemeleri yapılmasını istedim. Başkasına ait kaynağın varlığını açıklamayan `404` yaklaşımını uygun buldum.

**Sonuç**

Kullanıcıya özel roadmap, week ve task akışı oluştu; cross-user erişimler engellendi.

### Aşama 5 — Dashboard ve ilerleme

**Amaç**

Task tamamlandığında progress, overdue ve Dashboard bilgilerinin tutarlı değişmesini sağlamak.

**AI'a verdiğim prompt örneği**

> Bir görevi tamamlayınca roadmap ve hafta ilerlemesi değişsin. Tamamlanan görev gecikmiş görünmesin ve completion zamanı doğru tutulsun.

**AI'ın yaptığı**

Overall, roadmap ve week progress hesaplarını tamamlanan görev oranına bağladı. Completion timestamp oluşturma/temizleme, overdue kuralı ve learning streak hesaplarını uyguladı. İlgili backend testlerini ekledi.

**Benim kontrolüm / müdahalem**

Gerçek PostgreSQL akışında üç görev oluşturdum ve birini tamamladım. Progress değerinin `%0` değerinden `%33,3` değerine çıktığını, timestamp oluştuğunu ve completed görevin overdue olmadığını kontrol ettim.

**Sonuç**

Task completion, Dashboard sayaçları ve progress göstergeleri birbiriyle tutarlı çalıştı.

### Aşama 6 — Bildirimler

**Amaç**

Bugün tamamlanacak ve geciken görevleri kullanıcıya uygulama içinde hatırlatmak.

**AI'a verdiğim prompt örneği**

> Bugün teslim edilecek ve geciken görevler için basit uygulama içi bildirim ekleyelim. Aynı görev için tekrar tekrar bildirim oluşmasın.

**AI'ın yaptığı**

Reminder listeleme, okunmamış filtreleme, üretme ve read endpoint'lerini oluşturdu. `DueToday`/`Overdue` ayrımını ve `(UserId, EventKey)` unique yaklaşımını ekledi.

**Benim kontrolüm / müdahalem**

Reminder'ı gerçek uygulamada ürettim, ekranda gördüm ve read yaptım. Aynı event için ikinci üretimin duplicate kayıt oluşturmadığını kontrol ettim.

**Sonuç**

Background scheduler gerektirmeyen, kullanıcı tarafından tetiklenen in-app reminder akışı tamamlandı.

### Aşama 7 — AI Assistant

**Amaç**

Harici API key gerektirmeden mevcut görev verilerinden yararlı öğrenme önerileri sunmak.

**AI'a verdiğim prompt örneği**

> AI Asistan mevcut görevleri kullansın. Bugün, haftalık özet, sonraki görev, geride kalma ve yeniden planlama önerisi versin. Ama görevleri kendi kendine değiştirmesin.

**AI'ın yaptığı**

`IAiLearningService` abstraction'ını ve `local-fallback-v1` implementasyonunu hazırladı. Beş action için structured sonuçlar, AI traceability kayıtları ve reschedule proposal/apply endpoint'leri oluşturdu. Provider çıktısı, task ID'leri, ownership ve tarihler backend'de doğrulandı.

**Benim kontrolüm / müdahalem**

“AI suggests, backend validates, user decides” kuralını proje kararı olarak koydum. Proposal oluşturmanın task tarihini değiştirmediğini ve yalnızca seçip **Öneriyi Uygula** dediğimde tarihin değiştiğini gerçek veride kontrol ettim.

**Sonuç**

Harici LLM olmadan çalışan ve önemli veri değişikliğini kullanıcı onayına bırakan AI Assistant tamamlandı.

### Aşama 8 — CORS hatası

**Amaç**

Frontend ve backend ayrı portlarda çalışırken Register isteğinin neden başarısız olduğunu bulmak.

**AI'a verdiğim prompt örneği**

> Frontend çalışıyor ama kayıt olurken sunucuya ulaşılamıyor diyor. Backend 5000, frontend 5173'te. CORS tarafını kontrol eder misin? Çalışan yerleri ve veritabanını bozma.

**AI'ın yaptığı**

Backend CORS registration ve middleware sırasını inceledi. Development ortamında yalnızca `http://localhost:5173` origin'ine; gerekli HTTP method'larına, `Content-Type` ve `Authorization` header'larına izin veren policy'yi düzeltti. `AllowAnyOrigin` kullanmadı ve OPTIONS preflight için otomatik test ekledi.

**Benim kontrolüm / müdahalem**

İlk olarak Register ekranında **“Sunucuya ulaşılamıyor. Backend bağlantısını kontrol et.”** mesajını gördüm. Backend health endpoint'i çalıştığı için sorunun yalnızca sunucunun kapalı olması olmadığını düşündüm. Chrome DevTools Console ve Network panelinde **“No 'Access-Control-Allow-Origin' header”** hatasını tespit edip AI'a ilettim. Değişiklikten sonra backend'i yeniden başlattım ve Register akışını tekrar tarayıcıdan denedim.

**Sonuç**

Preflight doğru CORS header'larıyla yanıt verdi ve kayıt akışı tarayıcıdan çalıştı.

### Aşama 9 — AI Asistan dil problemi

**Amaç**

Türkçe arayüz içinde İngilizce kalan fallback çıktılarını tutarlı hâle getirmek.

**AI'a verdiğim prompt örneği**

> AI Asistan kısmında bazı sonuçlar hâlâ İngilizce geliyor. Uygulamanın geri kalanı Türkçe, burayı da Türkçeleştirelim ama AI mantığını değiştirme.

**AI'ın yaptığı**

`Reschedule proposal`, `Falling-behind explanation`, İngilizce açıklamalar, adımlar, reason ve disclaimer gibi kullanıcıya görünen `local-fallback-v1` metinlerini doğal Türkçeye çevirdi. Provider identifier, action logic, endpoint, entity ve migration'ları değiştirmedi. Beş action başlığı için regression kontrolü ekledi.

**Benim kontrolüm / müdahalem**

Sorunu otomatik testten değil, AI Asistan ekranındaki gerçek çıktıları okuyarak fark ettim. Backend'i yeniden başlatıp önerileri ekrandan tekrar ürettim ve Türkçe metinleri kontrol ettim.

**Sonuç**

AI Assistant çıktıları arayüz diliyle tutarlı hâle geldi ve backend testleri 19/19 başarılı kaldı.

### Aşama 10 — Son kalite kontrolü

**Amaç**

MVP'yi teslim öncesinde bütün olarak incelemek ve yalnızca kanıtlanan sorunları düzeltmek.

**AI'a verdiğim prompt örneği**

> Son kalite kontrolünü yapalım. Authentication, CRUD, dashboard, bildirimler ve AI akışlarını kontrol et. Loading, error, empty state ve Türkçe metinlere de bak; çalışan özellikleri bozma.

**AI'ın yaptığı**

Backend controller'larını, ownership filtrelerini, frontend route ve state yapılarını inceledi. Build, test, lint ve production build komutlarını çalıştırdı. Kullanıcıya görünen İngilizce validation/reminder metinlerini ve görev formundaki enum etiketlerini Türkçeleştirdi.

**Benim kontrolüm / müdahalem**

Yeni özellik eklenmemesini, PostgreSQL verilerinin ve migration'ların değişmemesini istedim. Tarayıcıyla doğrulanamayan noktaların başarılı gibi yazılmamasını talep ettim.

**Sonuç**

Backend sıfır warning/sıfır error ile build edildi, 19 test geçti; frontend lint ve production build başarılı tamamlandı. Kalan test otomasyonu sınırlamaları açıkça raporlandı.

### Aşama 11 — Dokümantasyon

**Amaç**

Projeyi GitHub, değerlendirme ve gelecekteki geliştirme çalışmaları için anlaşılır biçimde belgelemek.

**AI'a verdiğim prompt örneği**

> Şimdi gerçek projeye göre README, kalıcı PROJECT kuralları ve analiz, mimari, kullanıcı ve test dokümanlarını hazırlayalım. Olmayan özellikleri yazma.

**AI'ın yaptığı**

Kurulum ve proje tanıtımı için README, AI coding assistant bağlamı için `PROJECT.md`, analiz/gereksinim, teknik tasarım, kullanıcı kılavuzu, test/QA raporu ve bu çalışma günlüğünü hazırladı. Komutları, endpoint'leri, entity'leri ve test sayılarını kaynak koddan kontrol etti.

**Benim kontrolüm / müdahalem**

Her dokümanın farklı amaca hizmet etmesini, birbirinin kopyası olmamasını ve yapılmayan testlerin yapılmış gibi gösterilmemesini istedim. Harici LLM bulunmadığının açık yazılmasına özellikle dikkat ettim.

**Sonuç**

Kodun yanında analiz, teknik karar, kullanım ve kalite kanıtlarını açıklayan proje dokümantasyonu oluştu.

## 4. AI'ın Verdiği Kararlar ve Benim Verdiğim Kararlar

| AI'ın önerdiği veya uyguladığı | Benim kontrol ettiğim veya karar verdiğim |
|---|---|
| Katmanlı backend klasörlerinin teknik görev dağılımı | MVP'nin tek uygulama olması ve microservice kullanılmaması |
| EF Core entity ilişkileri ve endpoint implementasyonları | Roadmap → week → task ürün akışının uygunluğu |
| JWT ve ownership filtrelerinin teknik uygulanması | Kullanıcının yalnızca kendi verisine erişmesi gerektiği |
| CORS hatasının kaynağını araştırma ve scoped policy düzeltmesi | İzin verilecek gerçek frontend origin'i ve `AllowAnyOrigin` kullanılmaması |
| Progress, overdue ve reminder hesaplarının kodlanması | Bu hesapların kullanıcıya anlaşılır görünmesi |
| `IAiLearningService` ve fallback teknik yapısı | Harici LLM olmadan MVP'nin çalışabilmesi |
| Reschedule proposal validation ve apply endpoint'i | AI'ın otomatik veri değiştirmemesi ve açık kullanıcı onayı gerektirmesi |
| Backend testlerinin ve regression kontrollerinin yazılması | Hangi iş kurallarının kritik kabul edildiği |
| Türkçe fallback ve API mesajlarının uygulanması | Uygulamanın kullanıcıya görünen dilinin Türkçe olması |
| Build/test/lint çalıştırma ve sonuç raporlama | Tarayıcıdaki sonucun kabul edilmesi ve sonraki değişikliğe onay verilmesi |

AI teknik seçenekler sundu ve verilen sınırlar içinde uyguladı. Ürün kapsamı, UX beklentisi, güvenlik seviyesi ve son kabul kararları kullanıcı/geliştirici tarafından verildi.

## 5. AI Çıktısını Düzelttiğim Örnekler

### İnsan müdahalesi 1 — CORS problemi

Backend health endpoint'i çalıştığı ve frontend açıldığı için sistem ilk bakışta hazır görünüyordu. Buna rağmen Register isteği tarayıcıdan başarısız oldu. AI'ın daha önce ürettiği yapı kod seviyesinde build ve test alabilse de gerçek browser origin/preflight akışında sorun vardı.

Chrome DevTools Console'da `No 'Access-Control-Allow-Origin' header` mesajını gördüm. Network panelinden frontend'in `localhost:5173`, backend'in `localhost:5000` olduğunu doğruladım ve hatayı tekrar AI'a ilettim. AI CORS policy'yi hedefli şekilde düzeltti. Backend'i yeniden başlattıktan sonra Register akışını tekrar deneyerek sonucu ben kabul ettim.

Bu örnek bana health endpoint'in çalışmasının frontend entegrasyonunun da çalıştığını kanıtlamadığını gösterdi.

### İnsan müdahalesi 2 — İngilizce AI fallback metinleri

AI Assistant teknik olarak suggestion üretiyordu ve endpoint başarılı yanıt veriyordu. Ancak tarayıcıda `Reschedule proposal`, `Falling-behind explanation` ve İngilizce açıklamalar görünüyordu. Bu sorun iş mantığını bozmadığı için backend testlerinden kaçmıştı, fakat Türkçe uygulama deneyimini bozuyordu.

Sorunu manuel kullanımda fark edip yalnızca kullanıcıya görünen fallback metinlerinin değiştirilmesini istedim. AI logic, endpoint ve migration'ların korunması için kapsamı açıkça sınırladım. Çeviriden sonra backend'i yeniden başlatıp ekranı tekrar kontrol ettim.

Bu örnek, çalışan bir özelliğin kullanıcı deneyimi açısından tamamlanmış olmayabileceğini gösterdi.

### İnsan müdahalesi 3 — AI önerisini reddetme deneyimi

Reschedule için temel güvenlik kuralı, önerinin otomatik uygulanmamasıydı. Preview ve apply akışı çalışırken kullanıcının öneriyi istemediğinde sonucu açıkça reddetmesi ve hiçbir görevin değişmediğini görmesi de gerekliydi.

AI akışı incelenirken bu UX ihtiyacı belirlendi. **Öneriyi Reddet** davranışı eklendi; öneri ekrandan temizlendi ve kullanıcıya hiçbir görevin değiştirilmediği mesajı gösterildi. Bu karar yeni bir AI yeteneği eklemek için değil, mevcut “kullanıcı karar verir” ilkesini arayüzde anlaşılır yapmak için alındı.

## 6. AI Kullanımından Öğrendiklerim

Bu projede AI'ın ürettiği kodu çalıştırmadan doğru kabul etmemem gerektiğini öğrendim. Kod build alabilir ve otomatik testler geçebilir, ama frontend ile backend birlikte çalışırken CORS gibi farklı bir sorun çıkabilir.

Browser console ve Network panelini kullanmak özellikle entegrasyon hatalarında çok işe yaradı. Sadece ekrandaki genel hata mesajına bakmak yerine gerçek preflight isteğini görmek sorunun kaynağını netleştirdi.

Build, test, lint ve production build sonuçlarını kendim kontrol etmenin önemli olduğunu gördüm. “Başarılı” ifadesinin gerçekten çalıştırılmış bir komuta dayanması gerekiyor. Aynı şekilde, gerçek PostgreSQL akışını test etmek InMemory testlerin göremediği entegrasyon sorunlarına karşı ek güven sağladı.

AI'a sınırları açık söylemek de sonucu iyileştirdi. “Migration'lara dokunma”, “veriyi silme”, “yeni özellik ekleme” veya “yalnızca görünen metni değiştir” gibi talimatlar gereksiz değişiklikleri önledi. Veritabanı ve migration gibi kritik alanlarda özellikle dikkatli davranmak gerektiğini öğrendim.

Son olarak, dil tutarsızlığı ve reject mesajı gibi küçük UX sorunlarının otomatik backend testlerinden kaçabildiğini gördüm. Bu nedenle manuel kullanım ve kullanıcı gözüyle son kontrol, teknik testlerin yerini almayan ama onları tamamlayan önemli bir adım oldu.

## 7. Sonuç

AI coding assistant bu projede kod üretimini, hata araştırmasını, test yazımını ve dokümantasyonu hızlandıran yararlı bir araç oldu. Özellikle geniş bir repository içinde ilgili dosyaları bulma, tutarlı değişiklik yapma ve tekrar build/test çalıştırma sürecinde zaman kazandırdı.

Buna rağmen son ürün kararlarını AI vermedi. MVP kapsamı, Türkçe arayüz, veri güvenliği, AI'ın otomatik görev değiştirmemesi ve hangi sonucun kabul edileceği benim kararlarımdı. Tarayıcı kontrolleri, gerçek PostgreSQL akışları ve kullanıcı deneyimi değerlendirmesi de insan kontrolüyle yapıldı.

Bu çalışma biçimini kısaca şöyle özetleyebilirim: AI önerdi ve uygulamaya yardım etti; ben kapsamı belirledim, sonucu çalıştırdım, hataları geri bildirdim ve son kararı verdim.
