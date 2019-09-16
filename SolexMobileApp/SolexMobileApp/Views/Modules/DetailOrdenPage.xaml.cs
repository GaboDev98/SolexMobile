using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Plugin.Connectivity;
using SolexMobileApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SolexMobileApp.Views.Modules
{
    // Le decimos que compile desde la vista
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailOrdenCarguePage : ContentPage
    {
        // orden de cargue seleccionada
        private readonly OrdenCargue ordenSelected = new OrdenCargue();
        // Lista de estado de la orden de cargue
        private readonly List<Estado> listaEstadosOrdenes = new List<Estado>();
        // Estado seleccionado para la actual orden de cargue
        private readonly Estado selectedEstadoOrden = new Estado();

        public DetailOrdenCarguePage(OrdenCargue orden)
        {
            InitializeComponent();
            // Título de la página
            Title = "Detalle de la Orden de Cargue";
            // Capturamos la orden de cargue seleccionada
            ordenSelected = orden;
            // Modificamos los labels y entries de la UI 
            // con los valores de la orden de cargue
            Lbl_Cliente.Text = "Dest: " + orden.Destinatario;
            Lbl_DireccionDestino.Text = "Dir: " + orden.DireccionDestino;
            Lbl_Ciudad.Text = "Ciudad: " + orden.Ciudad;
            Lbl_Orden.Text = "Orden: " + orden.OrdenId.ToString();
            Lbl_Planilla.Text = "Planilla: " + orden.PlanillaNumero.ToString();
            Lbl_Unidades.Text = "Unidades: " + orden.UnidadesProgramadas;
            Entry_Receives.Text = orden.Receives;
            Entry_DocsRecogidos.Text = (orden.DocumentosRecogidos > 0) ? orden.DocumentosRecogidos.ToString() : "";
            Entry_UndsRecogidas.Text = (orden.UnidadesRecogidas > 0) ? orden.UnidadesRecogidas.ToString() : "";
            // Capturamos la hora actual
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // Se verifica que la orden de cargue no tenga fecha de llegada al punto
            if (orden.ArrivalDate == null || string.IsNullOrWhiteSpace(orden.ArrivalDate))
                Entry_ArrivalToThePoint.Text = dateTime;
            else
                Entry_ArrivalToThePoint.Text = orden.ArrivalDate;
            // Si la orden tiene un estado definitivo ocultamos los botones
            if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_EXITOSA
                || ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_NO_EXITOSA)
            {
                Btn_ArrivalToThePoint.IsVisible = false;
                Btn_SaveClearDetails.IsVisible = false;
                Btn_SaveDoneDetails.IsVisible = false;
            }
            // Capturamos los eventos de las entradas
            Entry_Receives.Completed += (s, e) =>
            {
                Entry_DocsRecogidos.Focus();
            };
            // Capturamos los eventos de las entradas
            Entry_DocsRecogidos.Completed += (s, e) =>
            {
                Entry_UndsRecogidas.Focus();
            };
            // Evento de llegada al punto
            Btn_ArrivalToThePoint.Clicked += async (s, e) =>
            {
                await Btn_ArrivalToThePointAsync();
            };
            // Evento para marcar la orden de cargue como ORDEN NO EXITOSA
            Btn_SaveClearDetails.Clicked += async (s, e) =>
            {
                await Btn_SaveClearOrdenAsync();
            };
            // Entrada completa de la identificación 
            Btn_SaveDoneDetails.Clicked += async (s, e) =>
            {
                // Evento para marcar la orden de cargue como ORDEN EXITOSA
                await Btn_SaveDoneOrdenAsync();
            };
            // Verificamos que haya conexión a internet
            CheckConnectivity();
            // Inicializamos algunos componentes
            InitComponents();
        }

        // Método personalizado que inicializa 
        // los componentes de la página
        void InitComponents() {
            // Seteamos la propiedad de color de los botones
            Btn_ArrivalToThePoint.BackgroundColor = Color.FromHex("00549F");
            Btn_SaveDoneDetails.BackgroundColor = Color.FromHex("00549F");
            Btn_SaveClearDetails.BackgroundColor = Color.FromHex("FF5252");
            // Verificamos si la orden de cargue ya registra llegada al punto
            if (ordenSelected.Arrival || ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_LLEGO_PUNTO)
            {
                // Actualizamos algunas propiedades de la UI
                Btn_ArrivalToThePoint.BackgroundColor = Color.FromHex("8BC34A");
            }
            // Verificamos si la orden de cargue ya registra ORDEN EXITOSA
            if (ordenSelected.Delivered || ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_EXITOSA)
            {
                // Actualizamos algunas propiedades de la UI
                Btn_SaveDoneDetails.BackgroundColor = Color.FromHex("8BC34A");
            }
            // Definimos una ruta dinámica para las imágenes de los botones dependiendo del sistema operativo
            Btn_ArrivalToThePoint.Image = Device.RuntimePlatform == Device.Android ? ("ic_update_white_24dp.png") : ("ic_update_white.png");
            Btn_SaveDoneDetails.Image = Device.RuntimePlatform == Device.Android ? ("ic_done_white_24dp.png") : ("ic_done_white_24dp.png");
            Btn_SaveClearDetails.Image = Device.RuntimePlatform == Device.Android ? ("ic_clear_white_24dp.png") : ("ic_clear_white_24dp.png");
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
            if (ordenSelected.UltimoEstadoId != Constants.ESTADO_ORDEN_EXITOSA
                && ordenSelected.UltimoEstadoId != Constants.ESTADO_ORDEN_NO_EXITOSA)
            {
                await Btn_ArrivalToThePointAsync();
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
            if (ordenSelected.ArrivalDate != null && ordenSelected.Arrival)
                await DisplayAlert("Mensaje de confirmación", "Esta orden de cargue ya regista la hora y fecha de llegada al punto!", "ACEPTAR");
            else
            {
                // Confirmamos que el usuario esté seguro de que va a marcar la hora de llegada al punto
                var action = await DisplayAlert("Mensaje de confirmación", "¿Desea registrar la fecha y hora de llegada al punto?.", "ACEPTAR", "CANCELAR");
                if (action)
                {
                    // Asignamos el estado de llegada al punto
                    selectedEstadoOrden.IdSolex = Constants.ESTADO_ORDEN_LLEGO_PUNTO;
                    // Enviamos el estado a Solex
                    await ChangeStateOrdenAsync();
                }
                else
                {
                    // Método que nos retorna al listado de táreas
                    MainListOpen();
                }
            }
        }

        // Método que se utiliza para registrar la orden de cargue como no existosa
        async Task Btn_SaveDoneOrdenAsync()
        {
            // Se verifica que la orden de cargue existosa no haya sido
            // marcada con anterioridad
            if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_EXITOSA)
                await DisplayAlert("Mensaje de confirmación", "Esta orden de cargue ya regista el estado de orden exitosa!", "ACEPTAR");
            else
            {
                // Confirmamos que el usuario esté seguro de que va a marcar la hora de llegada al punto
                var action = await DisplayAlert("Mensaje de confirmación", "¿Desea registrar esta orden de cargue como exitosa?.", "ACEPTAR", "CANCELAR");
                if (action)
                {
                    // Asignamos el estado de orden de cargue existosa
                    selectedEstadoOrden.IdSolex = Constants.ESTADO_ORDEN_EXITOSA;
                    // Enviamos el estado a Solex
                    await ChangeStateOrdenAsync();
                }
            }
        }

        // Método que se utiliza para registrar la orden de cargue como no existosa
        async Task Btn_SaveClearOrdenAsync()
        {
            // Se verifica que la orden de cargue existosa no haya sido
            // marcada con anterioridad
            if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_NO_EXITOSA)
                await DisplayAlert("Mensaje de confirmación", "Esta orden de cargue ya regista el estado de no exitosa!", "ACEPTAR");
            else
            {
                // Confirmamos que el usuario esté seguro de que va a marcar la hora de llegada al punto
                var action = await DisplayAlert("Mensaje de confirmación", "¿Desea registrar esta orden de cargue como no exitosa?.", "ACEPTAR", "CANCELAR");
                if (action)
                {
                    // Asignamos el estado de orden de cargue No existosa
                    selectedEstadoOrden.IdSolex = Constants.ESTADO_ORDEN_NO_EXITOSA;
                    // Enviamos el estado a Solex
                    await ChangeStateOrdenAsync();
                }
            }
        }

        // Método que válida los campos necesarios
        private bool CheckInputs()
        {
            if (selectedEstadoOrden.IdSolex != 0)
            {
                if (selectedEstadoOrden.IdSolex == Constants.ESTADO_ORDEN_EXITOSA)
                {
                    if (string.IsNullOrWhiteSpace(Entry_Receives.Text))
                    {
                        DisplayAlert("Validación de campos", "Por favor digita el nombre de la persona que entrega!", "ACEPTAR");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(Entry_DocsRecogidos.Text))
                    {
                        DisplayAlert("Validación de campos", "Por favor digita la cantidad de documentos recogidos!", "ACEPTAR");
                        return false;
                    }
                    if (!Utils.Utils.IsNumeric(Entry_DocsRecogidos.Text))
                    {
                        DisplayAlert("Validación de campos", "Por favor digita un número válido en el campo de documentos recogidos!", "ACEPTAR");
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

        private async Task ChangeStateOrdenAsync(bool direct = true)
        {
            if (CheckNetworkConnection())
            {
                // Preguntanmos al usuario si desea enviar la información a Solex
                if (!direct)
                {
                    direct = await DisplayAlert("Mensaje de Confirmación!.", "¿Desea enviar el estado de la orden de cargue a Solex?", "ACEPTAR", "CANCELAR");
                }
                if (direct)
                {
                    // Creamos un hilo de ejecución
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // Se captura el input de la cantidad de documentos recogidos
                        var documentos_recogidos = (!string.IsNullOrWhiteSpace(Entry_DocsRecogidos.Text)) ? (Convert.ToDecimal(Entry_DocsRecogidos.Text)) : 0;
                        // Se captura el input de la cantidad de unidades recogidas
                        var unidades_recogidas = (!string.IsNullOrWhiteSpace(Entry_UndsRecogidas.Text)) ? (Convert.ToDecimal(Entry_UndsRecogidas.Text)) : 0;
                        // Se verifica si el usuario digitó la cantidad de documentos recogidos
                        ordenSelected.DocumentosRecogidos = (documentos_recogidos > 0) ? documentos_recogidos : ordenSelected.DocumentosRecogidos;
                        // Se verifica si el usuario digitó la cantidad de unidades recogidas
                        ordenSelected.UnidadesRecogidas = (unidades_recogidas > 0) ? unidades_recogidas : ordenSelected.UnidadesRecogidas;
                        // Fecha de registro del estado
                        string fecha_registro_estado = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        // Mostramos el spinner de carga
                        ActivitySpinner.IsVisible = true;
                        // Se mapean los campos necesarios para el objeto estado
                        RequestEstadoOrden request_estado = new RequestEstadoOrden
                        {
                            OrdenId = ordenSelected.OrdenId,
                            EstadoId = selectedEstadoOrden.IdSolex,
                            Fecha = fecha_registro_estado,
                            Descripcion = "Estado generado desde SolexMobile.",
                            OperadorUsuarioId = Constants.CurrentUser.IdSolex,
                            DocumentosRecogidos = ordenSelected.DocumentosRecogidos,
                            UnidadesRecogidas = ordenSelected.UnidadesRecogidas
                        };
                        // Obtenemos la respuesta después de consumir el servicio
                        var response_estado = await App.RestClient.SaveEstadoOrden<ResponseSave>(request_estado);
                        // Verificamos el resultado de la petición
                        if (response_estado != null)
                        {
                            if (response_estado.CodError < 0)
                            {
                                // Marcamos la orden de cargue como que no guardó la llegada al punto
                                ordenSelected.Arrival = false;
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
                                    // Actualiamos el último estado de la orden de cargue
                                    ordenSelected.UltimoEstadoId = selectedEstadoOrden.IdSolex;
                                    // S el estado es LLEGÓ AL PUNTO
                                    if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_LLEGO_PUNTO)
                                    {
                                        // Asignamos la fecha de registro
                                        ordenSelected.ArrivalDate = fecha_registro_estado;
                                        // Marcamos la orden de cargue como que ya llegó al punto
                                        ordenSelected.Arrival = true;
                                        // Cambiamos los estilos del botón de llegada al punto
                                        Btn_ArrivalToThePoint.BackgroundColor = Color.FromHex(Constants.CODIGO_COLOR_LLEGADA_PUNTO.Replace("#", ""));
                                        // Actualizamos el color de la fila
                                        ordenSelected.ColorButtonRow = Constants.CODIGO_COLOR_LLEGADA_PUNTO;
                                        // Ocultamos el spinner de carga
                                        ActivitySpinner.IsVisible = false;
                                        // Mensaje informativo
                                        await DisplayAlert("Mensaje Informativo!", "La llegada al punto fue registrada correctamente.", "ACEPTAR");
                                    }
                                    // Si el estado es ORDEN EXITOSA
                                    else if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_EXITOSA)
                                    {
                                        // Asignamos la fecha de registro
                                        ordenSelected.DeliveredDate = fecha_registro_estado;
                                        // Marcamos la orden de cargue como que ya fue exitosa
                                        ordenSelected.Delivered = true;
                                        // Marcamos la orden de cargue como controlada
                                        ordenSelected.Controlled = true;
                                        // Cambiamos los estilos del botón de orden exitosa
                                        Btn_SaveDoneDetails.BackgroundColor = Color.FromHex(Constants.CODIGO_COLOR_ENTREGA_BLU.Replace("#", ""));
                                        // Actualizamos el color de la fila
                                        ordenSelected.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_BLU;
                                    }
                                    // Si el estado es ORDEN NO EXITOSA
                                    else if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_NO_EXITOSA)
                                    {
                                        // Asignamos la fecha de registro
                                        ordenSelected.DeliveredDate = fecha_registro_estado;
                                        // Marcamos la orden de cargue como que ya fue exitosa
                                        ordenSelected.Delivered = true;
                                        // Marcamos la orden de cargue como controlada
                                        ordenSelected.Controlled = true;
                                        // Cambiamos los estilos del botón de orden no exitosa
                                        Btn_SaveDoneDetails.BackgroundColor = Color.FromHex(Constants.CODIGO_COLOR_ORDEN_NO_EXITOSA.Replace("#", ""));
                                        // Actualizamos el color de la fila
                                        ordenSelected.ColorButtonRow = Constants.CODIGO_COLOR_ORDEN_NO_EXITOSA;
                                    }
                                }
                            }
                            // Llamamos al método que guarda los cambios de la orden de cargue en BD
                            App.OrdenCargueDatabase.SaveOrdenCargue(ordenSelected);
                            // Ocultamos el spinner de carga
                            ActivitySpinner.IsVisible = false;
                            // Verificamos que el estado sea diferente
                            // a la llegada al punto
                            if (ordenSelected.UltimoEstadoId != Constants.ESTADO_ORDEN_LLEGO_PUNTO)
                                // Nos devolvemos al listado de ordenes de cargue
                                MainListOpen();
                        }
                        else
                        {
                            // Ocultamos el spinner de carga
                            ActivitySpinner.IsVisible = false;
                            // Mensaje informativo
                            await DisplayAlert("Ocurrió un error!", "No se lograron enviar los datos al servidor.", "ACEPTAR");
                        }
                        // Ocultamos el spinner de carga
                        ActivitySpinner.IsVisible = false;
                    });
                }
            }
        }

        // Método general de esta clase utilizado para guardar
        // en base de datos todos los cambios que se hayan hecho en la orden de cargue
        private async Task SaveChangesAsync(bool direct = true)
        {
            // Verificamos los campos necesarios
            if (CheckInputs())
            {
                // Actualizamos el valor de algunos campos
                // en base a los entry de la página
                ordenSelected.Receives = Entry_Receives.Text;
                ordenSelected.DocumentosRecogidos = Convert.ToDecimal(Entry_DocsRecogidos.Text);
                ordenSelected.Updated_at = DateTime.Now;
                ordenSelected.UnidadesRecogidas = Convert.ToDecimal(Entry_UndsRecogidas.Text);
                // En dado caso de que haya conexión a internet
                if (CheckNetworkConnection())
                {
                    // Preguntanmos al usuario si desea enviar la información a Solex
                    if (!direct)
                    {
                        direct = await DisplayAlert("Mensaje de Confirmación!.", "¿Desea enviar la información de la orden de cargue a Solex?", "ACEPTAR", "CANCELAR");
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
                            RequestEstadoOrden request_estado = new RequestEstadoOrden
                            {
                                OrdenId = ordenSelected.Id,
                                EstadoId = selectedEstadoOrden.IdSolex,
                                Fecha = fecha_registro_estado,
                                Descripcion = "Estado generado desde SolexMobile.", 
                                OperadorUsuarioId = Constants.CurrentUser.IdSolex
                            };
                            // Se crea el objeto de envío a Solex
                            RequestDatosOrden request_orden = new RequestDatosOrden
                            {
                                EntregaNombre = ordenSelected.Receives,
                                DocumentosRecogidos = ordenSelected.DocumentosRecogidos,
                                UsuarioId = Constants.CurrentUser.IdSolex,
                                UsuarioLogin = Constants.CurrentUser.Username,
                                UltimoEstado = request_estado,
                                UnidadesRecogidas = ordenSelected.UnidadesRecogidas
                            };
                            // Obtenemos la respuesta después de consumir el servicio
                            var response_orden = await App.RestClient.SaveDetailOrdenCargue<ResponseSave>(request_orden);
                            // Verificamos el resultado de la petición
                            if (response_orden != null)
                            {
                                if (!response_orden.IsSuccess)
                                    await DisplayAlert("Mensaje Informativo!", response_orden.Error, "ACEPTAR");
                                else
                                {
                                    // Actualiamos el último estado de la orden de cargue
                                    ordenSelected.UltimoEstadoId = selectedEstadoOrden.IdSolex;
                                    // S el estado es LLEGÓ AL PUNTO
                                    if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_LLEGO_PUNTO)
                                    {
                                        // Asignamos la fecha de registro
                                        ordenSelected.ArrivalDate = fecha_registro_estado;
                                        // Marcamos la orden de cargue como que ya llegó al punto
                                        ordenSelected.Arrival = true;
                                        // Cambiamos los estilos del botón de llegada al punto
                                        Btn_ArrivalToThePoint.BackgroundColor = Color.FromHex(Constants.CODIGO_COLOR_LLEGADA_PUNTO.Replace("#", ""));
                                        // Actualizamos el color de la fila
                                        ordenSelected.ColorButtonRow = Constants.CODIGO_COLOR_LLEGADA_PUNTO;
                                        // Ocultamos el spinner de carga
                                        ActivitySpinner.IsVisible = false;
                                        // Mensaje informativo
                                        await DisplayAlert("Mensaje Informativo!", "La llegada al punto fue registrada correctamente.", "ACEPTAR");
                                    }
                                    // Si el estado es ORDEN EXITOSA
                                    else if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_EXITOSA)
                                    {
                                        // Asignamos la fecha de registro
                                        ordenSelected.DeliveredDate = fecha_registro_estado;
                                        // Marcamos la orden de cargue como que ya fue exitosa
                                        ordenSelected.Delivered = true;
                                        // Marcamos la orden de cargue como controlada
                                        ordenSelected.Controlled = true;
                                        // Cambiamos los estilos del botón de orden exitosa
                                        Btn_SaveDoneDetails.BackgroundColor = Color.FromHex(Constants.CODIGO_COLOR_ORDEN_EXITOSA.Replace("#", ""));
                                        // Actualizamos el color de la fila
                                        ordenSelected.ColorButtonRow = Constants.CODIGO_COLOR_ORDEN_EXITOSA;
                                    }
                                    // Si el estado es ORDEN NO EXITOSA
                                    else if (ordenSelected.UltimoEstadoId == Constants.ESTADO_ORDEN_NO_EXITOSA)
                                    {
                                        // Asignamos la fecha de registro
                                        ordenSelected.DeliveredDate = fecha_registro_estado;
                                        // Marcamos la orden de cargue como que ya fue exitosa
                                        ordenSelected.Delivered = true;
                                        // Marcamos la orden de cargue como controlada
                                        ordenSelected.Controlled = true;
                                        // Cambiamos los estilos del botón de orden no exitosa
                                        Btn_SaveDoneDetails.BackgroundColor = Color.FromHex(Constants.CODIGO_COLOR_ORDEN_NO_EXITOSA.Replace("#", ""));
                                        // Actualizamos el color de la fila
                                        ordenSelected.ColorButtonRow = Constants.CODIGO_COLOR_ORDEN_NO_EXITOSA;
                                    }
                                }
                                // Llamamos al método que guarda los cambios de la orden de cargue en BD
                                App.OrdenCargueDatabase.SaveOrdenCargue(ordenSelected);
                                // Ocultamos el spinner de carga
                                ActivitySpinner.IsVisible = false;
                                // Verificamos que el estado sea diferente
                                // a la llegada al punto
                                if (ordenSelected.UltimoEstadoId != Constants.ESTADO_ORDEN_LLEGO_PUNTO)
                                    // Nos devolvemos al listado de ordenes de cargue
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
        // que se hayan realizado en la orden de cargue
        async Task SaveClickedAsync(object sender, EventArgs e)
        {
            // Verificamos que existan cambios
            if(VerifiedIfExistChanges())
            {
                // Se confirma que el usuario esté completamente seguro de guardar los cambios realizados
                var action = await DisplayAlert("Mensaje de confirmación", "¿Realmente desea guardar los cambios realizados?.", "ACEPTAR", "CANCELAR");
                if (action)
                    // Llamamos al método que va a la base de datos local
                    // y registra los cambios realizados
                    await SaveChangesAsync();
            }
            else
                await DisplayAlert("Mensaje de confirmación", "No ha realizado ningún cambio!", "ACEPTAR");
        }

        // Función para verificar que existan cambios
        // en la orden de cargue actual
        bool VerifiedIfExistChanges()
        {
            // Capturamos las entradas de la página
            ordenSelected.Receives = Entry_Receives.Text;
            ordenSelected.DocumentosRecogidos = Convert.ToDecimal(Entry_DocsRecogidos.Text);
            OrdenCargue ordenDB = new OrdenCargue();
            // Consultamos la orden de cargue almanada en BD
            ordenDB = App.OrdenCargueDatabase.GetOrdenCargueById(ordenSelected.Id);
            // Comparamos los datos de la UI con los de la BD, 
            // para así saber si el usuario realizó algún cambio
            if (ordenSelected.Receives != ordenDB.Receives
                || ordenSelected.DocumentosRecogidos != ordenDB.DocumentosRecogidos
                || ordenSelected.DeliveredDate != ordenDB.DeliveredDate)
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
    }
}