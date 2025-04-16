using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CookCuisine
{
    public partial class principalForm : Form
    {
        // Variables et objets utiles
        private int buttonCounter = 1;
        private bool isDragging = false;
        private Button draggedButton;
        private Point offset;
        private bool isResizing = false;
        private Point lastMousePos;
        private Size zoneCuisineBaseSize = new Size(467, 361);
        private Point zoneCuisineBaseLocation = new Point(8,8);

        // Dictionnaire des meubles avec nom, taille et image
        private Dictionary<string, Meuble> meubles = new Dictionary<string, Meuble>()
        {
            {"Chaise", new Meuble("Chaise", new Size(60, 80), new Point(20, 20), "chair.png")},
            {"Table", new Meuble("Table", new Size(120, 70), new Point(20, 20), "table.png")},
            {"sofa", new Meuble("sofa", new Size(150, 80), new Point(20, 20), "sofa.png")},
            {"Armoire", new Meuble("Armoire", new Size(80, 120), new Point(20, 20), "wardrobe.png")},
            {"Armoire2", new Meuble("Armoire", new Size(80, 120), new Point(20, 20), "wardrobe.png")},
            {"Armoire3", new Meuble("Armoire", new Size(80, 120), new Point(20, 20), "wardrobe.png")},
            {"Armoire4", new Meuble("Armoire", new Size(80, 120), new Point(20, 20), "wardrobe.png")},
            {"Armoire5", new Meuble("Armoire", new Size(80, 120), new Point(20, 20), "wardrobe.png")},
            {"Armoire6", new Meuble("Armoire", new Size(80, 120), new Point(20, 20), "wardrobe.png")},
            {"evier_00", new Meuble("evier_00", new Size(73, 41), new Point(20, 20), "evier_00.png")},
            {"cuisiniere_abc", new Meuble("cuisiniere_abc", new Size(80, 76), new Point(20, 20), "cuisiniere_abc.png")},
            {"Canape_blanc", new Meuble("Canape_blanc", new Size(158, 86), new Point(20, 20), "canape_blanc.png")}
        };

        // Dictionnaire pour stocker les images chargées
        private Dictionary<string, Image> meubleImages = new Dictionary<string, Image>();
        // Dictionnaire pour stocker les position des meubles
        private Dictionary<String, Point> meublesPosition = new Dictionary<String, Point>();

        public principalForm()
        {
            InitializeComponent();
            SetupUI();
            LoadImages();
            CreateFurniturePanel();
            CreateAddFurnitureButtons();

            //Initialiser les événements pour le redimensionnement
            //zoneCuisine.MouseDown += zoneCuisine_MouseDown;
            //zoneCuisine.MouseMove += zoneCuisine_MouseMove;
            //zoneCuisine.MouseUp += zoneCuisine_MouseUp;
            //zoneCuisine.Paint += zoneCuisine_Paint;
        }

        private void SetupUI()
        {
            // Abonnement aux événements de redimensionnement de la fenêtre
            this.Resize += new EventHandler(principalForm_Resize);
            // Abonnement aux événements de redimensionnement de la zoneCuisine
            this.zoneCuisine.Resize += new EventHandler(zoneCuisine_Resize);
            // Gestion des événements pour le redimensionnement
            gripItem.MouseDown += GripItem_MouseDown;
            gripItem.MouseMove += GripItem_MouseMove;
            gripItem.MouseUp += GripItem_MouseUp;

        }
        private void InitializeComponent()
        {
            this.mainTLPanel = new System.Windows.Forms.TableLayoutPanel();
            this.menuFLPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.fichier = new System.Windows.Forms.ToolStripMenuItem();
            this.load = new System.Windows.Forms.ToolStripMenuItem();
            this.save = new System.Windows.Forms.ToolStripMenuItem();
            this.quitter = new System.Windows.Forms.ToolStripMenuItem();
            this.edition = new System.Windows.Forms.ToolStripMenuItem();
            this.aide = new System.Windows.Forms.ToolStripMenuItem();
            this.meubleFLPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.searchMeubles = new System.Windows.Forms.TextBox();
            this.meublesPanel = new System.Windows.Forms.Panel();
            this.cuisinePanel = new System.Windows.Forms.Panel();
            this.zoneCuisine = new System.Windows.Forms.Panel();
            this.gripItem = new System.Windows.Forms.Panel();
            this.mainTLPanel.SuspendLayout();
            this.menuFLPanel.SuspendLayout();
            this.menuBar.SuspendLayout();
            this.meubleFLPanel.SuspendLayout();
            this.searchPanel.SuspendLayout();
            this.cuisinePanel.SuspendLayout();
            this.zoneCuisine.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTLPanel
            // 
            this.mainTLPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainTLPanel.ColumnCount = 2;
            this.mainTLPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.mainTLPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84F));
            this.mainTLPanel.Controls.Add(this.menuFLPanel, 0, 0);
            this.mainTLPanel.Controls.Add(this.meubleFLPanel, 0, 1);
            this.mainTLPanel.Controls.Add(this.cuisinePanel, 1, 1);
            this.mainTLPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTLPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTLPanel.MinimumSize = new System.Drawing.Size(900, 600);
            this.mainTLPanel.Name = "mainTLPanel";
            this.mainTLPanel.RowCount = 2;
            this.mainTLPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.mainTLPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94F));
            this.mainTLPanel.Size = new System.Drawing.Size(900, 600);
            this.mainTLPanel.TabIndex = 0;
            // 
            // menuFLPanel
            // 
            this.menuFLPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainTLPanel.SetColumnSpan(this.menuFLPanel, 2);
            this.menuFLPanel.Controls.Add(this.menuBar);
            this.menuFLPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuFLPanel.Location = new System.Drawing.Point(0, 0);
            this.menuFLPanel.Margin = new System.Windows.Forms.Padding(0);
            this.menuFLPanel.Name = "menuFLPanel";
            this.menuFLPanel.Size = new System.Drawing.Size(900, 36);
            this.menuFLPanel.TabIndex = 2;
            // 
            // menuBar
            // 
            this.menuBar.AutoSize = false;
            this.menuBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(201)))), ((int)(((byte)(170)))));
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fichier,
            this.edition,
            this.aide});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(900, 36);
            this.menuBar.TabIndex = 0;
            // 
            // fichier
            // 
            this.fichier.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.load,
            this.save,
            this.quitter});
            this.fichier.Name = "fichier";
            this.fichier.Size = new System.Drawing.Size(54, 32);
            this.fichier.Text = "Fichier";
            // 
            // load
            // 
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(139, 22);
            this.load.Text = "Charger";
            // 
            // save
            // 
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(139, 22);
            this.save.Text = "Sauvegarder";
            // 
            // quitter
            // 
            this.quitter.Name = "quitter";
            this.quitter.Size = new System.Drawing.Size(139, 22);
            this.quitter.Text = "Quitter";
            this.quitter.Click += new System.EventHandler(this.quitter_Click);
            // 
            // edition
            // 
            this.edition.Name = "edition";
            this.edition.Size = new System.Drawing.Size(56, 32);
            this.edition.Text = "Edition";
            // 
            // aide
            // 
            this.aide.Name = "aide";
            this.aide.Size = new System.Drawing.Size(43, 32);
            this.aide.Text = "Aide";
            // 
            // meubleFLPanel
            // 
            this.meubleFLPanel.AutoScroll = true;
            this.meubleFLPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(161)))), ((int)(((byte)(128)))));
            this.meubleFLPanel.Controls.Add(this.searchPanel);
            this.meubleFLPanel.Controls.Add(this.meublesPanel);
            this.meubleFLPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meubleFLPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.meubleFLPanel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.meubleFLPanel.Location = new System.Drawing.Point(0, 36);
            this.meubleFLPanel.Margin = new System.Windows.Forms.Padding(0);
            this.meubleFLPanel.Name = "meubleFLPanel";
            this.meubleFLPanel.Size = new System.Drawing.Size(144, 564);
            this.meubleFLPanel.TabIndex = 0;
            this.meubleFLPanel.WrapContents = false;
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = System.Drawing.Color.Transparent;
            this.searchPanel.Controls.Add(this.searchMeubles);
            this.searchPanel.Location = new System.Drawing.Point(10, 6);
            this.searchPanel.Margin = new System.Windows.Forms.Padding(10, 6, 10, 3);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(124, 25);
            this.searchPanel.TabIndex = 0;
            // 
            // searchMeubles
            // 
            this.searchMeubles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchMeubles.Location = new System.Drawing.Point(0, 0);
            this.searchMeubles.Multiline = true;
            this.searchMeubles.Name = "searchMeubles";
            this.searchMeubles.Size = new System.Drawing.Size(124, 25);
            this.searchMeubles.TabIndex = 0;
            // 
            // meublesPanel
            // 
            this.meublesPanel.AccessibleName = "furniturePanel";
            this.meublesPanel.BackColor = System.Drawing.Color.Transparent;
            this.meublesPanel.Location = new System.Drawing.Point(10, 37);
            this.meublesPanel.Margin = new System.Windows.Forms.Padding(10, 3, 10, 8);
            this.meublesPanel.Name = "meublesPanel";
            this.meublesPanel.Size = new System.Drawing.Size(124, 519);
            this.meublesPanel.TabIndex = 0;
            // 
            // cuisinePanel
            // 
            this.cuisinePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(241)))));
            this.cuisinePanel.Controls.Add(this.zoneCuisine);
            this.cuisinePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cuisinePanel.Location = new System.Drawing.Point(147, 39);
            this.cuisinePanel.Name = "cuisinePanel";
            this.cuisinePanel.Size = new System.Drawing.Size(750, 558);
            this.cuisinePanel.TabIndex = 3;
            // 
            // zoneCuisine
            // 
            this.zoneCuisine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.zoneCuisine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zoneCuisine.Controls.Add(this.gripItem);
            this.zoneCuisine.Location = new Point(8, 8);
            this.zoneCuisine.Margin = new System.Windows.Forms.Padding(8);
            this.zoneCuisine.MaximumSize = new System.Drawing.Size(734, 542);
            this.zoneCuisine.Name = "zoneCuisine";
            this.zoneCuisine.Size = new System.Drawing.Size(467, 361);
            this.zoneCuisine.TabIndex = 1;
            // 
            // gripItem
            // 
            this.gripItem.BackColor = System.Drawing.Color.Gray;
            this.gripItem.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.gripItem.Location = new System.Drawing.Point(455, 349);
            this.gripItem.Margin = new System.Windows.Forms.Padding(0);
            this.gripItem.Name = "gripItem";
            this.gripItem.Size = new System.Drawing.Size(10, 10);
            this.gripItem.TabIndex = 0;
            // 
            // principalForm
            // 
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.mainTLPanel);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuBar;
            this.MinimumSize = new System.Drawing.Size(916, 639);
            this.Name = "principalForm";
            this.Text = "CookCuisine";
            this.mainTLPanel.ResumeLayout(false);
            this.menuFLPanel.ResumeLayout(false);
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.meubleFLPanel.ResumeLayout(false);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.cuisinePanel.ResumeLayout(false);
            this.zoneCuisine.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void GripItem_MouseDown(object sender, MouseEventArgs e)
        {
            isResizing = true;
            lastMousePos = this.PointToClient(Control.MousePosition);
        }

        private void GripItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                Point currentMousePos = this.PointToClient(Control.MousePosition);
                int dx = currentMousePos.X - lastMousePos.X;
                int dy = currentMousePos.Y - lastMousePos.Y;
                int newWidth = Math.Max(50, zoneCuisine.Width + dx);
                int newHeight = Math.Max(50, zoneCuisine.Height + dy);

                lastMousePos = currentMousePos;
                zoneCuisine.Size = new Size(newWidth, newHeight);
                zoneCuisineBaseSize = new Size(newWidth, newHeight);
                gripItem.Location = new Point(zoneCuisine.Width - gripItem.Width, zoneCuisine.Height - gripItem.Height);
            }
        }
        private void GripItem_MouseUp(object sender, MouseEventArgs e)
        {
            isResizing = false;
        }

        private void CreateFurniturePanel()
        {
            searchMeubles.TextChanged += SearchBox_TextChanged;
        }

        // Méthode pour réagir au redimensionnement de la fenêtre
        private void principalForm_Resize(object sender, EventArgs e)
        {
            // Mettre à jour la taille de searchPanel et meublesPanel 
            searchPanel.Width = meubleFLPanel.Width - 20; // marges de 20
            searchPanel.Height = Math.Min((int)(meubleFLPanel.Height / 22.56),35); // hauteur max de 35
            meublesPanel.Width = meubleFLPanel.Width - 20; // marges de 20
            meublesPanel.Height = meubleFLPanel.Height - searchPanel.Height - 20; // marges de 20
            // Mettre à jour la taille de texte de la barre de recherche
            searchMeubles.Font = new System.Drawing.Font("Segoe UI", Math.Min(searchMeubles.Height/2.58f,15));
            // Mettre à jour la taille du menu et de ses éléments
            menuBar.Size = menuFLPanel.Size;
            // Mettre à jour la taille de la police du menu avec echelle de 5.33 par rapport à la hauteur
            menuBar.Font = new Font(menuBar.Font.FontFamily, Math.Min(menuFLPanel.Height / 4.33f,12));
            // Calculons le ratio de redimentionnement dans la zone de cuisine
            float ratioZC_W = (float)cuisinePanel.Width / 756f;
            float ratioZC_H = (float)cuisinePanel.Height / 564f;
            //Console.WriteLine($"{ratioZC_W} ... {ratioZC_H} + ************");

            // Redimentionner zoneCuisine et mettre à jour sa taille maximale
            zoneCuisine.MaximumSize = new Size(  // Ici on ajuste la taille max pour la zoneCuisine
                cuisinePanel.Width - 2* (int)(zoneCuisineBaseLocation.X * ratioZC_W), 
                cuisinePanel.Height - 2* (int)(zoneCuisineBaseLocation.Y * ratioZC_H)
            );
            zoneCuisine.Size = new Size(
                (int)(zoneCuisineBaseSize.Width * ratioZC_W),
                (int)(zoneCuisineBaseSize.Height * ratioZC_H)
            );
            zoneCuisine.Location = new Point((int)(zoneCuisineBaseLocation.X*ratioZC_W), (int)(zoneCuisineBaseLocation.Y*ratioZC_H));
            // Mettre à jour la position du gripItem (carré pour redimentionner la cuisine)
            gripItem.Location = new Point(zoneCuisine.Width - gripItem.Width, zoneCuisine.Height - gripItem.Height);
            // Redimentionner les boutons d'ajout de meubles
            ResizeFurnitureButtons();
            // Redimentionner les meubles selon la taille de la fenetre
            ResizeFurniture(ratioZC_W,ratioZC_H);
        }
        // Méthode pour réagir au redimentionnement de la zone de cuisine
        private void zoneCuisine_Resize(object sender, EventArgs e)
        {
            foreach (Control ctrl in cuisinePanel.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("btn_"))
                {
                    // Vérifier si les meubles sont toujours dans la cuisine
                    checkMeubleDansCUisine(btn);
                }
            }

        }

        // Méthode pour la barre de recherche
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            // Récupérer le texte de la barre de recherche et le convertir en minuscules
            string searchText = searchMeubles.Text.ToLower();
            int yPosition = 10;
            // Filtrer les boutons en fonction du texte de recherche
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
            // Mettre à jour la taille minimale de défilement
            meublesPanel.AutoScrollMinSize = new Size(0, yPosition);
        }

        // Charger les images
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

        // Méthode pour créer les boutons d'ajout de Meubles
        private void CreateAddFurnitureButtons()
        {
            meublesPanel.AutoScroll = true;
            meublesPanel.VerticalScroll.Enabled = true;
            meublesPanel.VerticalScroll.Visible = true;
            int yPosition = 10;
            int xPosition = meublesPanel.Width / 20;
            int buttonHeight = (meublesPanel.Height - 40) / 10;
            int buttonWidth = meublesPanel.Width - (2*xPosition + 15);

            foreach (var meuble in meubles.OrderBy(m => m.Key))
            {
                if (meubleImages.TryGetValue(meuble.Key, out var image))
                {
                    // Créer le bouton
                    Button addButton = new Button
                    {
                        Name = $"add_Button_{meuble.Key}",
                        Text = " " + meuble.Key,
                        Location = new Point(xPosition, yPosition),
                        Size = new Size(buttonWidth, buttonHeight),
                        Tag = meuble.Key,
                        Font = new Font("Arial", 8),
                        Image = new Bitmap(image, new Size(buttonHeight, buttonHeight)),
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ImageAlign = ContentAlignment.TopLeft
                    };
                    // Ajouter l'événement de clic
                    addButton.Click += AddFurnitureButton_Click;
                    // Ajouter le bouton au panel
                    meublesPanel.Controls.Add(addButton);
                    yPosition += addButton.Height + addButton.Height/5;
                }
            }

            // Mettre à jour la taille minimale de défilement
            meublesPanel.AutoScrollMinSize = new Size(0, yPosition);
        }

        // Méthode pour redimentionner les boutons d'ajout de meubles
        private void ResizeFurnitureButtons()
        {
            int yPosition = 10;
            int xPosition = meublesPanel.Width/20;
            int buttonHeight = (meublesPanel.Height - 40) / 10;
            int buttonWidth = meublesPanel.Width - (2*xPosition + 15);

            foreach (Control ctrl in meublesPanel.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("add_Button_"))
                {
                    btn.Size = new Size( buttonWidth, buttonHeight);
                    btn.Location = new Point(xPosition, yPosition);
                    // Ajuster taille police
                    float newFontSize = Math.Min(btn.Height * 0.175f, 11);
                    btn.Font = new Font(btn.Font.FontFamily, newFontSize, FontStyle.Regular);
                    // Ajuster la taille de l'image du bouton
                    btn.Image = ResizeImage(meubleImages[(string)btn.Tag], new Size(btn.Height, btn.Height));
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btn.TextAlign = ContentAlignment.MiddleCenter;
                    btn.ImageAlign = ContentAlignment.TopLeft;
                    yPosition += btn.Height + btn.Height/5;
                }
            }
        }

        // Méthode pour redimentionner les meubles quand la fenetre s'agrandit
        private void ResizeFurniture(float ratio_W, float ratio_H)
        {

            foreach (Control ctrl in cuisinePanel.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("btn_"))
                {
                    Size originalSize = meubles[(string)btn.Tag].Size;
                    Point previousPosition = meublesPosition[ctrl.Name];
                    // Redimentionner la taille et la position du meuble
                    btn.Size = new Size((int)(originalSize.Width*ratio_W), (int)(originalSize.Height*ratio_H));
                    btn.Location = new Point((int)(previousPosition.X*ratio_W), (int)(previousPosition.Y*ratio_H));
                    // Ajuster la taille de l'image du bouton
                    btn.Image = ResizeImage(meubleImages[(string)btn.Tag], new Size(btn.Width, btn.Height));
                }
            }
        }

        // Méthode pour redimentionner nos images proprement
        public static Bitmap ResizeImage(Image image, Size newSize)
        {
            var resized = new Bitmap(newSize.Width, newSize.Height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0, newSize.Width, newSize.Height);
            }
            return resized;
        }

        // Méthode pour créer un meuble quand on clique sur un bouton
        private void AddFurnitureButton_Click(object sender, EventArgs e)
        {
            Button addButton = (Button)sender;
            string meubleType = (string)addButton.Tag;
            float ratioZC_W = (float)cuisinePanel.Width / 756f;
            float ratioZC_H = (float)cuisinePanel.Height / 564f;
            createMeuble(meubleType,ratioZC_W,ratioZC_H);
        }

        // Méthode pour générer le meuble
        private void createMeuble(string meubleType, float ratio_W, float ratio_H)
        {
            if (meubles.TryGetValue(meubleType, out var properties) &&
                meubleImages.TryGetValue(meubleType, out var image))
            {
                Size b_size = new Size((int)(properties.Size.Width * ratio_W), (int)(properties.Size.Height * ratio_H));
                Point b_position = new Point((int)(properties.Location.X*ratio_W), (int)(properties.Location.Y*ratio_H));
                Image resizedImage = new Bitmap(image, b_size);
                Button newButton = new Button
                {
                    Name = "btn_" + meubleType + "_" + (buttonCounter++),
                    Text = "",
                    Size = b_size,
                    Location = b_position,
                    FlatStyle = FlatStyle.Flat,
                    Tag = meubleType,
                    Image = resizedImage,
                    BackgroundImageLayout = ImageLayout.Stretch,         
                    BackColor = Color.Transparent
                };
                meublesPosition[newButton.Name] = newButton.Location;
                Console.WriteLine(meublesPosition[newButton.Name]);

                SetupButton(newButton);
                this.cuisinePanel.Controls.Add(newButton);
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

        // Méthode pour déplacer le meuble
        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedButton != null)
            {
                // Obtenir la position de la souris par rapport à cuisinePanel
                Point mousePosInZone = cuisinePanel.PointToClient(Control.MousePosition);
                Point newLocation = new Point(mousePosInZone.X - offset.X, mousePosInZone.Y - offset.Y);
                draggedButton.Location = newLocation;
                //meubles[(string)draggedButton.Tag].Location = newLocation;
                meublesPosition[draggedButton.Name] = newLocation;

                // Vérifier si le bouton est entièrement dans cuisinePanel
                checkMeubleDansCUisine(draggedButton);
            }
        }

        // Méthode pour vérifier si un meuble est dans la cuisine
        private void checkMeubleDansCUisine (Control meuble)
        {
            if (objetEstDansContainer(zoneCuisine, meuble))
            {
                meuble.ForeColor = Color.Black; // le contour devient noir
            }
            else
            {
                meuble.ForeColor = Color.Red; // le contour devient rouge
            }

        }

        // Méthode pour vérifier si un objet est dans un autre (comme un meuble dans une zone)
        private bool objetEstDansContainer(Control container, Control objet)
        {
            if (objet == null) return false;
            Rectangle containerRect = container.RectangleToScreen(container.ClientRectangle);
            Rectangle controlRect = objet.RectangleToScreen(objet.ClientRectangle);
            return containerRect.Contains(controlRect);
        }

        // Méthode pour la rotation
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

        // Méthode pour dupliquer le meuble
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
                this.cuisinePanel.Controls.Add(newButton);
                newButton.BringToFront();
            }
        }

        // Méthode pour supprimer le meuble
        private void DeleteButton(Button btn)
        {
            DialogResult result = MessageBox.Show("Supprimer ce meuble ?", "Confirmation",
                MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Controls.Remove(btn);
                btn.Dispose();
            }
        }


        // Bouton pour quitter l'application
        private void quitter_Click(object sender, EventArgs e)
        {
            // Quitter l'application
            Application.Exit();
        }

        // Commentaire .... ?
        private void button1_Click(object sender, EventArgs e)
        {
            meublesPanel.Visible = !meublesPanel.Visible;
        }

        // Méthodes vides nécessaires
        private void button2_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
    }
    // Classe pour représenter un meuble
    public class Meuble
    {
        public string Text { get; set; }
        public Size Size { get; set; }
        public Point Location { get; set; }
        public string ImageName { get; set; }
        public Meuble(string text, Size size, Point location, string imageName) //  Constructeur
        {
            Text = text; // Texte du meuble
            Size = size; // Taille du meuble
            Location = location; // Position du meuble
            ImageName = imageName; // Nom de l'image du meuble
        }
    }
}