using System.Text;
using KutuphaneOtomasyonu.App.BusinessLogic.Interfaces;
using KutuphaneOtomasyonu.App.BusinessLogic.Services;
using KutuphaneOtomasyonu.App.DataAccess.Interfaces;
using KutuphaneOtomasyonu.App.DataAccess.Repositories;
using KutuphaneOtomasyonu.App.Domain.Entities;

namespace KutuphaneOtomasyonu.App;

public class Program
{
    private static IKitapRepository _kitapRepo = null!;
    private static IKullaniciRepository _kullaniciRepo = null!;
    private static IOduncRepository _oduncRepo = null!;

    private static IKitapServisi _kitapServisi = null!;
    private static IOduncServisi _oduncServisi = null!;
    private static IRaporServisi _raporServisi = null!;

    public static void Main(string[] args)
    {
        // Türkçe karakterlerin konsolda doğru görünmesi için UTF-8 yapılandırması
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Katmanların ve Bağımlılıkların Başlatılması (Dependency Injection)
        InitializeDependencies();

        bool devam = true;
        while (devam)
        {
            Console.Clear();
            YazdirBanner();
            YazdirMenu();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nLütfen bir işlem seçiniz (0-10): ");
            Console.ResetColor();

            string? secim = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (secim)
            {
                case "1":
                    TumKitaplariListele();
                    break;
                case "2":
                    KitapAra();
                    break;
                case "3":
                    KitapOduncAl();
                    break;
                case "4":
                    KitapIadeEt();
                    break;
                case "5":
                    OgrenciOduncGecmisi();
                    break;
                case "6":
                    YeniKitapEkle();
                    break;
                case "7":
                    KitapStokGuncelle();
                    break;
                case "8":
                    GecikmeRaporuGoruntule();
                    break;
                case "9":
                    GenelIstatistikleriGoruntule();
                    break;
                case "10":
                    UmlSenaryosunuCalistir();
                    break;
                case "0":
                    devam = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Kütüphane sisteminden çıkış yapıldı. İyi çalışmalar dileriz!");
                    Console.ResetColor();
                    break;
                default:
                    HataMesajiYaz("Geçersiz seçim! Lütfen menüdeki numaralardan birini tuşlayınız.");
                    break;
            }

            if (devam)
            {
                Console.WriteLine("\nAna menüye dönmek için bir tuşa basınız...");
                Console.ReadKey();
            }
        }
    }

    private static void InitializeDependencies()
    {
        // Data Access Layer
        _kitapRepo = new InMemoryKitapRepository();
        _kullaniciRepo = new InMemoryKullaniciRepository();
        _oduncRepo = new InMemoryOduncRepository(_kullaniciRepo, _kitapRepo);

        // Business Logic Layer
        _kitapServisi = new KitapServisi(_kitapRepo);
        _oduncServisi = new OduncServisi(_oduncRepo, _kitapRepo, _kullaniciRepo);
        _raporServisi = new RaporServisi(_oduncRepo, _kitapRepo);
    }

    private static void YazdirBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("===============================================================================");
        Console.WriteLine("           ÜNİVERSİTE KÜTÜPHANE YÖNETİM SİSTEMİ (UML MİMARİSİ)                ");
        Console.WriteLine("===============================================================================");
        Console.ResetColor();
    }

    private static void YazdirMenu()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" [ÖĞRENCİ İŞLEMLERİ]");
        Console.WriteLine("   1. Tüm Kitapları Listele");
        Console.WriteLine("   2. Kitap Ara (Başlık, Yazar veya ISBN)");
        Console.WriteLine("   3. Kitap Ödünç Al");
        Console.WriteLine("   4. Kitap İade Et");
        Console.WriteLine("   5. Öğrenci Ödünç Durumu & Geçmişi Sorgula");
        Console.WriteLine();
        Console.WriteLine(" [YÖNETİCİ İŞLEMLERİ]");
        Console.WriteLine("   6. Yeni Kitap Ekle");
        Console.WriteLine("   7. Kitap Stok Miktarını Güncelle");
        Console.WriteLine("   8. Geciken Kitaplar ve Ceza Raporu");
        Console.WriteLine("   9. Genel Kütüphane İstatistikleri");
        Console.WriteLine();
        Console.WriteLine(" [UML & SİSTEM]");
        Console.WriteLine("  10. 🎯 UML Object Diagram Senaryosu Simülasyonu (Ali -> Sefiller)");
        Console.WriteLine("   0. Çıkış");
        Console.ResetColor();
    }

    private static void TumKitaplariListele()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- KÜTÜPHANE KİTAP KATALOĞU ---");
        Console.ResetColor();

        var kitaplar = _kitapServisi.TumKitaplariGetir().ToList();
        if (!kitaplar.Any())
        {
            Console.WriteLine("Kütüphanede kayıtlı kitap bulunmamaktadır.");
            return;
        }

        Console.WriteLine("{0,-18} | {1,-26} | {2,-20} | {3,-6} | {4,-10}", "ISBN", "Başlık", "Yazar", "Yıl", "Stok");
        Console.WriteLine(new string('-', 90));

        foreach (var k in kitaplar)
        {
            string stokDurum = $"{k.MevcutStok}/{k.ToplamStok} " + (k.MusaitMi() ? "(Müsait)" : "(Tükendi)");
            ConsoleColor renk = k.MusaitMi() ? ConsoleColor.White : ConsoleColor.DarkGray;
            Console.ForegroundColor = renk;
            Console.WriteLine("{0,-18} | {1,-26} | {2,-20} | {3,-6} | {4,-10}", k.ISBN, Kisalt(k.Baslik, 26), Kisalt(k.Yazar, 20), k.YayinYili, stokDurum);
            Console.ResetColor();
        }
    }

    private static void KitapAra()
    {
        Console.Write("Aramak istediğiniz kitap adı, yazar veya ISBN giriniz: ");
        string? arama = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(arama))
        {
            HataMesajiYaz("Arama terimi boş olamaz.");
            return;
        }

        var sonuclar = _kitapServisi.KitapAra(arama).ToList();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n'{arama}' terimi için {sonuclar.Count} adet sonuç bulundu:");
        Console.ResetColor();

        foreach (var k in sonuclar)
        {
            Console.WriteLine($" • {k}");
        }
    }

    private static void KitapOduncAl()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- KİTAP ÖDÜNÇ ALMA ---");
        Console.ResetColor();

        Console.Write("Öğrenci Numarası (Örn: 2023105021 - Ali Yılmaz): ");
        string? ogrenciNo = Console.ReadLine()?.Trim();
        var ogrenci = _kullaniciRepo.GetOgrenciByNo(ogrenciNo ?? "");

        if (ogrenci == null)
        {
            HataMesajiYaz($"'{ogrenciNo}' numaralı öğrenci bulunamadı.");
            return;
        }

        Console.WriteLine($"Öğrenci Doğrulandı: {ogrenci.TamAd} ({ogrenci.Bolum})");

        Console.Write("Ödünç Alınacak Kitabın ISBN'i (Örn: 978-975-07-2645-3): ");
        string? isbn = Console.ReadLine()?.Trim();

        var sonuc = _oduncServisi.KitapOduncAl(ogrenci.Id, isbn ?? "");
        if (sonuc.Basarili)
        {
            BasariMesajiYaz(sonuc.Mesaj);
            if (sonuc.Islem != null)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Oluşturulan İşlem ID : {sonuc.Islem.IslemId}");
                Console.WriteLine($"Ödünç Veriliş Tarihi : {sonuc.Islem.OduncTarihi:dd.MM.yyyy}");
                Console.WriteLine($"Son Teslim Tarihi    : {sonuc.Islem.SonTeslimTarihi:dd.MM.yyyy}");
                Console.ResetColor();
            }
        }
        else
        {
            HataMesajiYaz(sonuc.Mesaj);
        }
    }

    private static void KitapIadeEt()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- KİTAP İADE ETME ---");
        Console.ResetColor();

        Console.Write("İade Edilecek Ödünç İşlem ID'si (Örn: ODNC-SEED-001): ");
        string? islemId = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(islemId))
        {
            HataMesajiYaz("İşlem ID boş olamaz.");
            return;
        }

        var sonuc = _oduncServisi.KitapIadeEt(islemId);
        if (sonuc.Basarili)
        {
            BasariMesajiYaz(sonuc.Mesaj);
        }
        else
        {
            HataMesajiYaz(sonuc.Mesaj);
        }
    }

    private static void OgrenciOduncGecmisi()
    {
        Console.Write("Öğrenci Numarası giriniz (Örn: 2023105021 / 2023105023): ");
        string? no = Console.ReadLine()?.Trim();
        var ogrenci = _kullaniciRepo.GetOgrenciByNo(no ?? "");

        if (ogrenci == null)
        {
            HataMesajiYaz("Öğrenci bulunamadı.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n--- {ogrenci.TamAd} ({ogrenci.OgrenciNo}) ÖDÜNÇ GEÇMİŞİ ---");
        Console.ResetColor();

        var islemler = _oduncServisi.OgrencininOduncleriniGetir(ogrenci.Id).ToList();
        if (!islemler.Any())
        {
            Console.WriteLine("Bu öğrenciye ait ödünç kaydı bulunmamaktadır.");
            return;
        }

        foreach (var islem in islemler)
        {
            int gecikme = islem.GecikmeGunuHesapla();
            ConsoleColor renk = gecikme > 0 ? ConsoleColor.Red : ConsoleColor.White;
            Console.ForegroundColor = renk;
            Console.WriteLine($" • {islem}");
            if (gecikme > 0)
            {
                Console.WriteLine($"   --> DİKKAT: {gecikme} gün gecikmede! Ceza Tutarı: {islem.CezaUcretiHesapla():C2}");
            }
            Console.ResetColor();
        }
    }

    private static void YeniKitapEkle()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [YÖNETİCİ] YENİ KİTAP EKLEME ---");
        Console.ResetColor();

        Console.Write("ISBN: ");
        string isbn = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Kitap Başlığı: ");
        string baslik = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Yazar: ");
        string yazar = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Yayın Yılı: ");
        if (!int.TryParse(Console.ReadLine(), out int yayinYili))
        {
            HataMesajiYaz("Geçersiz yayın yılı.");
            return;
        }

        Console.Write("Toplam Stok Adedi: ");
        if (!int.TryParse(Console.ReadLine(), out int stok) || stok < 1)
        {
            HataMesajiYaz("Stok adedi en az 1 olmalıdır.");
            return;
        }

        try
        {
            var yeniKitap = new Kitap(isbn, baslik, yazar, yayinYili, stok);
            var sonuc = _kitapServisi.KitapEkle(yeniKitap);
            if (sonuc.Basarili)
                BasariMesajiYaz(sonuc.Mesaj);
            else
                HataMesajiYaz(sonuc.Mesaj);
        }
        catch (Exception ex)
        {
            HataMesajiYaz($"Hata: {ex.Message}");
        }
    }

    private static void KitapStokGuncelle()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [YÖNETİCİ] KİTAP STOK GÜNCELLEME ---");
        Console.ResetColor();

        Console.Write("Güncellenecek Kitabın ISBN'i: ");
        string isbn = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Yeni Toplam Stok Adedi: ");
        if (!int.TryParse(Console.ReadLine(), out int yeniStok))
        {
            HataMesajiYaz("Geçersiz stok adedi.");
            return;
        }

        var sonuc = _kitapServisi.StokGuncelle(isbn, yeniStok);
        if (sonuc.Basarili)
            BasariMesajiYaz(sonuc.Mesaj);
        else
            HataMesajiYaz(sonuc.Mesaj);
    }

    private static void GecikmeRaporuGoruntule()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("===============================================================================");
        Console.WriteLine("               [YÖNETİCİ] GECİKEN KİTAPLAR VE CEZA RAPORU                      ");
        Console.WriteLine("===============================================================================");
        Console.ResetColor();

        var gecikenler = _raporServisi.GecikenKitaplariRaporla().ToList();
        if (!gecikenler.Any())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Tebrikler! Şu an kütüphanede teslim süresi geçmiş aktif bir ödünç kaydı bulunmamaktadır.");
            Console.ResetColor();
            return;
        }

        double toplamCeza = 0;
        Console.WriteLine("{0,-15} | {1,-18} | {2,-20} | {3,-12} | {4,-10} | {5,-10}",
            "İşlem ID", "Öğrenci", "Kitap", "Son Teslim", "Gecikme", "Ceza");
        Console.WriteLine(new string('-', 95));

        foreach (var islem in gecikenler)
        {
            int gun = islem.GecikmeGunuHesapla();
            double ceza = islem.CezaUcretiHesapla();
            toplamCeza += ceza;

            Console.WriteLine("{0,-15} | {1,-18} | {2,-20} | {3,-12:dd.MM.yyyy} | {4,-10} | {5,-10:C2}",
                islem.IslemId,
                Kisalt(islem.Ogrenci.TamAd, 18),
                Kisalt(islem.Kitap.Baslik, 20),
                islem.SonTeslimTarihi,
                $"{gun} gün",
                ceza);
        }

        Console.WriteLine(new string('-', 95));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Toplam Geciken Kitap Sayısı : {gecikenler.Count} adet");
        Console.WriteLine($"Tahakkuk Eden Toplam Ceza    : {toplamCeza:C2}");
        Console.ResetColor();
    }

    private static void GenelIstatistikleriGoruntule()
    {
        var ist = _raporServisi.OzetIstatistikleriGetir();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- KÜTÜPHANE GENEL PERFORMANS VE DURUM RAPORU ---");
        Console.ResetColor();

        Console.WriteLine($" • Toplam Kitap Çeşidi          : {ist.ToplamKitapCesidi}");
        Console.WriteLine($" • Toplam Fiziksel Kitap Sayısı : {ist.ToplamFizikselKitap}");
        Console.WriteLine($" • Şu Anda Ödünçte Olan Kitaplar: {ist.OdunctekiKitap}");
        Console.WriteLine($" • Gecikmedeki Kitap Sayısı     : {ist.GecikenAdet}");
        Console.ForegroundColor = ist.GecikenAdet > 0 ? ConsoleColor.Red : ConsoleColor.Green;
        Console.WriteLine($" • Toplam Ceza Alacağı          : {ist.ToplamTahakkukEdenCeza:C2}");
        Console.ResetColor();
    }

    private static void UmlSenaryosunuCalistir()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("===============================================================================");
        Console.WriteLine("     🎯 UML OBJECT DIAGRAM SENARYO CANLANDIRMASI (Ali -> Sefiller)");
        Console.WriteLine("===============================================================================");
        Console.ResetColor();

        Console.WriteLine("Senaryo Açıklaması:");
        Console.WriteLine("UML Modellemesinde hazırlanan Ali Yılmaz isimli öğrencinin 'Sefiller' kitabını");
        Console.WriteLine("ödünç alma işlemi kod üzerinden somut nesnelerle çalıştırılıyor...\n");

        var ali = _kullaniciRepo.GetOgrenciById(101); // Ali Yılmaz
        var sefiller = _kitapRepo.GetByIsbn("978-975-07-2645-3"); // Sefiller

        if (ali == null || sefiller == null)
        {
            HataMesajiYaz("Gerekli seed verileri bulunamadı.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"1) Başlangıç Durumu:");
        Console.WriteLine($"   - {ali}");
        Console.WriteLine($"   - {sefiller}");
        Console.WriteLine($"   - Ali'nin aktif ödünç sayısı: {_oduncServisi.OgrencininAktifOduncleriniGetir(ali.Id).Count()}\n");

        Console.WriteLine($"2) Ödünç Alma İşlemi Başlatılıyor...");
        var sonuc = _oduncServisi.KitapOduncAl(ali.Id, sefiller.ISBN);

        if (sonuc.Basarili && sonuc.Islem != null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   ✔ {sonuc.Mesaj}");
            Console.ResetColor();

            Console.WriteLine($"\n3) Bellekteki Somut Nesne Durumu (Object Diagram State):");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   [object] ali:Ogrenci         => Id: {ali.Id}, Ad: '{ali.TamAd}', Limit: {ali.OduncLimiti}");
            Console.WriteLine($"   [object] sefiller:Kitap       => ISBN: {sefiller.ISBN}, Stok: {sefiller.MevcutStok}/{sefiller.ToplamStok}");
            Console.WriteLine($"   [object] islem:OduncIslemi    => ID: {sonuc.Islem.IslemId}, Durum: {sonuc.Islem.Durum}, Son Teslim: {sonuc.Islem.SonTeslimTarihi:dd.MM.yyyy}");
            Console.ResetColor();

            Console.WriteLine($"\n4) Doğrulama:");
            Console.WriteLine($"   - Sefiller mevcut stoku 5'ten {sefiller.MevcutStok}'e indi.");
            Console.WriteLine($"   - Ali'nin aktif ödünç adedi 1 oldu.");
        }
        else
        {
            HataMesajiYaz($"İşlem başarısız: {sonuc.Mesaj}");
        }
    }

    private static void BasariMesajiYaz(string mesaj)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✔ {mesaj}");
        Console.ResetColor();
    }

    private static void HataMesajiYaz(string mesaj)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✖ {mesaj}");
        Console.ResetColor();
    }

    private static string Kisalt(string metin, int maxUzunluk)
    {
        if (string.IsNullOrEmpty(metin) || metin.Length <= maxUzunluk)
            return metin;

        return metin.Substring(0, maxUzunluk - 3) + "...";
    }
}
