using KutuphaneOtomasyonu.App.Domain.Entities;

namespace KutuphaneOtomasyonu.App.DataAccess.Interfaces;

public interface IKitapRepository
{
    IEnumerable<Kitap> GetAll();
    Kitap? GetByIsbn(string isbn);
    IEnumerable<Kitap> Search(string query);
    void Add(Kitap kitap);
    void Update(Kitap kitap);
}
