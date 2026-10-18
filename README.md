# 📚 Üniversite Kütüphane Yönetim Sistemi (UML & C# Clean Architecture)

[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Language-C%23%2012-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%203--Tier-brightgreen.svg)]()
[![License](https://img.shields.io/badge/License-MIT-green.svg)]()

Bir üniversite kütüphanesinin temel gereksinimlerini (kitap arama, ödünç alma, iade, stok takibi, gecikme ve ceza raporlama, kitap ekleme/düzenleme) karşılamak üzere Nesne Yönelimli Programlama (OOP) ve Katmanlı Mimari prensipleriyle geliştirilmiş C# projesi.

---

## 🏛️ 1. Mimari ve Katman Dağılımı (Logical Architecture)

Proje, yazılım mühendisliği standartlarına uygun olarak 3 temel mantıksal pakete (katmana) ayrılmıştır:

```
KutuphaneOtomasyonu/
│
├── .gitignore
├── README.md
├── KutuphaneOtomasyonu.sln
│
└── src/
    └── KutuphaneOtomasyonu.App/
        ├── KutuphaneOtomasyonu.App.csproj
        ├── Program.cs                        # Sunum Katmanı (Presentation Layer - CLI UI)
        │
        ├── Domain/                           # Çekirdek İş Varlıkları (Core Domain)
        │   ├── Entities/
        │   │   ├── Base/
        │   │   │   └── Kullanici.cs          # Soyut Üst Sınıf (Inheritance)
        │   │   ├── Ogrenci.cs                # Öğrenci Üye Varlığı
        │   │   ├── Yonetici.cs               # Kütüphane Yöneticisi Varlığı
        │   │   ├── Kitap.cs                  # Kitap ve Stok Varlığı
        │   │   └── OduncIslemi.cs            # Ödünç Alma & Ceza Hesabı Varlığı
        │   └── Enums/
        │       └── OduncDurumu.cs            # Aktif, IadeEdildi, Gecikmis
        │
        ├── DataAccess/                       # Veri Erişim Katmanı (DAL - Repository Pattern)
        │   ├── Interfaces/
        │   │   ├── IKitapRepository.cs
        │   │   ├── IOduncRepository.cs
        │   │   └── IKullaniciRepository.cs
        │   └── Repositories/
        │       ├── InMemoryKitapRepository.cs
        │       ├── InMemoryOduncRepository.cs
        │       └── InMemoryKullaniciRepository.cs
        │
        └── BusinessLogic/                    # İş Mantığı Katmanı (Services)
            ├── Interfaces/
            │   ├── IKitapServisi.cs
            │   ├── IOduncServisi.cs
            │   └── IRaporServisi.cs
            └── Services/
                ├── KitapServisi.cs
                ├── OduncServisi.cs
                └── RaporServisi.cs
```

---

## 📊 2. UML Diyagramları

### A. Sınıf Diyagramı (Class Diagram)

```mermaid
classDiagram
    class Kullanici {
        <<abstract>>
        #int Id
        #string Ad
        #string Soyad
        #string Email
        #string Sifre
        +GirisYap(email, sifre) bool
        +CikisYap() void
    }

    class Ogrenci {
        -string OgrenciNo
        -string Bolum
        -int OduncLimiti
        +OduncLimitiniGuncelle(yeniLimit) void
    }

    class Yonetici {
        -string PersonelKodu
        -string YetkiSeviyesi
    }

    class Kitap {
        -string ISBN
        -string Baslik
        -string Yazar
        -int YayinYili
        -int ToplamStok
        -int MevcutStok
        +MusaitMi() bool
        +StokAzalt() bool
        +StokArtir() void
        +StokMiktariniGuncelle(yeniStok) void
    }

    class OduncIslemi {
        -string IslemId
        -DateTime OduncTarihi
        -DateTime SonTeslimTarihi
        -DateTime? IadeTarihi
        -OduncDurumu Durum
        +GecikmeGunuHesapla() int
        +CezaUcretiHesapla() double
        +TeslimAl(iadeTarihi) void
    }

    Kullanici <|-- Ogrenci : Kalıtım (Inheritance)
    Kullanici <|-- Yonetici : Kalıtım (Inheritance)
    Ogrenci "1" --> "0..*" OduncIslemi : ödünç alır
    Kitap "1" <-- "0..*" OduncIslemi : referans alır
    Yonetici ..> Kitap : yönetir
    Yonetici ..> OduncIslemi : denetler
```

### B. Somut Nesne Diyagramı (Object Diagram)
*Ali Yılmaz adlı öğrencinin "Sefiller" kitabını ödünç aldığı anın bellekteki durumu:*

```mermaid
classDiagram
    class ali_Ogrenci["ali : Ogrenci"] {
        Id = 101
        Ad = "Ali"
        Soyad = "Yılmaz"
        OgrenciNo = "2023105021"
        Bolum = "Bilgisayar Mühendisliği"
        OduncLimiti = 3
    }

    class islem_402_OduncIslemi["islem_402 : OduncIslemi"] {
        IslemId = "ODNC-2026-402"
        OduncTarihi = "12.10.2026"
        SonTeslimTarihi = "27.10.2026"
        IadeTarihi = null
        Durum = Aktif
    }

    class sefiller_Kitap["sefiller : Kitap"] {
        ISBN = "978-975-07-2645-3"
        Baslik = "Sefiller"
        Yazar = "Victor Hugo"
        ToplamStok = 5
        MevcutStok = 4
    }

    ali_Ogrenci --> islem_402_OduncIslemi : ödünç aldı
    islem_402_OduncIslemi --> sefiller_Kitap : ait olduğu kitap
```

---

## ⚡ 3. Öne Çıkan Özellikler

* **Katalog & Stok Yönetimi:** Kitapların raftaki mevcut stoku ve toplam stoku ayrı izlenir. Kitap ödünç verildiğinde stok otomatik düşer, iade edildiğinde artar.
* **Ödünç Alma & Limit Kuralları:** Öğrencinin ödünç limiti (varsayılan: 3) denetlenir. Öğrencinin üzerinde teslim süresi geçmiş kitap varsa ceza ödenmeden yeni kitap verilmez.
* **Gecikme & Ceza Motoru:** Son teslim tarihini geçen her gün için otomatik ceza (günlük 5 TL) hesaplanır.
* **Yönetici Paneli:** Yeni kitap ekleme, stok güncelleme, kütüphane genel istatistikleri ve geciken kitaplar için raporlama ekranı.
* **Canlı UML Simülasyonu:** Menüdeki 10. seçenek ile UML Object Diagram senaryosu (Ali'nin Sefiller'i alması) tek tıkla simüle edilip bellek durumu ekrana yazdırılır.

---

## 🚀 4. Kurulum ve Çalıştırma

### Gereksinimler
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) veya üzeri

### Adımlar

1. **Projeyi klonlayın:**
   ```bash
   git clone https://github.com/enveralas/KutuphaneOtomasyonu.git
   cd KutuphaneOtomasyonu
   ```

2. **Projeyi derleyin:**
   ```bash
   dotnet build
   ```

3. **Uygulamayı çalıştırın:**
   ```bash
   dotnet run --project src/KutuphaneOtomasyonu.App
   ```

---

## 📝 Lisans

Bu proje [MIT](LICENSE) lisansı ile lisanslanmıştır. Eğitim ve akademik projelerde serbestçe kullanılabilir.
