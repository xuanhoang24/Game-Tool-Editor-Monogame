using Editor;
using Editor.Editor;
using System.Threading;

Thread t = Thread.CurrentThread;
t.SetApartmentState(ApartmentState.Unknown);
t.SetApartmentState(ApartmentState.STA);

FormEditor editor = new();
editor.Game = new GameEditor(editor);
editor.Show();
editor.Game.Run();