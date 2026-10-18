using KutuphaneOtomasyonu.App.Domain.Entities;

namespace KutuphaneOtomasyonu.App.BusinessLogic.Interfaces;

public interface IKitapServisi
{
    (bool Basarili, string Mesaj) KitapEkle(Kitap kitap);
    (bool Basarili, string Mesaj) StokGuncelle(string isbn, int yeniStok);
    IEnumerable<Kitap> KitapAra(string aramaMetni);
    IEnumerable<Kitap> TumKitaplariGetir();
    Kitap? KitapGetir(string isbn);
}
