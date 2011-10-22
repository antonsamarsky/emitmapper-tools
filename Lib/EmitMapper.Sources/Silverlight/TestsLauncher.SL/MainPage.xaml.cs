using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Reflection;
using System.Windows.Resources;
using System.IO;
using NUnit.Framework;

namespace TestsLauncher.SL
{
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();

			var result = new StringBuilder();

			StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("EmitMapperTests.SL.dll", UriKind.Relative));
			var testFixtures = new AssemblyPart()
				.Load(streamInfo.Stream)
				.GetTypes()
				.Where(t => t.GetCustomAttributes(typeof(TestFixtureAttribute), false).Length > 0 )
				.ToArray();

            int cntAll = 0;
            int cntPassed = 0;
			foreach (var t in testFixtures)
			{
				var obj = Activator.CreateInstance(t);
				foreach (var mi in
					t.GetMethods()
					.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Length > 0 && m.GetParameters().Length == 0)
				)
				{
					result.Append("Executing " + t.ToString() + "." + mi.Name + "... ");
					try
					{
                        cntAll++;
						mi.Invoke(obj, null);
                        cntPassed++;
						result.AppendLine("OK");
					}
					catch (Exception e)
					{
						result.AppendLine("ERROR");
						result.AppendLine(e.ToString());
						result.AppendLine("--------------------------------");
					}
				}
			}

            result.AppendLine("Done. " + cntPassed + "(" + cntAll + ")");

			txtOut.Text = result.ToString();
		}
	}
}
