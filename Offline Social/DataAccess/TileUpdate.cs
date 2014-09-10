using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TileUpdate
    {
        public void btnIconicTile_Click(int count)
        {
            UserName names = new UserName();
            IconicTileData oIcontile = new IconicTileData();
            oIcontile.Title = names.fbname;
            oIcontile.Count = count;

            oIcontile.IconImage = new Uri("Assets/Tiles/app_202x202.png", UriKind.Relative);
            oIcontile.SmallIconImage = new Uri("Assets/Tiles/app_110x110.png", UriKind.Relative);

            oIcontile.WideContent1 = "Offline Social";
            //oIcontile.WideContent2 = "Name";
            oIcontile.WideContent3 = "Queued Updates On Phone";


            oIcontile.BackgroundColor = System.Windows.Media.Colors.Orange;

            // find the tile object for the application tile that using "Iconic" contains string in it.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("Iconic".ToString()));

            if (TileToFind != null && TileToFind.NavigationUri.ToString().Contains("Iconic"))
            {
                TileToFind.Delete();
                ShellTile.Create(new Uri("/MainPage.xaml?id=Iconic", UriKind.Relative), oIcontile, true);
            }
            else
            {
                ShellTile.Create(new Uri("/MainPage.xaml?id=Iconic", UriKind.Relative), oIcontile, true);
            }

        }

    }
}
