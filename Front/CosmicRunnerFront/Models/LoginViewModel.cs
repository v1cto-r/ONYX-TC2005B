using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CosmicRunnerFront.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [StringLength(255, ErrorMessage = "Máximo 255 caracteres")]
    public string email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(255, ErrorMessage = "Máximo 255 caracteres")]
    public string password { get; set; }
}