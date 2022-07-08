#region Usings

using System;
using System.IO;
using System.Text;
using AiCup22.Model;

#endregion

namespace AiCup22.Debugging
{
    /// <summary>
    /// Data for debug rendering
    /// </summary>
    public abstract class DebugData
    {
        /// <summary> Write DebugData to writer </summary>
        public abstract void WriteTo(BinaryWriter writer);

        /// <summary> Read DebugData from reader </summary>
        public static DebugData ReadFrom(BinaryReader reader)
        {
            switch (reader.ReadInt32())
            {
                case PlacedText.TAG:
                    return PlacedText.ReadFrom(reader);
                case Circle.TAG:
                    return Circle.ReadFrom(reader);
                case GradientCircle.TAG:
                    return GradientCircle.ReadFrom(reader);
                case Ring.TAG:
                    return Ring.ReadFrom(reader);
                case Pie.TAG:
                    return Pie.ReadFrom(reader);
                case Arc.TAG:
                    return Arc.ReadFrom(reader);
                case Rect.TAG:
                    return Rect.ReadFrom(reader);
                case Polygon.TAG:
                    return Polygon.ReadFrom(reader);
                case GradientPolygon.TAG:
                    return GradientPolygon.ReadFrom(reader);
                case Segment.TAG:
                    return Segment.ReadFrom(reader);
                case GradientSegment.TAG:
                    return GradientSegment.ReadFrom(reader);
                case PolyLine.TAG:
                    return PolyLine.ReadFrom(reader);
                case GradientPolyLine.TAG:
                    return GradientPolyLine.ReadFrom(reader);
                default:
                    throw new Exception("Unexpected tag value");
            }
        }

        /// <summary>
        /// Text
        /// </summary>
        public class PlacedText : DebugData
        {
            public const int TAG = 0;

            /// <summary>
            /// Position
            /// </summary>
            public Vec2 Position { get; set; }

            /// <summary>
            /// Text
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Alignment, separate for x and y. From 0 to 1. 0.5 - center alignment
            /// </summary>
            public Vec2 Alignment { get; set; }

            /// <summary>
            /// Size
            /// </summary>
            public double Size { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public PlacedText()
            {
            }

            public PlacedText(Vec2 position, string text, Vec2 alignment, double size, Color color)
            {
                Position = position;
                Text = text;
                Alignment = alignment;
                Size = size;
                Color = color;
            }

            /// <summary> Read PlacedText from reader </summary>
            public static new PlacedText ReadFrom(BinaryReader reader)
            {
                var result = new PlacedText();
                result.Position = Vec2.ReadFrom(reader);
                result.Text = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                result.Alignment = Vec2.ReadFrom(reader);
                result.Size = reader.ReadDouble();
                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write PlacedText to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Position.WriteTo(writer);
                var textData = Encoding.UTF8.GetBytes(Text);
                writer.Write(textData.Length);
                writer.Write(textData);
                Alignment.WriteTo(writer);
                writer.Write(Size);
                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of PlacedText </summary>
            public override string ToString()
            {
                string stringResult = "PlacedText { ";
                stringResult += "Position: ";
                stringResult += Position.ToString();
                stringResult += ", ";
                stringResult += "Text: ";
                stringResult += "\"" + Text + "\"";
                stringResult += ", ";
                stringResult += "Alignment: ";
                stringResult += Alignment.ToString();
                stringResult += ", ";
                stringResult += "Size: ";
                stringResult += Size.ToString();
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Circle
        /// </summary>
        public class Circle : DebugData
        {
            public const int TAG = 1;

            /// <summary>
            /// Position of the center
            /// </summary>
            public Vec2 Position { get; set; }

            /// <summary>
            /// Radius
            /// </summary>
            public double Radius { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public Circle()
            {
            }

            public Circle(Vec2 position, double radius, Color color)
            {
                Position = position;
                Radius = radius;
                Color = color;
            }

            /// <summary> Read Circle from reader </summary>
            public static new Circle ReadFrom(BinaryReader reader)
            {
                var result = new Circle();
                result.Position = Vec2.ReadFrom(reader);
                result.Radius = reader.ReadDouble();
                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write Circle to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Position.WriteTo(writer);
                writer.Write(Radius);
                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of Circle </summary>
            public override string ToString()
            {
                string stringResult = "Circle { ";
                stringResult += "Position: ";
                stringResult += Position.ToString();
                stringResult += ", ";
                stringResult += "Radius: ";
                stringResult += Radius.ToString();
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Circle with gradient fill
        /// </summary>
        public class GradientCircle : DebugData
        {
            public const int TAG = 2;

            /// <summary>
            /// Position of the center
            /// </summary>
            public Vec2 Position { get; set; }

            /// <summary>
            /// Radius
            /// </summary>
            public double Radius { get; set; }

            /// <summary>
            /// Color of the center
            /// </summary>
            public Color InnerColor { get; set; }

            /// <summary>
            /// Color of the edge
            /// </summary>
            public Color OuterColor { get; set; }

            public GradientCircle()
            {
            }

            public GradientCircle(Vec2 position, double radius, Color innerColor, Color outerColor)
            {
                Position = position;
                Radius = radius;
                InnerColor = innerColor;
                OuterColor = outerColor;
            }

            /// <summary> Read GradientCircle from reader </summary>
            public static new GradientCircle ReadFrom(BinaryReader reader)
            {
                var result = new GradientCircle();
                result.Position = Vec2.ReadFrom(reader);
                result.Radius = reader.ReadDouble();
                result.InnerColor = Color.ReadFrom(reader);
                result.OuterColor = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write GradientCircle to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Position.WriteTo(writer);
                writer.Write(Radius);
                InnerColor.WriteTo(writer);
                OuterColor.WriteTo(writer);
            }

            /// <summary> Get string representation of GradientCircle </summary>
            public override string ToString()
            {
                string stringResult = "GradientCircle { ";
                stringResult += "Position: ";
                stringResult += Position.ToString();
                stringResult += ", ";
                stringResult += "Radius: ";
                stringResult += Radius.ToString();
                stringResult += ", ";
                stringResult += "InnerColor: ";
                stringResult += InnerColor.ToString();
                stringResult += ", ";
                stringResult += "OuterColor: ";
                stringResult += OuterColor.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Ring
        /// </summary>
        public class Ring : DebugData
        {
            public const int TAG = 3;

            /// <summary>
            /// Position of the center
            /// </summary>
            public Vec2 Position { get; set; }

            /// <summary>
            /// Radius
            /// </summary>
            public double Radius { get; set; }

            /// <summary>
            /// Width
            /// </summary>
            public double Width { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public Ring()
            {
            }

            public Ring(Vec2 position, double radius, double width, Color color)
            {
                Position = position;
                Radius = radius;
                Width = width;
                Color = color;
            }

            /// <summary> Read Ring from reader </summary>
            public static new Ring ReadFrom(BinaryReader reader)
            {
                var result = new Ring();
                result.Position = Vec2.ReadFrom(reader);
                result.Radius = reader.ReadDouble();
                result.Width = reader.ReadDouble();
                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write Ring to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Position.WriteTo(writer);
                writer.Write(Radius);
                writer.Write(Width);
                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of Ring </summary>
            public override string ToString()
            {
                string stringResult = "Ring { ";
                stringResult += "Position: ";
                stringResult += Position.ToString();
                stringResult += ", ";
                stringResult += "Radius: ";
                stringResult += Radius.ToString();
                stringResult += ", ";
                stringResult += "Width: ";
                stringResult += Width.ToString();
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Sector of a circle
        /// </summary>
        public class Pie : DebugData
        {
            public const int TAG = 4;

            /// <summary>
            /// Position of the center
            /// </summary>
            public Vec2 Position { get; set; }

            /// <summary>
            /// Radius
            /// </summary>
            public double Radius { get; set; }

            /// <summary>
            /// Start angle
            /// </summary>
            public double StartAngle { get; set; }

            /// <summary>
            /// End angle
            /// </summary>
            public double EndAngle { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public Pie()
            {
            }

            public Pie(Vec2 position, double radius, double startAngle, double endAngle, Color color)
            {
                Position = position;
                Radius = radius;
                StartAngle = startAngle;
                EndAngle = endAngle;
                Color = color;
            }

            /// <summary> Read Pie from reader </summary>
            public static new Pie ReadFrom(BinaryReader reader)
            {
                var result = new Pie();
                result.Position = Vec2.ReadFrom(reader);
                result.Radius = reader.ReadDouble();
                result.StartAngle = reader.ReadDouble();
                result.EndAngle = reader.ReadDouble();
                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write Pie to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Position.WriteTo(writer);
                writer.Write(Radius);
                writer.Write(StartAngle);
                writer.Write(EndAngle);
                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of Pie </summary>
            public override string ToString()
            {
                string stringResult = "Pie { ";
                stringResult += "Position: ";
                stringResult += Position.ToString();
                stringResult += ", ";
                stringResult += "Radius: ";
                stringResult += Radius.ToString();
                stringResult += ", ";
                stringResult += "StartAngle: ";
                stringResult += StartAngle.ToString();
                stringResult += ", ";
                stringResult += "EndAngle: ";
                stringResult += EndAngle.ToString();
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Arc
        /// </summary>
        public class Arc : DebugData
        {
            public const int TAG = 5;

            /// <summary>
            /// Position of the center
            /// </summary>
            public Vec2 Position { get; set; }

            /// <summary>
            /// Radius
            /// </summary>
            public double Radius { get; set; }

            /// <summary>
            /// Width
            /// </summary>
            public double Width { get; set; }

            /// <summary>
            /// Start angle
            /// </summary>
            public double StartAngle { get; set; }

            /// <summary>
            /// End angle
            /// </summary>
            public double EndAngle { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public Arc()
            {
            }

            public Arc(Vec2 position, double radius, double width, double startAngle, double endAngle, Color color)
            {
                Position = position;
                Radius = radius;
                Width = width;
                StartAngle = startAngle;
                EndAngle = endAngle;
                Color = color;
            }

            /// <summary> Read Arc from reader </summary>
            public static new Arc ReadFrom(BinaryReader reader)
            {
                var result = new Arc();
                result.Position = Vec2.ReadFrom(reader);
                result.Radius = reader.ReadDouble();
                result.Width = reader.ReadDouble();
                result.StartAngle = reader.ReadDouble();
                result.EndAngle = reader.ReadDouble();
                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write Arc to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Position.WriteTo(writer);
                writer.Write(Radius);
                writer.Write(Width);
                writer.Write(StartAngle);
                writer.Write(EndAngle);
                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of Arc </summary>
            public override string ToString()
            {
                string stringResult = "Arc { ";
                stringResult += "Position: ";
                stringResult += Position.ToString();
                stringResult += ", ";
                stringResult += "Radius: ";
                stringResult += Radius.ToString();
                stringResult += ", ";
                stringResult += "Width: ";
                stringResult += Width.ToString();
                stringResult += ", ";
                stringResult += "StartAngle: ";
                stringResult += StartAngle.ToString();
                stringResult += ", ";
                stringResult += "EndAngle: ";
                stringResult += EndAngle.ToString();
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Rectancle
        /// </summary>
        public class Rect : DebugData
        {
            public const int TAG = 6;

            /// <summary>
            /// Bottom left position
            /// </summary>
            public Vec2 BottomLeft { get; set; }

            /// <summary>
            /// Size
            /// </summary>
            public Vec2 Size { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public Rect()
            {
            }

            public Rect(Vec2 bottomLeft, Vec2 size, Color color)
            {
                BottomLeft = bottomLeft;
                Size = size;
                Color = color;
            }

            /// <summary> Read Rect from reader </summary>
            public static new Rect ReadFrom(BinaryReader reader)
            {
                var result = new Rect();
                result.BottomLeft = Vec2.ReadFrom(reader);
                result.Size = Vec2.ReadFrom(reader);
                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write Rect to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                BottomLeft.WriteTo(writer);
                Size.WriteTo(writer);
                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of Rect </summary>
            public override string ToString()
            {
                string stringResult = "Rect { ";
                stringResult += "BottomLeft: ";
                stringResult += BottomLeft.ToString();
                stringResult += ", ";
                stringResult += "Size: ";
                stringResult += Size.ToString();
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Polygon (convex)
        /// </summary>
        public class Polygon : DebugData
        {
            public const int TAG = 7;

            /// <summary>
            /// Positions of vertices in order
            /// </summary>
            public Vec2[] Vertices { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public Polygon()
            {
            }

            public Polygon(Vec2[] vertices, Color color)
            {
                Vertices = vertices;
                Color = color;
            }

            /// <summary> Read Polygon from reader </summary>
            public static new Polygon ReadFrom(BinaryReader reader)
            {
                var result = new Polygon();
                result.Vertices = new Vec2[reader.ReadInt32()];
                for (int verticesIndex = 0; verticesIndex < result.Vertices.Length; verticesIndex++)
                {
                    result.Vertices[verticesIndex] = Vec2.ReadFrom(reader);
                }

                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write Polygon to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                writer.Write(Vertices.Length);
                foreach (var verticesElement in Vertices)
                {
                    verticesElement.WriteTo(writer);
                }

                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of Polygon </summary>
            public override string ToString()
            {
                string stringResult = "Polygon { ";
                stringResult += "Vertices: ";
                stringResult += "[ ";
                int verticesIndex = 0;
                foreach (var verticesElement in Vertices)
                {
                    if (verticesIndex != 0)
                    {
                        stringResult += ", ";
                    }

                    stringResult += verticesElement.ToString();
                    verticesIndex++;
                }

                stringResult += " ]";
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Polygon with gradient fill
        /// </summary>
        public class GradientPolygon : DebugData
        {
            public const int TAG = 8;

            /// <summary>
            /// List of vertices in order
            /// </summary>
            public ColoredVertex[] Vertices { get; set; }

            public GradientPolygon()
            {
            }

            public GradientPolygon(ColoredVertex[] vertices)
            {
                Vertices = vertices;
            }

            /// <summary> Read GradientPolygon from reader </summary>
            public static new GradientPolygon ReadFrom(BinaryReader reader)
            {
                var result = new GradientPolygon();
                result.Vertices = new ColoredVertex[reader.ReadInt32()];
                for (int verticesIndex = 0; verticesIndex < result.Vertices.Length; verticesIndex++)
                {
                    result.Vertices[verticesIndex] = ColoredVertex.ReadFrom(reader);
                }

                return result;
            }

            /// <summary> Write GradientPolygon to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                writer.Write(Vertices.Length);
                foreach (var verticesElement in Vertices)
                {
                    verticesElement.WriteTo(writer);
                }
            }

            /// <summary> Get string representation of GradientPolygon </summary>
            public override string ToString()
            {
                string stringResult = "GradientPolygon { ";
                stringResult += "Vertices: ";
                stringResult += "[ ";
                int verticesIndex = 0;
                foreach (var verticesElement in Vertices)
                {
                    if (verticesIndex != 0)
                    {
                        stringResult += ", ";
                    }

                    stringResult += verticesElement.ToString();
                    verticesIndex++;
                }

                stringResult += " ]";
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Segment
        /// </summary>
        public class Segment : DebugData
        {
            public const int TAG = 9;

            /// <summary>
            /// Position of the first end
            /// </summary>
            public Vec2 FirstEnd { get; set; }

            /// <summary>
            /// Position of the second end
            /// </summary>
            public Vec2 SecondEnd { get; set; }

            /// <summary>
            /// Width
            /// </summary>
            public double Width { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public Segment()
            {
            }

            public Segment(Vec2 firstEnd, Vec2 secondEnd, double width, Color color)
            {
                FirstEnd = firstEnd;
                SecondEnd = secondEnd;
                Width = width;
                Color = color;
            }

            /// <summary> Read Segment from reader </summary>
            public static new Segment ReadFrom(BinaryReader reader)
            {
                var result = new Segment();
                result.FirstEnd = Vec2.ReadFrom(reader);
                result.SecondEnd = Vec2.ReadFrom(reader);
                result.Width = reader.ReadDouble();
                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write Segment to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                FirstEnd.WriteTo(writer);
                SecondEnd.WriteTo(writer);
                writer.Write(Width);
                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of Segment </summary>
            public override string ToString()
            {
                string stringResult = "Segment { ";
                stringResult += "FirstEnd: ";
                stringResult += FirstEnd.ToString();
                stringResult += ", ";
                stringResult += "SecondEnd: ";
                stringResult += SecondEnd.ToString();
                stringResult += ", ";
                stringResult += "Width: ";
                stringResult += Width.ToString();
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Segment with gradient fill
        /// </summary>
        public class GradientSegment : DebugData
        {
            public const int TAG = 10;

            /// <summary>
            /// Position of the first end
            /// </summary>
            public Vec2 FirstEnd { get; set; }

            /// <summary>
            /// Color of the first end
            /// </summary>
            public Color FirstColor { get; set; }

            /// <summary>
            /// Position of the second end
            /// </summary>
            public Vec2 SecondEnd { get; set; }

            /// <summary>
            /// Color of the second end
            /// </summary>
            public Color SecondColor { get; set; }

            /// <summary>
            /// Width
            /// </summary>
            public double Width { get; set; }

            public GradientSegment()
            {
            }

            public GradientSegment(Vec2 firstEnd, Color firstColor, Vec2 secondEnd, Color secondColor, double width)
            {
                FirstEnd = firstEnd;
                FirstColor = firstColor;
                SecondEnd = secondEnd;
                SecondColor = secondColor;
                Width = width;
            }

            /// <summary> Read GradientSegment from reader </summary>
            public static new GradientSegment ReadFrom(BinaryReader reader)
            {
                var result = new GradientSegment();
                result.FirstEnd = Vec2.ReadFrom(reader);
                result.FirstColor = Color.ReadFrom(reader);
                result.SecondEnd = Vec2.ReadFrom(reader);
                result.SecondColor = Color.ReadFrom(reader);
                result.Width = reader.ReadDouble();
                return result;
            }

            /// <summary> Write GradientSegment to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                FirstEnd.WriteTo(writer);
                FirstColor.WriteTo(writer);
                SecondEnd.WriteTo(writer);
                SecondColor.WriteTo(writer);
                writer.Write(Width);
            }

            /// <summary> Get string representation of GradientSegment </summary>
            public override string ToString()
            {
                string stringResult = "GradientSegment { ";
                stringResult += "FirstEnd: ";
                stringResult += FirstEnd.ToString();
                stringResult += ", ";
                stringResult += "FirstColor: ";
                stringResult += FirstColor.ToString();
                stringResult += ", ";
                stringResult += "SecondEnd: ";
                stringResult += SecondEnd.ToString();
                stringResult += ", ";
                stringResult += "SecondColor: ";
                stringResult += SecondColor.ToString();
                stringResult += ", ";
                stringResult += "Width: ";
                stringResult += Width.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Poly line
        /// </summary>
        public class PolyLine : DebugData
        {
            public const int TAG = 11;

            /// <summary>
            /// List of points in order
            /// </summary>
            public Vec2[] Vertices { get; set; }

            /// <summary>
            /// Width
            /// </summary>
            public double Width { get; set; }

            /// <summary>
            /// Color
            /// </summary>
            public Color Color { get; set; }

            public PolyLine()
            {
            }

            public PolyLine(Vec2[] vertices, double width, Color color)
            {
                Vertices = vertices;
                Width = width;
                Color = color;
            }

            /// <summary> Read PolyLine from reader </summary>
            public static new PolyLine ReadFrom(BinaryReader reader)
            {
                var result = new PolyLine();
                result.Vertices = new Vec2[reader.ReadInt32()];
                for (int verticesIndex = 0; verticesIndex < result.Vertices.Length; verticesIndex++)
                {
                    result.Vertices[verticesIndex] = Vec2.ReadFrom(reader);
                }

                result.Width = reader.ReadDouble();
                result.Color = Color.ReadFrom(reader);
                return result;
            }

            /// <summary> Write PolyLine to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                writer.Write(Vertices.Length);
                foreach (var verticesElement in Vertices)
                {
                    verticesElement.WriteTo(writer);
                }

                writer.Write(Width);
                Color.WriteTo(writer);
            }

            /// <summary> Get string representation of PolyLine </summary>
            public override string ToString()
            {
                string stringResult = "PolyLine { ";
                stringResult += "Vertices: ";
                stringResult += "[ ";
                int verticesIndex = 0;
                foreach (var verticesElement in Vertices)
                {
                    if (verticesIndex != 0)
                    {
                        stringResult += ", ";
                    }

                    stringResult += verticesElement.ToString();
                    verticesIndex++;
                }

                stringResult += " ]";
                stringResult += ", ";
                stringResult += "Width: ";
                stringResult += Width.ToString();
                stringResult += ", ";
                stringResult += "Color: ";
                stringResult += Color.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Poly line with gradient fill
        /// </summary>
        public class GradientPolyLine : DebugData
        {
            public const int TAG = 12;

            /// <summary>
            /// List of points and colors in order
            /// </summary>
            public ColoredVertex[] Vertices { get; set; }

            /// <summary>
            /// Width
            /// </summary>
            public double Width { get; set; }

            public GradientPolyLine()
            {
            }

            public GradientPolyLine(ColoredVertex[] vertices, double width)
            {
                Vertices = vertices;
                Width = width;
            }

            /// <summary> Read GradientPolyLine from reader </summary>
            public static new GradientPolyLine ReadFrom(BinaryReader reader)
            {
                var result = new GradientPolyLine();
                result.Vertices = new ColoredVertex[reader.ReadInt32()];
                for (int verticesIndex = 0; verticesIndex < result.Vertices.Length; verticesIndex++)
                {
                    result.Vertices[verticesIndex] = ColoredVertex.ReadFrom(reader);
                }

                result.Width = reader.ReadDouble();
                return result;
            }

            /// <summary> Write GradientPolyLine to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                writer.Write(Vertices.Length);
                foreach (var verticesElement in Vertices)
                {
                    verticesElement.WriteTo(writer);
                }

                writer.Write(Width);
            }

            /// <summary> Get string representation of GradientPolyLine </summary>
            public override string ToString()
            {
                string stringResult = "GradientPolyLine { ";
                stringResult += "Vertices: ";
                stringResult += "[ ";
                int verticesIndex = 0;
                foreach (var verticesElement in Vertices)
                {
                    if (verticesIndex != 0)
                    {
                        stringResult += ", ";
                    }

                    stringResult += verticesElement.ToString();
                    verticesIndex++;
                }

                stringResult += " ]";
                stringResult += ", ";
                stringResult += "Width: ";
                stringResult += Width.ToString();
                stringResult += " }";
                return stringResult;
            }
        }
    }
}