using System;
using SDL2;

class Program
{
    const double POINT_RADIUS = 10.0;
    const int N_POINTS = 4;

    struct Point
    {
        public double X;
        public double Y;
    }

    static void Main()
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
        {
            Console.WriteLine($"[ERROR] SDL initialization error: {SDL.SDL_GetError()}");
            return;
        }

        IntPtr window = SDL.SDL_CreateWindow(
            "Splines",
            SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
            800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        Point[] points = new Point[N_POINTS]
        {
            new Point { X = 200, Y = 200 },
            new Point { X = 400, Y = 200 },
            new Point { X = 200, Y = 400 },
            new Point { X = 400, Y = 400 }
        };

        Point? selectedPoint = null;
        bool running = true;

        while (running)
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) > 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        int mouseX, mouseY;
                        SDL.SDL_GetMouseState(out mouseX, out mouseY);
                        selectedPoint = null;
                        foreach (var point in points)
                        {
                            double dx = point.X - mouseX;
                            double dy = point.Y - mouseY;
                            if (Math.Sqrt(dx * dx + dy * dy) < POINT_RADIUS)
                            {
                                selectedPoint = point;
                                break;
                            }
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEMOTION:
                        if (selectedPoint.HasValue)
                        {
                            SDL.SDL_GetMouseState(out mouseX, out mouseY);
                            selectedPoint = new Point { X = mouseX, Y = mouseY };
                        }
                        SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                        SDL.SDL_RenderClear(renderer);

                        SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);
                        foreach (var point in points)
                        {
                            SDL.SDL_RenderDrawLine(renderer, (int)(point.X - POINT_RADIUS), (int)point.Y, (int)(point.X + POINT_RADIUS), (int)point.Y);
                            SDL.SDL_RenderDrawLine(renderer, (int)point.X, (int)(point.Y - POINT_RADIUS), (int)point.X, (int)(point.Y + POINT_RADIUS));
                        }

                        SDL.SDL_SetRenderDrawColor(renderer, 160, 160, 160, 255);
                        for (int i = 1; i < N_POINTS; i++)
                        {
                            SDL.SDL_RenderDrawLine(renderer, (int)points[i - 1].X, (int)points[i - 1].Y, (int)points[i].X, (int)points[i].Y);
                        }

                        SDL.SDL_RenderPresent(renderer);
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                        selectedPoint = null;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_q)
                            running = false;
                        break;
                    case SDL.SDL_EventType.SDL_QUIT:
                        running = false;
                        break;
                }
            }
        }

        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}