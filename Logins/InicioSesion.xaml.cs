using System;
using Microsoft.Maui.Controls;
// Agregamos los namespaces de tus nuevos servicios y modelos
using Aula013._0.Services;
using Aula013._0.Models;

namespace Aula013._0
{
    public partial class InicioSesion : ContentPage
    {
        // Cambiamos ApiService por tu servicio de base de datos local
        private readonly LocalDbService _dbService;

        public InicioSesion()
        {
            InitializeComponent();
            // Inicializamos el servicio de base de datos
            _dbService = new LocalDbService();
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            // 1. Validaciones básicas (que no esté vacío)
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                await DisplayAlert("Error", "Debes ingresar correo y contraseña", "OK");
                return;
            }

            // 2. Intentamos hacer Login en la Base de Datos Local
            // Esto busca en el archivo interno del celular si existe el usuario
            var usuarioEncontrado = await _dbService.Login(txtEmail.Text, txtPassword.Text);

            if (usuarioEncontrado != null)
            {
                // ✅ LOGIN EXITOSO

                // Guardamos los datos en tu clase estática 'SesionActual'
                // para poder usarlos luego al lanzar Godot desde la MainPage
                SesionActual.Usuario = usuarioEncontrado.Nombre;
                SesionActual.UsuarioId = usuarioEncontrado.Id;
                SesionActual.Email = usuarioEncontrado.Email;

                // (Opcional) Mensaje de bienvenida rápido
                // await DisplayAlert("Bienvenido", $"Hola {usuarioEncontrado.Nombre}", "Entrar");

                // ✅ Navegar a la página principal (donde estará el botón de "Jugar")
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                // ❌ LOGIN FALLIDO
                await DisplayAlert("Error", "Correo o contraseña incorrectos", "OK");
            }
        }

        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            // Navegar a la pantalla de registro
            await Shell.Current.GoToAsync("//Register");
        }
    }
}