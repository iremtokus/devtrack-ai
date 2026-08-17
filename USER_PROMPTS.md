# DevTrack AI — Kullanıcı Promptları

Bu dosya DevTrack AI geliştirme sürecinde kullanılan anlamlı kullanıcı promptlarını içerir; "devam", "oldu" gibi kısa etkileşimler dahil edilmemiştir.

Promptlar kronolojik sıraya yakın tutulmuştur. İlk geliştirme aşamalarındaki 001–006 numaralı uzun talepler, okunabilirlik için temel kapsam ve teknik kararlar korunarak kısa Türkçe özetler hâline getirilmiştir. 007 ve sonrasındaki kısa promptlar mümkün olan yerlerde kullanıcının yazdığı şekliyle korunmuştur. Sistem mesajları, AI cevapları ve terminal çıktıları bu dosyada yer almaz.

## 001 — İlk proje isteği

> DevTrack AI adında, yazılım öğrenme sürecimi roadmap, haftalar ve görevlerle takip edebileceğim bir mezuniyet projesi geliştirelim. Kayıt/giriş, dashboard, günlük ve haftalık takip, ilerleme, bildirimler ve mevcut görevleri kullanan bir AI Asistan olsun. .NET 8, PostgreSQL ve React/TypeScript kullanalım; önce repository ile ortamı inceleyip kısa bir plan çıkar, sonra güvenlik, test ve dokümantasyonu atlamadan junior seviyesinde anlaşılır bir yapı kur.

## 002 — Mevcut projenin denetlenmesi

> Yeni özellik eklemeden önce mevcut DevTrack AI repository'sini baştan sona incele. Backend ve frontend build durumunu, PostgreSQL bağlantısını, migration'ları, authentication/authorization yapısını ve mevcut testleri kontrol et. Tamamlanan, eksik ve hatalı bölümleri açıkça raporla; hiçbir şeyi sessizce düzeltmeden denetim sonunda onayımı bekle.

## 003 — Phase 1: backend temelinin onarılması

> İlk aşamada yalnızca backend temelini çalışır hâle getir. EF Core ve Npgsql paketlerini uyumlu .NET 8 sürümlerine hizala, hard-coded JWT ve PostgreSQL bilgilerini kaldırıp environment variable kullan, yerel `devtrack` veritabanı için migration'ı oluşturup uygula. Solution build edilsin ve registration, invalid login, overdue ve mümkünse ownership için anlamlı başlangıç testleri geçsin; frontend veya dokümantasyona başlama.

## 004 — Phase 2: backend MVP ve AI service layer

> İkinci aşamada backend MVP'yi tamamla; roadmap week ve task CRUD, ownership, tarih ve completion kuralları tutarlı çalışsın. Dashboard/progress ve duplicate üretmeyen reminder akışını ekle. AI tarafında `local-fallback-v1` ile beş öneri işlemi olsun; yeniden planlama yalnızca proposal üretsin, backend doğrulasın ve görevler kullanıcı açıkça apply etmeden değişmesin. Gerekli migration ve backend testlerini çalıştır, frontend'e başlama.

## 005 — Phase 3: React frontend

> Üçüncü aşamada mevcut backend'e bağlanan React, TypeScript, Vite ve Tailwind frontend'i oluştur. Arayüz Türkçe, sade ve responsive olsun; login/register, dashboard, Bugün, Bu Hafta, roadmap detayları, bildirimler ve AI Asistan ekranlarını gerçek endpointlerle çalıştır. JWT, protected routes, loading/error/empty state'leri ekle ve production build başarılı olunca dur; backend mimarisini gereksiz yere değiştirme.

## 006 — Phase 4: uçtan uca doğrulama

> Dördüncü aşamada gerçek yerel `devtrack` PostgreSQL veritabanıyla backend ve frontend'i birlikte çalıştırıp uçtan uca kontrol et. Kayıt, roadmap/week/task, completion, progress, reminder ve AI proposal/apply akışlarını; ikinci kullanıcı ownership güvenliğini ve önemli edge case'leri doğrula. Build, test, lint ve production build sonuçlarını çalıştır; yalnızca kanıtlanan hataları düzelt, yeni özellik veya final dokümantasyon ekleme ve mevcut veriyi sıfırlama.

## 007 — CORS hatasının düzeltilmesi

> Frontend ve backend şu anda çalışıyor ancak tarayıcıdan kayıt işlemi CORS nedeniyle engelleniyor.
>
> Tarayıcı hatası:
>
> Access to fetch at 'http://localhost:5000/api/auth/register'
> from origin 'http://localhost:5173'
> has been blocked by CORS policy:
> Response to preflight request doesn't pass access control check:
> No 'Access-Control-Allow-Origin' header is present on the requested resource.
>
> Backend health endpoint'i tarayıcıdan başarıyla çalışıyor:
> http://localhost:5000/health
> → {"status":"healthy"}
>
> Frontend:
> http://localhost:5173
>
> Backend:
> http://localhost:5000
>
> Görevin:
> 1. Backend'deki mevcut CORS yapılandırmasını incele.
> 2. Sorunun gerçek nedenini tespit et.
> 3. Development ortamında http://localhost:5173 origin'inden gelen frontend isteklerine izin ver.
> 4. Mevcut güvenlik yaklaşımını bozma; AllowAnyOrigin kullanma.
> 5. Authorization header ve gerekli HTTP methodlarının çalışmasını sağla.
> 6. OPTIONS preflight isteğinin doğru CORS header'larıyla cevap verdiğini doğrula.
> 7. Backend build ve mevcut testleri tekrar çalıştır.
> 8. Frontend/backend endpoint veya API base URL uyumsuzluğu varsa onu da düzelt.
> 9. Yaptığın değişiklikleri ve doğrulama sonuçlarını özetle.
>
> PostgreSQL verisini silme veya resetleme.
> Mevcut migration'ları değiştirme.
> Gereksiz refactor yapma.

## 008 — AI fallback çıktılarının Türkçeleştirilmesi

> AI Asistan ekranındaki tüm kullanıcıya görünen çıktıları Türkçeleştir. Şu anda LOCAL-FALLBACK-V1 tarafından üretilen başlık, açıklama, öneri ve alt bilgilendirme metinlerinin bazıları İngilizce geliyor. Uygulamanın arayüz dili Türkçe olduğu için AI Assistant'ın tüm fallback çıktıları da doğal ve anlaşılır Türkçe olmalı. “Today's study recommendation”, “Falling-behind explanation”, “Focus on...”, “Keep the current plan...” ve benzeri sabit İngilizce metinleri Türkçeleştir. Sadece kullanıcıya gösterilen metinleri değiştir; AI öneri mantığını, endpointleri, veritabanını, migration'ları ve mevcut çalışan özellikleri değiştirme. Sonrasında ilgili testleri çalıştır.

## 009 — MVP son kalite kontrolü

> DevTrack AI MVP'nin son kalite kontrolünü yap. Mevcut çalışan özellikleri veya veritabanı verilerini bozma/silme. Frontend ve backend'i incele; build ve testleri çalıştır. Authentication, roadmap/week/task CRUD, görev tamamlama, dashboard istatistikleri, notifications ve AI assistant akışlarında kritik hata olup olmadığını kontrol et. Responsive görünüm, loading/error/empty state'ler ve Türkçe/İngilizce metin tutarlılığını da kontrol et. Kritik veya kullanıcı deneyimini bozan küçük sorunları düzelt. PostgreSQL verilerini ve mevcut migration'ları değiştirme. Sonunda yaptığın değişiklikleri, build/test sonuçlarını ve MVP'de hâlâ eksik kalan noktaları kısa bir rapor halinde ver.

## 010 — README hazırlanması

> DevTrack AI projesi için GitHub'da kullanılacak profesyonel ama yeni mezun seviyesine uygun bir README.md hazırla.
>
> README yalnızca projede gerçekten bulunan özellikleri ve teknolojileri içersin; olmayan özellikleri ekleme.
>
> Şu bölümler olsun: proje hakkında kısa açıklama, temel özellikler, kullanılan teknolojiler, proje mimarisi/klasör yapısı, gereksinimler, PostgreSQL kurulumu ve connection string yapılandırması, backend'i çalıştırma, frontend'i çalıştırma, environment variable örnekleri, testlerin çalıştırılması, AI Assistant'ın çalışma mantığı, güvenlik yaklaşımı ve mevcut MVP sınırlamaları.
>
> […]
>
> Mevcut kaynak kodu, migration'ları ve veritabanını değiştirme. Yalnızca README.md oluştur/güncelle.

## 011 — PROJECT.md hazırlanması

> DevTrack AI projesi için kök dizinde PROJECT.md oluştur.
>
> Bu dosya README'nin tekrarı olmasın. Amacı gelecekte Codex veya başka bir AI coding assistant projede çalışırken mimariyi, iş kurallarını, güvenlik sınırlarını ve AI kullanım kurallarını hızlıca anlayabilsin.
>
> Yalnızca mevcut projeyi kaynak koddan inceleyerek doğruladığın bilgileri yaz. Olmayan özellikleri ekleme.
>
> […]
>
> Kod, migration, README veya veritabanını değiştirme. Yalnızca PROJECT.md oluştur.

## 012 — Analiz ve gereksinimler dokümanı

> DevTrack AI projesi için docs/01-analiz-ve-gereksinimler.md dosyasını oluştur.
>
> Bu doküman projenin kodlamaya başlanmadan önce hazırlanmış analiz ve gereksinim dokümanı formatında olsun. Ancak yalnızca mevcut DevTrack AI kapsamıyla tutarlı bilgiler kullan; projede olmayan özellikleri uydurma.
>
> Dokümanın dili Türkçe, anlatımı öğrenci/yeni mezun seviyesinde açık ve anlaşılır olsun. Gereksiz kurumsal jargon kullanma.
>
> […]
>
> Kaynak kodu, README, PROJECT.md, migration veya veritabanını değiştirme. Yalnızca docs/01-analiz-ve-gereksinimler.md oluştur.

## 013 — Teknik tasarım ve mimari dokümanı

> DevTrack AI projesi için docs/02-teknik-tasarim-ve-mimari.md oluştur.
>
> Bu belge projenin teknik tasarımını ve mimarisini açıklasın. Yalnızca mevcut kaynak koddan doğrulanabilen yapıyı anlat; projede olmayan teknoloji veya mimari bileşen ekleme.
>
> Dil Türkçe olsun. Teknik terimler gerektiğinde İngilizce kullanılabilir. Anlatım yeni mezun bir yazılım geliştiricinin hazırlayabileceği kadar açık ve teknik olsun.
>
> […]
>
> Kod, migration, database veya diğer dokümanları değiştirme. Yalnızca docs/02-teknik-tasarim-ve-mimari.md oluştur.

## 014 — Kullanıcı kılavuzu

> DevTrack AI projesi için docs/03-kullanici-kilavuzu.md oluştur.
>
> Bu belge uygulamayı ilk kez kullanacak bir kişinin sistemi kurup kullanabilmesini sağlamalı. Dil Türkçe, anlatım sade ve adım adım olsun.
>
> Yalnızca mevcut projede gerçekten bulunan ekranları ve özellikleri anlat. Olmayan özellik ekleme.
>
> […]
>
> Kod, migration, database veya başka doküman değiştirme. Yalnızca docs/03-kullanici-kilavuzu.md oluştur.

## 015 — Test ve QA raporu

> DevTrack AI için docs/04-test-ve-qa-raporu.md oluştur.
>
> Bu doküman yalnızca gerçekten gerçekleştirilen testlere ve mevcut test koduna dayansın. Yapılmamış testi yapılmış gibi gösterme.
>
> Dil Türkçe olsun. Akademik/ödev teslimine uygun ama sade bir test raporu hazırla.
>
> […]
>
> Kod, test, migration, database veya diğer dokümanları değiştirme. Yalnızca docs/04-test-ve-qa-raporu.md oluştur.

## 016 — AI çalışma günlüğü

> DevTrack AI projesi için docs/05-ai-calisma-gunlugu.md oluştur.
>
> Bu belge proje geliştirilirken AI coding assistant ile nasıl çalışıldığını gösterecek. Akademik bir rapor gibi aşırı resmi yazma. Yeni mezun bir yazılım geliştiricinin proje sürecinde tuttuğu çalışma günlüğü gibi doğal, açık ve anlaşılır olsun.
>
> Önemli: Promptları çok uzun, kusursuz veya kurumsal hale getirme. Benim AI aracına yazabileceğim kısa ve doğal cümleler şeklinde yaz. Teknik gereksinimler gerekli olduğunda kullanılsın ama her prompt profesyonel specification gibi görünmesin.
>
> Gerçekte yapılmayan bir AI etkileşimi, hata veya karar uydurma.
>
> […]
>
> Kod, migration, database veya mevcut dokümanları değiştirme. Yalnızca docs/05-ai-calisma-gunlugu.md oluştur.

## 017 — Demo videosunun README'ye eklenmesi

> README dosyasına kısa bir "Demo Videosu" bölümü ekle. Aşağıdaki Google Drive bağlantısını kullan:
>
> https://drive.google.com/file/d/1VNLAyqV8IEvsXDxroHkARU0lWc1sEaI9/view?usp=sharing
>
> Bölümü README'nin proje açıklaması ve özellikler kısmına uygun bir yerde konumlandır. Mevcut içeriği değiştirme veya silme. Sadece demo bağlantısını ekle.

## 018 — Kullanıcı prompt arşivinin oluşturulması

> Kök dizinde USER_PROMPTS.md adında bir dosya oluştur.
>
> Bu dosyada DevTrack AI projesini geliştirirken benden aldığın anlamlı kullanıcı isteklerini/promptlarını kronolojik sıraya yakın şekilde biriktir.
>
> Kurallar:
> - "devam", "oldu", "çalışmıyor", "tamam" gibi tek başına anlam taşımayan kısa mesajları ekleme.
> - Mümkün olan yerlerde benim yazdığım promptları değiştirmeden kullan.
> - Elinde birebir kullanıcı mesajı bulunmayan bir adım için yeni veya uydurma prompt üretme.
> - Promptları daha profesyonel göstermek amacıyla yeniden yazma.
> - Sistem tarafından üretilen cevapları, terminal çıktılarını ve kendi açıklamalarını prompt olarak ekleme.
> - Şifre, JWT key, connection string veya başka secret/credential bilgilerini dosyaya koyma.
> - Her anlamlı promptu numaralandır.
> - Dosyanın başına kısa bir açıklama ekle: Bu dosya DevTrack AI geliştirme sürecinde kullanılan anlamlı kullanıcı promptlarını içerir; "devam", "oldu" gibi kısa etkileşimler dahil edilmemiştir.
> - Mevcut docs/05-ai-calisma-gunlugu.md dosyasını değiştirme.
> - Kaynak kodu, migrationları, veritabanını, README ve diğer dokümanları değiştirme.
> - Yalnızca USER_PROMPTS.md dosyasını oluştur.
