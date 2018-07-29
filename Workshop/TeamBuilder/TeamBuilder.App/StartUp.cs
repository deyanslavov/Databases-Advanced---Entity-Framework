namespace TeamBuilder.App
{
    using Core;
    using TeamBuilder.Models;

    public class StartUp
    {
        public static void Main()
        {
            Engine engine = new Engine(new CommandInterpreter());
            engine.Run();

            //using (Data.TeamBuilderContext db = new Data.TeamBuilderContext())
            //{
            //    db.Database.EnsureDeleted();
            //    db.Database.EnsureCreated();
            //}
        }
    }
}
