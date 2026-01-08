using Aula013._0.Services; // <--- Importante: Usamos Services en lugar de BasedeDatos

namespace Aula013._0;

public partial class Register : ContentPage
{
    // Declaramos tu servicio de base de datos local
    private readonly LocalDbService _dbService;

    public Register()
    {
        InitializeComponent();
        // Inicializamos la conexión a la base de datos local
        _dbService = new LocalDbService();
    }

    private async void Resgisterbt_Clicked(object sender, EventArgs e)
    {
        // 1. Validaciones visuales (igual que antes)
        if (txtPassword.Text != txtPassword2.Text)
        {
            await DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
        {
            await DisplayAlert("Error", "Debes ingresar un correo válido", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
        {
            await DisplayAlert("Error", "Debes ingresar un nombre de usuario", "OK");
            return;
        }

        // 2. Intentamos registrar en la Base de Datos Local
        // El método Registrar que creamos devuelve 'true' si se guardó, 'false' si el email ya existía
        bool registroExitoso = await _dbService.Registrar(txtUsuario.Text, txtEmail.Text, txtPassword.Text);

        if (registroExitoso)
        {
            // ✅ ÉXITO
            await DisplayAlert("Éxito", "Usuario registrado correctamente en el celular", "OK");
            LimpiarCampos();

            // Regresamos al Login para que inicie sesión
            await Shell.Current.GoToAsync("//InicioSesion");
        }
        else
        {
            // ❌ ERROR (Probablemente el email ya existe)
            await DisplayAlert("Error", "Ese correo ya está registrado, intenta con otro.", "OK");
        }
    }

    private async void volverResgister_Clicked(object sender, EventArgs e)
    {
        LimpiarCampos();
        await Shell.Current.GoToAsync("//InicioSesion");
    }

    private void LimpiarCampos()
    {
        // El operador ?. evita errores si el control aún no se ha creado (buena práctica)
        if (txtUsuario != null) txtUsuario.Text = string.Empty;
        if (txtEmail != null) txtEmail.Text = string.Empty;
        if (txtPassword != null) txtPassword.Text = string.Empty;
        if (txtPassword2 != null) txtPassword2.Text = string.Empty;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LimpiarCampos();
    }
}