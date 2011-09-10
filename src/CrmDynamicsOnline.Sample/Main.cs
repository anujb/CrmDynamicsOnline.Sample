using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Net;
using RestSharp.Contrib;

namespace CrmDynamicsOnline.Sample
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}
	
	public partial class AppDelegate : UIApplicationDelegate
	{
		public static Dictionary<string, Cookie> CredentialCookies { get; set; }
		public static string OrganizationName { get; set; }
		
		MainNavController controller;
		
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			CredentialCookies = new Dictionary<string, Cookie>{ };
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, ssl) => { return true; };
			
			controller = new MainNavController();
			
			window.RootViewController = controller;
			window.MakeKeyAndVisible ();
	
			return true;
		}
	
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

