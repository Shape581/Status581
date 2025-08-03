using Life;
using Life.DB;
using Life.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Status581
{
    public class Main : Plugin
    {
        public List<int> openedBizs = new List<int>();

        public Main(IGameAPI api) : base(api) { }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            new SChatCommand("/ouverture", new string[] { "/o" }, "Ouvre l'entreprise", "/fermeture", (player, args) =>
            {
                if (player.character.BizId > 0)
                {
                    if (openedBizs.Contains(player.character.BizId) == false)
                    {
                        OpenBiz(player.biz);
                    }
                    else
                        player.Notify("Status581", "L'entreprise est déjà ouverte.", NotificationManager.Type.Error);
                }
                else
                    player.Notify("Status581", "Vous n'avez pas d'entreprise.", NotificationManager.Type.Error);
            }).Register();
            new SChatCommand("/fermeture", new string[] { "/f" }, "Ferme l'entreprise", "/fermeture", (player, args) =>
            {
                if (player.character.BizId > 0)
                {
                    if (openedBizs.Contains(player.character.BizId))
                    {
                        CloseBiz(player.biz);
                    }
                    else
                        player.Notify("Status581", "L'entreprise n'est ouverte.", NotificationManager.Type.Error);
                }
                else
                    player.Notify("Status581", "Vous n'avez pas d'entreprise.", NotificationManager.Type.Error);
            }).Register();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} initialise !");
            Console.ResetColor();
        } 

        public void OpenBiz(Bizs biz)
        {
            openedBizs.Add(biz.Id);
            Nova.server.Players.Where(obj => obj.isSpawned).ToList().ForEach(player => player.SendText($"<color={LifeServer.COLOR_GREEN}>L'entreprise {biz.BizName} est désormais ouverte.</color>"));   
        }

        public void CloseBiz(Bizs biz)
        {
            openedBizs.Remove(biz.Id);
            Nova.server.Players.Where(obj => obj.isSpawned).ToList().ForEach(player => player.SendText($"<color={LifeServer.COLOR_RED}>L'entreprise {biz.BizName} est désormais fermé.</color>"));
        }
    }
}
