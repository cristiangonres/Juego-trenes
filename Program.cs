using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ferroviario
{
    public class Tren
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float X_Ini { get; set; }
        public float Y_Ini { get; set; }
        public float X_Fi { get; set; }
        public float Y_Fi { get; set; }
        public int Velocidad { get; set; }
        public int Iterador_X { get; set; }
        public string Via { get; set; }


    }

    public class DatosVias
    {
        public const float LimiteInf_x = 0.0f;
        public const float LimiteSup_x = 1150.0f;
        public const float LimiteInf_y = 0.0f;
        public const float LimiteSup_y = 750.0f;

        public const float ViaAB_y = 200.0f;
        public const float ViaBA_y = 400.0f;
        public const float ViaU_y = 300.0f;
        public const float ViaU_x_Ini = 400.0f;
        public const float ViaU_x_Fi = 800.0f;

        public const float Ancho_Tren = 42.0f;
        public const float Alto_Tren = 42.0f;
        public const float Ancho_Via = 2.0f;

        public List<Tren> Trenes { get; set; }

        public Queue<Tren> GraficosTrenes { get; set; }

        public DatosVias()
        {
            this.Trenes = new List<Tren>();
            this.GraficosTrenes = new Queue<Tren>();
        }

    }
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D trenAB;
        Texture2D trenBA;

        public DatosVias DatosVias;

        public Game1(DatosVias InputDatosVias)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.DatosVias = InputDatosVias;
        }

        protected override void Initialize()
        {
            //Cargamos las imagenes que configuran nuestros sprites. 
            FileStream fileStream = new FileStream("Content/trenAB.png", FileMode.Open);
            trenAB = Texture2D.FromStream(this.GraphicsDevice, fileStream);
            fileStream.Close();

            fileStream = new FileStream("Content/trenBA.png", FileMode.Open);
            trenBA = Texture2D.FromStream(this.GraphicsDevice, fileStream);
            fileStream.Close();


            //Configuramos tamaño de ventana de 1200 x 800
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //TODO: use this.Content to load your game content here 
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();


            DibujarLinea(DatosVias.LimiteInf_x, DatosVias.ViaAB_y, DatosVias.ViaU_x_Ini, DatosVias.ViaAB_y, Color.White, DatosVias.Ancho_Via);
            DibujarLinea(DatosVias.LimiteInf_x, DatosVias.ViaBA_y, DatosVias.ViaU_x_Ini, DatosVias.ViaBA_y, Color.White, DatosVias.Ancho_Via);
            DibujarLinea(DatosVias.ViaU_x_Fi, DatosVias.ViaAB_y, DatosVias.LimiteSup_x, DatosVias.ViaAB_y, Color.White, DatosVias.Ancho_Via);
            DibujarLinea(DatosVias.ViaU_x_Fi, DatosVias.ViaBA_y, DatosVias.LimiteSup_x, DatosVias.ViaBA_y, Color.White, DatosVias.Ancho_Via);
            DibujarLinea(DatosVias.ViaU_x_Ini, DatosVias.ViaU_y, DatosVias.ViaU_x_Fi, DatosVias.ViaU_y, Color.White, DatosVias.Ancho_Via);
            DibujarLinea(DatosVias.ViaU_x_Ini, DatosVias.ViaAB_y, DatosVias.ViaU_x_Ini + DatosVias.Ancho_Via, DatosVias.ViaBA_y, Color.White, DatosVias.ViaBA_y - DatosVias.ViaAB_y + DatosVias.Ancho_Via);
            DibujarLinea(DatosVias.ViaU_x_Fi, DatosVias.ViaAB_y, DatosVias.ViaU_x_Fi + DatosVias.Ancho_Via, DatosVias.ViaBA_y, Color.White, DatosVias.ViaBA_y - DatosVias.ViaAB_y + DatosVias.Ancho_Via);

            MostrarTrenes();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DibujarLinea(float xIni, float yIni, float xFinal, float yFinal, Color color, float ancho)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { color });

            var origin = new Vector2(0f, 0f);
            var scale = new Vector2(xFinal - xIni, ancho);
            var point = new Vector2(xIni, yIni);
            spriteBatch.Draw(pixel, point, null, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        private void MostrarTrenes()
        {
            //Comprobamos si hay coche pendientes de mostrar en la interficie gráfica
            if (this.DatosVias.GraficosTrenes.Count > 0)
            {
                this.DatosVias.Trenes.Add(this.DatosVias.GraficosTrenes.Dequeue());
            }

            //Mostramos los coches incorporados a la interfície gráfica
            foreach (Tren t in this.DatosVias.Trenes)
            {
                Vector2 V = new Vector2(t.X, t.Y - DatosVias.Alto_Tren);
                switch (t.Via)
                {
                    case "AB":
                        spriteBatch.Draw(trenAB, V, null, Color.Blue);
                        break;
                    case "BA":
                        spriteBatch.Draw(trenBA, V, null, Color.Red);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    class Program
    {
        public static Game1 game;
        public static DatosVias DatosVia_obj = new DatosVias();
        public static Tren trenAB = new Tren();
        public static Tren trenBA = new Tren();
        public static bool ViaU = false;

        static void Main(string[] args)
        {
            InicializarTrenes();

            //Iniciamos interfaz gráfica    
            using (game = new Game1(DatosVia_obj))
            {
                game.Run();
            }
        }

        //Este método es una prueba de carga de coches a la interfaz gráfica para comprobar que se visualizan correctamente
        static void InicializarTrenes()
        {
            var rand = new Random();

            trenAB.X = DatosVias.LimiteInf_x;
            trenAB.Y = DatosVias.ViaAB_y;
            trenAB.X_Ini = DatosVias.LimiteInf_x;
            trenAB.X_Fi = DatosVias.LimiteSup_x;
            trenAB.Y_Ini = DatosVias.ViaAB_y;
            trenAB.Y_Fi = DatosVias.ViaAB_y;
            trenAB.Velocidad = rand.Next(2, 10);
            trenAB.Iterador_X = 1;
            trenAB.Via = "AB";
            DatosVia_obj.GraficosTrenes.Enqueue(trenAB);


            trenBA.X = DatosVias.LimiteSup_x;
            trenBA.Y = DatosVias.ViaBA_y;
            trenBA.X_Ini = DatosVias.LimiteSup_x;
            trenBA.X_Fi = DatosVias.LimiteInf_x;
            trenBA.Y_Ini = DatosVias.ViaBA_y;
            trenBA.Y_Fi = DatosVias.ViaBA_y;
            trenBA.Velocidad = rand.Next(2, 10);
            trenBA.Iterador_X = -1;
            trenBA.Via = "BA";
            DatosVia_obj.GraficosTrenes.Enqueue(trenBA);

            Thread TAB = new Thread(MoverTren);
            Thread TBA = new Thread(MoverTren);

            TAB.Start(trenAB);
            TBA.Start(trenBA);

        }

        static void MoverTren(object Tren)
        {

            Tren tren = (Tren)Tren;

            while (DentroLimites(tren))
            {

                if (trenAB.X > 400 && trenAB.X < 800 && ViaU == false) 

                {   
                    if (trenBA.X < 802 ) {

                        trenBA.X = 801;
                    }
                }

                if (trenBA.X > 400 && trenBA.X < 800 && ViaU == false )
                {
                    if (trenAB.X > 398 ) {
                    trenAB.X = 399;
                    }
                }


                if ((trenAB.X > 800 || trenBA.X < 400) && ViaU == false)
                    {
                        ViaU = true;
                    }

                tren.X += tren.Iterador_X;
                Console.WriteLine($"Tren: {tren.Via} - {tren.X}");
                Thread.Sleep(tren.Velocidad);

                if (tren.X > 400 && tren.X < 800)
                {
                    tren.Y = DatosVias.ViaU_y;
                }
                else
                {
                    if (tren.Via == "BA")
                    {
                        tren.Y = DatosVias.ViaBA_y;
                    }
                    else
                    {
                        tren.Y = DatosVias.ViaAB_y;
                    }
                }
            }

        }

        static bool DentroLimites(Tren tren)
        {
            if (tren.X < DatosVias.LimiteInf_x || tren.X > DatosVias.LimiteSup_x)
            {
                return false;
            }

            return true;

        }
    }
}
