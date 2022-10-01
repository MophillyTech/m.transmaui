using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class AspectRatioButton : Button
	{
		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			double newWidthConstraint = Math.Max(widthConstraint, heightConstraint*2);
			double newHeightConstraint = Math.Max(heightConstraint, newWidthConstraint / 2);
			return new SizeRequest(new Size(newWidthConstraint, newHeightConstraint));
		}
	}
}
