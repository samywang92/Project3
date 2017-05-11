using Cecs475.BoardGames.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.WpfApplication {
	/// <summary>
	/// Interaction logic for GameChoiceWindow.xaml
	/// </summary>
	public partial class GameChoiceWindow : Window {
		public GameChoiceWindow() {
			InitializeComponent();
            //create directory for dll files
            //C:/Users/Stephanie/Desktop/New folder/Project3/Project3/src/Cecs475.BoardGames.WpfApplication/bin/Debug/lib
            //C:/Users/samue/Documents/CECS 475/Project3/Project3/src/Cecs475.BoardGames.WpfApplication/bin/Debug/lib
            string dir = "C:/Users/samue/Documents/CECS 475/Project3/Project3/src/Cecs475.BoardGames.WpfApplication/bin/Debug/lib";
            Type IGameType = typeof(IGameType);
            List<Assembly> GameSearch = new List<Assembly>();
            foreach (var dll in Directory.GetFiles(dir,"*.dll")){
                var y = Assembly.LoadFrom(dll);
                GameSearch.Add(y);
            }
            //filters the assemblies with goodes that are assignable and are classes
            var gameTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(t => IGameType.IsAssignableFrom(t) && t.IsClass);
            List<IGameType> GameList = new List<IGameType>();
            foreach(var GameTypes in gameTypes) {
                var x = GameTypes.GetConstructor(Type.EmptyTypes); // empty type are constructors that have no parameters
                IGameType instance = (IGameType)x.Invoke(null);
                GameList.Add(instance);
            }
            //add the list to the resource
            this.Resources.Add("GameTypes", GameList);
        }

		private void Button_Click(object sender, RoutedEventArgs e) {
			Button b = sender as Button;
			IGameType gameType = b.DataContext as IGameType;
			var gameWindow = new MainWindow(gameType, 
				mHumanBtn.IsChecked.Value ? NumberOfPlayers.Two : NumberOfPlayers.One) {
				Title = gameType.GameName
			};
			gameWindow.Closed += GameWindow_Closed;

			gameWindow.Show();
			this.Hide();
		}

		private void GameWindow_Closed(object sender, EventArgs e) {
			this.Show();
		}
	}
}
