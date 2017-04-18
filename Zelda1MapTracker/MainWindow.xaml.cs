using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
                new Uri(@"D:\code\c#\Projects2015\Zelda1MapTracker\Zelda1MapTracker\assets\maps\z1_overworld_quest1_scale45.png"));

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
                rect.MouseWheel += denoteDungeonTile_MouseWheel;
                rectangleGrid.Children.Add(rect);
            }
        }

        /// <summary>
        /// on click - switch overlayed rect fill color
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
                rect.Fill = Brushes.Transparent;
            }
        }

        /// <summary>
        /// On mousewheel - denote dungeon tile and switch click handler
        /// OR if tile is already denoted, remove dungeon characteristics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void denoteDungeonTile_MouseWheel(object sender, MouseWheelEventArgs e) {
            Rectangle rect = sender as Rectangle;

            //****NOTE****
            //logic below uses rectangle's opacity as a flag
            //to check the state of the tile
            //************

            //denote whether or not tile has a dungeon
            //adjust click handlers appropriately
            if (rect.Opacity == 0.6) {
                rect.Opacity = 0.7;
                rect.Fill = Brushes.Red;
                rect.MouseLeftButtonDown -= switchRectangleFillColor_Click;
                addDungeonIconsToTile(rect);
            } else {
                rect.Opacity = 0.6;
                rect.Fill = Brushes.Transparent;
                rect.MouseLeftButtonDown += switchRectangleFillColor_Click;
            }

        }

        private void addDungeonIconsToTile(Rectangle rect) {

            //get coordinates of tile
            Point p = rect.TranslatePoint(new Point(0, 0), rectangleGrid);
            double x = p.X;
            double y = p.Y;


            UniformGrid dungeonIconGrid = new UniformGrid();
            dungeonIconGrid.Height = rect.Height;
            dungeonIconGrid.Width = rect.Width;
            dungeonIconGrid.Rows = 2;
            dungeonIconGrid.Columns = 2;

            //add mouse wheel handler to remove dungeon grid for mis-fires
            dungeonIconGrid.MouseWheel += removeDungeonIconsFromTile_Mousewheel;

            //add canvases to uniformGrid from sprite images
            String[] filePaths = System.IO.Directory.GetFiles(@"D:\code\c#\Projects2015\Zelda1MapTracker\Zelda1MapTracker\assets\sprites");
            foreach (String path in filePaths) {
                if (path.EndsWith("db")) continue;
                Canvas canvas = createDungeonIconCanvas(path);
                dungeonIconGrid.Children.Add(canvas);
            }

            Canvas.SetLeft(dungeonIconGrid, x);
            Canvas.SetTop(dungeonIconGrid, y);

            overworldCanvas.Children.Add(dungeonIconGrid);
        }

        private void removeDungeonIconsFromTile_Mousewheel(object sender, MouseWheelEventArgs e) {
            UniformGrid dungeonGrid = sender as UniformGrid;
            overworldCanvas.Children.Remove(dungeonGrid);
        }

        private void removeDungeonIconsFromTile(Rectangle rect) {

            //get coordinates of tile
            Point p = rect.TranslatePoint(new Point(0, 0), rectangleGrid);
            double x = p.X;
            double y = p.Y;
            int xInt = (int)x;
            int yInt = (int)y;

            //find grid in overworld's children and remove it
            UniformGrid dungeonGrid = overworldCanvas.FindName($"dungeonGrid{xInt}{yInt}") as UniformGrid;
            overworldCanvas.Children.Remove(dungeonGrid);
        }

        private Canvas createDungeonIconCanvas(String filePath) {
            Canvas canvas = new Canvas();

            //add background image to canvas
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(filePath));
            ib.Stretch = Stretch.None;
            canvas.Background = ib;

            //toggle background image visibility on click
            canvas.Background.Opacity = 0;
            canvas.MouseLeftButtonDown += toggleDungeonIcon_Click;

            //show image while mouse is over
            canvas.MouseEnter += showDungeonIcon_MouseEnter;
            canvas.MouseLeave += hideDungeonIcon_MouseLeave;

            return canvas;
        }

        private void hideDungeonIcon_MouseLeave(object sender, MouseEventArgs e) {
            Canvas canvas = sender as Canvas;
            canvas.Background.Opacity = 0;
        }

        private void showDungeonIcon_MouseEnter(object sender, MouseEventArgs e) {
            Canvas canvas = sender as Canvas;
            canvas.Background.Opacity = 0.5;
        }

        /// <summary>
        /// sets the properties of the dungeon icon
        /// checks state via the opacity of the background image 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleDungeonIcon_Click(object sender, MouseButtonEventArgs e) {
            Canvas canvas = sender as Canvas;
            if (canvas.Background.Opacity < 1) {
                canvas.Background.Opacity = 1;
                canvas.MouseEnter -= showDungeonIcon_MouseEnter;
                canvas.MouseLeave -= hideDungeonIcon_MouseLeave;
            } else {
                canvas.Background.Opacity = 0;
                canvas.MouseEnter += showDungeonIcon_MouseEnter;
                canvas.MouseLeave += hideDungeonIcon_MouseLeave;
            }
        }

        private void switchDungeonIcons(object sender, MouseButtonEventArgs e) {
            throw new NotImplementedException();
        }
    }
}
