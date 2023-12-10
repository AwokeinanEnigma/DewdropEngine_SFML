using DewDrop;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.Resources;
using DewDrop.Scenes;
using DewDrop.Scenes.Transitions;
using DewDrop.UserInput;
using DewDrop.Utilities;
using DewDrop.Wren;
using ImGuiNET;
using Prototype.Scenes;
using SFML.Graphics;
using SFML.System;

namespace Prototype;

public class DebugPlayground : SceneBase
{
        private Player _playerEntity;
        public Text funnyText;
        ShapeEntity2 overlayEntity;
        public ScrollableList List;

        RenderPipeline _pipeline;
         Wreno _wreno;

         bool firsta;
        public DebugPlayground(bool first = false)
        {
            #region Initialize
            firsta = first; ;

            // save it to a file
            Type type = typeof(LineRenderer);
            string code =  WrenWrapperGenerator.GenerateWrapper(type) ;
            File.WriteAllText(Directory.GetCurrentDirectory() + $"/{WrenWrapperGenerator.GetWrapperClassName(type)}.cs", code);
            
            
            
            #endregion   
        }
    

        protected override void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                _pipeline.Clear(true);
               //_wreno.Dispose();
                GC.Collect();
                TextureManager.Instance.Purge();
                clock.Dispose();
                disposed = true;
            }
            List = null;
            _pipeline = null;
           // _wreno = null;
            clock = null;
            base.Dispose(disposing);
        }

        private void EngineOnRenderImGUI()
        {
            ImGui.Begin("Dewdrop Debug Utilities");
            ImGui.Text($"Garbage Allocated: {GC.GetTotalMemory(false) / 1024L}KB");
            ImGui.Separator();

            if (ImGui.Button("Force GC Collection")) GC.Collect();
            ImGui.End();
        }
        
        bool initialized = false;
        public override void Focus()
        {
            base.Focus();
            Outer.Log(GlobalData.GetString("fu"));
            if (!initialized) {
                _pipeline = new RenderPipeline(Engine.RenderTexture);
                List = new ScrollableList(new Vector2f(32f, 0), 0, new[] {
                        new ScrollableList.SelectAction() {
                            OnSelect = (list) => {
                                if (!SceneManager.IsTransitioning) {
                                    SceneManager.Transition = new ColorFuckTransition(0.45f, new [] {
                                        Color.Red,
                                        Color.Blue,
                                        Color.Blue,
                                        Color.Red,
                                    });
                                    SceneManager.Push(new TestScene(), true);
                                    return true;
                                }
                                return false;
                            },
                            Text = "Go to play scene."
                        },
                        new ScrollableList.SelectAction() {
                            OnSelect = (list) => {

                                _wreno.Call("blue");
                                return false;
                            },
                            Text = "Blue!"
                        },
                        new ScrollableList.SelectAction() {
                            OnSelect = (list) => {
                                _wreno.Call("red");
                                return false;
                            },
                            Text = "Red!"
                        },
                        new ScrollableList.SelectAction() {
                            OnSelect = (list) => {
                                _wreno.Call("green");
                                return false;
                            },
                            Text = "Green!"
                        },

                    },
                    3, 11,
                    new FontData());
                WrenPipelineWrapper.Pipeline = _pipeline;
                _pipeline.Add(List);
                TextureManager.Instance.DumpLoadedTextures();
                clock = new Clock();

                // create wren 
                //_wreno = WrenManager.MakeWreno(File.ReadAllText(Directory.GetCurrentDirectory() + "/test.wren"));
                // run the script
                //_wreno.Run();
                // get the text renderer from wrenland
                // add it to the render pipeline
                //_pipeline.Add(_wreno.GetVariable<WrenTextRendererWrapper>("render").TextRenderer);
                initialized = true;

            }
            

        }

        public Clock clock;

        public override void Update()
        {
            base.Update();

        }

        public override void Draw()
        {
            if (initialized && !disposed) {
                _pipeline.Draw();
            }
            //Engine.RenderTexture.Draw(funnyText);
            base.Draw();
        }


    }
