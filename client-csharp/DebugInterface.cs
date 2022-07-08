#region Usings

using System.IO;
using AiCup22.Codegame;
using AiCup22.Debugging;
using AiCup22.Model;

#endregion

namespace AiCup22
{
    public class DebugInterface
    {
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;

        public DebugInterface(BinaryReader reader, BinaryWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public void AddPlacedText(Vec2 position, string text, Vec2 alignment, double size, Color color)
        {
            Add(new DebugData.PlacedText(position, text, alignment, size, color));
        }

        public void AddCircle(Vec2 position, double radius, Color color)
        {
            Add(new DebugData.Circle(position, radius, color));
        }

        public void AddGradientCircle(Vec2 position, double radius, Color innerColor, Color outerColor)
        {
            Add(new DebugData.GradientCircle(position, radius, innerColor, outerColor));
        }

        public void AddRing(Vec2 position, double radius, double width, Color color)
        {
            Add(new DebugData.Ring(position, radius, width, color));
        }

        public void AddPie(Vec2 position, double radius, double startAngle, double endAngle, Color color)
        {
            Add(new DebugData.Pie(position, radius, startAngle, endAngle, color));
        }

        public void AddArc(Vec2 position, double radius, double width, double startAngle, double endAngle, Color color)
        {
            Add(new DebugData.Arc(position, radius, width, startAngle, endAngle, color));
        }

        public void AddRect(Vec2 bottomLeft, Vec2 size, Color color)
        {
            Add(new DebugData.Rect(bottomLeft, size, color));
        }

        public void AddPolygon(Vec2[] vertices, Color color)
        {
            Add(new DebugData.Polygon(vertices, color));
        }

        public void AddGradientPolygon(ColoredVertex[] vertices)
        {
            Add(new DebugData.GradientPolygon(vertices));
        }

        public void AddSegment(Vec2 firstEnd, Vec2 secondEnd, double width, Color color)
        {
            Add(new DebugData.Segment(firstEnd, secondEnd, width, color));
        }

        public void AddGradientSegment(Vec2 firstEnd, Color firstColor, Vec2 secondEnd, Color secondColor, double width)
        {
            Add(new DebugData.GradientSegment(firstEnd, firstColor, secondEnd, secondColor, width));
        }

        public void AddPolyLine(Vec2[] vertices, double width, Color color)
        {
            Add(new DebugData.PolyLine(vertices, width, color));
        }

        public void AddGradientPolyLine(ColoredVertex[] vertices, double width)
        {
            Add(new DebugData.GradientPolyLine(vertices, width));
        }

        public void Add(DebugData debugData)
        {
            Send(new DebugCommand.Add(debugData));
        }

        public void Clear()
        {
            Send(new DebugCommand.Clear());
        }

        public void SetAutoFlush(bool enable)
        {
            Send(new DebugCommand.SetAutoFlush(enable));
        }

        public void Flush()
        {
            Send(new DebugCommand.Flush());
        }

        public void Send(DebugCommand command)
        {
            new ClientMessage.DebugMessage(command).WriteTo(writer);
            writer.Flush();
        }

        public DebugState GetState()
        {
            new ClientMessage.RequestDebugState().WriteTo(writer);
            writer.Flush();
            return DebugState.ReadFrom(reader);
        }
    }
}