using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Programacion2ClientesAPI.Models;

[Table("clientes")]
public class Cliente : IValidatableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Range(0, int.MaxValue)]
    public int Id_cliente { get; set; }

    [Required(ErrorMessage = "El CUI es obligatorio.")]
    [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "El CUI debe contener exactamente 13 dígitos.")]
    [StringLength(13)]
    public string CUI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El NIT es obligatorio.")]
    [StringLength(15)]
    public string NIT { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(250)]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20)]
    [RegularExpression(@"^\+?[0-9][0-9 ()-]{6,19}$", ErrorMessage = "Ingrese un teléfono válido de 7 a 20 caracteres.")]
    public string Telefono { get; set; } = string.Empty;

    public DateOnly Fecha_Nacimiento { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Fecha_Nacimiento == default || Fecha_Nacimiento > DateOnly.FromDateTime(DateTime.Today))
        {
            yield return new ValidationResult(
                "La fecha de nacimiento es obligatoria y no puede estar en el futuro.",
                [nameof(Fecha_Nacimiento)]);
        }
    }
}
