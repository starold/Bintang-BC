using FormulirPendaftaranSekolahCRUD.Enums;

namespace FormulirPendaftaranSekolahCRUD.Interfaces
{
    public interface IStudent
    {
        string NamaLengkap { get; }
        string Alamat { get; }
        GenderType Gender { get; }
        DateTime TanggalLahir { get; }
        string NamaOrangTua { get; }
    }
}
