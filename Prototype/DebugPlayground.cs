using DewDrop;
using DewDrop.Entities;
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
using Mother4.GUI;
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
         EntityManager _entityManager;
         bool firsta;
        public DebugPlayground(bool first = false)
        {
            #region Initialize
            firsta = first; ;

            // save it to a file
            Type type = typeof(TextBox);
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
            ImGui.Text($"render pipeline viewer");
            //foreach (var renderable in _pipeline.pIRenderables)
            {
             //   ImGui.Text($"{renderable.GetType().Name} : {renderable.Visible} : {renderable.IsBeingDrawn}");
            }
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

                               // _wreno.Call("blue");
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
                _entityManager = new EntityManager();
                //WindowBox box = new WindowBox("window.gdat", 0, new Vector2(-160,20), new Vector2(320, 70), 1000);
                //var renderer = new TextRenderer(new Vector2(box.RenderPosition.x + 10, box.RenderPosition.y + 10), 2000, "");
                //_pipeline.Add(renderer);
                
                Engine.RenderImGUI += EngineOnRenderImGUI;
                ViewManager.Instance.Center = Engine.HalfScreenSize;
                TextBox box = new TextBox(_pipeline, 0);
                //box.Reset("@wah wah wah wah wah wah wah wah wah wah wah wah wah wah", "gya", false, false);
                box.Reset("@Honey!" + Environment.NewLine + 
                    "@I'm home!" + Environment.NewLine + 
                    "@AHHHHH IM GOOOOOONING!!!"+ Environment.NewLine + 
                    "@STOP FUCKIN GOOONING!", "gya", false ,false);
                box.Show();
                box.OnTypewriterComplete += () => {
                    //box.Reset("@sugma", "ligma", false, false);
                    
                };
                _entityManager.AddEntity(box);
               // ViewManager.Instance.Center = Engine.HalfScreenSize;
                /*TextWriter writer = new TextWriter( .05f);
                _entityManager.AddEntity(writer);
                _pipeline.Add(box);
                //45
                writer.Write("asflasfhasfkhasfkhaksfkhafsfashjkfhakshkjfsajasflasfhasfkhasfkhaksfkhafsfashjkfhakshkjfsaj");
//asflasfhasfkhasfkhaksfkhafsfashjkfhakshkjfsaj*/

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

        public override void Update () {
            base.Update();
            if (initialized) {
                _entityManager.Update();
            }
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
