using System;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using SolexMobileApp.Models;
using Plugin.Media.Abstractions;
using System.Collections.Generic;

namespace SolexMobileApp.Views.Modules
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageGuiaPage : ContentPage
    {
        private Guia guiaSelected = new Guia();
        private List<ImageGuia> listImages = new List<ImageGuia>();

        public ImageGuiaPage(Guia guia)
        {
            InitializeComponent();
            guiaSelected = guia;
            Lbl_Guia.Text = guia.GuiaNumero;
            InitComponents();
        }

        void InitComponents()
        {
            // Cuando haya cargado el listado de imagenes de la guía ocultamos el spinner
            ActivitySpinner.IsVisible = false;

            // Refrescamos el listado de imágenes
            RefreshList();
        }

        private void RefreshList()
        {
            try
            {
                // Consultamos las imagenes asociadas a la guía correspondiente
                listImages = App.ImageGuiaDatabase.GetImagesByIdGuia(guiaSelected.Id);

                // Creamos una variable de contador
                int contador = 1;

                // Recorremos el listado de imágenes
                foreach (var image in listImages)
                {
                    switch (contador)
                    {
                        case 1:
                            image_guia_1.Source = ImageSource.FromFile(image.Path);
                            Fecha_1.Text = image.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        case 2:
                            image_guia_2.Source = ImageSource.FromFile(image.Path);
                            Fecha_2.Text = image.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        default:
                            image_guia_3.Source = ImageSource.FromFile(image.Path);
                            Fecha_3.Text = image.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                    };
                    contador++;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }
        }

        public async void CapturePhotoAsync(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No hay cámara", "La cámara no se encuentra disponible", "OK");
                    return;
                }

                var lastImage = App.ImageGuiaDatabase.GetLastImageGuia();

                int id = (lastImage != null) ? (lastImage.Id + 1) : 1;

                string fileName = guiaSelected.GuiaNumero + "-" + id;

                var _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    Directory = "ImagesGuias",
                    Name = fileName + ".jpg"
                });

                if (_mediaFile == null)
                    return;

                // Mapeamos la imagen, para así poderla
                // asociar a su guía correspondiente
                ImageGuia imageGuia = new ImageGuia
                {
                    Path = _mediaFile.AlbumPath,
                    Comment = "",
                    RegisterDate = DateTime.Now,
                    Id_guia = guiaSelected.Id
                };

                // Guardamos el registro de la imagen en la BD
                App.ImageGuiaDatabase.SaveImageGuia(imageGuia);

                // Por último refrescamos el listado de imágenes
                RefreshList();
            }
            catch (Exception error)
            {
                Debug.WriteLine("Error: " + error.Message);
            }
        }
    }
}