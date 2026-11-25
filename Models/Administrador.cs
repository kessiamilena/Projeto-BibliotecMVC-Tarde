using System;
using System.Collections.Generic;

namespace Projeto_Bibliotec_MVC_tarde.Models;

public partial class Administrador
{
    public int IdAdministrador { get; set; }

    public string NIF { get; set; } = null!;

    public int NivelAcesso { get; set; }

    public virtual Usuario IdAdministradorNavigation { get; set; } = null!;
}
