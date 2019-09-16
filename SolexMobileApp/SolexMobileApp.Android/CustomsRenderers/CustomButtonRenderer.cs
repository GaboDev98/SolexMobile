using System;
using Xamarin.Forms;
using Android.Content;
using System.ComponentModel;
using SolexMobileApp.Models;
using SolexMobileApp.Controls;
using Android.Graphics.Drawables;
using Xamarin.Forms.Platform.Android;
using SolexMobileApp.Droid.CustomsRenderers;

[assembly: ExportRenderer(typeof(XButton), typeof(CustomButtonRenderer))]
namespace SolexMobileApp.Droid.CustomsRenderers
{
    public class CustomButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer
    {
        private GradientDrawable _normal, _pressed;

        public CustomButtonRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var button = (XButton)e.NewElement;

                // Setup properties
                button.TextColor = Color.White;

                // Button items properties
                button.SizeChanged += (s, args) =>
                {
                    var radius = (float)Math.Min(button.Width, button.Height);

                    // Create a drawable for the button's normal state
                    _normal = new GradientDrawable();
                    _normal.SetColor(Android.Graphics.Color.ParseColor(Constants.CODIGO_COLOR_BOTON_AZUL));
                    _normal.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
                    _normal.SetCornerRadius(radius);

                    // Create a drawable for the button's pressed state
                    _pressed = new GradientDrawable();
                    var highlight = Context.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.ColorActivatedHighlight }).GetColor(0, Android.Graphics.Color.Gray);
                    _pressed.SetColor(highlight);
                    _pressed.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
                    _pressed.SetCornerRadius(radius);

                    // Add the drawables to a state list and assign the state list to the button
                    var sld = new StateListDrawable();
                    sld.AddState(new int[] { Android.Resource.Attribute.StatePressed }, _pressed);
                    sld.AddState(new int[] { }, _normal);
                    Control.Background = sld;
                };
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //var button = (XButton)sender;

            //if (_normal != null && _pressed != null)
            //{
            //    if (e.PropertyName == "BorderRadius")
            //    {
            //        _normal.SetCornerRadius(button.CornerRadius);
            //        _pressed.SetCornerRadius(button.CornerRadius);
            //    }
            //    if (e.PropertyName == "BorderWidth" || e.PropertyName == "BorderColor")
            //    {
            //        _normal.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
            //        _pressed.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
            //    }
            //}
        }
    }
}