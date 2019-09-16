package md5e8e8bdad197c8a4cf953d1b2e7341680;


public class CustomButtonRenderer
	extends md58432a647068b097f9637064b8985a5e0.ButtonRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("SolexMobileApp.Droid.CustomsRenderers.CustomButtonRenderer, SolexMobileApp.Android", CustomButtonRenderer.class, __md_methods);
	}


	public CustomButtonRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == CustomButtonRenderer.class)
			mono.android.TypeManager.Activate ("SolexMobileApp.Droid.CustomsRenderers.CustomButtonRenderer, SolexMobileApp.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public CustomButtonRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == CustomButtonRenderer.class)
			mono.android.TypeManager.Activate ("SolexMobileApp.Droid.CustomsRenderers.CustomButtonRenderer, SolexMobileApp.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public CustomButtonRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == CustomButtonRenderer.class)
			mono.android.TypeManager.Activate ("SolexMobileApp.Droid.CustomsRenderers.CustomButtonRenderer, SolexMobileApp.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
