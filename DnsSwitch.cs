// DNS Switch by mysik — переключение DNS в один клик с замером скорости.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using Mysik;

[assembly: AssemblyTitle("DNS Switch")]
[assembly: AssemblyDescription("DNS Switch by mysik — переключение DNS в один клик")]
[assembly: AssemblyCompany("mysik")]
[assembly: AssemblyProduct("DNS Switch by mysik")]
[assembly: AssemblyCopyright("by mysik")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace DnsSwitch
{
    static class Program
    {
        [STAThread]
        static void Main() { Boot.Run("DNS Switch", "DnsSwitchByMysik_SingleInstance", () => new MainWindow()); }
    }

    class Provider
    {
        public string Name, Desc;
        public string[] V4, V6;
        public bool IsAuto, IsCustom;
        public RadioButton Card;
        public TextBlock Ms;
        public double LastMs = -1;

        public Provider(string name, string desc, string[] v4, string[] v6) { Name = name; Desc = desc; V4 = v4; V6 = v6 ?? new string[0]; }
    }

    class Adapter
    {
        public int Index;
        public string Name, Id;
        public List<string> Dns = new List<string>();
        public bool Static;
        public string Gateway;
    }

    class MainWindow : ShellWindow
    {
        readonly List<Provider> providers = new List<Provider>
        {
            new Provider("Автоматически", "DNS от провайдера или роутера, как было по умолчанию", new string[0], null) { IsAuto = true },
            new Provider("Cloudflare", "Один из самых быстрых, не хранит историю запросов",
                new[] { "1.1.1.1", "1.0.0.1" }, new[] { "2606:4700:4700::1111", "2606:4700:4700::1001" }),
            new Provider("Google", "Стабильный и надёжный, работает везде",
                new[] { "8.8.8.8", "8.8.4.4" }, new[] { "2001:4860:4860::8888", "2001:4860:4860::8844" }),
            new Provider("Quad9", "Блокирует сайты с вирусами и фишингом",
                new[] { "9.9.9.9", "149.112.112.112" }, new[] { "2620:fe::fe", "2620:fe::9" }),
            new Provider("AdGuard", "Убирает рекламу и трекеры на всех сайтах и в приложениях",
                new[] { "94.140.14.14", "94.140.15.15" }, new[] { "2a10:50c0::ad1:ff", "2a10:50c0::ad2:ff" }),
            new Provider("Яндекс", "Быстрый внутри России",
                new[] { "77.88.8.8", "77.88.8.1" }, new[] { "2a02:6b8::feed:0ff", "2a02:6b8:0:1::feed:0ff" }),
            new Provider("Comss", "Помогает открыть сервисы, недоступные из России (только IPv4)",
                new[] { "212.109.221.20", "212.109.195.93" }, null),
            new Provider("Свой DNS", "Впиши адреса сам", new string[0], null) { IsCustom = true }
        };

        Provider selected, current;
        bool busy;
        List<Adapter> adapters = new List<Adapter>();

        TextBlock currentName, currentSub, currentIcon, speedHint, testBtnText, applyText;
        Border currentIconBg, customBox;
        StackPanel providerList;
        Button refreshBtn, testBtn, flushBtn, applyBtn;
        TextBox customPrimary, customSecondary;

        public MainWindow()
        {
            LoadUi("DNS Switch by mysik", 440, 760);

            currentName = F<TextBlock>("CurrentName");
            currentSub = F<TextBlock>("CurrentSub");
            currentIcon = F<TextBlock>("CurrentIcon");
            currentIconBg = F<Border>("CurrentIconBg");
            speedHint = F<TextBlock>("SpeedHint");
            testBtnText = F<TextBlock>("TestBtnText");
            applyText = F<TextBlock>("ApplyText");
            customBox = F<Border>("CustomBox");
            providerList = F<StackPanel>("ProviderList");
            refreshBtn = F<Button>("RefreshBtn");
            testBtn = F<Button>("TestBtn");
            flushBtn = F<Button>("FlushBtn");
            applyBtn = F<Button>("ApplyBtn");
            customPrimary = F<TextBox>("CustomPrimary");
            customSecondary = F<TextBox>("CustomSecondary");

            refreshBtn.Click += (s, e) => { ReadCurrent(); Toast("Обновлено"); };
            testBtn.Click += async (s, e) => await TestSpeed();
            flushBtn.Click += async (s, e) => await Flush();
            applyBtn.Click += async (s, e) => await Apply();
            customPrimary.TextChanged += (s, e) => UpdateApply();
            customSecondary.TextChanged += (s, e) => UpdateApply();

            BuildCards();
        }

        public override async void Init()
        {
            ReadCurrent();
            await TestSpeed();
        }

        // -------------------------------------------------- карточки

        void BuildCards()
        {
            var style = S("ChoiceCard");
            foreach (var p in providers)
            {
                var prov = p;
                var g = Columns(Star, Auto);
                var left = new StackPanel();
                var title = new StackPanel { Orientation = Orientation.Horizontal };
                title.Children.Add(Txt(p.Name, 14, CText, true));
                if (p.V4.Length > 0)
                {
                    var ip = Txt("  " + p.V4[0], 12, CFaint, false);
                    ip.VerticalAlignment = VerticalAlignment.Bottom;
                    ip.Margin = new Thickness(0, 0, 0, 1);
                    title.Children.Add(ip);
                }
                left.Children.Add(title);
                var d = Txt(p.Desc, 12, CSub, false);
                d.TextWrapping = TextWrapping.Wrap;
                d.Margin = new Thickness(0, 2, 10, 0);
                left.Children.Add(d);
                g.Children.Add(left);

                p.Ms = Txt(p.IsAuto || p.IsCustom ? "" : "—", 13, CFaint, true);
                p.Ms.VerticalAlignment = VerticalAlignment.Center;
                g.Children.Add(At(p.Ms, 1));

                p.Card = new RadioButton { Style = style, Content = g, GroupName = "dns" };
                p.Card.Checked += (s, e) =>
                {
                    selected = prov;
                    customBox.Visibility = prov.IsCustom ? Visibility.Visible : Visibility.Collapsed;
                    if (prov.IsCustom) customPrimary.Focus();
                    UpdateApply();
                };
                providerList.Children.Add(p.Card);
            }
        }

        void UpdateApply()
        {
            if (selected == null) { applyText.Text = "Выбери DNS"; applyBtn.IsEnabled = false; return; }
            bool same = ReferenceEquals(selected, current) && !selected.IsCustom;
            applyBtn.IsEnabled = !busy && !same && (!selected.IsCustom || CustomAddresses() != null);
            applyText.Text = same ? "Уже используется" : selected.IsCustom ? "Применить свой DNS" : "Применить " + selected.Name;
        }

        string[] CustomAddresses()
        {
            var list = new List<string>();
            foreach (var box in new[] { customPrimary, customSecondary })
            {
                string t = box.Text.Trim();
                if (t.Length == 0) continue;
                IPAddress ip;
                if (!IPAddress.TryParse(t, out ip)) return null;
                list.Add(ip.ToString());
            }
            return list.Count > 0 ? list.ToArray() : null;
        }

        // -------------------------------------------------- текущее состояние

        static List<Adapter> ReadAdapters()
        {
            var res = new List<Adapter>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                try
                {
                    if (ni.OperationalStatus != OperationalStatus.Up) continue;
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback || ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel) continue;
                    var props = ni.GetIPProperties();
                    var gw = props.GatewayAddresses.Select(g => g.Address)
                        .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork && !a.Equals(IPAddress.Any));
                    if (gw == null) continue; // без шлюза адаптер не выходит в интернет
                    var v4 = props.GetIPv4Properties();
                    if (v4 == null) continue;

                    var a2 = new Adapter { Index = v4.Index, Name = ni.Name, Id = ni.Id, Gateway = gw.ToString() };
                    foreach (var d in props.DnsAddresses)
                        if (d.AddressFamily == AddressFamily.InterNetwork) a2.Dns.Add(d.ToString());
                    try
                    {
                        using (var k = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + ni.Id))
                        {
                            var ns = k != null ? k.GetValue("NameServer") as string : null;
                            a2.Static = !string.IsNullOrWhiteSpace(ns);
                        }
                    }
                    catch { }
                    res.Add(a2);
                }
                catch { }
            }
            return res;
        }

        void ReadCurrent()
        {
            adapters = ReadAdapters();
            if (adapters.Count == 0)
            {
                current = null;
                currentName.Text = "Нет подключения";
                currentSub.Text = "Не нашёл активный сетевой адаптер с доступом в интернет";
                SetCurrentIcon(false);
                UpdateApply();
                return;
            }

            var main = adapters[0];
            string first = main.Dns.FirstOrDefault();
            current = null;
            if (!main.Static) current = providers.First(p => p.IsAuto);
            else if (first != null) current = providers.FirstOrDefault(p => p.V4.Contains(first));

            if (current == null && main.Static) current = providers.First(p => p.IsCustom);

            currentName.Text = current == null ? "Неизвестно" : current.IsCustom ? "Свой DNS" : current.Name;
            currentSub.Text = main.Name + (main.Dns.Count > 0 ? " · " + string.Join(", ", main.Dns.Take(2)) : "") +
                              (adapters.Count > 1 ? " · ещё " + Plural(adapters.Count - 1, "адаптер", "адаптера", "адаптеров") : "");
            SetCurrentIcon(true);

            if (current != null)
            {
                if (current.IsCustom)
                {
                    customPrimary.Text = main.Dns.Count > 0 ? main.Dns[0] : "";
                    customSecondary.Text = main.Dns.Count > 1 ? main.Dns[1] : "";
                }
                if (selected == null || ReferenceEquals(selected, current)) current.Card.IsChecked = true;
            }
            foreach (var p in providers) MarkCurrent(p);
            UpdateApply();
        }

        void SetCurrentIcon(bool ok)
        {
            currentIcon.Foreground = Br(ok ? CGreen : CRed);
            currentIconBg.Background = Br(ok ? Hex("#1A2C24") : Hex("#2B1D21"));
        }

        void MarkCurrent(Provider p)
        {
            var title = ((StackPanel)((StackPanel)((Grid)p.Card.Content).Children[0]).Children[0]);
            if (title.Children.Count > 0 && title.Children[title.Children.Count - 1] is Border)
                title.Children.RemoveAt(title.Children.Count - 1);
            if (!ReferenceEquals(p, current)) return;
            var badge = new Border
            {
                Background = Br(Hex("#1A2C24")), CornerRadius = new CornerRadius(6), Padding = new Thickness(6, 1, 6, 1),
                Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center,
                Child = Txt("сейчас", 10.5, CGreen, true)
            };
            title.Children.Add(badge);
        }

        // -------------------------------------------------- замер скорости

        async Task TestSpeed()
        {
            if (busy) return;
            busy = true;
            testBtn.IsEnabled = false;
            testBtnText.Text = "Проверяю…";
            speedHint.Text = "Отправляю настоящие DNS-запросы каждому серверу";
            foreach (var p in providers.Where(p => p.V4.Length > 0)) { p.Ms.Text = "…"; p.Ms.Foreground = Br(CFaint); }

            var jobs = providers.Where(p => p.V4.Length > 0).Select(p => new { P = p, T = Task.Run(() => Measure(p.V4[0])) }).ToList();
            foreach (var j in jobs)
            {
                double ms = await j.T;
                j.P.LastMs = ms;
                j.P.Ms.Text = ms < 0 ? "нет ответа" : ((int)Math.Round(ms)) + " мс";
            }

            var ok = providers.Where(p => p.LastMs >= 0).OrderBy(p => p.LastMs).ToList();
            foreach (var p in providers.Where(p => p.V4.Length > 0))
            {
                if (p.LastMs < 0) { p.Ms.Foreground = Br(CRed); continue; }
                double best = ok[0].LastMs;
                p.Ms.Foreground = Br(p.LastMs <= best * 1.5 + 5 ? CGreen : p.LastMs <= best * 3 + 10 ? CAmber : CSub);
            }
            speedHint.Text = ok.Count > 0
                ? "Быстрее всего у тебя: " + ok[0].Name + " (" + (int)Math.Round(ok[0].LastMs) + " мс)"
                : "Ни один сервер не ответил — проверь подключение";
            testBtnText.Text = "Проверить";
            testBtn.IsEnabled = true;
            busy = false;
            UpdateApply();
        }

        // Время ответа на DNS-запрос (лучшее из трёх), -1 — нет ответа.
        static double Measure(string server)
        {
            double best = -1;
            IPAddress ip;
            if (!IPAddress.TryParse(server, out ip)) return -1;
            string[] names = { "www.google.com", "www.wikipedia.org", "yandex.ru" };
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    using (var udp = new UdpClient(ip.AddressFamily))
                    {
                        udp.Client.ReceiveTimeout = 1500;
                        var query = BuildQuery(names[i]);
                        var ep = new IPEndPoint(ip, 53);
                        var sw = Stopwatch.StartNew();
                        udp.Send(query, query.Length, ep);
                        var from = new IPEndPoint(IPAddress.Any, 0);
                        var resp = udp.Receive(ref from);
                        sw.Stop();
                        if (resp.Length >= 12 && resp[0] == query[0] && resp[1] == query[1])
                        {
                            double ms = sw.Elapsed.TotalMilliseconds;
                            if (best < 0 || ms < best) best = ms;
                        }
                    }
                }
                catch { }
            }
            return best;
        }

        static byte[] BuildQuery(string name)
        {
            var b = new List<byte>();
            var id = new byte[2];
            new Random().NextBytes(id);
            b.AddRange(id);
            b.AddRange(new byte[] { 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            foreach (var label in name.Split('.'))
            {
                b.Add((byte)label.Length);
                b.AddRange(System.Text.Encoding.ASCII.GetBytes(label));
            }
            b.AddRange(new byte[] { 0x00, 0x00, 0x01, 0x00, 0x01 });
            return b.ToArray();
        }

        // -------------------------------------------------- применение

        async Task Apply()
        {
            if (busy || selected == null) return;
            adapters = ReadAdapters();
            if (adapters.Count == 0) { await Ask("Нет подключения", "Не нашёл активный сетевой адаптер с доступом в интернет.", "Понятно", null); return; }

            string[] servers;
            if (selected.IsAuto) servers = null;
            else if (selected.IsCustom)
            {
                servers = CustomAddresses();
                if (servers == null) { Toast("Проверь адреса: например 1.1.1.1"); return; }
            }
            else servers = selected.V4.Concat(selected.V6).ToArray();

            busy = true;
            applyBtn.IsEnabled = false;
            applyText.Text = "Применяю…";

            string idx = string.Join(",", adapters.Select(a => a.Index.ToString()));
            string script = servers == null
                ? "Set-DnsClientServerAddress -InterfaceIndex " + idx + " -ResetServerAddresses"
                : "Set-DnsClientServerAddress -InterfaceIndex " + idx + " -ServerAddresses @(" +
                  string.Join(",", servers.Select(s => "'" + s + "'")) + ")";
            script += "; Clear-DnsClientCache";

            var r = await Runner.PowerShell(script);
            busy = false;

            if (r.Key != 0)
            {
                // запасной путь через netsh — на случай урезанного PowerShell
                bool ok = await ApplyNetsh(servers);
                if (!ok)
                {
                    await Ask("Не получилось сменить DNS", ShortError(r.Value), "Понятно", null);
                    ReadCurrent();
                    return;
                }
            }

            await Task.Delay(400);
            ReadCurrent();
            Toast(selected.IsAuto ? "DNS снова автоматический" : "Готово: теперь " + (selected.IsCustom ? "свой DNS" : selected.Name));
        }

        async Task<bool> ApplyNetsh(string[] servers)
        {
            bool all = true;
            foreach (var a in adapters)
            {
                string n = Runner.Q(a.Name);
                if (servers == null)
                {
                    all &= (await Runner.Capture("netsh.exe", "interface ipv4 set dnsservers name=" + n + " source=dhcp", false)).Key == 0;
                    await Runner.Capture("netsh.exe", "interface ipv6 set dnsservers name=" + n + " source=dhcp", false);
                    continue;
                }
                var v4 = servers.Where(s => !s.Contains(":")).ToList();
                var v6 = servers.Where(s => s.Contains(":")).ToList();
                for (int i = 0; i < v4.Count; i++)
                {
                    string cmd = i == 0
                        ? "interface ipv4 set dnsservers name=" + n + " source=static address=" + v4[i] + " register=primary validate=no"
                        : "interface ipv4 add dnsservers name=" + n + " address=" + v4[i] + " index=" + (i + 1) + " validate=no";
                    all &= (await Runner.Capture("netsh.exe", cmd, false)).Key == 0;
                }
                for (int i = 0; i < v6.Count; i++)
                {
                    string cmd = i == 0
                        ? "interface ipv6 set dnsservers name=" + n + " source=static address=" + v6[i] + " register=primary validate=no"
                        : "interface ipv6 add dnsservers name=" + n + " address=" + v6[i] + " index=" + (i + 1) + " validate=no";
                    await Runner.Capture("netsh.exe", cmd, false); // IPv6 может быть выключен — это не ошибка
                }
            }
            await Runner.Capture("ipconfig.exe", "/flushdns", false);
            return all;
        }

        async Task Flush()
        {
            var r = await Runner.Capture("ipconfig.exe", "/flushdns", false);
            Toast(r.Key == 0 ? "Кеш DNS очищен" : "Не получилось очистить кеш");
        }

        static string ShortError(string s)
        {
            s = (s ?? "").Trim();
            if (s.Length == 0) return "Запусти программу от имени администратора и попробуй ещё раз.";
            var line = s.Split('\n').Select(l => l.Trim()).FirstOrDefault(l => l.Length > 0) ?? s;
            return line.Length > 220 ? line.Substring(0, 217) + "…" : line;
        }
    }
}
