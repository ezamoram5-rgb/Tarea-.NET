using System.ComponentModel.DataAnnotations;

namespace ClientesBlazor.Models;

public class Cliente
{

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

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [FechaNacimientoValida]
    public DateOnly? Fecha_Nacimiento { get; set; }
}

public sealed class FechaNacimientoValidaAttribute : ValidationAttribute
{
    public FechaNacimientoValidaAttribute() : base("La fecha de nacimiento debe ser válida y no puede estar en el futuro.") { }
    public override bool IsValid(object? value) => value is null ||
        value is DateOnly fecha && fecha != default && fecha <= DateOnly.FromDateTime(DateTime.Today);
}


