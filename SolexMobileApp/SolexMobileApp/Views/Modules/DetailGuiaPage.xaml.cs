using System;
using System.Linq;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Plugin.Connectivity;
using SolexMobileApp.Models;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using System.Collections.Generic;

namespace SolexMobileApp.Views.Modules
{
    // Le decimos que compile desde la vista
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailGuiaPage : ContentPage
    {
        // Guía seleccionada
        private readonly Guia guiaSelected = new Guia();
        // Lista de estado de la guía
        private List<Estado> listaEstadosGuia = new List<Estado>();
        // Estado seleccionado para la actual guía
        private Estado selectedEstadoGuia = new Estado();
        // Indice del estado seleccionado
        private int selectedIndexEstado = -1;
        // Variable de carga de la página
        private bool loadingPage = true;

        public DetailGuiaPage(Guia guia)
        {
            InitializeComponent();
            // Capturamos la guía seleccionada
            guiaSelected = guia;
            // Modificamos los labels y entries de la UI 
            // con los valores de la guía
            Lbl_Cliente.Text = "Dest: " + guia.Destinatario;
            Lbl_DireccionDestino.Text = "Dir: " + guia.DireccionDestino;
            Lbl_Ciudad.Text = "Ciudad: " + guia.Ciudad;
            Lbl_Guia.Text = "Guía: " + guia.GuiaNumero;
            Lbl_Planilla.Text = "Planilla: " + guia.PlanillaNumero.ToString();
            Lbl_Unidades.Text = "Unidades: " + guia.Unidades;
            Entry_Receives.Text = guia.Receives;
            Entry_Identification.Text = guia.Receives_Doc;
            // Capturamos la hora actual
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // Se verifica que la guía no tenga fecha de llegada al punto
            if (guia.ArrivalDate == null || string.IsNullOrWhiteSpace(guia.ArrivalDate))
                Entry_ArrivalToThePoint.Text = dateTime;
            else
                Entry_ArrivalToThePoint.Text = guia.ArrivalDate;
            // Si la guía tiene un estado definitivo ocultamos los botones
            if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_BLU 
                || guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_PARCIAL
                || guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_NO_ENTREGADA)
            {
                Btn_AttachImage.IsVisible = false;
                Btn_SavePartialDelivered.IsVisible = false;
                Btn_SaveClearDelivered.IsVisible = false;
                Btn_SaveDoneDelivered.IsVisible = false;
            }
            // Cargamos el listado de estados
            GetEstadosLocal();
            // Verificamos que la guía ya contenga un estado
            if (guiaSelected.UltimoEstadoId != 0)
            {
                selectedIndexEstado++;
                foreach (var item in listaEstadosGuia)
                {
                    if (item.IdSolex == guiaSelected.UltimoEstadoId)
                        ListEstadosPicker.SelectedIndex = selectedIndexEstado;
                    selectedIndexEstado++;
                }
            }
            // Evento del botón del toolbar
            BtnDetailGeo.Clicked += async (s, e) =>
            {
                await GeoGuiaClickedAsync(s, e);
            };
            // Capturamos los eventos de las entradas
            Entry_Receives.Completed += (s, e) =>
            {
                Entry_Identification.Focus();
            };
            // Evento para adjuntar imágen
            Btn_AttachImage.Clicked += async (s, e) =>
            {
                await TakePhotoButton_OnClickedAsync(s, e);
            };
            // Evento para marcar la guía como entregada
            Btn_SaveDoneDelivered.Clicked += async (s, e) =>
            {
                await Btn_SaveDoneGuiaAsync();
            };
            // Evento para marcar la guía como No Entrega Parcial
            Btn_SavePartialDelivered.Clicked += async (s, e) =>
            {
                await Btn_SavePartialGuiaAsync();
            };
            // Evento para marcar la guía como No Entregada
            Btn_SaveClearDelivered.Clicked += async (s, e) =>
            {
                await Btn_SaveClearGuiaAsync();
            };
            // Verificamos que haya conexión a internet
            CheckConnectivity();
            // Inicializamos algunos componentes
            InitComponents();
        }

        // Método personalizado que inicializa 
        // los componentes de la página
        void InitComponents()
        {
            // Seteamos la propiedad de color de los botones
            // Btn_ArrivalToThePoint.BackgroundColor = Color.FromHex("00549F");
            Btn_AttachImage.BackgroundColor = Color.FromHex("00549F");
            Btn_SaveClearDelivered.BackgroundColor = Color.FromHex("FF5252");
            Btn_SaveDoneDelivered.BackgroundColor = Color.FromHex("00549F");
            // Verificamos si la guía ya registra entrega blu
            if (guiaSelected.Delivered || guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_BLU)
            {
                // Actualizamos algunas propiedades de la UI
                Btn_SaveDoneDelivered.BackgroundColor = Color.FromHex("8BC34A");
            }
            // Definimos una ruta dinámica para las imágenes de los botones dependiendo del sistema operativo
            // Btn_ArrivalToThePoint.Image = Device.RuntimePlatform == Device.Android ? ("ic_update_white_24dp.png") : ("ic_update_white.png");
            Btn_AttachImage.Image = Device.RuntimePlatform == Device.Android ? ("ic_camera_enhance_white_24dp.png") : ("ic_camera_enhance_white.png");
            Btn_SavePartialDelivered.Image = Device.RuntimePlatform == Device.Android ? ("ic_partial_white_24dp.png") : ("ic_partial_white_24dp.png");
            Btn_SaveClearDelivered.Image = Device.RuntimePlatform == Device.Android ? ("ic_clear_white_24dp.png") : ("ic_clear_white_24dp.png");
            Btn_SaveDoneDelivered.Image = Device.RuntimePlatform == Device.Android ? ("ic_done_white_24dp.png") : ("ic_done_white_24dp.png");
        }

        // Método para verificar la conexión a internet
        private void CheckConnectivity()
        {
            // Validamos que haya conexión a internet
            Constants.hasInternet = CrossConnectivity.Current.IsConnected;

            if (!Constants.hasInternet)
                Labl_Connectivity.Text = Constants.NoInternetText;
            else
                Labl_Connectivity.IsVisible = false;
        }

        // Evento que se ejecuta al cargar la página
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Preguntamos por la llegada al punto 
            // en dado caso de que no la registre
            if (string.IsNullOrWhiteSpace(guiaSelected.FechaLlegoAlPunto))
                await Btn_ArrivalToThePointAsync();
            // Actualizamos la variable de carga de la página
            loadingPage = false;
        }

        public void GetEstadosLocal()
        {
            // Obtenemos las guías de la BD local
            listaEstadosGuia = App.EstadoGuiaDatabase.GetAllEstados();
            // Agregamos la lista al ListView
            ListEstadosPicker.ItemsSource = listaEstadosGuia;
        }

        async Task SelectedEstadoChangedAsync(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            selectedIndexEstado = picker.SelectedIndex;
            // Verificamos que haya habido un cambio en el combo
            if (selectedIndexEstado != -1)
            {
                selectedEstadoGuia = (Estado)picker.ItemsSource[selectedIndexEstado];
            }
            // Verificamos que haya cargado la página
            if (!loadingPage)
            {
                // Verificamos si el estado seleccionado amerita tomar una foto
                if (selectedEstadoGuia.IdSolex == Constants.ESTADO_GUIA_ENTREGA_BLU)
                    // Abrimos la cámara del dispositivo
                    await OpenCamera(sender, e);
                else
                    // Cambiamos el estado de la guía en Solex
                    await ChangeStateGuiaAsync(false);
            }
        }

        // Método para válidara que exista o no conexión a internet
        private bool CheckNetworkConnection()
        {
            // Validamos que haya conexión a internet
            Constants.hasInternet = CrossConnectivity.Current.IsConnected;

            // Si hay conexión actualizamos la data
            // desde el servicio de Solex
            if (Constants.hasInternet)
                return true;
            else
                return false;
        }

        // Método que se utiliza para registrar la hora de llegada al punto
        async Task Btn_ArrivalToThePointAsync()
        {
            // Se verifica que la llegada al punto no haya sido
            // marcada con anterioridad
            if (guiaSelected.ArrivalDate != null && guiaSelected.Arrival)
            {
                // Marcamos la guía como que ya llegó al punto
                guiaSelected.Delivered = true;
                // Marcamos la guía como controlada
                guiaSelected.Controlled = false;
                // Actualizamos el color de la fila
                guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_LLEGADA_PUNTO;
                // Mensaje de alerta
                await DisplayAlert("Mensaje de confirmación", "Esta guía ya regista la hora y fecha de llegada al punto!", "ACEPTAR");
            }
            else
            {
                // Confirmamos que el usuario esté seguro de que va a marcar la hora de llegada al punto
                var action = await DisplayAlert("Mensaje de confirmación", "¿Desea registrar la fecha y hora de llegada al punto?.", "ACEPTAR", "CANCELAR");
                if (action)
                {
                    // Asignamos el estado de llegada al punto
                    selectedEstadoGuia.IdSolex = Constants.ESTADO_GUIA_LLEGO_PUNTO;
                    // Enviamos el estado a Solex
                    await ChangeStateGuiaAsync();
                }
                else
                {
                    // Método que nos retorna al listado de táreas
                    MainListOpen();
                }
            }
        }

        // Método que se utiliza para registrar la guía como no entregada
        async Task Btn_SaveDoneGuiaAsync()
        {
            // Se verifica que la guía no haya sido
            // marcada con anterioridad
            if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_BLU)
            {
                // Marcamos la guía como que ya llegó al punto
                guiaSelected.Delivered = true;
                // Marcamos la guía como controlada
                guiaSelected.Controlled = true;
                // Actualizamos el color de la fila
                guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_BLU;
                // Mensaje de alerta
                await DisplayAlert("Mensaje de confirmación", "Esta guía ya regista el estado entregada!", "ACEPTAR");
            }
            else
            {
                // Confirmamos que el usuario esté seguro de que va a marcar la hora de llegada al punto
                var action = await DisplayAlert("Mensaje de confirmación", "¿Desea registrar esta guía como entregada?.", "ACEPTAR", "CANCELAR");
                if (action)
                {
                    // Asignamos el estado de guía entregada
                    selectedEstadoGuia.IdSolex = 1;
                    // Enviamos el estado a Solex
                    await ChangeStateGuiaAsync();
                }
            }
        }

        // Método que se utiliza para registrar la guía como no entregada
        async Task Btn_SavePartialGuiaAsync()
        {
            // Se verifica que la guía no haya sido
            // marcada con anterioridad
            if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_PARCIAL)
            {
                // Marcamos la guía como que ya se marco como entrega parcial
                guiaSelected.Delivered = true;
                // Marcamos la guía como controlada
                guiaSelected.Controlled = true;
                // Actualizamos el color de la fila
                guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_PARCIAL;
                // Mensaje de alerta
                await DisplayAlert("Mensaje de confirmación", "Esta guía ya regista el estado de entrega parcial!", "ACEPTAR");
            }
            else
            {
                // Confirmamos que el usuario esté seguro de que va a marcar la hora de llegada al punto
                var action = await DisplayAlert("Mensaje de confirmación", "¿Desea registrar esta guía como entrega parcial?.", "ACEPTAR", "CANCELAR");
                if (action)
                {
                    // Asignamos el estado de guía No entregada
                    selectedEstadoGuia.IdSolex = Constants.ESTADO_GUIA_ENTREGA_PARCIAL;
                    // Enviamos el estado a Solex
                    await ChangeStateGuiaAsync();
                }
            }
        }

        // Método que se utiliza para registrar la guía como no entregada
        async Task Btn_SaveClearGuiaAsync()
        {
            // Se verifica que la guía no haya sido
            // marcada con anterioridad
            if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_NO_ENTREGADA)
            {
                // Marcamos la guía como que ya se marco como entrega parcial
                guiaSelected.Delivered = false;
                // Marcamos la guía como controlada
                guiaSelected.Controlled = true;
                // Actualizamos el color de la fila
                guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_PARCIAL;
                // Mensaje de alerta
                await DisplayAlert("Mensaje de confirmación", "Esta guía ya regista el estado de no entregada!", "ACEPTAR");
            }
            else
            {
                // Confirmamos que el usuario esté seguro de que va a marcar la hora de llegada al punto
                var action = await DisplayAlert("Mensaje de confirmación", "¿Desea registrar esta guía como no entregada?.", "ACEPTAR", "CANCELAR");
                if (action)
                {
                    // Asignamos el estado de guía No entregada
                    selectedEstadoGuia.IdSolex = Constants.ESTADO_GUIA_NO_ENTREGADA;
                    // Enviamos el estado a Solex
                    await ChangeStateGuiaAsync();
                }
            }
        }

        // Método que válida los campos necesarios
        // para marcar la guía como entregada
        private bool CheckInputs()
        {
            if (selectedEstadoGuia.IdSolex != 0)
            {
                if (selectedEstadoGuia.IdSolex == Constants.ESTADO_GUIA_ENTREGA_BLU)
                {
                    if (string.IsNullOrWhiteSpace(Entry_Receives.Text))
                    {
                        DisplayAlert("Validación de campos", "Por favor digita el nombre de la persona que recibe!", "ACEPTAR");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(Entry_Identification.Text))
                    {
                        DisplayAlert("Validación de campos", "Por favor digita el número de documento de quien recibe!", "ACEPTAR");
                        return false;
                    }
                    if (!Utils.Utils.IsNumeric(Entry_Identification.Text))
                    {
                        DisplayAlert("Validación de campos", "Por favor digita un número en el campo de identificación!", "ACEPTAR");
                        return false;
                    }
                }
            }
            else
            {
                DisplayAlert("Validación de campos", "Por favor seleccione un estado!", "ACEPTAR");
                return false;
            }
            return true;
        }

        private async Task ChangeStateGuiaAsync(bool direct = true)
        {
            if (CheckNetworkConnection())
            {
                // Preguntanmos al usuario si desea enviar la información a Solex
                if (!direct)
                {
                    direct = await DisplayAlert("Mensaje de Confirmación!.", "¿Desea enviar el estado de la guía a Solex?", "ACEPTAR", "CANCELAR");
                }
                if (direct)
                {
                    // Creamos un hilo de ejecución
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // Fecha de registro de l estado
                        string fecha_registro_estado = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        // Mostramos el spinner de carga
                        ActivitySpinner.IsVisible = true;
                        // Se mapean los campos necesarios para el objeto estado
                        RequestEstadoGuia request_estado = new RequestEstadoGuia
                        {
                            GuiaId = guiaSelected.IdGuia,
                            EstadoId = selectedEstadoGuia.IdSolex,
                            Fecha = fecha_registro_estado,
                            Descripcion = "Estado generado desde SolexMobile.",
                            OperadorUsuarioId = Constants.CurrentUser.IdSolex
                        };
                        // Obtenemos la respuesta después de consumir el servicio
                        var response_estado = await App.RestClient.SaveEstadoGuia<ResponseSave>(request_estado);
                        // Verificamos el resultado de la petición
                        if (response_estado != null)
                        {
                            if (response_estado.CodError < 0)
                            {
                                // Marcamos la guía como que no guardó la llegada al punto
                                guiaSelected.Arrival = false;
                                // Mensaje de error
                                await DisplayAlert("Mensaje Informativo!", response_estado.Error, "ACEPTAR");
                            }
                            else
                            {
                                // Verificamos si efectivamente se creo el nuevo estado
                                if (response_estado.CodError == 0)
                                    // Mensaje de solex
                                    await DisplayAlert("Mensaje Informativo!", response_estado.Error, "ACEPTAR");
                                else
                                {
                                    // Actualiamos el último estado de la guía
                                    guiaSelected.UltimoEstadoId = selectedEstadoGuia.IdSolex;
                                    // Si el estado es LLEGÓ AL PUNTO DE ENTREGA
                                    if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_LLEGO_PUNTO)
                                    {
                                        // Asignamos la fecha de registro
                                        guiaSelected.ArrivalDate = fecha_registro_estado;
                                        // Marcamos la guía como que ya llegó al punto
                                        guiaSelected.Arrival = true;
                                        // Actualizamos el color de la fila
                                        guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_LLEGADA_PUNTO;
                                        // Ocultamos el spinner de carga
                                        ActivitySpinner.IsVisible = false;
                                        // Mensaje informativo
                                        await DisplayAlert("Mensaje Informativo!", "La llegada al punto fue registrada correctamente.", "ACEPTAR");
                                    }
                                    // Si el estado es ENTREGA BLU
                                    else if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_BLU)
                                    {
                                        // Asignamos la fecha de registro
                                        guiaSelected.DeliveredDate = fecha_registro_estado;
                                        // Marcamos la guía como que ya llegó al punto
                                        guiaSelected.Delivered = true;
                                        // Marcamos la guía como controlada
                                        guiaSelected.Controlled = true;
                                        // Actualizamos el color de la fila
                                        guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_BLU;
                                    }
                                    // Si el estado es ENTREGA PARCIAL
                                    else if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_PARCIAL)
                                    {
                                        // Asignamos la fecha de registro
                                        guiaSelected.DeliveredDate = fecha_registro_estado;
                                        // Marcamos la guía como que ya llegó al punto
                                        guiaSelected.Delivered = true;
                                        // Marcamos la guía como controlada
                                        guiaSelected.Controlled = true;
                                        // Actualizamos el color de la fila
                                        guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_PARCIAL;
                                    }
                                    // Si el estado es GUÍA NO ENTREGADA
                                    else if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_NO_ENTREGADA)
                                    {
                                        // Asignamos la fecha de registro
                                        guiaSelected.DeliveredDate = null;
                                        // Marcamos la guía como que ya llegó al punto
                                        guiaSelected.Delivered = false;
                                        // Marcamos la guía como controlada
                                        guiaSelected.Controlled = true;
                                        // Actualizamos el color de la fila
                                        guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_NO_ENTREGADA;
                                    }
                                }
                            }
                            // Llamamos al método que guarda los cambios de la guía en BD
                            App.GuiaDatabase.SaveGuia(guiaSelected);
                            // Ocultamos el spinner de carga
                            ActivitySpinner.IsVisible = false;
                            // Verificamos que el estado sea diferente
                            // a la llegada al punto
                            if (guiaSelected.UltimoEstadoId != Constants.ESTADO_GUIA_LLEGO_PUNTO)
                                // Nos devolvemos al listado de guías
                                MainListOpen();
                        }
                        else
                        {
                            // Ocultamos el spinner de carga
                            ActivitySpinner.IsVisible = false;
                            // Mostramos el mensaje de error
                            await DisplayAlert("Ocurrió un error!", "No se lograron enviar los datos al servidor.", "ACEPTAR");
                        }
                    });
                }
            }
        }

        // Método general de esta clase utilizado para guardar
        // en base de datos todos los cambios que se hayan hecho en la guía
        private async Task SaveChangesAsync(bool direct = true)
        {
            // Verificamos los campos necesarios
            if (CheckInputs())
            {
                // Actualizamos el valor de algunos campos
                // en base a los entry de la página
                guiaSelected.Receives = Entry_Receives.Text;
                guiaSelected.Receives_Doc = Entry_Identification.Text;
                guiaSelected.Updated_at = DateTime.Now;
                // En dado caso de que haya conexión a internet
                if (CheckNetworkConnection())
                {
                    // Preguntanmos al usuario si desea enviar la información a Solex
                    if (!direct)
                    {
                        direct = await DisplayAlert("Mensaje de Confirmación!.", "¿Desea enviar la información de la guía a Solex?", "ACEPTAR", "CANCELAR");
                    }
                    if (direct)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            // Fecha de registro de l estado
                            string fecha_registro_estado = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            // Mostramos el spinner de carga
                            ActivitySpinner.IsVisible = true;
                            // Se mapean los campos necesarios para el objeto estado
                            RequestEstadoGuia request_estado = new RequestEstadoGuia
                            {
                                GuiaId = guiaSelected.IdGuia,
                                EstadoId = selectedEstadoGuia.IdSolex,
                                Fecha = fecha_registro_estado,
                                Descripcion = "Estado generado desde SolexMobile.",
                                OperadorUsuarioId = Constants.CurrentUser.IdSolex
                            };
                            // Se crea el objeto de envío a Solex
                            RequestDatosGuia request_guia = new RequestDatosGuia
                            {
                                Guia = guiaSelected.GuiaNumero,
                                GuiaId = guiaSelected.IdGuia,
                                RecibeNombre = guiaSelected.Receives,
                                RecibeDocumento = guiaSelected.Receives_Doc,
                                Imagen = guiaSelected.Imagen,
                                UsuarioId = Constants.CurrentUser.IdSolex,
                                UsuarioLogin = Constants.CurrentUser.Username,
                                UltimoEstado = request_estado
                            };
                            // Obtenemos la respuesta después de consumir el servicio
                            var response_guia = await App.RestClient.SaveDetailGuia<ResponseSave>(request_guia);
                            // Verificamos el resultado de la petición
                            if (response_guia != null)
                            {
                                if (!response_guia.IsSuccess)
                                    await DisplayAlert("Mensaje Informativo!", response_guia.Error, "ACEPTAR");
                                else
                                {
                                    // Actualiamos el último estado de la guía
                                    guiaSelected.UltimoEstadoId = selectedEstadoGuia.IdSolex;
                                    // Si el estado es LLEGÓ AL PUNTO DE ENTREGA
                                    if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_LLEGO_PUNTO)
                                    {
                                        // Asignamos la fecha de registro
                                        guiaSelected.ArrivalDate = fecha_registro_estado;
                                        // Marcamos la guía como que ya llegó al punto
                                        guiaSelected.Arrival = true;
                                        // Actualizamos el color de la fila
                                        guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_LLEGADA_PUNTO;
                                        // Mensaje informativo
                                        await DisplayAlert("Mensaje Informativo!", "La llegada al punto fue registrada correctamente.", "ACEPTAR");
                                    }
                                    // Si el estado es ENTREGA BLU
                                    else if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_BLU)
                                    {
                                        // Asignamos la fecha de registro
                                        guiaSelected.DeliveredDate = fecha_registro_estado;
                                        // Marcamos la guía como que ya llegó al punto
                                        guiaSelected.Delivered = true;
                                        // Marcamos la guía como controlada
                                        guiaSelected.Controlled = true;
                                        // Actualizamos el color de la fila
                                        guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_BLU;
                                    }
                                    // Si el estado es ENTREGA PARCIAL
                                    else if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_PARCIAL)
                                    {
                                        // Asignamos la fecha de registro
                                        guiaSelected.DeliveredDate = fecha_registro_estado;
                                        // Marcamos la guía como que ya llegó al punto
                                        guiaSelected.Delivered = true;
                                        // Marcamos la guía como controlada
                                        guiaSelected.Controlled = true;
                                        // Actualizamos el color de la fila
                                        guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_NO_ENTREGADA;
                                    }
                                    // Si el estado es GUÍA NO ENTREGADA
                                    else if (guiaSelected.UltimoEstadoId == Constants.ESTADO_GUIA_NO_ENTREGADA)
                                    {
                                        // Asignamos la fecha de registro
                                        guiaSelected.DeliveredDate = null;
                                        // Marcamos la guía como que ya llegó al punto
                                        guiaSelected.Delivered = false;
                                        // Marcamos la guía como controlada
                                        guiaSelected.Controlled = true;
                                        // Actualizamos el color de la fila
                                        guiaSelected.ColorButtonRow = Constants.CODIGO_COLOR_NO_ENTREGADA;
                                    }
                                    // Abrimos el listado con los datos actualizados
                                    MainListOpen();
                                }
                                // Llamamos al método que guarda los cambios de la guía en BD
                                App.GuiaDatabase.SaveGuia(guiaSelected);
                                // Ocultamos el spinner de carga
                                ActivitySpinner.IsVisible = false;
                                // Verificamos que el estado sea diferente
                                // a la llegada al punto
                                if (guiaSelected.UltimoEstadoId != Constants.ESTADO_GUIA_LLEGO_PUNTO)
                                    // Nos devolvemos al listado de guías
                                    MainListOpen();
                            }
                            else
                            {
                                await DisplayAlert("Ocurrió un error!", "No se lograron enviar los datos al servidor.", "ACEPTAR");
                            }
                            // Ocultamos el spinner de carga
                            ActivitySpinner.IsVisible = false;
                        });
                    }
                }
            }
        }

        // Método que se activa desde la UI para guardar los cambios
        // que se hayan realizado en la guía
        async Task SaveClickedAsync(object sender, EventArgs e)
        {
            // Verificamos que existan cambios
            if(VerifiedIfExistChanges())
            {
                await SaveChangesAsync();
            }
            else
                await DisplayAlert("Mensaje de confirmación", "No ha realizado ningún cambio!", "ACEPTAR");
        }

        // Método que muestra vista de Google Maps
        async Task GeoGuiaClickedAsync(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new GeoGuiaPage(guiaSelected));
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);
            }
        }

        // Función para verificar que existan cambios
        // en la guía actual
        bool VerifiedIfExistChanges()
        {
            // Capturamos las entradas de la página
            guiaSelected.Receives = Entry_Receives.Text;
            guiaSelected.Receives_Doc = Entry_Identification.Text;
            _ = new Guia();
            // Consultamos la guía almanada en BD
            Guia guiaDB = App.GuiaDatabase.GetGuiaById(guiaSelected.Id);
            // Comparamos los datos de la UI con los de la BD, 
            // para así saber si el usuario realizó algún cambio
            if (guiaSelected.Receives != guiaDB.Receives
                || guiaSelected.Receives_Doc != guiaDB.Receives_Doc
                || guiaSelected.DeliveredDate != guiaDB.DeliveredDate)
                return true;
            else
                return false;
        }

        // Función que se activa al momento que la página
        // esta desapareciendo de la interfaz gráfica
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // Función o método que nos redirige al listado principal
        async void MainListOpen()
        {
            try
            {
                // Se verifica la plataforma para definir el tipo de transición
                if (Device.RuntimePlatform == Device.Android)
                    await Navigation.PushAsync(new TabbedListPage(false));
                else if (Device.RuntimePlatform == Device.iOS)
                    await Navigation.PushModalAsync(new TabbedListPage(false));
                // Eliminamos del histórico la página actual
                int contador = 0;
                var type_page_detail = GetType();
                var type_page_tabbed = new TabbedListPage(false).GetType();
                var existingPages = Navigation.NavigationStack.ToList();
                foreach (var page in existingPages)
                {
                    contador++;
                    if (page.GetType() == type_page_detail || page.GetType() == type_page_tabbed)
                    {
                        Navigation.RemovePage(page);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        // Método o función que se activa desde la UI al querer tomar una foto como evidencia
        private async Task TakePhotoButton_OnClickedAsync(object sender, EventArgs e)
        {
            try
            {
                // Preguntamos al usuario si realmente desea tomar una nueva foto o solo quiere ver el listado
                // de las imágenes que se han guardado con anterioridad
                if (await DisplayAlert("Listado de Imágenes", "¿Desea tomar una nueva foto?", "ACEPTAR", "VER LISTADO"))
                    // Abrimos el recurso de la cámara
                    await OpenCamera(sender, e);
                else
                    // Abrimos la vista que muestra el listado de imágenes de la guía
                    await Navigation.PushAsync(new ImageGuiaPage(guiaSelected));
            }
            catch (Exception error)
            {
                Debug.WriteLine("Error: " + error.Message);
            }
        }

        private async Task OpenCamera(object sender, EventArgs e)
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

                MediaFile _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
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

                // Convertimos la imagen en base 64
                var stream_file = _mediaFile.GetStream();
                var bytes = new byte[stream_file.Length];
                await stream_file.ReadAsync(bytes, 0, (int)stream_file.Length);
                string base64 = Convert.ToBase64String(bytes);

                // Guardamos el registro de la imagen en la BD
                App.ImageGuiaDatabase.SaveImageGuia(imageGuia);

                // Actualizamos el valor de la imagen en bd local
                guiaSelected.Imagen = base64;

                // Llamamos al método que guarda los cambios de la guía en BD
                App.GuiaDatabase.SaveGuia(guiaSelected);

                // Guardamos y posterioremente enviamos la información al servicio
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
            }
        }
    }
}