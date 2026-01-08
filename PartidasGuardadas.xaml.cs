using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Aula013._0
{
    public partial class PartidasGuardadas : ContentPage
    {
        // Diccionario de slots persistente por usuario
        public static readonly Dictionary<string, ObservableCollection<Slot>> SlotsPorUsuario = new();

        // Ruta interna de MAUI para su propia base de datos de slots
        private string rutaDbMaui = Path.Combine(FileSystem.AppDataDirectory, "launcher_slots.json");

        public PartidasGuardadas()
        {
            InitializeComponent();
            CargarBaseDeDatosLocal();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarBaseDeDatosLocal();
            CargarSlots();
        }

        #region Persistencia de la Lista de Slots (MAUI)

        private void GuardarBaseDeDatosLocal()
        {
            try
            {
                string json = JsonSerializer.Serialize(SlotsPorUsuario);
                File.WriteAllText(rutaDbMaui, json);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "No se pudo guardar la lista de slots: " + ex.Message, "OK");
            }
        }

        private void CargarBaseDeDatosLocal()
        {
            if (!File.Exists(rutaDbMaui)) return;

            try
            {
                string json = File.ReadAllText(rutaDbMaui);
                var datosGuardados = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<Slot>>>(json);

                if (datosGuardados != null)
                {
                    SlotsPorUsuario.Clear();
                    foreach (var kvp in datosGuardados)
                    {
                        SlotsPorUsuario.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al cargar BD MAUI: " + ex.Message);
            }
        }

        #endregion

        private void CargarSlots()
        {
            var usuario = SesionActual.Usuario;
            if (string.IsNullOrWhiteSpace(usuario)) return;

            if (!SlotsPorUsuario.ContainsKey(usuario))
                SlotsPorUsuario[usuario] = new ObservableCollection<Slot>();

            var slots = SlotsPorUsuario[usuario];
            string rutaGodot;

            // DETERMINAR RUTA SEGÚN PLATAFORMA
#if ANDROID
            // En Android, Godot guarda en la carpeta interna del paquete
            // Reemplaza "Aula013 master" si el nombre del proyecto en Godot es distinto
            var context = Android.App.Application.Context;
            rutaGodot = Path.Combine(context.GetExternalFilesDir(null).AbsolutePath, "Godot", "app_userdata", "Aula013 master");
#else
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            rutaGodot = Path.Combine(appData, "Godot", "app_userdata", "Aula013 master");
#endif

            foreach (var slot in slots)
            {
                string nombreArchivo = $"save_{slot.Id}.json";
                string rutaArchivo = Path.Combine(rutaGodot, nombreArchivo);
                slot.ExisteArchivo = File.Exists(rutaArchivo);

                Debug.WriteLine($"Buscando: {rutaArchivo} | Encontrado: {slot.ExisteArchivo}");
            }

            ActualizarInterfaz(slots);
        }

        private void ActualizarInterfaz(ObservableCollection<Slot> slots)
        {
            if (slots.Count == 0)
            {
                lbler.IsVisible = true;
                slotList.IsVisible = false;
            }
            else
            {
                lbler.IsVisible = false;
                slotList.IsVisible = true;
                slotList.ItemsSource = slots;
            }
        }

        private void btnNuevoSlot_Clicked(object sender, EventArgs e)
        {
            var usuario = SesionActual.Usuario;
            if (string.IsNullOrWhiteSpace(usuario)) return;

            var nuevo = new Slot
            {
                Id = Guid.NewGuid().ToString(),
                Nombre = $"Partida {DateTime.Now:dd/MM HH:mm}",
                TiempoJugado = "0h",
                Ubicacion = "--",
                ExisteArchivo = false
            };

            SlotsPorUsuario[usuario].Add(nuevo);
            GuardarBaseDeDatosLocal();
            if (SlotsPorUsuario[usuario].Count == 1) CargarSlots();
        }

        private void btnLanzar_Clicked(object sender, EventArgs e)
        {
            var slot = (sender as Button)?.BindingContext as Slot;
            if (slot == null) return;

#if ANDROID
            try
            {
                // CAMBIA ESTO por el Package Name que configures en Godot al exportar
                string packageName = "com.example.$genname";

                var intent = Android.App.Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
                if (intent != null)
                {
                    // Enviamos el ID como un "Extra" que Godot leerá
                    intent.PutExtra("slot_id", slot.Id);
                    intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);

                    slot.ExisteArchivo = true;
                    GuardarBaseDeDatosLocal();
                }
                else
                {
                    DisplayAlert("Error", "Asegúrate de tener el juego instalado.", "OK");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error en Android", ex.Message, "OK");
            }
#else
            // Lógica para Windows
            string rutaJuego = @"C:\Users\sergi\OneDrive\Escritorio\aula-013-master\Ejecutables windows\Prerelease\Aula013_prerelease.exe";
            if (!File.Exists(rutaJuego))
            {
                DisplayAlert("Error", "No se encuentra el ejecutable en Windows", "OK");
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = rutaJuego,
                    Arguments = $"--slot-id \"{slot.Id}\"",
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(rutaJuego)
                };
                Process.Start(startInfo);
                slot.ExisteArchivo = true;
                GuardarBaseDeDatosLocal();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error en Windows", ex.Message, "OK");
            }
#endif
        }

        private async void btnEditar_Clicked(object sender, EventArgs e)
        {
            var slot = (sender as Button)?.BindingContext as Slot;
            if (slot == null) return;

            string nuevoNombre = await DisplayPromptAsync("Editar", "Nombre de la partida:", initialValue: slot.Nombre);
            if (!string.IsNullOrWhiteSpace(nuevoNombre))
            {
                slot.Nombre = nuevoNombre;
                GuardarBaseDeDatosLocal();
            }
        }

        private async void btnEliminar_Clicked(object sender, EventArgs e)
        {
            var usuario = SesionActual.Usuario;
            var slot = (sender as Button)?.BindingContext as Slot;

            if (usuario != null && slot != null)
            {
                bool confirmar = await DisplayAlert("Eliminar", $"¿Borrar {slot.Nombre}?", "Sí", "No");
                if (confirmar)
                {
                    SlotsPorUsuario[usuario].Remove(slot);
                    GuardarBaseDeDatosLocal();
                    if (SlotsPorUsuario[usuario].Count == 0) CargarSlots();
                }
            }
        }

        private async void btnRegresar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    public class Slot : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public string Id { get; set; }

        private string nombre;
        public string Nombre
        {
            get => nombre;
            set { nombre = value; OnPropertyChanged(nameof(Nombre)); }
        }

        private string tiempoJugado;
        public string TiempoJugado
        {
            get => tiempoJugado;
            set { tiempoJugado = value; OnPropertyChanged(nameof(TiempoJugado)); }
        }

        private string ubicacion;
        public string Ubicacion
        {
            get => ubicacion;
            set { ubicacion = value; OnPropertyChanged(nameof(Ubicacion)); }
        }

        private bool existeArchivo;
        public bool ExisteArchivo
        {
            get => existeArchivo;
            set
            {
                existeArchivo = value;
                OnPropertyChanged(nameof(ExisteArchivo));
                OnPropertyChanged(nameof(EsNuevaPartida));
            }
        }

        public bool EsNuevaPartida => !ExisteArchivo;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}