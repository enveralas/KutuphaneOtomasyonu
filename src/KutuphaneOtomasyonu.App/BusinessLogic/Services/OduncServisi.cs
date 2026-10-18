using KutuphaneOtomasyonu.App.BusinessLogic.Interfaces;
using KutuphaneOtomasyonu.App.DataAccess.Interfaces;
using KutuphaneOtomasyonu.App.Domain.Entities;
using KutuphaneOtomasyonu.App.Domain.Enums;

namespace KutuphaneOtomasyonu.App.BusinessLogic.Services;

public class OduncServisi : IOduncServisi
{
    private readonly IOduncRepository _oduncRepository;
    private readonly IKitapRepository _kitapRepository;
    private readonly IKullaniciRepository _kullaniciRepository;

    public OduncServisi(
        IOduncRepository oduncRepository,
        IKitapRepository kitapRepository,
        IKullaniciRepository kullaniciRepository)
    {
        _oduncRepository = oduncRepository;
        _kitapRepository = kitapRepository;
        _kullaniciRepository = kullaniciRepository;
    }

    public (bool Basarili, string Mesaj, OduncIslemi? Islem) KitapOduncAl(int ogrenciId, string isbn, int gun = 14)
    {
        var ogrenci = _kullaniciRepository.GetOgrenciById(ogrenciId);
        if (ogrenci == null)
            return (false, $"ID: {ogrenciId} olan öğrenci bulunamadı.", null);

        var kitap = _kitapRepository.GetByIsbn(isbn);
        if (kitap == null)
            return (false, $"ISBN '{isbn}' olan kitap bulunamadı.", null);

        if (!kitap.MusaitMi())
            return (false, $"\"{kitap.Baslik}\" kitabının stokları tükenmiştir. Şu an ödünç verilemez.", null);

        // Öğrencinin aktif ödünç sayısını denetle
        var aktifOduncler = OgrencininAktifOduncleriniGetir(ogrenciId).ToList();
        if (aktifOduncler.Count >= ogrenci.OduncLimiti)
            return (false, $"{ogrenci.TamAd} maksimum ödünç alma limitine ({ogrenci.OduncLimiti} kitap) ulaşmıştır. Yeni kitap alabilmek için önce mevcutları iade etmelidir.", null);

        // Öğrencinin gecikmiş kitabı var mı kontrol et
        var gecikmisVarMi = aktifOduncler.Any(o => o.GecikmeGunuHesapla() > 0);
        if (gecikmisVarMi)
            return (false, $"{ogrenci.TamAd} adına teslim süresi geçmiş kitap bulunmaktadır! Ceza ödenip kitap iade edilmeden yeni kitap ödünç alınamaz.", null);

        // Kitap stokunu azalt
        kitap.StokAzalt();
        _kitapRepository.Update(kitap);

        // Yeni ödünç kaydı oluştur
        string islemId = $"ODNC-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        var yeniIslem = new OduncIslemi(islemId, ogrenci, kitap, DateTime.Now, gun);
        _oduncRepository.Add(yeniIslem);

        return (true, $"\"{kitap.Baslik}\" kitabı {ogrenci.TamAd} adlı öğrenciye başarıyla ödünç verildi. (Son Teslim: {yeniIslem.SonTeslimTarihi:dd.MM.yyyy})", yeniIslem);
    }

    public (bool Basarili, string Mesaj, double Ceza) KitapIadeEt(string islemId)
    {
        var islem = _oduncRepository.GetById(islemId);
        if (islem == null)
            return (false, $"'{islemId}' numaralı ödünç işlemi bulunamadı.", 0);

        if (islem.Durum != OduncDurumu.Aktif)
            return (false, $"Bu işlem zaten daha önce '{islem.Durum}' olarak kapatılmış.", 0);

        DateTime iadeTarihi = DateTime.Now;
        islem.TeslimAl(iadeTarihi);

        // Kitabın stoğunu geri artır
        islem.Kitap.StokArtir();
        _kitapRepository.Update(islem.Kitap);
        _oduncRepository.Update(islem);

        double ceza = islem.CezaUcretiHesapla();
        if (ceza > 0)
        {
            int gecikmeGunu = islem.GecikmeGunuHesapla();
            return (true, $"Kitap teslim alındı. DİKKAT: {gecikmeGunu} gün gecikme tespit edildi! Tahsil edilecek ceza tutarı: {ceza:C2}", ceza);
        }

        return (true, $"\"{islem.Kitap.Baslik}\" kitabı zamanında ve eksiksiz olarak teslim alındı.", 0);
    }

    public IEnumerable<OduncIslemi> OgrencininOduncleriniGetir(int ogrenciId)
    {
        return _oduncRepository.GetByOgrenciId(ogrenciId);
    }

    public IEnumerable<OduncIslemi> OgrencininAktifOduncleriniGetir(int ogrenciId)
    {
        return _oduncRepository.GetByOgrenciId(ogrenciId).Where(o => o.Durum == OduncDurumu.Aktif);
    }
}
