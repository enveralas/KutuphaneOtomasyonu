using KutuphaneOtomasyonu.App.Domain.Entities;
using KutuphaneOtomasyonu.App.Domain.Entities.Base;

namespace KutuphaneOtomasyonu.App.DataAccess.Interfaces;

public interface IKullaniciRepository
{
    Ogrenci? GetOgrenciById(int id);
    Ogrenci? GetOgrenciByNo(string ogrenciNo);
    IEnumerable<Ogrenci> GetAllOgrenciler();
    Yonetici? GetYoneticiById(int id);
    Kullanici? GetByEmail(string email);
    void AddOgrenci(Ogrenci ogrenci);
}
