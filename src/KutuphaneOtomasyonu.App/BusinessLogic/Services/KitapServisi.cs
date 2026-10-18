using KutuphaneOtomasyonu.App.BusinessLogic.Interfaces;
using KutuphaneOtomasyonu.App.DataAccess.Interfaces;
using KutuphaneOtomasyonu.App.Domain.Entities;

namespace KutuphaneOtomasyonu.App.BusinessLogic.Services;

public class KitapServisi : IKitapServisi
{
    private readonly IKitapRepository _kitapRepository;

    public KitapServisi(IKitapRepository kitapRepository)
    {
        _kitapRepository = kitapRepository;
    }

    public (bool Basarili, string Mesaj) KitapEkle(Kitap kitap)
    {
        if (kitap == null)
            return (false, "Kitap bilgisi boş olamaz.");

        if (_kitapRepository.GetByIsbn(kitap.ISBN) != null)
            return (false, $"ISBN '{kitap.ISBN}' olan bir kitap sistemde zaten mevcuttur.");

        _kitapRepository.Add(kitap);
        return (true, $"\"{kitap.Baslik}\" kitabı başarıyla kütüphane envanterine eklendi.");
    }

    public (bool Basarili, string Mesaj) StokGuncelle(string isbn, int yeniStok)
    {
        var kitap = _kitapRepository.GetByIsbn(isbn);
        if (kitap == null)
            return (false, $"'{isbn}' numaralı kitap bulunamadı.");

        try
        {
            kitap.StokMiktariniGuncelle(yeniStok);
            _kitapRepository.Update(kitap);
            return (true, $"\"{kitap.Baslik}\" kitabının yeni toplam stoğu {yeniStok} adet olarak güncellendi. (Mevcut Raftaki: {kitap.MevcutStok})");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public IEnumerable<Kitap> KitapAra(string aramaMetni)
    {
        return _kitapRepository.Search(aramaMetni);
    }

    public IEnumerable<Kitap> TumKitaplariGetir()
    {
        return _kitapRepository.GetAll();
    }

    public Kitap? KitapGetir(string isbn)
    {
        return _kitapRepository.GetByIsbn(isbn);
    }
}
