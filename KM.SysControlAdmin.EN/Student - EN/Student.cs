#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KM.SysControlAdmin.EN.User___EN;

#endregion

namespace KM.SysControlAdmin.EN.Student___EN
{
    public class Student
    {
        #region ATRIBUTOS DE LA ENTIDAD
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El Codigo de Estudiante es Requerido")]
        [StringLength(6, ErrorMessage = "Maximo 6 caracteres")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9]+$", ErrorMessage = "Debe contener solo Letras y Numeros")]
        [Display(Name = "Codigo de Estudiante")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Codigo de Proyecto es Requerido")]
        [StringLength(6, ErrorMessage = "Maximo 6 caracteres")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9]+$", ErrorMessage = "Debe contener solo Letras y Numeros")]
        [Display(Name = "Codigo de Proyecto")]
        public string ProjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Codigo de Participante es Requerido")]
        [StringLength(5, ErrorMessage = "Maximo 5 caracteres")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Debe contener solo Numeros")]
        [Display(Name = "Codigo de Participante")]
        public string ParticipantCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Nombre Es Requerido")]
        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        [Display(Name = "Nombres")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "Debe contener solo Letras")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Apellido Es Requerido")]
        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        [Display(Name = "Apellidos")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "Debe contener solo Letras")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Fecha De Nacimiento Es Requerida")]
        [Display(Name = "Fecha De Nacimiento")]
        [DataType(DataType.Date, ErrorMessage = "Por favor, Introduce una fecha válida")]
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;

        [Required(ErrorMessage = "La Edad Es Requerida")]
        [StringLength(3, ErrorMessage = "Maximo 3 caracteres")]
        [Display(Name = "Edad")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "La edad debe contener solo números")]
        public string Age { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Maximo 100 caracteres")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ/. ]+$", ErrorMessage = "Debe contener solo Letras")]
        [Display(Name = "Nombre de la Iglesia")]
        public string? ChurchName { get; set; }

        [Required(ErrorMessage = "El Estado Es Requerido")]
        [Display(Name = "Estado")]
        public byte Status { get; set; }

        [Display(Name = "Fotografia")]
        public byte[]? ImageData { get; set; }

        [Display(Name = "Fecha De Creación")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Fecha De Modificación")]
        public DateTime DateModification { get; set; }

        [Required(ErrorMessage = "El Genero Es Requerido")]
        [StringLength(20, ErrorMessage = "Maximo 20 caracteres")]
        [Display(Name = "Genero")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "Debe contener solo Letras")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Correo Personal es requerido")]
        [MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        [Display(Name = "Correo Electronico Personal")]
        public string PersonalEmail { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Maximo 300 caracteres")]
        [Display(Name = "Comentarios u Observaciones")]
        public string? CommentsOrObservations { get; set; }

        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        [Display(Name = "Nombres")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "Debe contener solo Letras")]
        public string? RepresentativeName { get; set; }

        [StringLength(50, ErrorMessage = "Maximo 50 caracteres")]
        [Display(Name = "Apellidos")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "Debe contener solo Letras")]
        public string? RepresentativeLastName { get; set; }

        [MaxLength(9, ErrorMessage = "Máximo 9 caracteres")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ/ ]+$", ErrorMessage = "Debe contener solo Letras")]
        [Display(Name = "Parentesco")]
        public string? Relationship { get; set; }

        [StringLength(9, ErrorMessage = "Maximo 9 caracteres")]
        [Display(Name = "Telefono Del Responsable")]
        [RegularExpression("^[0-9-]+$", ErrorMessage = "El Telefono debe contener solo números")]
        public string? TelephoneResponsible { get; set; }
        #endregion

        #region ATRIBUTOS NO MAPEABLES
        // Propiedad para formatear la fecha automáticamente
        [NotMapped]
        public string DateOfBirthFormatted => DateOfBirth.ToString(@"dd/MM/yyyy");
        [NotMapped]
        public string DateCreatedFormatted => DateCreated.ToString(@"dd/MM/yyyy");
        [NotMapped]
        public string DateModificationFormatted => DateModification.ToString(@"dd/MM/yyyy");

        // Propiedad para formatear la hora con AM/PM
        [NotMapped]
        public string TimeCreatedFormatted => DateCreated.ToString("hh:mm tt");
        [NotMapped]
        public string TimeModificationFormatted => DateModification.ToString("hh:mm tt");
        #endregion

        #region ATRIBUTOS NO MAPEABLES EXTERNOS
        [NotMapped]
        [Required(ErrorMessage = "El Correo Electronico es requerido")]
        [MaxLength(50, ErrorMessage = "Máximo 30 caracteres")]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; } = string.Empty;

        [NotMapped]
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [StringLength(32, ErrorMessage = "La contraseña debe estar entre 6 y 32 caracteres", MinimumLength = 6)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        [ForeignKey("Role")]
        [Required(ErrorMessage = "El rol es requerido")]
        [Display(Name = "Rol")]
        public int IdRole { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "La confirmación de la contraseña es requerida")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [StringLength(32, ErrorMessage = "La contraseña debe tener entre 6 y 32 caracteres", MinimumLength = 6)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword_Aux { get; set; } = string.Empty; //propiedad auxiliar

        [NotMapped]
        [Required(ErrorMessage = "El Correo De Recuperacion es requerido")]
        [MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        [Display(Name = "Correo De Recuperacion")]
        public string RecoveryEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Codigo de Usuario es requerido")]
        [MaxLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Codigo de Usuario")]
        public string CodeUser { get; set; } = string.Empty;

        [NotMapped]
        public User? User { get; set; } //propiedad de navegación
        #endregion
    }
}
