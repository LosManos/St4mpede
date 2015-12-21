using System.Collections.Generic;

namespace St4mpede.Settings
{
	public class Menu
	{
		public Menu()
		{
			SubMenus = new List<Menu>();
			MenuItems = new List<MenuItem>();
		}

		public IList<Menu> SubMenus { get; set; }
		public IList<MenuItem> MenuItems { get; set; }
		public string Name { get; set; }
	}
}