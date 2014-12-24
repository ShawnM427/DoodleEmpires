using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectToShader
{
    class EffectReader
    {
        public static EffectParameterCollection ReadEffect(BinaryReader reader)
        {
            MGFXHeader header = new MGFXHeader();
            header.Signature = reader.ReadInt32();
            header.Version = (int)reader.ReadByte();
            header.Profile = (int)reader.ReadByte();
            header.EffectKey = reader.ReadInt32();
            header.HeaderSize = 10;

            // Read in all the constant buffers.
            var buffers = (int)reader.ReadByte();
            for (var c = 0; c < buffers; c++)
            {

#if OPENGL
				string name = reader.ReadString ();               
#else
                string name = null;
#endif

                // Create the backing system memory buffer.
                var sizeInBytes = (int)reader.ReadInt16();

                // Read the parameter index values.
                var parameters = new int[reader.ReadByte()];
                var offsets = new int[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = (int)reader.ReadByte();
                    offsets[i] = (int)reader.ReadUInt16();
                }
            }

            // Read in all the shader objects.
            var shaders = (int)reader.ReadByte();
            for (var s = 0; s < shaders; s++)
            {
                var isVertexShader = reader.ReadBoolean();

                var shaderLength = reader.ReadInt32();
                var shaderBytecode = reader.ReadBytes(shaderLength);

                var samplerCount = (int)reader.ReadByte();
                for (var ss = 0; s < samplerCount; s++)
                {
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();

                    if (reader.ReadBoolean())
                    {
                        reader.ReadBytes(4 + sizeof(int) * 2 + sizeof(float));
                    }
                    reader.ReadString();
                    reader.ReadByte();
                }

                var cbufferCount = (int)reader.ReadByte();
                for (var c = 0; c < cbufferCount; c++)
                    reader.ReadByte();

                var attributeCount = (int)reader.ReadByte();
                for (var a = 0; a < attributeCount; a++)
                {
                    reader.ReadString();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadInt16(); //format, unused
                }
            }

            // Read in the parameters.
            return ReadParameters(reader);
        }

        private static EffectParameterCollection ReadParameters(BinaryReader reader)
        {
            var count = (int)reader.ReadByte();
            if (count == 0)
                return EffectParameterCollection.Empty;

            var parameters = new EffectParameter[count];
            for (var i = 0; i < count; i++)
            {
                var class_ = (EffectParameterClass)reader.ReadByte();
                var type = (EffectParameterType)reader.ReadByte();
                var name = reader.ReadString();
                var semantic = reader.ReadString();
                var annotations = ReadAnnotations(reader);
                var rowCount = (int)reader.ReadByte();
                var columnCount = (int)reader.ReadByte();

                var elements = ReadParameters(reader);
                var structMembers = ReadParameters(reader);

                object data = null;
                if (elements.Count == 0 && structMembers.Count == 0)
                {
                    switch (type)
                    {
                        case EffectParameterType.Bool:
                        case EffectParameterType.Int32:
#if DIRECTX
                            // Under DirectX we properly store integers and booleans
                            // in an integer type.
                            //
                            // MojoShader on the otherhand stores everything in float
                            // types which is why this code is disabled under OpenGL.
					        {
					            var buffer = new int[rowCount * columnCount];								
                                for (var j = 0; j < buffer.Length; j++)
                                    buffer[j] = reader.ReadInt32();
                                data = buffer;
                                break;
					        }
#endif

                        case EffectParameterType.Single:
                            {
                                var buffer = new float[rowCount * columnCount];
                                for (var j = 0; j < buffer.Length; j++)
                                    buffer[j] = reader.ReadSingle();
                                data = buffer;
                                break;
                            }

                        case EffectParameterType.String:
                            // TODO: We have not investigated what a string
                            // type should do in the parameter list.  Till then
                            // throw to let the user know.
                            throw new NotSupportedException();

                        default:
                            // NOTE: We skip over all other types as they 
                            // don't get added to the constant buffer.
                            break;
                    }
                }

                parameters[i] = new EffectParameter(
                    class_, type, name, rowCount, columnCount, semantic,
                    annotations, elements, structMembers, data);
            }

            return new EffectParameterCollection(parameters);
        }

        private static EffectAnnotationCollection ReadAnnotations(BinaryReader reader)
        {
            var count = (int)reader.ReadByte();
            if (count == 0)
                return EffectAnnotationCollection.Empty;

            var annotations = new EffectAnnotation[count];

            // TODO: Annotations are not implemented!

            return new EffectAnnotationCollection(annotations);
        }

    }
}
