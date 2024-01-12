using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace UIComponents.Web.ViewEngines;

public class UICViewEngine : RazorViewEngine
{
	public UICViewEngine()
	{
		MasterLocationFormats = new string[]
		{
			"~/Views/Shared/Components/UIComponent"
		};
	}
}
