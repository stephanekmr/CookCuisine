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

        // Dictionnaire des meubles avec nom, taille et image.
        //Les images se trouvent dans "bin/Debug/Images"
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
        }

        //Pour la recherche
        private void CreateFurniturePanel()
        {
            searchMeubles.TextChanged += SearchBox_TextChanged;

            this.Controls.Add(searchMeubles);
            this.Controls.Add(meublesPanel);
        }

        //Procédure d'une recherche qui permet de cacher et réorganiser le menu des meubles
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

            //Ici, on crée les menus

            // Create MenuStrip
            MenuStrip menuBar = new MenuStrip();

            // Create Menus
            ToolStripMenuItem fichier = new ToolStripMenuItem("Fichier");
            ToolStripMenuItem edition = new ToolStripMenuItem("Edition");
            ToolStripMenuItem aide = new ToolStripMenuItem("Aide");

            // Create Submenu
            ToolStripMenuItem subMenu = new ToolStripMenuItem("Sub menu");

            // Create Menu Items
            ToolStripMenuItem load = new ToolStripMenuItem("Charger");
            ToolStripMenuItem save = new ToolStripMenuItem("Sauvegarder");
            ToolStripMenuItem quitter = new ToolStripMenuItem("Quitter");

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


        //On charge les images pour pouvoir les utiliser
        private void LoadImages()
        {
            // Chemin vers votre dossier d'images (à adapter)
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
                        // Image par défaut si fichier non trouvé
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

        //Liste des boutons pour ajouter des meubles à partir du dictionnaire
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
                    // Création d'un bouton avec image réduite
                    Image resizedImage = new Bitmap(image, new Size(30, 30));

                    Button addButton = new Button
                    {
                        Text = " " + meuble.Key, // Espace pour l'image
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

        //Bouton pour ajouter le meuble correspondant
        private void AddFurnitureButton_Click(object sender, EventArgs e)
        {
            Button addButton = (Button)sender;
            string meubleType = (string)addButton.Tag;
            CreateFurnitureButton(meubleType);
        }


        //Les propriétés des boutons pour ajouter des meubles
        private void CreateFurnitureButton(string meubleType)
        {
            if (meubles.TryGetValue(meubleType, out var properties) &&
                meubleImages.TryGetValue(meubleType, out var image))
            {
                // Redimensionner l'image pour qu'elle s'adapte au bouton
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
                newButton.BringToFront(); // S'assure que le bouton est au premier plan
            }
        }

        //Propriétés des boutons représentant des meubles qu'on vient d'ajouter
        private void SetupButton(Button btn)
        {
            btn.MouseDown += Button_MouseDown;
            btn.MouseUp += Button_MouseUp;
            btn.MouseMove += Button_MouseMove;

            // Création du menu contextuel
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Option de rotation
            ToolStripMenuItem rotateItem = new ToolStripMenuItem("Rotation 90°");
            rotateItem.Click += (sender, e) => RotateButton(btn);
            contextMenu.Items.Add(rotateItem);

            // Option de duplication
            ToolStripMenuItem duplicateItem = new ToolStripMenuItem("Dupliquer");
            duplicateItem.Click += (sender, e) => DuplicateButton(btn);
            contextMenu.Items.Add(duplicateItem);

            // Option de suppression
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Supprimer");
            deleteItem.Click += (sender, e) => DeleteButton(btn);
            contextMenu.Items.Add(deleteItem);

            // Assigner le menu contextuel au bouton
            btn.ContextMenuStrip = contextMenu;

            btn.BringToFront();
        }

        // Procédure pour vérifier si on déplace le meuble
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
                // Afficher le menu contextuel
                ((Button)sender).ContextMenuStrip?.Show((Button)sender, e.Location);
            }
        }

        //Procédure pour annuler le déplacement
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            if (draggedButton != null)
            {
                draggedButton.Capture = false;
                draggedButton = null;
            }
        }

        //Procédure pour déplacer les meubles
        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedButton != null)
            {
                Point newLocation = draggedButton.PointToScreen(e.Location);
                newLocation.Offset(-offset.X, -offset.Y);
                draggedButton.Location = this.PointToClient(newLocation);
            }
        }

        // Gestion du clic sur le bouton de menu
        private void button1_Click(object sender, EventArgs e)
        {
            meublesPanel.Visible = !meublesPanel.Visible;
        }

        //Pour tourner les meubles
        private void RotateButton(Button btn)
        {
            // Sauvegarder l'image originale
            Image originalImage = btn.Image;

            // Créer une nouvelle image tournée
            Bitmap rotatedImage = new Bitmap(originalImage.Height, originalImage.Width);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(rotatedImage.Width / 2, rotatedImage.Height / 2);
                g.RotateTransform(90);
                g.TranslateTransform(-originalImage.Width / 2, -originalImage.Height / 2);
                g.DrawImage(originalImage, Point.Empty);
            }

            // Échanger largeur/hauteur
            btn.Size = new Size(btn.Height, btn.Width);
            btn.Image = rotatedImage;
        }

        //Pour dupliquer les meubles
        private void DuplicateButton(Button originalBtn)
        {
            if (originalBtn.Tag is string meubleType)
            {
                // Créer un nouveau bouton à côté de l'original
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

        //Pour supprimer les boutons
        private void DeleteButton(Button btn)
        {
            // Confirmation avant suppression
            DialogResult result = MessageBox.Show("Supprimer ce meuble ?", "Confirmation",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Controls.Remove(btn);
                btn.Dispose();
            }
        }

        // Vides car sinon ça ne compilait pas !
        private void button2_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
    }
}