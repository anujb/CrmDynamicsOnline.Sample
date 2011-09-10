using System;
using System.Linq;
using MonoTouch.UIKit;
using RestSharp;
using System.Xml;
using System.Xml.Linq;
using MonoTouch.Foundation;
using System.IO;
using System.Drawing;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.Net;


namespace CrmDynamicsOnline.Sample
{
	public class LoginViewController : DialogViewController
	{
		RestClient Client { get; set; }
		RestResponse Response { get; set; }
		NSHttpCookieStorage _CookieJar;
		
		UIWebView _WebView { get; set; }
		EntryElement _UserNameElement { get; set; }
		EntryElement _PasswordElement { get; set; }
		EntryElement _OrganizationElement { get; set; }
		StringElement _SubmitElement { get; set; }
		
		Dictionary<string, string> JsFiles;
		
		public Action<bool> RetreivedResultAction { get; set; }
		
		public LoginViewController ()
			: base(new RootElement(""))
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			_CookieJar = NSHttpCookieStorage.SharedStorage;
			foreach (var cookie in _CookieJar.Cookies)
				_CookieJar.DeleteCookie(cookie);
			
			 JsFiles = new DirectoryInfo(Path.Combine(NSBundle.MainBundle.BundlePath, @"js"))
				.GetFiles("*.js", SearchOption.AllDirectories)
				.ToDictionary(x=>x.Name, x=>File.ReadAllText(x.FullName));
			
 			Client = new RestClient(@"https://signin.crm.dynamics.com/portal/signin/signin.aspx");
			var req = new RestRequest();
			req.AddHeader("User-Agent", @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.5 Safari/535.2");
			
			Response = Client.Execute(req);
			
			_WebView = new UIWebView();
			
			_WebView.LoadFinished += HandleWebViewLoadFinished;
			
			_WebView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_WebView.MultipleTouchEnabled = true;
			
			_UserNameElement = new EntryElement("UserName: ", "", "foocrm2011@gmail.com");
			_PasswordElement = new EntryElement("Password: ", "", "mscrm2011");
			_OrganizationElement = new EntryElement("Organization:", "", "emergedata");
			_SubmitElement = new StringElement("Submit", Handle_SubmitButtonClicked);
			_SubmitElement.Alignment = UITextAlignment.Center;
			
			
			var section = new Section("Login")
			{
				_UserNameElement,
				_PasswordElement,
				_OrganizationElement,
				_SubmitElement,
			};
			
			this.Root.Add(section);
		}

		void Handle_SubmitButtonClicked()
		{
			if(_WebView.IsLoading)
				return;
			
			using(var pool = new NSAutoreleasePool())
			{
				pool.BeginInvokeOnMainThread(()=>{
					_SubmitElement.Caption = "Processing...";
					Root[0].Add(new ActivityElement());
					this.TableView.ReloadData();
				});
			}
			
			
			_WebView.LoadHtmlString(Response.Content, new NSUrl(@"https://signin.crm.dynamics.com"));
		}
		
		
		void HandleWebViewLoadFinished (object sender, EventArgs e)
		{
			if(_WebView.EvaluateJavascript(@"document.readyState").Trim().ToLower() != "complete")
				return;
			
			if(!AppDelegate.CredentialCookies.Any())
			{
				_WebView.EvaluateJavascript(JsFiles["jquery-1.6.3.min.js"]);
				
				var json = "$('body').data('credentials', { " +
					string.Format(@"username: '{0}', password: '{1}' ", _UserNameElement.Value, _PasswordElement.Value) + "});";
			
				_WebView.EvaluateJavascript(json);
				
				_WebView.EvaluateJavascript(JsFiles["login.js"]);
				
				SetCredentialCookie();
			}
		}
		
		void SetCredentialCookie()
		{
			if(_CookieJar.Cookies == null)
				return;
			
			AppDelegate.OrganizationName = _OrganizationElement.Value;
			var cookies = _CookieJar.Cookies.Where(c=>c.Name == "MSISAuth" || c.Name == "MSISAuth1")
				.ToDictionary(x => x.Name, x => new Cookie{ Name = x.Name, Domain = x.Domain, Value = x.Value });
			
			if(cookies.Any())
			{
				AppDelegate.CredentialCookies = cookies;
				
				RetreivedResultAction.BeginInvoke(true, (ar)=>{}, null);
				
			}
			
		}
		
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			_WebView.LoadFinished -= HandleWebViewLoadFinished;
		}
	}
}

