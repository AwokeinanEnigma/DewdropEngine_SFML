using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.Maps;
using DewDrop.Scenes;
using DewDrop.Scenes.Transitions;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using Prototype.Scenes;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Prototype;

public class GUIText : SceneBase
{
        private Player _playerEntity;
        public Text funnyText;
        private ShapeEntity2 overlayEntity;

        private RenderPipeline pipeline;

        private TextRenderer title;
        public ScrollableList List;
        public GUIText()
        {
            #region Initialize

            funnyText = new Text("swag", new FontData().Font);
            funnyText.FillColor = Color.Red;
            
            pipeline = new RenderPipeline(Engine.RenderTexture);
            List = new ScrollableList(new Vector2f(32f, 0), 0, new[]
                {
                    "Under",
                    "My",
                    "Skin"
                },
                3,11, 
                new FontData());

            pipeline.Add(List);
            AxisManager.PostAxisChanged += () =>
            {

                if (AxisManager.Instance.Axis.Y > 0.5f)
                {
                    List.SelectNext();
                }
                else if (AxisManager.Instance.Axis.Y < -0.5f)
                {
                    List.SelectPrevious();
                }
            };

            Input.Instance.OnButtonPressed += (sender, button) =>
            {
                if (button == DButtons.Select)
                {
                    if (List.SelectedItem == "Skin")
                    {
                        SceneManager.Instance.Transition = new ColorFadeTransition(0.5f, Color.Black);
                        SceneManager.Instance.Push(new TestScene());
                        DDDebug.Log("Skin");
                    }
                }
            };

            #endregion
        }


        private void EngineOnRenderImGUI()
        {
            ImGui.Begin("Dewdrop Debug Utilities");
            ImGui.Text($"Garbage Allocated: {GC.GetTotalMemory(false) / 1024L}KB");
            ImGui.Separator();

            if (ImGui.Button("Force GC Collection")) GC.Collect();
            ImGui.End();
        }
        
        public override void Focus()
        {
            base.Focus();
        }


        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            pipeline.Draw();
            Engine.RenderTexture.Draw(funnyText);
            base.Draw();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                // dispose here
            }

            base.Dispose(disposing);
        }
    }
