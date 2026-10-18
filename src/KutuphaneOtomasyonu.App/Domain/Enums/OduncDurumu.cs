namespace KutuphaneOtomasyonu.App.Domain.Enums;

/// <summary>
/// Ödünç alma işleminin güncel durumunu belirtir.
/// </summary>
public enum OduncDurumu
{
    Aktif,      // Kitap öğrencide, henüz teslim edilmedi
    IadeEdildi, // Kitap kütüphaneye teslim edildi
    Gecikmis    // Teslim süresi aşıldı
}
