using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zelda1MapTracker {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        const int COL_COUNT = 16;
        const int ROW_COUNT = 8;

        public MainWindow() {
            InitializeComponent();
            setupGrid();
        }

        /// <summary>
        /// sets up the overworld map as background image
        /// adds rectangles to uniform grid
        /// </summary>
        private void setupGrid() {

            //set background of canvas
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(
                new Uri(@"D:\code\c#\Projects2015\Zelda1MapTracker\Zelda1MapTracker\images\z1_overworld_quest1_scale45.png"));

            overworldCanvas.Height = (ib.ImageSource as BitmapSource).PixelHeight;
            overworldCanvas.Width = (ib.ImageSource as BitmapSource).PixelWidth;
            overworldCanvas.Background = ib;

            //add rectangles to inner grid
            for (int i = 0; i < COL_COUNT * ROW_COUNT; i++) {
                Rectangle rect = new Rectangle();
                rect.Height = overworldCanvas.Height / ROW_COUNT;
                rect.Width = overworldCanvas.Width / COL_COUNT;
                rect.Opacity = 0.6;
                rect.Fill = Brushes.Transparent;

                rect.MouseLeftButtonDown += switchRectangleFillColor_Click;
                rect.MouseWheel += denoteImportantTile_MouseWheel;
                rectangleGrid.Children.Add(rect);
            }
        }

        /// <summary>
        /// mouse left button down handler to toggle overlayed rect fill color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switchRectangleFillColor_Click(object sender, MouseButtonEventArgs e) {
            Rectangle rect = sender as Rectangle;

            if (rect.Fill == Brushes.Transparent) {
                rect.Fill = Brushes.Green;
            } else if (rect.Fill == Brushes.Green) {
                rect.Fill = Brushes.Orange;
            } else if (rect.Fill == Brushes.Orange) {
                rect.Fill = Brushes.Purple;
            } else if (rect.Fill == Brushes.Purple) {
                rect.Fill = Brushes.Red;
            } else if (rect.Fill == Brushes.Red) {
                rect.Fill = Brushes.Transparent;
            }
        }

        /// <summary>
        /// Mousewheel handler to denote a tile as important
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void denoteImportantTile_MouseWheel(object sender, MouseWheelEventArgs e) {
            Rectangle rect = sender as Rectangle;

            //****NOTE****
            //logic below uses rectangle's opacity as a flag
            //to check the state of the tile
            //************

            //TODO: add thread to make tile flashing?
            if (rect.Opacity == 0.6) {
                rect.Opacity = 0.7;
                rect.Fill = Brushes.Maroon;
                rect.MouseLeftButtonDown -= switchRectangleFillColor_Click;
            } else {
                rect.Opacity = 0.6;
                rect.Fill = Brushes.Transparent;
                rect.MouseLeftButtonDown += switchRectangleFillColor_Click;
            }

        }
    }
}
