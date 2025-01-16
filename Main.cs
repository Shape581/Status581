using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Life;
using Life.BizSystem;
using Life.Network;
using ModKit.Helper;
using ModKit.ORM;
using SQLite;
using Format = ModKit.Helper.TextFormattingHelper;
using Life.UI;
using ModKit.Utils;
using JetBrains.Annotations;
using Life.DB;

namespace Status581
{
    public class Main : ModKit.ModKit
    {
        public static Dictionary<int, bool> Status = new Dictionary<int, bool>();

        public Main(IGameAPI api) : base(api) 
        {
            PluginInformations = new ModKit.Interfaces.PluginInformations(AssemblyHelper.GetName(), "1.0.0", "Shape581");
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            AAMenu.Menu.AddBizTabLine(PluginInformations, new List<Life.BizSystem.Activity.Type> { Activity.Type.None }, null, $"{Format.Color("Ouvrir", Format.Colors.Success)} {Format.Color("/", Format.Colors.Grey)} {Format.Color("Fermer", Format.Colors.Error)}", aaMenu =>
            {
                var player = PanelHelper.ReturnPlayerFromPanel(aaMenu);
                if (Status.ContainsKey(player.biz.Id))
                {
                    if (Status[player.biz.Id] == true)
                    {
                        SendClose(player.biz.BizName);
                        player.Notify(Format.Color("Succès", Format.Colors.Success), "Vous avez fermé l'entreprise", NotificationManager.Type.Success);
                        Status.Remove(player.biz.Id);
                    }
                }
                else
                {
                    Status.Add(player.biz.Id, true);
                    SendOpen(player.biz.BizName);
                    player.Notify(Format.Color("Succès", Format.Colors.Success), "Vous avez ouvert l'entreprise", NotificationManager.Type.Success);
                }
            });
            AAMenu.Menu.AddDocumentTabLine(PluginInformations, Format.Color("Annuaire des Entreprises", Format.Colors.Purple), aaMenu =>
            {
                var player = PanelHelper.ReturnPlayerFromPanel(aaMenu);
                OpenMenu(player);
            });
            new SChatCommand("/o", "Ouverture d'entreprise", "/o", (player, arg) =>
            {
                if (Status.ContainsKey(player.biz.Id))
                {
                    if (Status[player.biz.Id] == true)
                    {
                        player.Notify(Format.Color("Erreur", Format.Colors.Error), "L'entreprise est déjà ouverte.", NotificationManager.Type.Error);
                    }
                }
                else
                {
                    Status.Add(player.biz.Id, true);
                    SendOpen(player.biz.BizName);
                    player.Notify(Format.Color("Succès", Format.Colors.Success), "Vous avez ouvert l'entreprise", NotificationManager.Type.Success);
                }
            }).Register();
            new SChatCommand("/ouverture", "Ouverture d'entreprise", "/ouverture", (player, arg) =>
            {
                if (Status.ContainsKey(player.biz.Id))
                {
                    if (Status[player.biz.Id] == true)
                    {
                        player.Notify(Format.Color("Erreur", Format.Colors.Error), "L'entreprise est déjà ouverte.", NotificationManager.Type.Error);
                    }
                }
                else
                {
                    Status.Add(player.biz.Id, true);
                    SendOpen(player.biz.BizName);
                    player.Notify(Format.Color("Succès", Format.Colors.Success), "Vous avez ouvert l'entreprise", NotificationManager.Type.Success);
                }
            }).Register();
            new SChatCommand("/f", "Fermeture d'entreprise", "/f", (player, arg) =>
            {
                if (Status.ContainsKey(player.biz.Id))
                {
                    if (Status[player.biz.Id] == true)
                    {
                        SendClose(player.biz.BizName);
                        player.Notify(Format.Color("Succès", Format.Colors.Success), "Vous avez fermé l'entreprise", NotificationManager.Type.Success);
                        Status.Remove(player.biz.Id);
                    }
                }
                else
                {
                    player.Notify(Format.Color("Erreur", Format.Colors.Error), "L'entreprise n'est pas encore ouverte.", NotificationManager.Type.Error);
                }
            }).Register();
            new SChatCommand("/fermeture", "Fermeture d'entreprise", "/fermeture", (player, arg) =>
            {
                if (Status.ContainsKey(player.biz.Id))
                {
                    if (Status[player.biz.Id] == true)
                    {
                        SendClose(player.biz.BizName);
                        player.Notify(Format.Color("Succès", Format.Colors.Success), "Vous avez fermé l'entreprise", NotificationManager.Type.Success);
                        Status.Remove(player.biz.Id);
                    }
                }
                else
                {
                    player.Notify(Format.Color("Erreur", Format.Colors.Error), "L'entreprise n'est pas encore ouverte.", NotificationManager.Type.Error);
                }
            }).Register();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Status581] - Intialisé !");
            Console.ResetColor();
        }

        public void SendOpen(string bizName)
        {
            Nova.server.SendMessageToAll($"{Format.Color("[OUVERTURE]", Format.Colors.Success)} L'entreprise {Format.Color(bizName, Format.Colors.Info)} est désormais {Format.Color("Ouverte", Format.Colors.Success)} !");
        }

        public void SendClose(string bizName)
        {
            Nova.server.SendMessageToAll($"{Format.Color("[FERMETURE]", Format.Colors.Error)} L'entreprise {Format.Color(bizName, Format.Colors.Info)} est désormais {Format.Color("Fermé", Format.Colors.Error)} !");
        }

        public void OpenMenu(Player player)
        {
            var panel = PanelHelper.Create(Format.Color("Annuaire des Entreprise", Format.Colors.Purple), Life.UI.UIPanel.PanelType.TabPrice, player, () => OpenMenu(player));
            panel.CloseButton();
            panel.AddButton(Format.Color("Séléctionner", Format.Colors.Success), ui => ui.SelectTab());
            panel.PreviousButton();
            foreach (var bizs in Nova.biz.bizs)
            {
                int icon = 0;
                if (bizs.IsActivity(Activity.Type.LawEnforcement))
                {
                    icon = ItemUtils.GetIconIdByItemId(6);
                }
                if (bizs.IsActivity(Activity.Type.Mecanic))
                {
                    icon = ItemUtils.GetIconIdByItemId(1066);
                }
                if (bizs.IsActivity(Activity.Type.Medical))
                {
                    icon = ItemUtils.GetIconIdByItemId(1102);
                }
                if (bizs.IsActivity(Activity.Type.Trash))
                {
                    icon = ItemUtils.GetIconIdByItemId(1277);
                }
                if (bizs.IsActivity(Activity.Type.Electrician))
                {
                    icon = ItemUtils.GetIconIdByItemId(1654);
                }
                if (bizs.IsActivity(Activity.Type.Taxi))
                {
                    icon = IconUtils.Accessories.Utils.PanneauTaxi.Id;
                }
                if (bizs.IsActivity(Activity.Type.Transport))
                {
                    icon = IconUtils.Vehicles.Master.Id;
                }
                if (bizs.IsActivity(Activity.Type.Bus))
                {
                    icon = IconUtils.Vehicles.BusLionSCity.Id;
                }
                string statut = "";
                if (Status.ContainsKey(bizs.Id))
                {
                    if (Status[bizs.Id] == true)
                    {
                        statut = Format.Color("Ouvert", Format.Colors.Error);
                    }
                }
                else
                {
                    statut = Format.Color("Fermé", Format.Colors.Error);
                }
                panel.AddTabLine(Format.Color(bizs.BizName, Format.Colors.Info), statut, icon, ui =>
                {
                    See(player, bizs);
                });
            }
            panel.Display();
        }

        public void See(Player player, Bizs biz)
        {
            var panel = PanelHelper.Create(Format.Color($"Entreprise - {biz.BizName}", Format.Colors.Purple), UIPanel.PanelType.TabPrice, player, () => See(player, biz));
            panel.CloseButton();
            panel.PreviousButton();
            panel.AddTabLine($"Nom de l'entreprise {Format.Color($"{biz.BizName}", Format.Colors.Info)}", ui =>
            {
                panel.Refresh();
                player.Notify(Format.Color("Information", Format.Colors.Info), "Aucune détail sur cette information.", NotificationManager.Type.Info);
            });
            panel.AddTabLine($"Description : {Format.Color($"{biz.Description}", Format.Colors.Info)}", ui =>
            {
                panel.Refresh();
                player.Notify(Format.Color("Information", Format.Colors.Info), "Aucune détail sur cette information.", NotificationManager.Type.Info);
            });
            panel.AddTabLine($"Information de Contact : {Format.Color($"{biz.Contact}", Format.Colors.Info)}", ui =>
            {
                panel.Refresh();
                player.Notify(Format.Color("Information", Format.Colors.Info), "Aucune détail sur cette information.", NotificationManager.Type.Info);
            });
            panel.AddTabLine($"Salaire : {Format.Color($"{biz.Salaire.ToString("F2") + "€"}", Format.Colors.Info)}", ui =>
            {
                panel.Refresh();
                player.Notify(Format.Color("Information", Format.Colors.Info), "Aucune détail sur cette information.", NotificationManager.Type.Info);
            });
            panel.AddTabLine($"Argent en Banque : {Format.Color($"{biz.Bank.ToString("F2") + "€"}", Format.Colors.Info)}", ui =>
            {
                panel.Refresh();
                player.Notify(Format.Color("Information", Format.Colors.Info), "Aucune détail sur cette information.", NotificationManager.Type.Info);
            });
            var owner = Nova.server.GetPlayer(biz.OwnerId);
            panel.AddTabLine($"Chef : {Format.Color($"{owner.GetFullName()}", Format.Colors.Info)}", ui =>
            {
                panel.Refresh();
                player.Notify(Format.Color("Information", Format.Colors.Info), "Aucune détail sur cette information.", NotificationManager.Type.Info);
            });
            panel.Display();
        }
    }
}
