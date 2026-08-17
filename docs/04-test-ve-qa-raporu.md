# DevTrack AI — Test ve QA Raporu

## Rapor Bilgisi

- **Proje:** DevTrack AI
- **Rapor tarihi:** 17 Ağustos 2026
- **Kapsam:** MVP backend, frontend ve gerçek PostgreSQL akışları
- **Son otomatik backend sonucu:** 19 başarılı, 0 başarısız, 0 atlanan

Bu raporda üç kanıt türü birbirinden ayrılmıştır:

- **AUTO:** Kaynak koddaki xUnit test suite ile otomatik çalıştırılan testler.
- **E2E-API:** Phase 4 sırasında gerçek `devtrack` PostgreSQL veritabanı ve çalışan API ile yapılan uçtan uca kontroller.
- **MANUAL-QA:** Kullanıcıyla birlikte Chrome üzerinden gözlemlenen ekran ve kullanıcı akışları.

Bir kontrol yapılmadıysa veya kanıtı eksikse başarılı kabul edilmemiştir.

## 1. Test Amacı ve Kapsamı

Test çalışmasının amacı, DevTrack AI MVP'nin temel kullanıcı akışlarının çalıştığını ve kullanıcı verilerinin birbirinden güvenli biçimde ayrıldığını doğrulamaktır. İncelenen alanlar:

- Authentication: registration, login ve JWT üretimi
- Authorization ve ownership isolation
- Roadmap/week/task oluşturma ve temel iş kuralları
- Task completion timestamp ve overdue davranışı
- Dashboard ve progress hesapları
- Reminder üretme, duplicate engelleme ve read işlemi
- AI fallback action'ları ve reschedule güvenliği
- Frontend lint, TypeScript ve production build
- Gerçek PostgreSQL ile API düzeyinde E2E akışı
- Chrome üzerinden manuel kullanıcı arayüzü doğrulaması

Mevcut otomatik suite bütün olası CRUD ve UI kombinasyonlarını kapsamaz. Bu sınırlamalar Bölüm 13'te ayrıca belirtilmiştir.

## 2. Test Ortamı

Test ve QA kayıtlarında doğrulanan ortam:

| Bileşen | Ortam / sürüm |
|---|---|
| İşletim sistemi | macOS, Apple Silicon (`arm64`) |
| Backend runtime | .NET 8 / `net8.0` |
| Test runner | VSTest 17.11.1 (`arm64`) |
| Veritabanı | Yerel PostgreSQL 16, `devtrack` database |
| Backend adresi | `http://localhost:5000` |
| Frontend | React + TypeScript + Vite |
| Frontend adresi | `http://localhost:5173` |
| Otomatik backend test database'i | EF Core InMemory; gerçek PostgreSQL'den ayrıdır |
| Manuel tarayıcı | Chrome |

EF Core ve Npgsql paketleri proje dosyalarında 8.0.11 olarak tanımlıdır. Frontend production build kaydında Vite 8.2.1 görülmüştür. Node.js'in kesin runtime sürümü bu rapor için kaydedilmediğinden belirtilmemiştir.

## 3. Otomatik Backend Testleri

Testler `backend/DevTrack.Tests/BackendFoundationTests.cs` dosyasındadır. Bir `[Theory]` beş farklı `InlineData` ile çalıştığı için toplam test case sayısı 19'dur.

### Authentication ve CORS

| Test ID | Mevcut test adı | Test edilen davranış | Beklenen sonuç | Sonuç |
|---|---|---|---|---|
| AUTO-01 | `Registration_WithValidDetails_ReturnsToken` | Geçerli kullanıcı kaydı | HTTP 200 ve boş olmayan JWT | PASS |
| AUTO-02 | `Login_WithWrongPassword_ReturnsUnauthorized` | Yanlış parola ile login | HTTP 401 | PASS |
| AUTO-03 | `RegisterPreflight_FromConfiguredFrontend_ReturnsCorsHeaders` | `localhost:5173` kaynaklı register OPTIONS preflight | 204; doğru origin, POST, content-type ve authorization CORS header'ları | PASS |

### Ownership, week ve task kuralları

| Test ID | Mevcut test adı | Test edilen davranış | Beklenen sonuç | Sonuç |
|---|---|---|---|---|
| AUTO-04 | `WeekNumber_CannotExceedRoadmapDuration` | Roadmap duration değerinden büyük week number | HTTP 400 | PASS |
| AUTO-05 | `Week_CannotBeChangedByAnotherUser` | İkinci kullanıcının başka kullanıcı week'ini güncellemesi | HTTP 404 | PASS |
| AUTO-06 | `Task_CannotBeReadByAnotherUser` | İkinci kullanıcının başka kullanıcı task'ını okuması | HTTP 404 | PASS |
| AUTO-07 | `CompleteThenIncomplete_MaintainsTimestamp` | Task completion ve geri alma | Complete durumda timestamp var ve overdue false; geri alındığında timestamp null ve geçmiş deadline için overdue true | PASS |
| AUTO-08 | `CompletedTask_WithPastDeadline_IsNotOverdue` | Geçmiş deadline'lı completed domain task | `IsOverdue` false | PASS |

### Reminder ve progress

| Test ID | Mevcut test adı | Test edilen davranış | Beklenen sonuç | Sonuç |
|---|---|---|---|---|
| AUTO-09 | `Reminder_CannotBeMarkedReadByAnotherUser` | Overdue reminder üretme, Türkçe mesaj, cross-user read ve duplicate üretim | Tek reminder; “Gecikmiş görev:” mesajı; diğer kullanıcıya 404; ikinci üretimde boş liste | PASS |
| AUTO-10 | `RoadmapProgress_IsCompletedPercentage` | İki task'tan birinin completed olması | Roadmap ve week progress yüzde 50 | PASS |

### AI fallback action'ları

Aşağıdaki beş case aynı `Fallback_ReturnsActionSpecificValidResult` theory metodundan üretilir.

| Test ID | Action | Test edilen davranış | Beklenen sonuç | Sonuç |
|---|---|---|---|---|
| AUTO-11 | `TodayRecommendation` | Bugünkü öneri fallback sonucu | “Bugünkü çalışma önerisi”, dolu summary, `local-fallback-v1`, reschedule proposal yok | PASS |
| AUTO-12 | `RescheduleUnfinished` | Yeniden planlama fallback sonucu | “Yeniden planlama önerisi”, dolu summary, `local-fallback-v1`, proposal var | PASS |
| AUTO-13 | `WeeklySummary` | Haftalık özet fallback sonucu | “Haftalık ilerleme özeti”, dolu summary, proposal yok | PASS |
| AUTO-14 | `NextTask` | Sonraki görev fallback sonucu | “Sonraki öğrenme görevi”, dolu summary, proposal yok | PASS |
| AUTO-15 | `FallingBehind` | Geride kalma fallback sonucu | “Geride kalma açıklaması”, dolu summary, proposal yok | PASS |

### AI apply ve provider validation

| Test ID | Mevcut test adı | Test edilen davranış | Beklenen sonuç | Sonuç |
|---|---|---|---|---|
| AUTO-16 | `CompletedTask_CannotBeAppliedFromRescheduleProposal` | Proposal üretildikten sonra completed yapılan task'ı apply etme | HTTP 400 | PASS |
| AUTO-17 | `AnotherUser_CannotApplyRecommendation` | İkinci kullanıcının recommendation'ı apply etmesi | HTTP 404 | PASS |
| AUTO-18 | `InvalidTaskReference_IsRejected` | Var olmayan recommendation/task ID ile apply | HTTP 404 | PASS |
| AUTO-19 | `InvalidProviderTaskReference_IsRejected` | Provider'ın kullanıcı context'i dışında task ID döndürmesi | HTTP 502 Bad Gateway | PASS |

## 4. Otomatik Test Sonucu

Çalıştırılan komut:

```bash
dotnet test backend/DevTrack.sln --no-build --no-restore
```

Gerçek son test sonucu:

| Metrik | Değer |
|---|---:|
| Passed | 19 |
| Failed | 0 |
| Skipped | 0 |
| Total | 19 |
| Test assembly | `DevTrack.Tests.dll (net8.0)` |

Sonuç: **PASS**.

## 5. Backend Build Kontrolü

Çalıştırılan komut:

```bash
dotnet build backend/DevTrack.sln --no-restore
```

Gerçek son sonuç:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

Domain, Application, Infrastructure, API ve Tests projelerinin tamamı build edilmiştir. Sonuç: **PASS**.

## 6. Frontend Kalite Kontrolleri

Çalıştırılan komutlar:

```bash
npm run lint --prefix frontend
npm run build --prefix frontend
```

Gerçek son sonuçlar:

| Kontrol | Sonuç | Kanıt |
|---|---|---|
| Oxlint | PASS | Komut hata vermeden tamamlandı. |
| TypeScript project build | PASS | `tsc -b` başarılı tamamlandı. |
| Vite production build | PASS | Vite build başarılı tamamlandı. |
| Dönüştürülen modül | PASS | 1817 modules transformed |
| JavaScript bundle | Bilgi | Yaklaşık 278.36 kB; gzip yaklaşık 85.96 kB |
| CSS bundle | Bilgi | Yaklaşık 17.24 kB; gzip yaklaşık 4.12 kB |

Frontend'de ayrı component test suite bulunmadığından bu bölüm lint, type checking ve production bundle üretimini ifade eder.

## 7. Dependency/Security Kontrolü

Phase 4 dependency incelemesinde backend NuGet ve frontend npm package durumları kontrol edilmiştir. Son Phase 4 kaydında bilinen vulnerable package bulunmadığı raporlanmıştır. Daha önce görülen package conflict/security uyarıları uyumlu sürümler seçilerek giderilmiş ve ana build/test komutları yeniden çalıştırılmıştır.

Son backend package yapısında EF Core ve Npgsql 8.0.11 stack'i birlikte kullanılmaktadır. Dependency düzeltmesi için gereksiz major upgrade yapılmamıştır.

Bu rapor hazırlanırken ek bir doğrulama denemesi yapılmıştır:

- `dotnet list ... package --vulnerable --no-restore` mevcut CLI tarafından `--no-restore` desteklenmediği için sonuç üretmemiştir.
- `npm audit --omit=dev` npm registry DNS erişimi olmadığı için tamamlanamamıştır.

Bu iki yeniden deneme **PASS olarak sayılmamıştır**. “Son durumda vulnerable package bulunmadı” ifadesinin kanıtı Phase 4'te tamamlanan dependency/security kontrolüdür; rapor yazımındaki ağsız yeniden deneme değildir.

## 8. Gerçek PostgreSQL E2E Testi

Phase 4 sırasında backend ve frontend localhost üzerinde çalıştırılmış, API doğrulamaları EF InMemory yerine gerçek yerel PostgreSQL 16 içindeki `devtrack` database ile yapılmıştır. Veritabanı resetlenmemiştir.

| E2E ID | Adım | Gerçek sonuç | Durum |
|---|---|---:|---|
| E2E-01 | Yeni kullanıcı register | HTTP 200 | PASS |
| E2E-02 | Aynı kullanıcının login işlemi | HTTP 200 | PASS |
| E2E-03 | Roadmap oluşturma | HTTP 201 | PASS |
| E2E-04 | Roadmap'e week ekleme | Başarılı response | PASS |
| E2E-05 | Week içine 3 task oluşturma | Üç task da kaydedildi | PASS |
| E2E-06 | Today endpoint'i | Planlanan günlük görevler döndü | PASS |
| E2E-07 | This Week endpoint'i | Haftalık görevler döndü | PASS |
| E2E-08 | Bir task'ı completed yapma | Başarılı response | PASS |
| E2E-09 | Roadmap progress değişimi | `%0 → %33,3` | PASS |
| E2E-10 | Completion timestamp | Dolu timestamp döndü | PASS |
| E2E-11 | Completed task overdue kontrolü | Completed task overdue değil | PASS |
| E2E-12 | Reminder generation | Reminder üretildi | PASS |
| E2E-13 | Reminder read | Reminder read yapıldı | PASS |
| E2E-14 | AI today recommendation | Başarılı structured suggestion | PASS |
| E2E-15 | AI weekly summary | Başarılı structured summary | PASS |
| E2E-16 | AI reschedule proposal | Proposal üretildi | PASS |
| E2E-17 | Proposal öncesi/sonrası tarih kontrolü | Proposal aşamasında task tarihi değişmedi | PASS |
| E2E-18 | Explicit apply | Kullanıcı apply sonrası task tarihi değişti | PASS |
| E2E-19 | Completed task reschedule | HTTP 400 | PASS |
| E2E-20 | Token olmadan dashboard | HTTP 401 | PASS |

Bu tablo Phase 4 çalışma notlarına dayanır. Aynı akış bu rapor hazırlanırken yeniden çalıştırılmamıştır; mevcut veriler değiştirilmemiştir.

## 9. Ownership / İkinci Kullanıcı Güvenlik Testi

Phase 4 sırasında ikinci bir test kullanıcısı oluşturulmuş ve ilk kullanıcıya ait resource ID'leriyle erişim denenmiştir.

| Kaynak / işlem | Beklenen güvenli davranış | Gerçek sonuç | Durum |
|---|---|---:|---|
| Roadmap read | Veri gösterilmemeli | 404 | PASS |
| Roadmap update | Değişiklik yapılmamalı | 404 | PASS |
| Roadmap delete | Silinmemeli | 404 | PASS |
| Week access/update | Erişim veya değişiklik olmamalı | 404 | PASS |
| Task read | Veri gösterilmemeli | 404 | PASS |
| Task update/delete | Değişiklik veya silme olmamalı | 404 | PASS |
| Reminder read işlemi | Başka kullanıcının reminder'ı değişmemeli | 404 | PASS |
| AI recommendation apply | Başka kullanıcının önerisi uygulanmamalı | 404 | PASS |

Bu kontroller IDOR (Insecure Direct Object Reference) riskine karşı önemlidir. Bir kullanıcının URL veya request içindeki GUID değerini değiştirmesi, başka kullanıcının özel kaynağını okumasına veya değiştirmesine yol açmamalıdır. API kaynakları JWT claim'inden alınan `UserId` ile filtrelediği için kullanıcıya kaynağın varlığı da açıklanmadan `404` döndürülür.

## 10. Edge Case Testleri

Aşağıdaki sonuçlar Phase 4 gerçek PostgreSQL/API ve UI doğrulama notlarından alınmıştır:

| EDGE ID | Senaryo | Gerçek sonuç | Durum |
|---|---|---:|---|
| EDGE-01 | Duplicate registration | HTTP 409 | PASS |
| EDGE-02 | Invalid login | HTTP 401 | PASS |
| EDGE-03 | Empty roadmap title | HTTP 400 | PASS |
| EDGE-04 | Roadmap duration = 0 | HTTP 400 | PASS |
| EDGE-05 | Week number > roadmap duration | HTTP 400 | PASS |
| EDGE-06 | Deadline < planned date | HTTP 400 | PASS |
| EDGE-07 | Task estimated duration = 0 | HTTP 400 | PASS |
| EDGE-08 | Resource URL `ftp://...` | HTTP 400 | PASS |
| EDGE-09 | Past deadline'lı completed task | Overdue false | PASS |
| EDGE-10 | Aynı reminder'ı ikinci kez üretme | Duplicate oluşturulmadı | PASS |
| EDGE-11 | Yeni/boş kullanıcı ekranları | Roadmap, Today ve This Week empty state'leri gösterildi | PASS |
| EDGE-12 | Harici AI key olmadan AI isteği | `local-fallback-v1` suggestion döndü | PASS |

## 11. Manuel Tarayıcı QA

Aşağıdaki kontroller **otomatik test değildir**. 17 Ağustos 2026 tarihinde kullanıcıyla birlikte Chrome üzerinden manuel olarak doğrulanmıştır.

| MANUAL-QA ID | Manuel kontrol | Sonuç |
|---|---|---|
| MANUAL-QA-01 | Register ekranı açıldı. | PASS |
| MANUAL-QA-02 | Gerçek kullanıcı kaydı yapıldı. | PASS |
| MANUAL-QA-03 | JWT alındıktan sonra Dashboard açıldı. | PASS |
| MANUAL-QA-04 | Roadmap oluşturuldu. | PASS |
| MANUAL-QA-05 | Roadmap'e week eklendi. | PASS |
| MANUAL-QA-06 | Week içine task oluşturuldu. | PASS |
| MANUAL-QA-07 | Task Bugün ekranında görüntülendi. | PASS |
| MANUAL-QA-08 | Task Bu Hafta ekranında görüntülendi. | PASS |
| MANUAL-QA-09 | AI Asistan ekranı açıldı. | PASS |
| MANUAL-QA-10 | Bugünkü çalışma önerisi üretildi ve mevcut task kullanıldı. | PASS |
| MANUAL-QA-11 | Reminder üretildi ve Bildirimler ekranında görüntülendi. | PASS |
| MANUAL-QA-12 | Task completed yapıldı. | PASS |
| MANUAL-QA-13 | Dashboard sayaçları ve progress değerinin güncellendiği gözlendi. | PASS |
| MANUAL-QA-14 | CORS hatası Chrome Network panelinde tespit edildi; düzeltme sonrası uygulama yeniden çalıştırıldı ve register akışı doğrulandı. | PASS |
| MANUAL-QA-15 | AI fallback çıktılarındaki İngilizce/Türkçe tutarsızlığı tespit edildi; metinler Türkçeleştirildikten sonra yeniden kontrol edildi. | PASS |

## 12. Bulunan Hatalar ve Düzeltmeler

| Sorun | Nasıl tespit edildi? | Çözüm | Son durum |
|---|---|---|---|
| EF Core / Npgsql version conflict | İlk backend restore/build ve package incelemesi | EF Core, Npgsql ve repository-local EF CLI uyumlu 8.x sürümlerine hizalandı. | Çözüldü; backend build/test başarılı. |
| PostgreSQL/configuration başlangıç problemi | API startup ve migration uygulama sırasında eksik/güvensiz fallback yapılandırması | Connection string ve JWT key environment/configuration zorunluluğu getirildi; güvenli example dosyaları kullanıldı. | Çözüldü; gerçek `devtrack` bağlantısı ve migration'lar Phase 4'te doğrulandı. |
| CORS preflight hatası | Chrome Network panelinde `localhost:5173 → localhost:5000` register isteği | Belirli development origin, gerekli header'lar ve HTTP method'ları için CORS policy düzeltildi; `AllowAnyOrigin` kullanılmadı. | Çözüldü; otomatik OPTIONS testi ve manuel register akışı başarılı. |
| AI reject UX eksikliği | AI proposal kullanıcı akışı incelemesi | Kullanıcının öneriyi reddedip sonucu temizleyebilmesi ve hiçbir görevin değişmediğini görmesi sağlandı. | Çözüldü; explicit apply yaklaşımı korunuyor. |
| Dependency security uyarıları | Phase 4 NuGet/npm dependency kontrolleri | Etkilenen paketler uyumlu sürümlere getirildi; major gereksiz upgrade yapılmadı. | Phase 4 son kontrolünde vulnerable package yok. Rapor sırasındaki online npm audit ağ nedeniyle tekrarlanamadı. |
| AI/UI Türkçe metin tutarsızlığı | MANUAL-QA sırasında `local-fallback-v1` çıktılarını inceleme | Beş fallback action'ın başlık, özet, adım, gerekçe ve disclaimer metinleri Türkçeleştirildi; regression assertion eklendi. | Çözüldü; backend 19/19 test başarılı. |

## 13. Test Edilemeyen / Otomatikleştirilmemiş Alanlar

- Frontend component testleri yoktur.
- Browser E2E automation yoktur; tarayıcı akışları MANUAL-QA olarak kaydedilmiştir.
- Gerçek PostgreSQL'e karşı sürekli çalışan otomatik integration test suite yoktur.
- Mevcut xUnit suite EF Core InMemory kullanır; PostgreSQL-specific constraint/type davranışlarını tek başına garanti etmez.
- Bütün CRUD endpoint'lerinin bütün validation ve hata kombinasyonları otomatik test edilmez.
- Roadmap sahibinin kendi roadmap update/delete ve task sahibinin kendi task update/delete işlemleri ayrı named otomatik test case'leri değildir.
- Responsive ekranların farklı cihaz ölçülerinde otomatik visual regression testi yoktur.
- Network kesintisi, uzun süreli yük ve yüksek veri hacmi için performans testi yapılmamıştır.
- JWT expiration'ın sekiz saat beklenerek doğrulandığı uzun süreli test yoktur.
- Rapor hazırlanırken npm registry erişimi olmadığı için güncel online npm audit tekrar edilememiştir.

## 14. Kabul Kriterleri Sonucu

Kriterler `docs/01-analiz-ve-gereksinimler.md` içindeki AC-01–AC-15 listesinden alınmıştır. **PASS** yalnızca otomatik, E2E veya manuel kanıt yeterliyse verilmiştir. Kaynak kodda endpoint bulunması tek başına test kanıtı sayılmamıştır.

| Kabul kriteri | Kanıt | Durum |
|---|---|---|
| AC-01 — Geçerli kayıt, JWT ve korumalı ekran | AUTO-01; E2E-01; MANUAL-QA-02/03 | PASS |
| AC-02 — Invalid login ve duplicate registration engeli | AUTO-02; EDGE-01/02 | PASS |
| AC-03 — Roadmap create/detail/update/delete | E2E-03 ve MANUAL-QA-04 create/detail kanıtlıyor; second-user update/delete güvenliği var. Sahibin kendi update/delete happy path'i ayrı kayıtta doğrulanmadı. | PARTIAL |
| AC-04 — Geçerli week ve duration dışı week reddi | E2E-04; AUTO-04; EDGE-05 | PASS |
| AC-05 — Task create/read/update/delete | E2E-05 ve Today/Week read kanıtı var; second-user update/delete güvenliği var. Sahibin kendi update/delete happy path'i ayrı test kaydında yok. | PARTIAL |
| AC-06 — Today ve This Week ekranları | E2E-06/07; MANUAL-QA-07/08 | PASS |
| AC-07 — Completion timestamp, overdue ve progress | E2E-08/09/10/11; AUTO-07/10 | PASS |
| AC-08 — Completion geri alma | AUTO-07 | PASS |
| AC-09 — İkinci kullanıcı ownership izolasyonu | Bölüm 9 E2E güvenlik tablosu; AUTO-05/06/09/17 | PASS |
| AC-10 — Reminder üretimi ve duplicate engeli | E2E-12; EDGE-10; AUTO-09 | PASS |
| AC-11 — Reminder read ve cross-user engeli | E2E-13; AUTO-09 | PASS |
| AC-12 — Beş AI action için uygun fallback | AUTO-11–AUTO-15; MANUAL-QA-10 | PASS |
| AC-13 — Proposal otomatik değiştirmez, explicit apply değiştirir | E2E-16/17/18 | PASS |
| AC-14 — Completed/invalid/cross-user AI apply reddi | E2E-19; AUTO-16–AUTO-19 | PASS |
| AC-15 — Backend build/test ve frontend lint/build | Bölüm 4, 5 ve 6 gerçek komut sonuçları | PASS |

Kabul kriteri özeti:

| Durum | Sayı |
|---|---:|
| PASS | 13 |
| PARTIAL | 2 |
| FAIL | 0 |

## 15. Genel Sonuç

DevTrack AI MVP'nin authentication, temel veri modeli, ownership isolation, completion/progress, reminders ve kontrollü AI rescheduling davranışları otomatik testler ve gerçek PostgreSQL E2E kontrolleriyle önemli ölçüde doğrulanmıştır. Son backend build sıfır warning/sıfır error ile, backend testleri 19/19 ve frontend lint/production build başarılı tamamlanmıştır.

Teslim açısından kritik bir açık hata kaydı bulunmamaktadır. Bununla birlikte kalite kanıtı tam değildir: iki kabul kriterindeki own-resource update/delete happy path'leri ayrı test kayıtlarına sahip değildir; frontend component/browser E2E otomasyonu ve PostgreSQL'e karşı otomatik integration suite bulunmamaktadır. Bu eksikler MVP'yi çalışmaz hâle getiren blocker değildir, fakat daha yüksek güven seviyesi için sonraki test çalışmalarının öncelikli alanlarıdır.
