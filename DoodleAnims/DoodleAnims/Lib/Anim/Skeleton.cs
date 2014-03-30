using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace DoodleAnims.Lib.Anim
{
    /// <summary>
    /// Represents a skeleton
    /// </summary>
    public class Skeleton
    {
        /// <summary>
        /// The current skeleton file version, should be updated whenever new parameters are added
        /// </summary>
        public const byte FILE_VERSION = 2;

        Limb _rootNode;
        PointF _orgin;
        string _name = "newSkeleton";

        Limb[] _idLimbs;

        /// <summary>
        /// Gets the root limb for this skeleton
        /// </summary>
        public Limb Root
        {
            get { return _rootNode; }
        }
        /// <summary>
        /// Gets or sets the orgin position of this skeleton
        /// </summary>
        public PointF Orgin
        {
            get { return _orgin; }
            set
            {
                _orgin = value;
                _rootNode.Orgin = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of this skeleton
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Gets the root treenode to represent this skeleton
        /// </summary>
        public TreeNode RootNode
        {
            get { return _rootNode.TreeNode; }
        }

        /// <summary>
        /// Creates a new skeleton
        /// </summary>
        /// <param name="name">The name for this skeleton</param>
        /// <param name="orgin">The orgin point of the skeleton</param>
        public Skeleton(string name, PointF orgin)
        {
            _orgin = orgin;
            _idLimbs = new Limb[1];
            _rootNode = new Limb(this, Color.Transparent, 0);
            _rootNode.Name = "Orgin";
        }

        /// <summary>
        /// Gets the first available ID
        /// </summary>
        /// <returns>The first available ID</returns>
        public int GetID()
        {
            if (_idLimbs.Length > 0)
                for (int i = 0; i < _idLimbs.Length; i++)
                    if (_idLimbs[i] == null)
                        return i;
            return _idLimbs.Length;
        }

        /// <summary>
        /// Gets the first available generic name
        /// </summary>
        /// <returns>The first available generic name</returns>
        public string GetName()
        {
            int i = 0;

            while (Array.Find<Limb>(_idLimbs, x => x.Name == "limb" + i) != null)
                i++;

            return "limb" + i;
        }

        /// <summary>
        /// Adds a limb refrence to this skeleton
        /// </summary>
        /// <param name="limb">The limb to add</param>
        public void AddLimbRef(Limb limb)
        {
            if (_idLimbs.Length - 1 < limb.ID)
                Array.Resize<Limb>(ref _idLimbs, limb.ID + 1);

            _idLimbs[limb.ID] = limb;
        }

        /// <summary>
        /// Removes a limb refrence to this skeleton
        /// </summary>
        /// <param name="limb">The limb to add</param>
        public void RemoveLimbRef(Limb limb)
        {
            if (_idLimbs.Length > limb.ID & limb.ID != 0)
            {
                _idLimbs[limb.ID] = null;
            }
        }

        /// <summary>
        /// Dispenses a new keyframe
        /// </summary>
        /// <param name="time">The time in seconds that the keyframe exists</param>
        /// <returns>A keyframe generated from this skeleton</returns>
        public AnimKeyFrame GetKeyFrame(double time = 0)
        {
            return new AnimKeyFrame(_idLimbs, time);
        }

        /// <summary>
        /// Applies a frame to this skeleton
        /// </summary>
        /// <param name="frame">The frame to apply</param>
        public void ApplyFrame(AnimKeyFrame frame)
        {
            if (frame.KeyStates.Length == _idLimbs.Length)
            {
                for (int i = 0; i < frame.KeyStates.Length; i++)
                {
                    if (_idLimbs[i] != null & frame.KeyStates[i] != null)
                    {
                        ApplyAnimState(frame.KeyStates[i]);
                    }
                    else
                    {
                        throw new InvalidOperationException("The keyframe must match this skeleton!");
                    }
                }
            }
        }

        /// <summary>
        /// Applies an animation state to this skeleton
        /// </summary>
        /// <param name="state">The state to apply</param>
        private void ApplyAnimState(AnimState state)
        {
            if (state.ID < _idLimbs.Length & state.ID >= 0)
            {
                _idLimbs[state.ID].Rotation = state.Rotation;
                _idLimbs[state.ID].OffsetX = state.OffsetX;
                _idLimbs[state.ID].OffsetY = state.OffsetY;
                _idLimbs[state.ID].ImageAngle = state.ImageAngle;
                _idLimbs[state.ID].Scale = state.Scale;
                _idLimbs[state.ID].Color = state.Color;
                _idLimbs[state.ID].XScale = state.XScale;
                _idLimbs[state.ID].YScale = state.YScale;
            }
            else
                throw new InvalidOperationException("The anim state must match up with this skeleton!");
        }

        /// <summary>
        /// Renders this skeleton
        /// </summary>
        /// <param name="graphics">The graphics device to use</param>
        public void Paint(Graphics graphics)
        {
            _rootNode.Paint(graphics);
        }
        
        /// <summary>
        /// Saves this skeleton to a stream
        /// </summary>
        /// <param name="w">The binary writer to use</param>
        public void Save(BinaryWriter w)
        {
            w.Write(Name);
            w.Write(FILE_VERSION);
            w.Write(_orgin.X);
            w.Write(_orgin.Y);
            _rootNode.Save(w);
        }

        /// <summary>
        /// Loads a skeleton from a stream
        /// </summary>
        /// <param name="r">The binary reader to use</param>
        /// <returns>A skeleton loaded from the stream</returns>
        public static Skeleton Load(BinaryReader r)
        {
            try
            {
                string name = r.ReadString();
                byte version = r.ReadByte();
                float x = r.ReadSingle();
                float y = r.ReadSingle();

                Skeleton s = new Skeleton(name, new PointF(x, y));

                switch (version)
                {
                    case 1:
                        s._rootNode = Limb.LoadV1(s, null, r);
                        break;
                    case 2:
                        s._rootNode = Limb.LoadV2(s, null, r);
                        break;
                    default:
                        s._rootNode = new Limb(s, Color.Transparent, 0);
                        break;
                }

                return s;
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured when loading the file: \n" + e.Message, "Error loading file", MessageBoxButtons.OK);
                return new Skeleton("newProject", new PointF(100,100));
            }
        }
    }
}
