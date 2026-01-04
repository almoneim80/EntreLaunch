namespace EntreLaunch.DTOs.CertificateDtos;

public class CertificateDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string CertificateFor { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public string CertificateId { get; set; }
    public DateTimeOffset ExpirationDate { get; set; } // if exist
    public ShippingStatus ShippingStatus { get; set; }
    public string ShippingAddress { get; set; }
    public StudentData Student { get; set; }
}

public class StudentData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public double NationalId { get; set; }
    public string PhoneNumber { get; set; }
    public string Specialization { get; set; }
}
