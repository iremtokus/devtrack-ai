# DevTrack AI — Persistent Project Context

Bu dosya, gelecekte projede çalışacak Codex veya başka bir AI coding assistant için kalıcı geliştirme bağlamıdır. README yerine geçmez; mevcut mimari sınırları, iş kuralları, güvenlik varsayımları ve değişiklik disiplinini tanımlar. Buradaki bilgiler mevcut kaynak koddan doğrulanmıştır.

## Project purpose

DevTrack AI, bireysel bir kullanıcının yazılım geliştirme öğrenme roadmap'ini haftalara ve görevlere ayırarak planlamasını, günlük/haftalık çalışmasını izlemesini ve tamamlanma ilerlemesini görmesini sağlayan eğitim amaçlı bir web uygulamasıdır. Uygulama generic todo list yerine öğrenme bağlamına odaklanır.

## MVP scope

- Kullanıcı kaydı, girişi, çıkışı ve JWT tabanlı oturum
- Kullanıcıya özel roadmap, hafta ve öğrenme görevi yönetimi
- Roadmap/week/task create, read, update ve delete işlemleri
- Görevi tamamlandı/tamamlanmadı olarak değiştirme
- Bugün ve Bu Hafta görev görünümleri
- Dashboard istatistikleri, progress yüzdeleri, overdue listesi ve learning streak
- Due-today ve overdue görevler için in-app reminder üretimi
- Reminder listeleme, okunmamış filtreleme ve okundu işaretleme
- Mevcut görev bağlamından deterministik AI learning suggestions
- AI reschedule proposal önizleme, seçim ve açık kullanıcı onayıyla uygulama
- Türkçe React arayüzü, protected routes, responsive layout ve temel loading/error/empty states
- Swagger/OpenAPI ve `/health` endpoint'i

## Out-of-scope

- Harici LLM veya üretken AI API entegrasyonu
- Native mobil/masaüstü push notification
- E-posta bildirimi, takvim entegrasyonu ve recurring task
- Şifre sıfırlama, e-posta doğrulama ve gelişmiş profil yönetimi
- Takım, organizasyon, rol veya paylaşım özellikleri
- Offline-first çalışma veya mobil uygulama
- Microservice, message broker, distributed cache veya ayrı AI servisi
- Production deployment altyapısı

## Technology stack

- Backend: .NET 8, ASP.NET Core Web API, C#, Entity Framework Core 8, Npgsql, PostgreSQL, JWT Bearer, Swagger
- Frontend: React, TypeScript, Vite, React Router, Tailwind CSS, Lucide React
- Tests: xUnit, ASP.NET Core `WebApplicationFactory`, EF Core InMemory provider
- Tooling: repository-local `dotnet-ef` 8.0.11, npm, Oxlint

Mevcut MVP **single application architecture** kullanır: bir React frontend, bir ASP.NET Core API ve bir PostgreSQL veritabanı vardır. Microservice, Kafka, Redis, event bus veya benzeri altyapılar açık gereksinim ve gerekçe olmadan eklenmemelidir.

## Repository structure

```text
backend/
  DevTrack.Api/             HTTP/API, authentication, authorization, CORS
  DevTrack.Application/     DTO, request/response contract, mapping, AI interface
  DevTrack.Domain/          Entity, enum ve domain-level rules
  DevTrack.Infrastructure/  EF Core DbContext, PostgreSQL provider, migrations, AI fallback
  DevTrack.Tests/           Backend API ve business-rule tests
  DevTrack.sln
frontend/
  src/components/           Reusable UI, layout ve task form/card
  src/context/              Auth state
  src/lib/                  API client ve presentation helpers
  src/pages/                Route-level pages
  src/types.ts              API-facing TypeScript types
.config/dotnet-tools.json   Local EF CLI manifest
.env.example                Safe configuration example
README.md                   İnsan odaklı kurulum ve proje tanıtımı
PROJECT.md                  AI coding assistant için kalıcı kurallar
```

## Backend architecture and responsibilities

- `DevTrack.Api` controller'ları authenticated user context'i çıkarır, request doğrular, ownership filtrelerini uygular, EF Core üzerinden işlemleri yürütür ve güvenli HTTP yanıtları döndürür.
- `ApiControllerBase.UserId`, JWT içindeki `ClaimTypes.NameIdentifier` claim'inden kullanıcı kimliğini üretir.
- `DevTrack.Application`, API contract'larını, DTO mapping'ini, yüzde hesabını ve `IAiLearningService` arayüzünü içerir.
- `DevTrack.Domain`, persistence entity'lerini, enum'ları ve `LearningTask.IsOverdue` gibi temel kuralları içerir.
- `DevTrack.Infrastructure`, `DevTrackDbContext`, relationship/delete davranışları, migration'lar, Npgsql provider ve `FallbackAiLearningService` implementasyonundan sorumludur.
- API JSON enum değerleri string olarak taşınır.
- Normal API tüketicilerine stack trace gösterilmez; exception handler ve `ProblemDetails` kullanılır.

## Frontend architecture and responsibilities

- React Router public `/login` ve `/register` sayfalarını, diğer tüm sayfalar için protected layout'u yönetir.
- `AuthProvider`, JWT ve temel kullanıcı bilgisini browser local storage içinde yönetir; logout ve `401` durumunda session temizlenir.
- Ortak API client, `VITE_API_BASE_URL` kullanır, JWT `Authorization: Bearer` header'ını ekler ve backend `ProblemDetails` mesajlarını kullanıcıya taşır.
- Sayfalar API state, loading, error ve empty state'lerini yönetir; ortak görsel parçalar `components` altında tutulur.
- Frontend backend business rule'larını kopyalamaz. Backend source of truth'tur. Frontend validation yalnızca hızlı kullanıcı geri bildirimi içindir; güvenlik veya veri bütünlüğü garantisi değildir.
- API enum değerleri backend ile aynı kalır; Türkçe etiketler yalnızca presentation katmanında gösterilir.

## Main domain entities

- `User`: email, password hash, display name ve oluşturulma zamanı
- `Roadmap`: kullanıcıya ait başlık, açıklama, süre, başlangıç tarihi ve haftalar
- `RoadmapWeek`: roadmap'e ait benzersiz hafta numarası, başlık ve görevler
- `LearningTask`: görev metadata'sı, durum, öncelik, tarihler, süre, completion timestamp, URL, hafta ve kullanıcı sahipliği
- `Reminder`: kullanıcı, isteğe bağlı task ilişkisi, tip, message, read state, event key ve zaman
- `AiRecommendation`: kullanıcı, action, status, summary, provider identifier, zaman ve reschedule items
- `AiRescheduleItem`: recommendation'a ait task reference, proposed dates ve reason

## Authentication and ownership rules

- Password hiçbir zaman plain text saklanmaz; ASP.NET Core `PasswordHasher<User>` ile hash'lenir.
- Login ve register dışındaki iş kaynakları `[Authorize]` ile korunur.
- Authenticated user kimliği JWT `NameIdentifier` claim'inden alınır.
- **UserId request body'dan güvenilir kabul edilmez; ownership JWT üzerinden doğrulanır.** Request contract'larında istemciden alınan bir `UserId` yoktur.
- Roadmap, task, reminder ve AI recommendation sorguları `UserId` ile filtrelenir. Week ownership, bağlı roadmap'in `UserId` değeri üzerinden doğrulanır.
- Kullanıcı başka bir kullanıcının kaynaklarına ID değiştirerek erişemez veya onları değiştiremez.
- Ownership dışındaki bir kaynak için controller'lar kaynak varlığını açığa çıkarmamak amacıyla çoğunlukla `404 Not Found` döndürür.
- Frontend protected routes yalnızca UX katmanıdır; gerçek koruma backend authorization ve ownership filtreleridir.

## Roadmap/week/task business rules

- Roadmap duration 1–104 hafta aralığındadır; title en fazla 150 karakterdir.
- Roadmap süresi, mevcut en yüksek week number değerinden daha kısa yapılamaz.
- Week number 1–104 aralığında ve roadmap duration içinde olmalıdır.
- Aynı roadmap içinde week number benzersizdir; veritabanında composite unique index vardır.
- Task yalnızca authenticated kullanıcının roadmap'ine bağlı bir week içine eklenebilir.
- Task title en fazla 180 karakterdir; estimated duration 1–1440 dakika aralığındadır.
- Deadline, planned date değerinden önce olamaz.
- Optional resource URL mutlak HTTP veya HTTPS URL olmalıdır.
- Task status `Completed` olduğunda `CompletedAtUtc` oluşturulur/korunur; incomplete duruma dönünce timestamp temizlenir.
- Completion PATCH incomplete duruma dönerken status'u `Todo` yapar.
- Roadmap silme roadmap weeks ve tasks kayıtlarını cascade siler.
- Week silme bağlı tasks kayıtlarını cascade siler.
- Task silme bağlı reminder kayıtlarını cascade siler.
- `LearningTask.UserId` user ilişkisi restrict delete kullanır; ilişki/delete davranışını değiştirirken mevcut cascade zincirleri ayrıca değerlendirilmelidir.

## Progress and overdue calculation rules

- Progress formülü `completed task count / total task count * 100` şeklindedir.
- Hiç task yoksa progress `0` döner.
- Yüzde bir ondalık basamağa yuvarlanır.
- Roadmap progress o roadmap'in bütün week task'larından, week progress yalnızca o week'in task'larından hesaplanır.
- Overall dashboard progress authenticated kullanıcının tüm task'larını kullanır.
- Bir task yalnızca status `Completed` değilse ve deadline bugünden önceyse overdue kabul edilir.
- Planned date geçmişte olsa fakat deadline geçmiş değilse domain `IsOverdue` kuralı tek başına overdue saymaz.
- Completed task hiçbir zaman overdue değildir.
- Today ve week listeleri `PlannedDate` üzerinden oluşturulur; week pazartesi başlayıp pazar biter.
- Learning streak, ayrı UTC completion günlerinin kesintisiz dizisidir; bugün completion yoksa hesap dünden başlar.

## Reminder rules

- Reminder yalnızca authenticated kullanıcıya ait incomplete tasks üzerinden üretilir.
- Task planned date bugünse, deadline bugünse veya deadline geçmişse reminder adayıdır.
- Deadline geçmişse tip `Overdue`; diğer adaylar `DueToday` olur.
- Aynı kullanıcı ve task/event için duplicate reminder üretilmez.
- Duplicate koruması `(UserId, EventKey)` unique index'i ve uygulama kontrolüyle sağlanır.
- Reminder yalnızca sahibi tarafından listelenebilir ve read olarak işaretlenebilir.
- Reminder in-app'tir; push veya e-posta göndermez.

## AI Assistant architecture

- AI servis sınırı `IAiLearningService` ile tanımlanır.
- Mevcut ve tek provider `local-fallback-v1` (`FallbackAiLearningService`) implementasyonudur.
- Şu anda harici LLM entegrasyonu, harici AI API çağrısı veya API key kullanımı yoktur.
- Provider deterministiktir; authenticated kullanıcının incomplete/completed tasks, dates, deadline, priority ve estimated duration verilerinden sonuç üretir.
- Desteklenen action'lar: today recommendation, reschedule unfinished, weekly summary, next task ve falling-behind explanation.
- API her AI isteği için yalnızca authenticated kullanıcıya ait task ve roadmap context'ini oluşturur.
- Provider başarısızlığı, timeout, boş/malformed output ve invalid task/date reference güvenli hata yanıtına çevrilir.

## AI recommendation and rescheduling rules

**AI suggests, backend validates, user decides.**

- AI hiçbir zaman doğrudan kullanıcı görevlerini değiştirmez.
- Reschedule çağrısı yalnızca proposal üretir; task date alanları bu aşamada değiştirilmez.
- Traceability için `AiRecommendation` ve `AiRescheduleItem` kayıtları proposal oluşturulurken saklanır. Ancak **yeniden planlama önerisindeki tarihler kullanıcı açıkça uygulamadan `LearningTask` kayıtlarına yazılmaz.**
- Kullanıcı frontend'de öneri kalemlerini görür, seçer ve **Öneriyi Uygula** ile açıkça onaylar.
- Apply endpoint recommendation ownership, action type, status, selected item membership ve task ownership kontrollerini yeniden yapar.
- Proposal içindeki task ID authenticated kullanıcının incomplete task context'inde bulunmalıdır.
- Completed task AI tarafından reschedule edilemez.
- Proposed planned date geçmişte olamaz; proposed deadline planned date değerinden önce olamaz.
- Başka bir kullanıcı recommendation ID veya task ID değiştirerek öneriyi uygulayamaz.
- Başarılı apply sonrası recommendation status `Applied` olur ve aynı recommendation tekrar uygulanamaz.

## AI privacy and safety rules

- AI'ya yalnızca öneriyi üretmek için gerekli learning context verilmelidir.
- Password, password hash, JWT, API key, database credential veya gereksiz kişisel veri AI context'ine eklenmemelidir.
- Password, JWT, API key, database credential ve complete sensitive AI prompt kaynak kodda tutulmaz veya loglanmaz.
- Mevcut log mesajı provider failure'ı kaydeder; prompt content loglanmaz.
- AI output güvenilir veri olarak kabul edilmez; structured fields backend tarafından doğrulanır.
- AI sonucu tavsiyedir. Kullanıcının önemli verilerini otomatik değiştiren yeni davranış eklenmemelidir.
- Yeni provider eklenirse unavailable, malformed response, empty response, timeout ve invalid references davranışları korunmalıdır.

## Configuration and secrets rules

- PostgreSQL runtime connection string `ConnectionStrings__DevTrack` veya eşdeğer ASP.NET configuration kaynağından gelmelidir.
- EF design-time factory `ConnectionStrings__DevTrack` environment variable'ını gerektirir.
- JWT signing key `Jwt__Key` üzerinden sağlanır ve en az 32 byte olmalıdır.
- JWT issuer/audience configuration'dan okunur; mevcut varsayılanlar `DevTrack.Api` ve `DevTrack.Web` değerleridir.
- Frontend API adresi `VITE_API_BASE_URL` ile yapılandırılır; development fallback `http://localhost:5000` değeridir.
- CORS origin listesi configuration ile sınırlıdır; `AllowAnyOrigin` kullanılmaz.
- Development'ta `http://localhost:5173` izinli origin'dir.
- Real `.env`, secret, local connection string veya credential commit edilmez.
- Example configuration yalnızca açık placeholder değerler içermelidir.
- Password, JWT, API key ve database credential kaynak kodda tutulmaz veya loglanmaz.

## Database/migration rules

- Runtime database PostgreSQL'dir; EF Core provider Npgsql'dır.
- Mevcut migration'lar `backend/DevTrack.Infrastructure/Migrations` altında tutulur.
- Migration dosyaları geçmiş schema kaydıdır; mevcut migration'lar geriye dönük olarak düzenlenmemelidir.
- Entity/model değişikliği schema'yı etkiliyorsa repository-local EF 8 CLI ile yeni migration oluşturulmalıdır.
- Migration gerektiren her değişiklik açıkça raporlanmalıdır.
- Database drop, reset, truncate veya mevcut veriyi silme işlemi açık kullanıcı izni olmadan yapılmaz.
- Migration apply öncesinde doğru environment ve connection string hedefi doğrulanır.
- Test suite EF Core InMemory kullanır ve yerel PostgreSQL verisini değiştirmemelidir.

## Coding conventions

- Kod junior developer tarafından takip edilebilir, doğrudan ve küçük sorumluluklu tutulur.
- Anlamlı isimler, DTO'lar, dependency injection ve uygun yerlerde async EF operations kullanılır.
- Domain, application contract, infrastructure ve HTTP/UI sorumlulukları mevcut katmanlara uygun yerleştirilir.
- Yeni abstraction yalnızca gerçek tekrar veya provider boundary gibi açık ihtiyaç varsa eklenir.
- Büyük UI framework, CQRS, repository/unit-of-work wrapper, event bus veya gereksiz enterprise pattern eklenmez.
- Frontend API contract değerlerini değiştirmez; kullanıcıya dönük yerelleştirme presentation mapping ile yapılır.
- API error response'ları güvenli ve mümkün olduğunca predictable `ProblemDetails` biçiminde kalır.
- Mevcut Türkçe UI dil bütünlüğü korunur.

## Testing expectations

- Backend davranış değişiklikleri için anlamlı regression test eklenir veya ilgili test güncellenir.
- En azından authentication, ownership isolation, validation, completion timestamp, overdue, progress, reminder duplication/ownership ve AI validation kuralları korunur.
- Authorization testlerinde ikinci kullanıcının başka kullanıcının kaynağına erişemediği doğrulanır.
- AI testleri fallback action behavior, invalid provider reference, completed task reschedule ve cross-user recommendation apply kurallarını korur.
- Frontend değişikliğinde TypeScript production build ve lint çalıştırılır.
- Test geçmediği halde başarılı olduğu iddia edilmez; atlanan veya çalıştırılamayan kontroller açıkça raporlanır.
- Mevcut frontend'de component/E2E test framework'ü yoktur; sırf test sayısını artırmak için büyük yeni test altyapısı eklenmez.

## Development workflow

1. İstenen kapsamı ve bu dosyadaki sınırları kontrol et.
2. İlgili backend/frontend kaynaklarını ve mevcut testleri incele.
3. Repository'nin mevcut dirty state'ini koru; ilgisiz dosyaları değiştirme.
4. Küçük, hedefli ve mevcut mimariye uygun değişiklik yap.
5. Schema değişiyorsa migration ihtiyacını değerlendir ve açıkça raporla.
6. Backend için `dotnet build backend/DevTrack.sln` ve `dotnet test backend/DevTrack.sln` çalıştır.
7. Frontend etkileniyorsa `npm run lint --prefix frontend` ve `npm run build --prefix frontend` çalıştır.
8. Çalıştırılan komutların gerçek sonuçlarını, değişen dosyaları ve kalan sınırlamaları raporla.

## Change Rules

- İstenmeyen yeni özellik ekleme ve task kapsamını kendi kendine büyütme.
- Mevcut single-application architecture'ı değiştirmeden önce somut ihtiyacı ve gerekçeyi açıkla.
- Yeni dependency eklemeden önce neden gerekli olduğunu, mevcut araçlarla neden çözülemediğini ve bakım etkisini belirt.
- Migration gerektiren model/schema değişikliklerini uygulamadan ve teslim ederken açıkça raporla.
- Mevcut testleri kırma; davranış değişiyorsa yalnızca gereksinimle uyumlu şekilde testleri güncelle.
- Değişiklikten sonra ilgili build/test/lint komutlarını çalıştır ve gerçek sonucu raporla.
- Kullanıcı istemedikçe production deployment, Docker, microservice, Kafka, Redis veya harici LLM entegrasyonu ekleme.
- Veri silme/reset, migration geçmişini yeniden yazma veya secret commit etme.
- Güvenlik, ownership ve explicit AI confirmation kurallarını kolaylık uğruna frontend'e taşıma ya da devre dışı bırakma.

## Definition of Done

Bir değişiklik ancak aşağıdaki koşullar sağlandığında tamamlanmış kabul edilir:

- İstenen davranış gerçekten uygulanmıştır; kapsam dışı özellik eklenmemiştir.
- Backend source of truth, ownership ve validation kuralları korunmuştur.
- AI değişikliklerinde “AI suggests, backend validates, user decides” ilkesi korunmuştur.
- Secret veya hassas veri kaynak koda/loglara eklenmemiştir.
- Schema değiştiyse yeni migration oluşturulmuş, hedef ve sonuç açıkça raporlanmıştır.
- İlgili otomatik testler anlamlıdır ve gerçekten çalıştırılmıştır.
- Etkilenen backend solution build'i başarılıdır.
- Frontend etkilendiyse lint ve production build başarılıdır.
- Kullanıcıya görünen ekranlarda uygun loading, error ve empty state davranışları korunmuştur.
- Değiştirilen dosyalar, doğrulama sonuçları ve kalan sınırlamalar dürüstçe raporlanmıştır.
- Mevcut kullanıcı verileri ve ilgisiz çalışma dosyaları korunmuştur.
