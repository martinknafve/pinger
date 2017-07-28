using Topshelf;

namespace Pinger
{
   class Program
   {
      static void Main(string[] args)
      {
         HostFactory.Run(x =>                                
         {
            x.Service<Pinger>(s =>                        
            {
               s.ConstructUsing(name => new Pinger());    
               s.WhenStarted(tc => tc.Start());           
               s.WhenStopped(tc => tc.Stop());            
            });
            x.RunAsLocalSystem();                         

            x.SetDescription("Pinger");        
            x.SetDisplayName("Pinger");        
            x.SetServiceName("Pinger");        
         });                                   

      }
   }
}
