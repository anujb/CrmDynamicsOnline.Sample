using System;
using System.Linq;
using MonoTouch.UIKit;
using OData;
using MonoTouch.Dialog;

namespace CrmDynamicsOnline.Sample
{
	public class FeedsController : DialogViewController
	{
		ServiceDocument doc;
		
		public FeedsController ()
			: base(new RootElement(""))
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			 
			Http.CredentialCookies = AppDelegate.CredentialCookies;
			var serviceUri = string.Format("https://{0}.api.crm.dynamics.com/XRMServices/2011/OrganizationData.svc", AppDelegate.OrganizationName);
			
			doc = new ServiceDocument(serviceUri);
			var section = new Section("Feeds");
			foreach(var feed in doc.Feeds)
			{
				section.Add(new StringElement(feed.Name));	
			}
			
			Root.Add(section);
					
					
			
			
		}
	}
}

