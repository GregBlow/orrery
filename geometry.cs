using System;
using System.Collections.Generic;
using System.Drawing;

namespace orrery
{
	public class Polyhedron
	{
		protected List<Vertex> surfacePoints;
		protected Color basecolour;
		public void DrawPolyhedron(Graphics g, Perspective p)
		{
			List<Vertex> drawnPoints = new List<Vertex>();
			foreach (Vertex element in this.surfacePoints) 
			{
				if (element != null && !drawnPoints.Contains(element)) {
					Pen pen = new Pen(Brushes.Red);
					if (basecolour != null) {
						pen = new Pen(new SolidBrush(basecolour));
					}

					//element.DrawPoint (g, new SolidBrush (Color.Yellow), p);
					if (element.Con_Vert.Count != 0) {
						foreach (Vertex connectedPoint in element.Con_Vert) {
							g.DrawLine (pen, connectedPoint.IsometricTransform (p), element.IsometricTransform (p));
						}
					}
					drawnPoints.Add (element);

				}
			}
		}
	}

	public class Sphere: Polyhedron
	{
		private double radius;
		private Point3 centre;
		private Point3 rotation;

		public Sphere ()
		{
			this.rotation = new Point3 ();
			this.centre = new Point3 (0, 0,0);
			this.radius = 1;
			this.calculateSurface (24, 24);
		}
		public Sphere (Point3 centre_point, double sphere_radius)
		{
			this.rotation = new Point3 ();
			this.centre = centre_point;
			this.radius = sphere_radius;
			this.calculateSurface (24, 24);
		}
		public Sphere (Point3 centre_point, double sphere_radius, Color spherecolour)
		{
			this.rotation = new Point3 ();
			this.centre = centre_point;
			this.radius = sphere_radius;
			this.calculateSurface (24, 24);
			this.basecolour = spherecolour;
		}
		public double Radius
		{
			get{return this.radius;}
			set{this.radius = value;}
		}

		public Point3 Centre
		{
			get{return this.centre;}
			set{this.centre = value;

			}
		}
		public void calculateSurface(int latitudinalDetail, int longitudinalDetail)
		{
			this.surfacePoints = new List<Vertex>(2 + latitudinalDetail*longitudinalDetail);



			surfacePoints.Add(new Vertex (this.Centre.X, this.Centre.Y, this.Centre.Z + this.Radius));

			for (int index = 0; index < latitudinalDetail; index++) {
				double latAngle = Math.PI * ((double)index / latitudinalDetail);

				for (int i = 0; i < longitudinalDetail; i++) {
					double lonAngle = (Math.PI * 2) * ((double)i / longitudinalDetail);
					Vertex newPoint = new Vertex (this.Centre.X + (Radius * Math.Sin (lonAngle))*Math.Sin(latAngle), this.Centre.Y + (Radius * Math.Cos (lonAngle))*Math.Sin(latAngle), this.Centre.Z+ this.Radius*Math.Cos(latAngle));
					surfacePoints.Add(newPoint);
					if ((surfacePoints.IndexOf (newPoint)) % longitudinalDetail != 1) {
						surfacePoints [surfacePoints.IndexOf (newPoint)].ConnectVertex (surfacePoints [surfacePoints.IndexOf (newPoint) - 1]);
					} 
					if ((surfacePoints.IndexOf (newPoint)) % longitudinalDetail == 0 && surfacePoints.IndexOf (newPoint) > longitudinalDetail) {
						surfacePoints [surfacePoints.IndexOf (newPoint)].ConnectVertex (surfacePoints [surfacePoints.IndexOf (newPoint) + 1 - longitudinalDetail]);
					}
					if (surfacePoints.IndexOf (newPoint) - longitudinalDetail > 0) {
						surfacePoints [surfacePoints.IndexOf (newPoint)].ConnectVertex (surfacePoints [surfacePoints.IndexOf (newPoint) - longitudinalDetail]);
					}

				}
			}
			surfacePoints.Add(new Vertex (this.Centre.X, this.Centre.Y, this.Centre.Z - this.Radius));
			for (int i = 0; i < longitudinalDetail; i++)
			{
				surfacePoints[1 + (longitudinalDetail * latitudinalDetail)].ConnectVertex(surfacePoints[(longitudinalDetail * latitudinalDetail) - i]);
			}

		}

		public void DrawSphere(Graphics g, Perspective p)
		{
			DrawPolyhedron (g, p);

		}
	}

	public class Point3
	{
		protected double x;
		protected double y;
		protected double z;

		public Point3()
		{
			this.x = 0;
			this.y = 0;
			this.z = 0;
		}
		public Point3(double x_coord, double y_coord, double z_coord)
		{
			this.x = x_coord;
			this.y = y_coord;
			this.z = z_coord;
		}

		public double X
		{
			get{return this.x;}
			set{this.x = value;}
		}

		public double Y
		{
			get{return this.y;}
			set{this.y = value;}
		}

		public double Z
		{
			get{return this.z;}
			set{this.z = value;}
		}

		public Point IsometricTransform(Perspective perspective)
		{

			PointF origin = new PointF ((float)(perspective.Parent.ClientSize.Width/2), (float)(perspective.Parent.ClientSize.Height/2));
			double transX = this.X - perspective.Position.X;
			double transY = this.Y - perspective.Position.Y;

			double deltaX = transX - origin.X;
			double deltaY = transY - origin.Y;


			double rotX = deltaX * Math.Cos (perspective.Rotation.X) - deltaY * Math.Sin (perspective.Rotation.X) + origin.X;
			double rotY = deltaY * Math.Cos (perspective.Rotation.X) + deltaX * Math.Sin (perspective.Rotation.X) + origin.Y;

			deltaY = rotY - origin.Y;

			double newX = rotX;
			double newY = deltaY * Math.Cos (perspective.Rotation.Y) + origin.Y - Math.Sin(perspective.Rotation.Y)*(this.Z);
			//double newY = (rotY * Math.Cos (perspective.Rotation.Y)) + Math.Sin(perspective.Rotation.Y)*(this.Z) + Math.Sin(perspective.Rotation.Y) * origin.Y; 

			return new Point ((int)newX, (int)newY);
		}

		public void DrawPoint(Graphics g, Brush brush, Perspective p)
		{
			Point projectedPoint = this.IsometricTransform (p);
			g.FillRectangle (brush, new RectangleF (projectedPoint.X-2, projectedPoint.Y-2, 5, 5));
		}

		public String ToString()
		{
			return String.Concat(String.Concat(String.Concat(String.Concat(String.Concat(String.Concat("[", this.X), ", "), this.Y), ", "), this.Z), "]");
		}

	}

	public class Vertex: Point3
	{
		private List<Vertex> con_Vert;
		


		public Vertex()
		{
			con_Vert = new List<Vertex>();
			this.x = 0;
			this.y = 0;
			this.z = 0;
		}
		public Vertex(Point3 location)
		{
			con_Vert = new List<Vertex>();
			this.x = location.X;
			this.y = location.Y;
			this.z = location.Z;
		}
		public Vertex(double x_coordinate, double y_coordinate, double z_coordinate)
		{
			con_Vert = new List<Vertex>();
			this.x = x_coordinate;
			this.y = y_coordinate;
			this.z = z_coordinate;
		}
		public List<Vertex> Con_Vert
		{
			get{return con_Vert;}
			set{this.con_Vert = value;}
		}
		public void DrawVertex(Graphics g, Brush b, Perspective p)
		{
			DrawPoint (g, b, p);
			Pen pen = new Pen (new SolidBrush (Color.Red));
			foreach (Vertex element in con_Vert) {
				g.DrawLine (pen, this.IsometricTransform (p), element.IsometricTransform (p));
			}

		}
		public void ConnectVertex(Vertex vertex)
		{
			this.Con_Vert.Add (vertex);
			vertex.Con_Vert.Add (this);
		}
	}
		
	public class Perspective
	{
		private Point3 rotation;
		private Point3 position;
		private MainForm parent;

		public Perspective(MainForm m)
		{
			this.parent = m;
			this.rotation = new Point3();
			this.position = new Point3 ();
		}
		public Perspective(MainForm m, Point3 origin)
		{
			this.position = origin;
			this.rotation = new Point3 ();
		}

		public Perspective(MainForm m, Point3 origin, Point3 rotation)
		{
			this.position = origin;
			this.rotation = rotation;
		}

		public MainForm Parent{
			get{ return this.parent; }
		}

		public Point3 Position{
			get{return this.position;}
			set{ this.position = value; }
		}

		public Point3 Rotation{
			get{ return this.rotation; }
			set{ this.rotation = value; }
		}

		public void Rotate(Direction d){
			if (d == Direction.Up) {
				this.RotateUp();
			}
			if (d == Direction.Down) {
				this.RotateDown ();
			}
			if (d == Direction.Left) {
				this.RotateClockwise ();
			}
			if (d == Direction.Right) {
				this.RotateCounterclockwise();
			}
		}

		private void RotateDown()
		{
			this.rotation.Y += 0.02 * Math.PI;
			if (this.rotation.Y > Math.PI) {
				this.rotation.Y = Math.PI;
			}
		}
		private void RotateUp()
		{
			this.rotation.Y -= 0.02 * Math.PI;

			if (this.rotation.Y < 0)
			{
				this.rotation.Y = 0;
			}
		}

		private void RotateClockwise(){
			this.rotation.X += 0.02 * Math.PI;
			this.rotation.X %= 2 * Math.PI;
		}
		private void RotateCounterclockwise(){
			this.rotation.X -= 0.02 * Math.PI;
			this.rotation.X += 2 * Math.PI;
			this.rotation.X %= 2 * Math.PI;
		}
		public void Translate(Direction d)
		{
			double xFactor = 0;
			double yFactor = 0;

			if (d == Direction.Up) {
				yFactor = Math.Cos(Rotation.X) * -1;
				xFactor = Math.Sin (Rotation.X) * -1; 
			}
			if (d == Direction.Down) {
				yFactor = Math.Cos(Rotation.X);
				xFactor = Math.Sin(Rotation.X);
			}
			if (d == Direction.Left) {
				xFactor = Math.Cos(Rotation.X) * -1;
				yFactor = Math.Sin(Rotation.X);
			}
			if (d == Direction.Right) {
				xFactor = Math.Cos(Rotation.X);
				yFactor = Math.Sin(Rotation.X) * -1;
			}

			this.Position.X += xFactor;
			this.Position.Y += yFactor;

		}



		public String ToString()
		{
			return String.Concat(String.Concat(this.Position,", "), this.Rotation); 
		}

	}

	public enum Direction{Up, Down, Left, Right};
}

