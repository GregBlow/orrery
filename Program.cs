using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;


namespace orrery
{
	public class MainForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private Random rand = new Random ();
		private Perspective p;
		private Sphere testSphere;
		private Sphere testSphere2;
		private Sphere testSphere3;
		private bool aDown = false;
		private bool dDown = false;

		private int counter;

		public MainForm()
		{
			InitializeComponent();
			p = new Perspective(this); 
			testSphere = new Sphere (new Point3(100, 100, 0), 100, Color.Yellow);
			testSphere2 = new Sphere (new Point3 (250, 250, 0), 50, Color.Blue);
			testSphere3 = new Sphere (new Point3 (250, 250, 0), 10, Color.Wheat);
		}

		private void InitializeComponent()
		{
			this.DoubleBuffered = true;
			this.components = new System.ComponentModel.Container ();
			this.timer = new System.Windows.Forms.Timer (this.components);
			this.timer.Enabled = true;
			this.timer.Interval = 25;
			this.timer.Tick += new System.EventHandler (this.timer_Tick);

			this.MouseDown += new System.Windows.Forms.MouseEventHandler (this.Form_MouseDown);

			this.AutoScaleBaseSize = new System.Drawing.Size (6, 15);
			this.ClientSize = new System.Drawing.Size(800,600);
			this.Name = "MainForm";
			this.Text = "Orrery";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler (this.Form_KeyDown);
			//this.Load += new System.EventHandler(this.MainForm_Load);
		}




		static void Main(){
			Application.Run(new MainForm());
		}

		private System.Windows.Forms.Timer timer;



		private void MainForm_Load(object sender, System.EventArgs e)
		{
			this.BackColor = Color.FromArgb (255, 0, 0, 0);

		}

		private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.E) {
				p.Rotate(Direction.Right);
			}
			if (e.KeyCode == Keys.Q) {
				p.Rotate(Direction.Left);
			}
			if (e.KeyCode == Keys.W) {
				p.Translate (Direction.Up);
			}
			if (e.KeyCode == Keys.A) {
				p.Translate (Direction.Left);
			}
			if (e.KeyCode == Keys.S) {
				p.Translate (Direction.Down);
			}
			if (e.KeyCode == Keys.D) {
				p.Translate (Direction.Right);
			}

			if (e.KeyCode == Keys.Z) {
				p.Rotate (Direction.Up);
			}
			if (e.KeyCode == Keys.X) {
				p.Rotate (Direction.Down);
			}
		}



		private void Form_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Console.Write (String.Concat(String.Concat(String.Concat(e.X, ", "), e.Y),"\n"));
			this.timer.Enabled = !this.timer.Enabled;
		}



		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillRectangle (Brushes.Red, 0, 0, 20, 20);
			UpdateGraphics (g);
			base.OnPaint (e);
		}

		private void timer_Tick(object sender, System.EventArgs e)
		{
			testSphere2.Centre = new Point3(testSphere.Centre.X + Math.Sin((double)counter/100)*300, testSphere.Centre.Y + Math.Cos((double)counter/100)*300, testSphere.Centre.Z + Math.Cos((double)counter/100)*20) ;
			testSphere2.calculateSurface (24, 24);

			testSphere3.Centre = new Point3(testSphere2.Centre.X + Math.Sin((double)counter/25)*100, testSphere2.Centre.Y + Math.Cos((double)counter/25)*100, testSphere2.Centre.Z + Math.Cos((double)(counter+10)/25)*5) ;
			testSphere3.calculateSurface (12, 12);
			counter += 1;
			this.Invalidate ();
			return;
		}

		private void ToggleTimer()
		{
		}

		private void UpdateGraphics (Graphics g)
		{
			g.Clear(Color.FromArgb (255, 0, 0, 0));

			g.FillRectangle(new SolidBrush (Color.White), new Rectangle(this.ClientSize.Width/2-2, this.ClientSize.Height/2-2, 5, 5));
			testSphere.DrawSphere (g, p);
			testSphere2.DrawSphere (g, p);
			testSphere3.DrawSphere (g, p);
			g.DrawString (p.Rotation.X.ToString(), new Font("Courier", 12),new SolidBrush(Color.White), this.Width-50, 0);
			g.DrawString (p.Position.X.ToString(), new Font("Courier", 12),new SolidBrush(Color.White), this.Width-50, 12);
			g.DrawString (p.Position.Y.ToString(), new Font("Courier", 12),new SolidBrush(Color.White), this.Width-50, 24);
			g.DrawString (p.Rotation.Y.ToString(), new Font("Courier", 12),new SolidBrush(Color.White), this.Width-50, 36);
		}
	}
}
