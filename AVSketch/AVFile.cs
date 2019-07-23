using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVSketch.VectorModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows;

namespace AVSketch
{
    class AVFile
    {

        public AVFile()
        {

        }

        public static void Convert(Screen screen, string path)
        {
            //MemoryStream stream = new MemoryStream();
            //XmlSerializer serializer = new XmlSerializer(typeof(Screen));
            //serializer.Serialize(stream, screen);
            //StreamReader reader = new StreamReader(stream);
            XmlDocument doc = new XmlDocument();
            XmlElement xmlScreen = doc.CreateElement("screen");
            xmlScreen.SetAttribute("colour", screen.current_colour);
            xmlScreen.SetAttribute("translateX", screen.translateX.ToString());
            xmlScreen.SetAttribute("translateY", screen.translateY.ToString());
            xmlScreen.SetAttribute("outlined", screen.outlinedObject);
            xmlScreen.SetAttribute("stroke", screen.stroke_thickness.ToString());
            xmlScreen.SetAttribute("fontSize", screen.font_size.ToString());
            xmlScreen.SetAttribute("currentFillIn", screen.current_fill_in.ToString());
            foreach(KeyValuePair<string, VectorObject> pair in screen.objects)
            {
                XmlElement element = doc.CreateElement(pair.Value.GetType().ToString().Remove(0, 27).ToLowerInvariant());
                element.SetAttribute("uid", pair.Key);
                element.SetAttribute("position", pair.Value.position.x.ToString() + "," + pair.Value.position.y.ToString());
                element.SetAttribute("colour", pair.Value.colour);
                switch (element.Name)
                {
                    case "line":
                        element.SetAttribute("stroke", ((VectorLine)pair.Value).strokeThickness.ToString());
                        element.SetAttribute("minX", ((VectorLine)pair.Value).minX.ToString());
                        element.SetAttribute("minY", ((VectorLine)pair.Value).minY.ToString());
                        element.SetAttribute("maxX", ((VectorLine)pair.Value).maxX.ToString());
                        element.SetAttribute("maxY", ((VectorLine)pair.Value).maxY.ToString());
                        foreach(VectorPoint point in ((VectorLine)pair.Value).points)
                        {
                            XmlElement xmlPoint = doc.CreateElement("point");
                            xmlPoint.SetAttribute("position", point.x.ToString() + "," + point.y.ToString());
                            element.AppendChild(xmlPoint);
                        }
                        break;
                    case "box":
                        element.SetAttribute("size", ((VectorBox)pair.Value).size.x.ToString() + "," + ((VectorBox)pair.Value).size.y.ToString());
                        element.SetAttribute("fillin", ((VectorBox)pair.Value).fillin.ToString());
                        element.SetAttribute("stroke", ((VectorBox)pair.Value).strokeThickness.ToString());
                        break;
                    case "ellipse":
                        element.SetAttribute("radii", ((VectorEllipse)pair.Value).radii.x.ToString() + "," + ((VectorEllipse)pair.Value).radii.y.ToString());
                        element.SetAttribute("fillin", ((VectorEllipse)pair.Value).fillin.ToString());
                        element.SetAttribute("stroke", ((VectorEllipse)pair.Value).strokeThickness.ToString());
                        break;
                    case "text":
                        element.SetAttribute("text", ((VectorText)pair.Value).text);
                        element.SetAttribute("fontSize", ((VectorText)pair.Value).fontSize.ToString());
                        break;
                }
                xmlScreen.AppendChild(element);
            }
            doc.AppendChild(xmlScreen);
            doc.Save(path);
        }

        public static Screen Unconvert(string path)
        {
            XmlDocument doc = new XmlDocument();
            Screen screen = new Screen();
            doc.Load(path);
            if (doc.HasChildNodes)
            {
                XmlElement xml = (XmlElement)doc.ChildNodes[0];
                screen.current_colour = xml.GetAttributeNode("colour").InnerXml;
                screen.translateX = float.Parse(xml.GetAttributeNode("translateX").InnerXml);
                screen.translateY = float.Parse(xml.GetAttributeNode("translateY").InnerXml);
                screen.outlinedObject = xml.GetAttributeNode("outlined").InnerXml;
                screen.stroke_thickness = float.Parse(xml.GetAttributeNode("stroke").InnerXml);
                screen.font_size = float.Parse(xml.GetAttributeNode("fontSize").InnerXml);
                screen.current_fill_in = bool.Parse(xml.GetAttributeNode("currentFillIn").InnerXml);
                if (xml.HasChildNodes)
                { 
                    for (int i = 0; i < xml.ChildNodes.Count; i++)
                    {
                        XmlElement child = (XmlElement)xml.ChildNodes[i];
                        string[] xandy = child.GetAttributeNode("position").InnerXml.Split(',');
                        float x = float.Parse(xandy[0]);
                        float y = float.Parse(xandy[1]);
                        VectorPoint position = new VectorPoint(x, y);
                        string colour = child.GetAttributeNode("colour").InnerXml;
                        string uid = child.GetAttributeNode("uid").InnerXml;
                        switch (child.Name)
                        {
                            case "line":
                                VectorLine line = new VectorLine(position);
                                line.minX = float.Parse(child.GetAttributeNode("minX").InnerXml);
                                line.minY = float.Parse(child.GetAttributeNode("minY").InnerXml);
                                line.maxX = float.Parse(child.GetAttributeNode("maxX").InnerXml);
                                line.maxY = float.Parse(child.GetAttributeNode("maxY").InnerXml);
                                line.colour = colour;
                                line.strokeThickness = float.Parse(child.GetAttributeNode("stroke").InnerXml);

                                for (int u = 0; u < child.ChildNodes.Count; u++)
                                {
                                    XmlElement point = (XmlElement)child.ChildNodes[u];
                                    xandy = point.GetAttributeNode("position").InnerXml.Split(',');
                                    x = float.Parse(xandy[0]);
                                    y = float.Parse(xandy[1]);
                                    line.addPoint(new VectorPoint(x, y));
                                }
                                screen.addObject(uid, line);
                                break;
                            case "box":
                                xandy = child.GetAttributeNode("size").InnerXml.Split(',');
                                x = float.Parse(xandy[0]);
                                y = float.Parse(xandy[1]);
                                VectorPoint size = new VectorPoint(x, y);
                                bool fillin = bool.Parse(child.GetAttributeNode("fillin").InnerXml);
                                float stroke = float.Parse(child.GetAttributeNode("stroke").InnerXml);
                                VectorBox box = new VectorBox(position, size, fillin);
                                box.strokeThickness = stroke;
                                screen.addObject(uid, box);
                                break;
                            case "ellipse":
                                xandy = child.GetAttributeNode("radii").InnerXml.Split(',');
                                x = float.Parse(xandy[0]);
                                y = float.Parse(xandy[1]);
                                VectorPoint radii = new VectorPoint(x, y);
                                fillin = bool.Parse(child.GetAttributeNode("fillin").InnerXml);
                                stroke = float.Parse(child.GetAttributeNode("stroke").InnerXml);
                                VectorEllipse ellipse = new VectorEllipse(position, radii, fillin);
                                ellipse.strokeThickness = stroke;
                                screen.addObject(uid, ellipse);
                                break;
                            case "text":
                                string text = child.GetAttributeNode("text").InnerXml;
                                float fontsize = float.Parse(child.GetAttributeNode("fontSize").InnerXml);
                                VectorText obj = new VectorText(position, text);
                                obj.colour = colour;
                                obj.fontSize = fontsize;
                                screen.addObject(uid, obj);
                                break;
                        }
                    }
                }
            }
            return screen;
        }

        /*

        public string getOperand(string line)
        {
            return line.Split(' ')[1];
        }

        public Screen getArray()
        {
            Screen screen = new Screen();

            string textfile = "";
            string[] lines = textfile.Split('\n');

            bool inobjects = false;
            VectorObject obj;

            foreach(string line in lines)
            {
                if (line == "[HEADER]")
                {
                    inobjects = false;
                    continue;
                }
                else if (line == "[OBJECTS]")
                {
                    inobjects = true;
                    continue;
                }

                if (inobjects)
                {
                    if (line.StartsWith("newobj"))
                    {
                        switch (getOperand(line))
                        {
                            case "line":
                                obj = new VectorObject();
                                break;
                            case "box":
                                break;
                            case "ellipse":
                                break;
                            case "text":
                                break;
                        }
                    }
                }
                else
                {
                    if (line.StartsWith("colour"))
                    {
                        screen.current_colour = getOperand(line);
                    }
                    else if (line.StartsWith("trans_x"))
                    {
                        screen.translateX = float.Parse(getOperand(line));
                    }
                    else if (line.StartsWith("trans_y"))
                    {
                        screen.translateY = float.Parse(getOperand(line));
                    }
                    else if (line.StartsWith("stroke"))
                    {
                        screen.stroke_thickness = float.Parse(getOperand(line));
                    }
                    else if (line.StartsWith("font_size"))
                    {
                        screen.font_size = float.Parse(getOperand(line));
                    }
                }
            }

            return screen;
        }

        */
    }
}
