using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CookCuisine
{
    public partial class Form1 : Form
    {
        private int buttonCounter = 1;
        private bool isDragging = false;
        private Button draggedButton;
        private Point offset;
        private bool isResizing = false;
        private Point resizeStartPoint;
        private Size originalSize;

        // Dictionnaire des meubles avec nom, taille et image
        private Dictionary<string, (string Text, Size Size, string ImageName)> meubles =
            new Dictionary<string, (string, Size, string)>()
        {
            {"Chaise", ("Chaise", new Size(60, 80), "chair.png")},
            {"Table", ("Table", new Size(120, 70), "table.png")},
            {"Canapé", ("Canapé", new Size(150, 80), "sofa.png")},
            {"Armoire", ("Armoire", new Size(80, 120), "wardrobe.png")},
            {"Armoire2", ("Armoire", new Size(80, 120), "wardrobe.png")},
            {"Armoire3", ("Armoire", new Size(80, 120), "wardrobe.png")},
            {"Armoire4", ("Armoire", new Size(80, 120), "wardrobe.png")},
            {"Armoire5", ("Armoire", new Size(80, 120), "wardrobe.png")},
            {"Armoire6", ("Armoire", new Size(80, 120), "wardrobe.png")},
            {"Lit", ("Lit", new Size(140, 100), "bed.png")}
        };

        // Dictionnaire pour stocker les images chargées
        private Dictionary<string, Image> meubleImages = new Dictionary<string, Image>();

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            LoadImages();
            CreateFurniturePanel();
            CreateAddFurnitureButtons();

            // Initialiser les événements pour le redimensionnement
            zoneCuisine.MouseDown += zoneCuisine_MouseDown;
            zoneCuisine.MouseMove += zoneCuisine_MouseMove;
            zoneCuisine.MouseUp += zoneCuisine_MouseUp;
            zoneCuisine.Paint += zoneCuisine_Paint;
        }

        private void zoneCuisine_MouseDown(object sender, MouseEventArgs e)
        {
            // Vérifier si le clic est dans la zone de redimensionnement (coin inférieur droit)
            Rectangle resizeZone = new Rectangle(
                zoneCuisine.Width - 16,
                zoneCuisine.Height - 16,
                16,
                16);

            if (resizeZone.Contains(e.Location))
            {
                isResizing = true;
                resizeStartPoint = e.Location;
                originalSize = zoneCuisine.Size;
                Cursor = Cursors.SizeNWSE;
            }
        }

        private void zoneCuisine_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle resizeZone = new Rectangle(
                zoneCuisine.Width - 16,
                zoneCuisine.Height - 16,
                16,
                16);

            // Changer le curseur quand on survole la zone de redimensionnement
            if (resizeZone.Contains(e.Location))
            {
                Cursor = Cursors.SizeNWSE;
            }
            else if (!isResizing)
            {
                Cursor = Cursors.Default;
            }

            // Redimensionnement en cours
            if (isResizing)
            {
                int newWidth = originalSize.Width + (e.X - resizeStartPoint.X);
                int newHeight = originalSize.Height + (e.Y - resizeStartPoint.Y);

                // Limiter la taille minimale
                newWidth = Math.Max(100, newWidth);
                newHeight = Math.Max(100, newHeight);

                zoneCuisine.Size = new Size(newWidth, newHeight);
            }
        }

        private void zoneCuisine_MouseUp(object sender, MouseEventArgs e)
        {
            isResizing = false;
            Cursor = Cursors.Default;
        }

        private void zoneCuisine_Paint(object sender, PaintEventArgs e)
        {
            // Dessiner uniquement dans la zone du triangle
            Rectangle triangleRect = new Rectangle(
                zoneCuisine.Width - 16,
                zoneCuisine.Height - 16,
                16,
                16);

            e.Graphics.SetClip(triangleRect);
            e.Graphics.Clear(zoneCuisine.BackColor);

            Point[] triangle = {
                new Point(zoneCuisine.Width - 16, zoneCuisine.Height),
                new Point(zoneCuisine.Width, zoneCuisine.Height - 16),
                new Point(zoneCuisine.Width, zoneCuisine.Height)
            };

            e.Graphics.FillPolygon(Brushes.Gray, triangle);
            e.Graphics.ResetClip();
        }

        private void CreateFurniturePanel()
        {
            searchMeubles.TextChanged += SearchBox_TextChanged;
            this.Controls.Add(searchMeubles);
            this.Controls.Add(meublesPanel);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchMeubles.Text.ToLower();
            int yPosition = 10;

            var buttons = meublesPanel.Controls.OfType<Button>()
                              .Where(b => b.Tag != null)
                              .OrderBy(b => b.Tag.ToString());

            foreach (var btn in buttons)
            {
                bool shouldShow = btn.Tag.ToString().ToLower().Contains(searchText) ||
                                 string.IsNullOrEmpty(searchText);

                btn.Visible = shouldShow;
                if (shouldShow)
                {
                    btn.Location = new Point(10, yPosition);
                    yPosition += btn.Height + 5;
                }
            }

            meublesPanel.AutoScrollMinSize = new Size(0, yPosition);
        }

        private void SetupUI()
        {
            // Create MenuStrip
            MenuStrip menuBar = new MenuStrip();

            // Create Menus
            ToolStripMenuItem fichier = new ToolStripMenuItem("Fichier");
            ToolStripMenuItem edition = new ToolStripMenuItem("Edition");
            ToolStripMenuItem aide = new ToolStripMenuItem("Aide");

            // Create Menu Items
            ToolStripMenuItem load = new ToolStripMenuItem("Charger");
            ToolStripMenuItem save = new ToolStripMenuItem("Sauvegarder");
            ToolStripMenuItem quitter = new ToolStripMenuItem("Quitter");
            quitter.Click += quitter_Click;

            // Add items to menu1
            fichier.DropDownItems.Add(load);
            fichier.DropDownItems.Add(save);
            fichier.DropDownItems.Add(quitter);

            // Add menus to menuBar
            menuBar.Items.Add(fichier);
            menuBar.Items.Add(edition);
            menuBar.Items.Add(aide);

            this.Controls.Add(menuBar);
            this.MainMenuStrip = menuBar;
        }

        private void LoadImages()
        {
            string imagePath = Path.Combine(Application.StartupPath, "Images");

            foreach (var meuble in meubles)
            {
                try
                {
                    string fullPath = Path.Combine(imagePath, meuble.Value.ImageName);
                    if (File.Exists(fullPath))
                    {
                        meubleImages[meuble.Key] = Image.FromFile(fullPath);
                    }
                    else
                    {
                        meubleImages[meuble.Key] = CreateDefaultImage(meuble.Value.Size, meuble.Key);
                    }
                }
                catch
                {
                    meubleImages[meuble.Key] = CreateDefaultImage(meuble.Value.Size, meuble.Key);
                }
            }
        }

        private Image CreateDefaultImage(Size size, string text)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                using (Font font = new Font("Arial", 8))
                {
                    g.DrawString(text, font, Brushes.Black,
                        new PointF(5, (size.Height - font.Height) / 2));
                }
                g.DrawRectangle(Pens.Black, 0, 0, size.Width - 1, size.Height - 1);
            }
            return bmp;
        }

        private void CreateAddFurnitureButtons()
        {
            meublesPanel.AutoScroll = true;
            meublesPanel.VerticalScroll.Enabled = true;
            meublesPanel.VerticalScroll.Visible = true;
            int yPosition = 10;

            foreach (var meuble in meubles.OrderBy(m => m.Key))
            {
                if (meubleImages.TryGetValue(meuble.Key, out var image))
                {
                    Image resizedImage = new Bitmap(image, new Size(30, 30));

                    Button addButton = new Button
                    {
                        Text = " " + meuble.Key,
                        Location = new Point(10, yPosition),
                        Size = new Size(100, 40),
                        Tag = meuble.Key,
                        Image = resizedImage,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        TextAlign = ContentAlignment.MiddleLeft,
                        ImageAlign = ContentAlignment.MiddleLeft
                    };

                    addButton.Click += AddFurnitureButton_Click;
                    meublesPanel.Controls.Add(addButton);
                    yPosition += addButton.Height + 5;
                }
            }
        }

        private void AddFurnitureButton_Click(object sender, EventArgs e)
        {
            Button addButton = (Button)sender;
            string meubleType = (string)addButton.Tag;
            CreateFurnitureButton(meubleType);
        }

        private void CreateFurnitureButton(string meubleType)
        {
            if (meubles.TryGetValue(meubleType, out var properties) &&
                meubleImages.TryGetValue(meubleType, out var image))
            {
                Image resizedImage = new Bitmap(image, properties.Size);

                Button newButton = new Button
                {
                    Name = "btn_" + meubleType + "_" + (buttonCounter++),
                    Text = "",
                    Size = properties.Size,
                    Location = new Point(300, 100),
                    FlatStyle = FlatStyle.Flat,
                    Tag = meubleType,
                    Image = resizedImage,
                    BackgroundImageLayout = ImageLayout.Stretch,
                    BackColor = Color.Transparent
                };

                SetupButton(newButton);
                this.Controls.Add(newButton);
                newButton.BringToFront();
            }
        }

        private void SetupButton(Button btn)
        {
            btn.MouseDown += Button_MouseDown;
            btn.MouseUp += Button_MouseUp;
            btn.MouseMove += Button_MouseMove;

            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem rotateItem = new ToolStripMenuItem("Rotation 90°");
            rotateItem.Click += (sender, e) => RotateButton(btn);
            contextMenu.Items.Add(rotateItem);

            ToolStripMenuItem duplicateItem = new ToolStripMenuItem("Dupliquer");
            duplicateItem.Click += (sender, e) => DuplicateButton(btn);
            contextMenu.Items.Add(duplicateItem);

            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Supprimer");
            deleteItem.Click += (sender, e) => DeleteButton(btn);
            contextMenu.Items.Add(deleteItem);

            btn.ContextMenuStrip = contextMenu;
            btn.BringToFront();
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                draggedButton = (Button)sender;
                offset = e.Location;
                draggedButton.Capture = true;
                draggedButton.BringToFront();
            }
            else if (e.Button == MouseButtons.Right)
            {
                ((Button)sender).ContextMenuStrip?.Show((Button)sender, e.Location);
            }
        }

        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            if (draggedButton != null)
            {
                draggedButton.Capture = false;
                draggedButton = null;
            }
        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedButton != null)
            {
                Point newLocation = draggedButton.PointToScreen(e.Location);
                newLocation.Offset(-offset.X, -offset.Y);
                draggedButton.Location = this.PointToClient(newLocation);

                if (draggedButton.Bounds.IntersectsWith(zoneCuisine.Bounds))
                {
                    draggedButton.BackColor = Color.Transparent;
                }
                else
                {
                    draggedButton.BackColor = Color.Red;
                }
            }
        }

        private void RotateButton(Button btn)
        {
            Image originalImage = btn.Image;
            Bitmap rotatedImage = new Bitmap(originalImage.Height, originalImage.Width);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(rotatedImage.Width / 2, rotatedImage.Height / 2);
                g.RotateTransform(90);
                g.TranslateTransform(-originalImage.Width / 2, -originalImage.Height / 2);
                g.DrawImage(originalImage, Point.Empty);
            }

            btn.Size = new Size(btn.Height, btn.Width);
            btn.Image = rotatedImage;
        }

        private void DuplicateButton(Button originalBtn)
        {
            if (originalBtn.Tag is string meubleType)
            {
                Button newButton = new Button
                {
                    Name = "btn_" + meubleType + "_" + (buttonCounter++),
                    Text = originalBtn.Text,
                    Size = originalBtn.Size,
                    Location = new Point(originalBtn.Right + 10, originalBtn.Top),
                    FlatStyle = originalBtn.FlatStyle,
                    Tag = originalBtn.Tag,
                    Image = (Image)originalBtn.Image.Clone(),
                    BackgroundImageLayout = originalBtn.BackgroundImageLayout,
                    BackColor = originalBtn.BackColor
                };

                SetupButton(newButton);
                this.Controls.Add(newButton);
                newButton.BringToFront();
            }
        }

        private void DeleteButton(Button btn)
        {
            DialogResult result = MessageBox.Show("Supprimer ce meuble ?", "Confirmation",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Controls.Remove(btn);
                btn.Dispose();
            }
        }

        private void quitter_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            meublesPanel.Visible = !meublesPanel.Visible;
        }

        // Méthodes vides nécessaires
        private void button2_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
    }
}