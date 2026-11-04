# DNS Switch by mysik

Смена DNS в Windows в один клик — без копания в настройках сетевых адаптеров.

## Что умеет

- **Готовые DNS** — Cloudflare, Google, Quad9, AdGuard, Яндекс, Comss или свой
- **Тест скорости** — настоящими DNS-запросами проверяет, какой DNS быстрее именно у вас
- **Текущий DNS** — показывает, что стоит сейчас
- **Все подключения сразу** — Wi-Fi и кабель
- **«Автоматически»** — возвращает всё как было
- **«Сбросить кеш»** — помогает, когда сайт сменил адрес и не открывается

## Какой выбрать

| DNS | Зачем |
|---|---|
| Cloudflare, Google | просто быстрый и стабильный |
| AdGuard | убирает рекламу почти везде, даже в приложениях |
| Quad9 | не пускает на сайты с вирусами |
| Comss | помогает открыть некоторые сервисы, недоступные из России. Если перестал работать — проверьте новые адреса на comss.ru и впишите их в «Свой DNS» |

DNS не обходит замедление YouTube и Discord — для этого есть [Zapret GUI](https://mysik-zapret.netlify.app). Вместе они работают нормально.

## Запуск

При запуске разрешите права администратора — без них Windows не даёт менять DNS.

**Если что-то пошло не так:** выберите «Автоматически» и нажмите «Применить» — DNS вернётся к тому, что выдаёт роутер или провайдер.

## Сборка

Нужна только Windows 10 или 11 — компилятор .NET Framework 4.x уже встроен в систему.

1. Скачайте репозиторий: зелёная кнопка **Code → Download ZIP**, и распакуйте.
2. Запустите `build.bat` — за несколько секунд рядом появится `DnsSwitch.exe`.
3. Перенесите `DnsSwitch.exe` куда удобно и запускайте.

Файлы в репозитории: `DnsSwitch.cs`, `Shell.cs`, `MainWindow.xaml`, `Theme.xaml`, `app.manifest`, `app.ico`, `build.bat`.

## Другие программы by mysik

- [Zapret GUI](https://mysik-zapret.netlify.app) — возвращает Discord, YouTube и Steam без VPN
- [Video Loader](https://github.com/mmmaksmog-commits/video-loader) — скачивание видео и музыки
- [Converter](https://github.com/mmmaksmog-commits/converter) — конвертер видео, музыки и картинок
- [Win Cleaner](https://github.com/mmmaksmog-commits/win-cleaner) — очистка и настройка Windows
- [DNS Switch](https://github.com/mmmaksmog-commits/dns-switch) — смена DNS в один клик

Все программы — на сайте **[mysik-zapret.netlify.app](https://mysik-zapret.netlify.app)**.
Сделано [mysik](https://mysik-portfolio.vercel.app).
