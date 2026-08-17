# DevTrack AI

DevTrack AI, yazılım geliştirme öğrenme sürecini roadmap, hafta ve görevler üzerinden takip etmeyi sağlayan kişisel bir öğrenme takip uygulamasıdır. Kullanıcı; bugün ve bu hafta çalışması gereken konuları görebilir, görevlerini tamamlayabilir, ilerlemesini izleyebilir, uygulama içi bildirimler oluşturabilir ve mevcut görev verilerine dayalı çalışma önerileri alabilir.

Proje, eğitim ve mezuniyet projesi kapsamında anlaşılır bir katmanlı mimari, backend doğrulaması, kullanıcı bazlı veri izolasyonu, otomatik testler ve sorumlu AI kullanımını gösterecek şekilde geliştirilmiştir.

## Temel özellikler

- Kayıt, giriş ve çıkış işlemleri
- JWT tabanlı API authentication
- Kullanıcıya özel roadmap verileri
- Roadmap oluşturma, görüntüleme, düzenleme ve silme
- Roadmap içine hafta ekleme, düzenleme ve silme
- Haftalara görev ekleme, görev düzenleme ve silme
- Görevleri tamamlandı veya tamamlanmadı olarak işaretleme
- Bugün ve Bu Hafta görünümleri
- Görev türü, durum, öncelik, planlanan tarih, son tarih, tahmini süre ve isteğe bağlı kaynak URL desteği
- Genel, roadmap ve hafta bazlı ilerleme yüzdeleri
- Dashboard üzerinde günlük/haftalık görevler, geciken görevler, yaklaşan teslimler, son tamamlananlar ve öğrenme serisi
- Bugün teslim edilecek veya geciken görevler için uygulama içi bildirimler
- Okunmuş/okunmamış bildirim takibi ve tekrar bildirim üretimini engelleme
- Mevcut öğrenme verilerini kullanan AI Learning Assistant fallback önerileri
- Türkçe, responsive React arayüzü; loading, error ve empty state'ler
- Swagger/OpenAPI arayüzü ve health endpoint'i

## Ana kullanım akışı

1. Kullanıcı kayıt olur veya hesabına giriş yapar.
2. Bir öğrenme roadmap'i oluşturur.
3. Roadmap'e haftalar ekler.
4. Haftalara öğrenme görevleri ekler.
5. Bugün ve Bu Hafta ekranlarından çalışma planını takip eder.
6. Bitirdiği görevleri tamamlandı olarak işaretler.
7. Dashboard üzerinden ilerleme değişimini ve yaklaşan işleri görür.
8. Geciken veya bugün tamamlanacak görevler için bildirim oluşturur.
9. AI Asistan'dan çalışma önerisi, haftalık özet veya yeniden planlama önerisi ister.
10. Yeniden planlama önerileri görevleri otomatik değiştirmez; kullanıcı seçtiği önerileri açıkça onaylayarak uygular.

## Kullanılan teknolojiler

### Backend

- .NET 8 ve ASP.NET Core Web API
- C#
- Entity Framework Core 8.0.11
- Npgsql Entity Framework Core Provider 8.0.11
- PostgreSQL
- JWT Bearer authentication
- ASP.NET Core Identity `PasswordHasher`
- Swagger / OpenAPI
- xUnit, ASP.NET Core MVC Testing ve EF Core InMemory test altyapısı

### Frontend

- React 19
- TypeScript
- Vite 8
- React Router
- Tailwind CSS 3
- Lucide React ikonları
- Oxlint

## Mimari ve klasör yapısı

Backend, sorumlulukları ayıran sade bir katmanlı yapı kullanır:

```text
devtrack-ai/
├── backend/
│   ├── DevTrack.Api/             # HTTP endpointleri, JWT, CORS ve uygulama başlangıcı
│   ├── DevTrack.Application/     # DTO'lar, sözleşmeler ve AI servis arayüzü
│   ├── DevTrack.Domain/          # Entity'ler, enum'lar ve temel iş kuralları
│   ├── DevTrack.Infrastructure/  # EF Core DbContext, PostgreSQL ve migration'lar
│   ├── DevTrack.Tests/           # Backend entegrasyon ve iş kuralı testleri
│   └── DevTrack.sln
├── frontend/
│   ├── src/components/           # Ortak UI, layout ve görev bileşenleri
│   ├── src/context/              # Authentication durumu
│   ├── src/lib/                  # API istemcisi ve format yardımcıları
│   ├── src/pages/                # Uygulama sayfaları
│   └── src/types.ts              # Frontend veri tipleri
├── .config/dotnet-tools.json     # Repository-local EF Core CLI tanımı
├── .env.example                  # Güvenli backend yapılandırma örneği
└── README.md
```

Frontend iş kurallarını yeniden üretmez; verilerin ve doğrulamaların ana kaynağı backend API'dir.

## Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) ve npm — Vite 8 ile uyumlu güncel bir sürüm
- [PostgreSQL](https://www.postgresql.org/download/) sunucusu
- Git

macOS'ta PostgreSQL Homebrew ile kurulacaksa örnek:

```bash
brew install postgresql@16
brew services start postgresql@16
```

Kurulu PostgreSQL sürümüne göre formula adı farklı olabilir.

## PostgreSQL kurulumu ve connection string

Önce yerel PostgreSQL rolünüzü kontrol edin:

```bash
psql postgres
```

Ardından `psql` içinde geliştirme veritabanını oluşturun:

```sql
CREATE DATABASE devtrack;
```

Alternatif olarak, mevcut işletim sistemi kullanıcınız PostgreSQL rolü olarak yetkiliyse:

```bash
createdb devtrack
```

Connection string gerçek yerel rolünüze göre yapılandırılmalıdır. Şifresiz yerel bağlantı örneği:

```text
Host=localhost;Port=5432;Database=devtrack;Username=YOUR_LOCAL_POSTGRES_ROLE
```

PostgreSQL rolünüz parola gerektiriyorsa parolayı yalnızca yerel environment variable içinde ekleyin:

```text
Host=localhost;Port=5432;Database=devtrack;Username=YOUR_LOCAL_POSTGRES_ROLE;Password=YOUR_LOCAL_POSTGRES_PASSWORD
```

Gerçek connection string veya parolayı Git'e eklemeyin.

## Environment variable örnekleri

Kök dizindeki `.env.example` güvenli bir referans sağlar. ASP.NET Core bu projede `.env` dosyasını otomatik olarak yüklemez; değişkenleri terminal oturumunda export etmek gerekir:

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://localhost:5000
export ConnectionStrings__DevTrack='Host=localhost;Port=5432;Database=devtrack;Username=YOUR_LOCAL_POSTGRES_ROLE;Password=YOUR_LOCAL_POSTGRES_PASSWORD'
export Jwt__Key='REPLACE_WITH_A_RANDOM_SECRET_OF_AT_LEAST_32_BYTES'
export Jwt__Issuer='DevTrack.Api'
export Jwt__Audience='DevTrack.Web'
export FrontendUrl='http://localhost:5173'
```

JWT anahtarı en az 32 byte uzunluğunda, rastgele ve yalnızca yerel ortama özel olmalıdır. PostgreSQL bağlantınız şifre gerektirmiyorsa `Password` bölümünü kaldırın.

Frontend için [frontend/.env.example](frontend/.env.example) temel alınarak `frontend/.env.local` oluşturulabilir:

```env
VITE_API_BASE_URL=http://localhost:5000
```

Bu değer bir secret değildir; tarayıcının erişeceği API adresidir.

## Migration'ları uygulama

Repository-local EF Core aracını yükleyin ve mevcut migration'ları `devtrack` veritabanına uygulayın:

```bash
dotnet tool restore
dotnet ef database update \
  --project backend/DevTrack.Infrastructure \
  --startup-project backend/DevTrack.Api
```

Bu komuttan önce `ConnectionStrings__DevTrack` environment variable'ı tanımlanmış olmalıdır.

## Backend'i çalıştırma

Repository kök dizininden:

```bash
dotnet restore backend/DevTrack.sln
dotnet build backend/DevTrack.sln
dotnet run --project backend/DevTrack.Api --no-launch-profile
```

`ASPNETCORE_URLS=http://localhost:5000` tanımlandığında:

- API: `http://localhost:5000`
- Health kontrolü: `http://localhost:5000/health`
- Development Swagger: `http://localhost:5000/swagger`

## Frontend'i çalıştırma

Yeni bir terminalde repository kök dizininden:

```bash
npm install --prefix frontend
VITE_API_BASE_URL=http://localhost:5000 npm run dev --prefix frontend
```

Vite varsayılan olarak uygulamayı `http://localhost:5173` adresinde sunar. Development CORS yapılandırması bu origin'e izin verir.

## Testler ve kalite kontrolleri

Backend solution'ı build etmek ve testleri çalıştırmak için:

```bash
dotnet build backend/DevTrack.sln
dotnet test backend/DevTrack.sln
```

Backend testleri; authentication, CORS preflight, ownership izolasyonu, hafta doğrulaması, görev tamamlama zamanları, overdue davranışı, ilerleme hesabı, bildirimler ve AI öneri doğrulaması gibi temel kuralları kapsar. Test veritabanı olarak EF Core InMemory provider kullanılır; testler yerel PostgreSQL verisini değiştirmez.

Frontend kontrolleri:

```bash
npm run lint --prefix frontend
npm run build --prefix frontend
```

`npm run build`, TypeScript project build ve Vite production build işlemlerini birlikte çalıştırır.

## AI Assistant nasıl çalışır?

Projede şu anda gerçek bir harici LLM veya AI API entegrasyonu yoktur. `IAiLearningService` soyutlamasının mevcut implementasyonu `local-fallback-v1` sağlayıcısıdır.

Bu sağlayıcı kullanıcının tamamlanmış ve tamamlanmamış görevlerini, planlanan tarihleri, son tarihleri, öncelikleri ve tahmini süreleri kullanarak deterministik öneriler üretir. AI servis bağlamına kullanıcının roadmap başlıkları da aktarılır; mevcut fallback algoritması önerilerini esas olarak görev verilerinden hesaplar. Desteklenen işlemler:

- Bugünkü çalışma önerisi
- Tamamlanmamış görevler için yeniden planlama önerisi
- Haftalık ilerleme özeti
- Sonraki öğrenme görevi önerisi
- Geride kalma açıklaması

AI çıktıları öneri olarak ele alınır. Yeniden planlama önerileri oluşturulurken görevler otomatik değiştirilmez. Backend görev kimliklerini, kullanıcı sahipliğini, görev durumunu ve önerilen tarihleri doğrular; değişiklik ancak kullanıcı arayüzündeki **Öneriyi Uygula** işlemiyle açıkça onaylandığında kaydedilir. Öneri kayıtlarında kullanıcı, işlem türü, zaman, durum, özet ve sağlayıcı kimliği tutulur; parola, JWT, API anahtarı veya tam hassas prompt içeriği saklanmaz.

## Güvenlik yaklaşımı

- Parolalar düz metin olarak saklanmaz; ASP.NET Core `PasswordHasher` ile hash'lenir.
- API erişimi imzalı ve süreli JWT token ile korunur.
- Roadmap, hafta, görev, bildirim ve AI önerisi sorguları authenticated kullanıcı kimliğiyle filtrelenir.
- İstemciden `UserId` alınmaz ve request body içindeki kullanıcı kimliğine güvenilmez.
- Başka kullanıcıya ait kaynak kimlikleri güvenli biçimde `404 Not Found` ile izole edilir.
- Kritik doğrulamalar yalnızca frontend'e bırakılmaz; backend tarafında da uygulanır.
- CORS yalnızca yapılandırılmış origin'lere izin verir; `AllowAnyOrigin` kullanılmaz.
- JWT anahtarı ve PostgreSQL connection string kaynak kodda tutulmaz.
- Normal API yanıtlarında stack trace gösterilmez.
- AI önerileri uygulanmadan önce backend tarafından doğrulanır.
- `.gitignore`, yerel environment dosyalarını ve üretilen build çıktılarını Git dışında tutar.

## Mevcut MVP sınırlamaları

- Harici LLM entegrasyonu yoktur; AI Asistan yalnızca `local-fallback-v1` kullanır.
- Bildirimler uygulama içidir; native mobil veya masaüstü push notification yoktur.
- Tekrarlanan görevler, takvim senkronizasyonu ve e-posta bildirimleri yoktur.
- Frontend için otomatik component veya end-to-end test altyapısı bulunmaz.
- Backend otomatik testleri EF Core InMemory provider kullanır; gerçek PostgreSQL'e karşı ayrı otomatik entegrasyon testi yoktur.
- Kullanıcı profili yönetimi, parola sıfırlama ve e-posta doğrulama bulunmaz.
- Uygulama tek kullanıcı oturumuna yönelik kişisel kullanım MVP'sidir; ekip/organizasyon özellikleri yoktur.
