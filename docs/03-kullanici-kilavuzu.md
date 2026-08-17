# DevTrack AI — Kullanıcı Kılavuzu

Bu kılavuz, DevTrack AI uygulamasını yerel bilgisayarında ilk kez kuracak ve kullanacak kişiler için hazırlanmıştır. Komutlar repository kök dizininde çalıştırılacak şekilde verilmiştir.

## 1. Uygulamaya Genel Bakış

DevTrack AI, yazılım öğrenme planını roadmap, hafta ve görevler hâlinde düzenlemene yardımcı olan kişisel bir takip uygulamasıdır. Bugün ve bu hafta çalışman gereken görevleri görebilir, tamamladığın işleri işaretleyebilir ve ilerlemeni Dashboard üzerinden takip edebilirsin.

Temel kullanım akışı şöyledir:

1. Kayıt ol ve hesabına giriş yap.
2. Bir öğrenme roadmap'i oluştur.
3. Roadmap'e haftalar ekle.
4. Haftalara öğrenme görevleri ekle.
5. **Bugün** ve **Bu Hafta** ekranlarından planını takip et.
6. Bitirdiğin görevleri tamamla.
7. Dashboard üzerinden ilerlemeni kontrol et.
8. Geciken ve bugün tamamlanacak görevler için bildirim oluştur.
9. AI Asistan'dan mevcut görevlerine dayalı öneri al.

## 2. Sistem Gereksinimleri

Uygulamayı yerel ortamda çalıştırmak için aşağıdaki araçlar gereklidir:

- .NET 8 SDK
- Node.js ve npm
- PostgreSQL
- Git

Kurulumları kontrol etmek için:

```bash
dotnet --version
node --version
npm --version
psql --version
git --version
```

.NET çıktısının ana sürümü `8` olmalıdır. Node.js sürümü, projede kullanılan Vite 8 ile uyumlu güncel bir sürüm olmalıdır.

## 3. PostgreSQL Hazırlığı

DevTrack AI, kalıcı veriler için PostgreSQL kullanır. Önce PostgreSQL servisinin çalıştığından emin ol.

macOS'ta Homebrew ile PostgreSQL 16 kurulum örneği:

```bash
brew install postgresql@16
brew services start postgresql@16
```

Bilgisayarındaki PostgreSQL sürümüne göre Homebrew formula adı farklı olabilir.

PostgreSQL'e bağlan:

```bash
psql postgres
```

Ardından `psql` içinde geliştirme veritabanını oluştur:

```sql
CREATE DATABASE devtrack;
```

Çıkmak için:

```text
\q
```

Connection string; sunucu, port, veritabanı ve PostgreSQL rolü bilgilerini bir araya getirir. Güvenli örnek:

```text
Host=localhost;Port=5432;Database=devtrack;Username=YOUR_LOCAL_POSTGRES_ROLE;Password=YOUR_LOCAL_POSTGRES_PASSWORD
```

Yerel PostgreSQL rolün parola istemiyorsa `Password` bölümünü kaldırabilirsin. Gerçek kullanıcı adı veya parolayı Git'e ekleme.

## 4. Backend’i Çalıştırma

Terminali proje kök dizininde aç. macOS için gerekli environment variable'ları kendi yerel bilgilerine göre tanımla:

```bash
export ConnectionStrings__DevTrack='Host=localhost;Port=5432;Database=devtrack;Username=YOUR_LOCAL_POSTGRES_ROLE;Password=YOUR_LOCAL_POSTGRES_PASSWORD'
export Jwt__Key='REPLACE_WITH_A_RANDOM_SECRET_OF_AT_LEAST_32_BYTES'
export ASPNETCORE_ENVIRONMENT='Development'
export ASPNETCORE_URLS='http://localhost:5000'
```

`Jwt__Key` en az 32 byte uzunluğunda, rastgele ve yalnızca yerel ortamına ait olmalıdır. Yukarıdaki placeholder değeri gerçek kullanımda güvenli bir yerel değerle değiştir.

Backend bağımlılıklarını yükle ve projeyi çalıştır:

```bash
dotnet restore backend/DevTrack.sln
dotnet build backend/DevTrack.sln
dotnet run --project backend/DevTrack.Api --no-launch-profile
```

`--no-launch-profile`, projedeki dinamik launch profile portunu kullanmak yerine `ASPNETCORE_URLS` ile verdiğin sabit `http://localhost:5000` adresinin kullanılmasını sağlar.

Backend çalıştığında şu adresleri kontrol edebilirsin:

- Health: `http://localhost:5000/health`
- Swagger: `http://localhost:5000/swagger`

Swagger yalnızca Development ortamında açılır.

## 5. Migration Uygulama

İlk çalıştırmadan önce mevcut EF Core migration'larını `devtrack` veritabanına uygula. `ConnectionStrings__DevTrack` aynı terminalde tanımlı olmalıdır.

Repository-local EF Core 8 aracını yükle:

```bash
dotnet tool restore
```

Migration'ları uygula:

```bash
dotnet ef database update \
  --project backend/DevTrack.Infrastructure \
  --startup-project backend/DevTrack.Api
```

Bu işlem mevcut migration geçmişini kullanarak gerekli tabloları oluşturur. Veritabanını tekrar oluşturmak için migration dosyalarını elle değiştirme.

## 6. Frontend’i Çalıştırma

Backend açık kalırken ikinci bir terminal aç. Proje kök dizininden frontend bağımlılıklarını yükle:

```bash
npm install --prefix frontend
```

Frontend'in backend adresini bilmesi için environment variable vererek Vite'ı başlat:

```bash
VITE_API_BASE_URL=http://localhost:5000 npm run dev --prefix frontend
```

Frontend varsayılan olarak şu adreste açılır:

```text
http://localhost:5173
```

Bu değişkeni her çalıştırmada yazmak istemiyorsan `frontend/.env.example` dosyasını örnek alarak `frontend/.env.local` oluşturabilirsin:

```env
VITE_API_BASE_URL=http://localhost:5000
```

## 7. Kayıt Olma

Tarayıcıda `http://localhost:5173/register` adresini aç veya giriş ekranındaki **Kayıt ol** bağlantısını kullan.

Kayıt formundaki alanlar:

- **Ad soyad:** Uygulamada gösterilecek adın.
- **E-posta:** Hesabına giriş yaparken kullanacağın geçerli e-posta adresi.
- **Şifre:** En az 8 karakter olmalıdır.

Formu doldurup **Kayıt ol** düğmesine bas. Kayıt başarılı olursa oturum otomatik açılır, JWT tarayıcıdaki yerel oturum alanına kaydedilir ve Dashboard'a yönlendirilirsin.

Aynı e-posta adresiyle ikinci kez kayıt olunamaz.

[EKRAN GÖRÜNTÜSÜ: Kayıt ekranı]

## 8. Giriş ve Çıkış

Mevcut hesabına giriş yapmak için `http://localhost:5173/login` adresini aç.

1. **E-posta** alanına kayıtlı e-posta adresini yaz.
2. **Şifre** alanına parolanı yaz.
3. **Giriş yap** düğmesine bas.

Bilgiler doğruysa backend bir JWT oluşturur. Frontend sonraki korumalı API isteklerinde bu token'ı otomatik olarak gönderir. Token geçersizse veya oturum sona ererse uygulama oturum bilgisini temizler ve giriş sayfasına yönlendirir.

Çıkış yapmak için masaüstünde sol menüde, mobilde açılır navigasyon içinde bulunan **Çıkış** düğmesini kullan. Çıkış işlemi tarayıcıda saklanan JWT ve kullanıcı bilgisini siler.

[EKRAN GÖRÜNTÜSÜ: Giriş ekranı]

## 9. Dashboard Kullanımı

Giriş yaptıktan sonra açılan ana sayfa Dashboard'dur. Bu ekran öğrenme durumunun kısa özetini gösterir.

- **Bugünkü görevler:** Planlanan tarihi bugün olan görev sayısı.
- **Tamamlanan:** Bütün roadmap'lerinde tamamladığın toplam görev sayısı.
- **Kalan:** Henüz tamamlanmamış toplam görev sayısı.
- **Öğrenme serisi:** Birbirini izleyen günlerde tamamlanan görevlere göre hesaplanan gün sayısı.
- **Genel ilerleme:** Tüm görevler içindeki tamamlanan görev yüzdesi.
- **Bu haftaki ilerleme:** Pazartesi–pazar aralığına planlanan görevlerin tamamlanma yüzdesi.
- **Yaklaşan teslimler:** Son tarihi bugün veya ileride olan incomplete görevlerden en yakın olanlar.
- **Geciken görevler:** Son tarihi geçmiş ve tamamlanmamış görevler.
- **Yakın zamanda tamamlananlar:** Completion zamanına göre en son tamamlanan görevler.

Henüz görevin yoksa ilgili kartlarda boş durum mesajları gösterilir. Yeni görev ekledikçe ve görevleri tamamladıkça Dashboard değerleri güncellenir.

[EKRAN GÖRÜNTÜSÜ: Dashboard]

## 10. Roadmap Oluşturma

Sol veya mobil menüden **Roadmapler** ekranını aç. Yeni bir öğrenme planı oluşturmak için **Yeni roadmap** düğmesine bas.

Form alanları:

- **Roadmap adı:** Öğrenme planının başlığıdır; zorunludur.
- **Açıklama:** Planın amacı veya kapsamı için isteğe bağlı alandır.
- **Süre (hafta):** Roadmap'in kaç hafta süreceğini belirtir. Geçerli aralık 1–104 haftadır.
- **Başlangıç tarihi:** Planın başlangıç günüdür.

**Kaydet** düğmesiyle roadmap oluşturulur. Roadmap kartında ilerleme yüzdesi, süre, eklenen hafta sayısı ve başlangıç tarihi görünür.

Roadmap kartındaki kalem simgesiyle düzenleme, çöp kutusu simgesiyle silme işlemi yapılır. Silme işlemi roadmap'e bağlı haftaları ve görevleri de kaldıracağı için uygulama önce onay ister.

[EKRAN GÖRÜNTÜSÜ: Roadmapler]

## 11. Hafta Yönetimi

Roadmap adına tıklayarak detay ekranını aç. **Hafta ekle** düğmesi yeni bir roadmap haftası oluşturur.

Hafta formundaki alanlar:

- **Hafta numarası:** En az 1 olmalı ve roadmap süresini aşmamalıdır.
- **Başlık:** O haftanın öğrenme konusunu açıklayan zorunlu başlıktır.

Aynı roadmap içinde aynı hafta numarası iki kez kullanılamaz. Her hafta kartında progress göstergesi, görevler ve işlem düğmeleri bulunur.

- Kalem simgesi haftayı düzenler.
- Çöp kutusu haftayı siler.
- **Görev** düğmesi o haftaya yeni görev ekler.

Hafta silindiğinde o haftaya bağlı görevler de silinir. Uygulama işlemden önce onay ister.

[EKRAN GÖRÜNTÜSÜ: Roadmap detayı]

## 12. Görev Oluşturma ve Yönetme

Roadmap detayındaki ilgili hafta kartında **Görev** düğmesine bas. Görev formu aşağıdaki alanları içerir:

- **Başlık:** Görevin kısa adı; zorunludur.
- **Açıklama:** Yapılacak çalışmanın ayrıntısı.
- **Tür:** Kurs, Video, Okuma, Egzersiz, Proje veya Tekrar.
- **Öncelik:** Düşük, Orta veya Yüksek.
- **Durum:** Yapılacak, Devam ediyor veya Tamamlandı.
- **Tahmini süre (dk):** 1–1440 dakika arasında beklenen çalışma süresi.
- **Planlanan tarih:** Görevin çalışılacağı gün; zorunludur.
- **Son tarih:** Görevin bitirilmesi gereken isteğe bağlı tarih. Planlanan tarihten önce olamaz.
- **Kaynak URL (isteğe bağlı):** Görevle ilgili mutlak `http://` veya `https://` adresi.

Görev kartında tür, öncelik, durum, süre, planlanan tarih, varsa son tarih ve kaynak bağlantısı gösterilir. Son tarihi geçmiş incomplete görevler **Gecikmiş** etiketi alır.

Kullanılabilir işlemler:

- **Tamamla:** Görevi completed yapar ve completion zamanını kaydeder.
- **Geri al:** Completed görevi yeniden **Yapılacak** durumuna getirir ve completion zamanını temizler.
- **Düzenle:** Görev formunu mevcut bilgilerle açar.
- **Sil:** Onay sonrasında görevi kaldırır.

Completed görevler son tarihi geçmiş olsa bile gecikmiş sayılmaz.

## 13. Bugün Ekranı

Navigasyondaki **Bugün** bağlantısı, planlanan tarihi bugün olan görevlerini gösterir. Her görev kartından:

- Görevin türünü, önceliğini, durumunu ve tahmini süresini görebilir,
- Roadmap/hafta bağlamını inceleyebilir,
- Başlığa basarak görev detayını açabilir,
- Görevi tamamlayabilir veya geri alabilir,
- Düzenleyebilir veya silebilirsin.

Bir işlem tamamlandığında liste backend'den tekrar yüklenir. Bugün için görev yoksa **“Bugün için görevin yok.”** mesajı gösterilir.

[EKRAN GÖRÜNTÜSÜ: Bugün]

## 14. Bu Hafta Ekranı

Navigasyondaki **Bu Hafta** bağlantısı, içinde bulunulan pazartesi–pazar dönemine planlanan görevleri gösterir. Görevler planlanan güne göre gruplanır.

Ekranın üst bölümünde tamamlanan görev sayısı ve haftalık progress bar bulunur. Görev kartlarından tamamlama, geri alma, düzenleme ve silme işlemleri yapılabilir. Geciken görevler kırmızı **Gecikmiş** etiketiyle ayırt edilir.

Bu hafta için görev yoksa **“Bu hafta için görevin yok.”** mesajı gösterilir.

[EKRAN GÖRÜNTÜSÜ: Bu Hafta]

## 15. Bildirimler

Navigasyondaki **Bildirimler** ekranı, bugün tamamlanacak ve geciken görevler için uygulama içi hatırlatmaları gösterir.

Yeni bildirimleri kontrol etmek için **Bildirimleri oluştur** düğmesine bas. Sistem yalnızca tamamlanmamış görevleri inceler:

- Planlanan tarihi bugün olan veya son tarihi bugün olan görev için **Bugün tamamlanacak** bildirimi,
- Son tarihi geçmiş görev için **Gecikmiş görev** bildirimi oluşturur.

Aynı görev ve bildirim tipi için tekrar tekrar duplicate kayıt oluşturulmaz.

- **Tümü:** Okunmuş ve okunmamış bildirimleri birlikte gösterir.
- **Okunmamış:** Yalnızca henüz okunmamış bildirimleri gösterir.
- **Okundu:** Bildirimi read durumuna getirir.

Okunmamış bildirimler **Yeni** etiketiyle gösterilir. Navigasyonda okunmamış bildirim sayısı, sayfa ilk açıldığında alınan veriye göre rozet olarak görünebilir.

[EKRAN GÖRÜNTÜSÜ: Bildirimler]

## 16. AI Asistan

Navigasyondaki **AI Asistan** ekranı, mevcut görevlerinden öğrenme önerileri üretir. Kullanılabilir beş işlem vardır:

### Bugünkü çalışma önerisi

Bugün teslim edilmesi gereken veya odağa alınması gereken incomplete görevleri seçer. Görev sayısı ve toplam tahmini süre hakkında kısa bir öneri verir.

### Tamamlanmayan görevleri yeniden planla

Planlanan tarihi geçmiş veya gecikmiş incomplete görevler için yeni tarihler önerir. Ekranda görevin eski tarihi, önerilen tarihi ve gerekçe gösterilir.

İstediğin proposal kalemlerini seçebilir ve **Öneriyi Uygula** düğmesine basabilirsin. Öneriyi kullanmak istemezsen **Öneriyi Reddet** düğmesi ekranı temizler; görev verileri değiştirilmez.

### Haftalık özet

Bu haftaya planlanan görevlerin kaçının tamamlandığını ve kaç görevin kaldığını özetler.

### Sonraki görev önerisi

Overdue durumu, tarih ve önceliğe göre sıradaki öğrenme görevini önerir ve yaklaşık çalışma süresini gösterir.

### Neden geride kaldım?

Geciken ve bugünden önce planlanmış incomplete görevlerden geride kalma sinyallerini açıklar.

Mevcut provider **`local-fallback-v1`** değeridir. Projede harici LLM veya dış AI API çağrısı kullanılmaz. Provider mevcut görev verilerinden deterministik öneriler üretir.

AI sonuçları tavsiyedir. AI görevleri kendi başına değiştirmez. Yeniden planlama önerisi önce önizlenir; task tarihleri yalnızca kullanıcı kalemleri seçip **Öneriyi Uygula** düğmesiyle açıkça onayladığında ve backend doğrulaması başarılı olduğunda değişir. Completed görevler yeniden planlanamaz.

[EKRAN GÖRÜNTÜSÜ: AI Asistan]

## 17. Sık Karşılaşılan Hatalar

### Backend çalışmıyor

Belirti: `http://localhost:5000/health` açılmaz veya frontend **“Sunucuya ulaşılamıyor”** mesajı gösterir.

Kontrol et:

```bash
curl http://localhost:5000/health
```

Backend terminalinin açık olduğundan, `ASPNETCORE_URLS=http://localhost:5000` tanımlandığından ve `dotnet run --project backend/DevTrack.Api --no-launch-profile` komutunun hata vermediğinden emin ol.

### Frontend backend’e ulaşamıyor

Frontend yanlış API adresiyle başlatılmış olabilir. Frontend'i şu şekilde yeniden başlat:

```bash
VITE_API_BASE_URL=http://localhost:5000 npm run dev --prefix frontend
```

Backend health endpoint'ini ayrıca kontrol et.

### CORS problemi

Tarayıcı `Access-Control-Allow-Origin` veya preflight hatası gösteriyorsa frontend'in `http://localhost:5173` adresinde çalıştığını kontrol et. Backend Development configuration bu origin'e izin verir. Farklı bir frontend origin'i kullanıyorsan backend `Cors__AllowedOrigins__0` yapılandırmasını o tam origin'e göre ayarlaman gerekir. `AllowAnyOrigin` kullanma.

### PostgreSQL bağlantı hatası

PostgreSQL servisinin ve `devtrack` veritabanının erişilebilir olduğunu doğrula:

```bash
pg_isready
psql -d devtrack
```

`ConnectionStrings__DevTrack` içindeki host, port, database ve role bilgilerinin yerel kurulumunla eşleştiğinden emin ol. Parola gerekiyorsa yalnızca yerel environment variable içinde belirt.

### Aynı e-posta ile kayıt

**“Bu e-posta adresiyle kayıtlı bir hesap zaten var.”** mesajı alırsan o e-posta daha önce kullanılmıştır. Mevcut hesabınla giriş yap veya farklı bir e-posta adresi kullan.

### Geçersiz giriş

**“E-posta adresi veya şifre hatalı.”** mesajında e-posta yazımını ve parolanı kontrol et. MVP'de password reset özelliği yoktur.

## 18. Uygulamayı Kapatma

Backend ve frontend ayrı terminal süreçleri olarak çalışır.

- Backend'in çalıştığı terminale geç ve `Ctrl+C` tuşlarına bas.
- Frontend'in çalıştığı terminale geç ve `Ctrl+C` tuşlarına bas.

Bu işlem process'leri durdurur; PostgreSQL'deki roadmap ve görev verilerini silmez. PostgreSQL servisini de kapatmak istersen kurulum yöntemine uygun servis komutunu kullanabilirsin. Homebrew PostgreSQL 16 örneği:

```bash
brew services stop postgresql@16
```

## 19. MVP Sınırlamaları

- AI Asistan gerçek bir harici LLM kullanmaz; yalnızca `local-fallback-v1` ile mevcut görevlerden kural tabanlı öneri üretir.
- Bildirimler uygulama içindedir; telefona veya masaüstüne native push gönderilmez.
- Bildirimleri otomatik oluşturan background scheduler yoktur; **Bildirimleri oluştur** işlemi kullanıcı tarafından başlatılır.
- E-posta doğrulama ve şifre sıfırlama bulunmaz.
- Takvim entegrasyonu, recurring task ve offline kullanım bulunmaz.
- Uygulama bireysel kullanım içindir; ortak roadmap veya ekip çalışması desteklenmez.
- Backend tarih hesapları UTC gününü temel alır; kullanıcıya özel timezone ayarı yoktur.
