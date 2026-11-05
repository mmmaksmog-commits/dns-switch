# DNS Switch by mysik

Смена DNS в Windows в один клик, без копания в настройках сетевых адаптеров. Программа сама замеряет, какой DNS быстрее именно у вас.

**Страница программы и скачивание:** https://mysik-portfolio.vercel.app/soft/dns-switch.html

## Что умеет

- **Готовые DNS** — cloudflare, Google, Quad9, AdGuard, Яндекс, Comss или свой.
- **Тест скорости** — настоящими DNS-запросами проверяет, какой сервер отвечает быстрее.
- **Все подключения** — применяет сразу к Wi-Fi и кабелю.
- **Текущий DNS** — видно, что стоит сейчас.
- **«Автоматически»** — возвращает всё как было, к DNS роутера или провайдера.
- **Сброс кеша** — помогает, когда сайт сменил адрес и не открывается.

## Как начать

Нужна только Windows 10 или 11 — компилятор .NET Framework 4.x уже встроен в систему.

1. Скачайте репозиторий: зелёная кнопка **Code → Download ZIP**, и распакуйте.
2. Запустите `build.bat` — через пару секунд рядом появится `DnsSwitch.exe`.
3. Откройте `DnsSwitch.exe`. Разрешите права администратора: без них программа не сможет работать.
4. **Нажмите «Проверить скорость».** Программа покажет, какой DNS быстрее.
5. **Выберите и примените.** AdGuard убирает рекламу, Quad9 защищает от вирусных сайтов, Cloudflare и Google просто быстрые.

## Полезно знать

- DNS не обходит замедление YouTube и Discord — для этого есть Zapret GUI. Вместе они работают нормально.
- Что-то пошло не так — выберите «Автоматически» и нажмите «Применить».

Файлы: `DnsSwitch.cs`, `MainWindow.xaml`, `README.txt`, `Shell.cs`, `Theme.xaml`, `app.ico`, `app.manifest`, `build.bat`.

## Другие программы by mysik

- [Zapret GUI](https://mysik-zapret.netlify.app) — Возвращает Discord, YouTube и Steam одной кнопкой. Без VPN: скорость и IP остаются вашими.
- [Пинг](https://mysik-portfolio.vercel.app/soft/ping.html) — Следит за интернетом каждую секунду и подсказывает, где проблема: Wi-Fi, роутер или провайдер.
- [Передача](https://mysik-portfolio.vercel.app/soft/peredacha.html) — Файлы, фото и тексты между компьютером и телефоном по QR-коду. Без проводов и облаков.
- [Video Loader](https://mysik-portfolio.vercel.app/soft/video-loader.html) — Скачивает видео и музыку с YouTube, VK Видео, RuTube и TikTok: до 4K, плейлисты, MP3 с обложкой.
- [Converter](https://mysik-portfolio.vercel.app/soft/converter.html) — Видео, музыка и картинки в нужный формат, включая HEIC с iPhone. Умеет сжимать перед отправкой.
- [Снимок](https://mysik-portfolio.vercel.app/soft/snimok.html) — Скриншот области по PrintScreen, стрелки, размытие и нумерация. Сразу в буфер.
- [Win Cleaner](https://mysik-portfolio.vercel.app/soft/win-cleaner.html) — Чистит мусор, убирает рекламу и Copilot, наводит порядок в автозагрузке. Всё можно вернуть.
- [Диск](https://mysik-portfolio.vercel.app/soft/disk.html) — Карта диска за минуту: видно, какие папки и файлы съели всё место.
- [Раскладка](https://mysik-portfolio.vercel.app/soft/raskladka.html) — Напечатали «ghbdtn»? Одна клавиша — и слово исправлено, раскладка переключена.
- [Буфер](https://mysik-portfolio.vercel.app/soft/bufer.html) — Помнит всё, что вы копировали: текст, ссылки, картинки. Ctrl+Shift+V — и вставлено.
- [Визитка](https://mysik-portfolio.vercel.app/soft/vizitka.html) — Конструктор сайтов для мастеров: маникюр, барбер, массаж. Сайт под ключ за 15–30 минут.

Все программы — в портфолио: https://mysik-portfolio.vercel.app/#programs  
Сделано [mysik](https://mysik-portfolio.vercel.app).
