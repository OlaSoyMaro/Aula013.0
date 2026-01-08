using Aula013._0.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Aula013._0
{
    public partial class MainPage : ContentPage
    {
        // Quitamos la variable _gameLauncherService porque aquí ya no lanzamos el juego.
        // La MainPage ahora es solo un menú de navegación.

        public MainPage()
        {
            InitializeComponent();
            // Eliminamos la inyección de dependencias compleja.
            // El constructor queda limpio y sin errores.
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Actualizamos el label con el correo del usuario logueado
            if (lblEmail != null)
            {
                lblEmail.Text = SesionActual.Email;
            }
        }

        private async void BtnCerrarSesion_Clicked(object sender, EventArgs e)
        {
            // Limpiamos la sesión estática
            SesionActual.Usuario = null;
            SesionActual.Email = null;
            SesionActual.UsuarioId = 0;

            // Volvemos al login
            await Shell.Current.GoToAsync("//InicioSesion");
        }

        // --- EL CAMBIO IMPORTANTE ---
        private async void BtnPartida_Clicked(object sender, EventArgs e)
        {
            // Ya no lanzamos Godot aquí.
            // Ahora navegamos a la pantalla de "Slots" o "Partidas Guardadas"
            await Navigation.PushAsync(new PartidasGuardadas());
        }

        private async void BtnOpciones_Clicked(object sender, EventArgs e)
        {
            // Asegúrate de que exista la página Opciones
            await Navigation.PushAsync(new Opciones());
        }

        private async void BtnLogros_Clicked(object sender, EventArgs e)
        {
            // Asegúrate de que exista la página Logros
            await Navigation.PushAsync(new Logros());
        }

        private async void BtnSalir_Clicked(object sender, EventArgs e)
        {
            var salir = await DisplayAlert("Salir", "¿Seguro que quieres salir?", "Sí", "No");
            if (salir)
            {
                // Forma correcta de cerrar la app en Android y Windows (MAUI)
                Application.Current.Quit();
            }
        }
    }
}