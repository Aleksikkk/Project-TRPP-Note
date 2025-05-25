<h1 id="приложение-zametkiapp">Приложение &quot;ZametkiApp&quot;</h1>

<h2 id="-описание">📋 Описание</h2>
<p><strong>ZametkiApp</strong> — это настольное приложение на платформе .NET, разработанное с использованием Windows Forms. Оно позволяет пользователю:</p>
<ul>
<li>создавать, просматривать и редактировать заметки;</li>
<li>добавлять напоминания;</li>
<li>использовать календарь для планирования;</li>
<li>управлять профилями пользователей.</li>
</ul>
<p>Программа ориентирована на персональное планирование и удобную работу с заметками и задачами.</p>
<h2 id="-зависимости">⚙️ Зависимости</h2>
<ul>
<li>.NET SDK версии 8.0 или выше</li>
<li>Windows (приложение использует WinForms)</li>
</ul>
<h2 id="-инструкция-по-запуску">▶️ Инструкция по запуску</h2>
<ol>
<li>Установите <a href="https://dotnet.microsoft.com/en-us/download/dotnet/8.0">.NET 8 SDK</a> (если ещё не установлен).</li>
<li>Откройте командную строку (или PowerShell) в папке с проектом.</li>
<li>Выполните сборку и запуск:</li>
</ol>
<pre><code class="lang-bash">dotnet build ZametkiApp.sln
dotnet run --project &quot;ZametkiApp&quot;
</code></pre>
<blockquote>
<p>Также вы можете открыть проект в Visual Studio и запустить его через интерфейс IDE.</p>
</blockquote>
<h2 id="-комментарии-в-коде">👨‍💻 Комментарии в коде</h2>
<p>Все классы и методы снабжены XML-документацией (<code>/// &lt;summary&gt;...&lt;/summary&gt;</code>), аналогичной Javadoc и Python docstrings. Это позволяет легко генерировать документацию.</p>
<h2 id="-генерация-документации">📚 Генерация документации</h2>
<p>Для генерации документации рекомендуется использовать <a href="https://dotnet.github.io/docfx/">DocFX</a>:</p>
<pre><code class="lang-bash">dotnet tool install -g docfx
docfx init -q
docfx build
</code></pre>
<p>Документация будет доступна в папке <code>_site</code>.</p>
<hr>
<h2 id="-структура">📁 Структура</h2>
<ul>
<li><code>ZametkiApp.sln</code> — главное решение</li>
<li><code>bin/</code>, <code>obj/</code> — служебные директории сборки</li>
<li><code>publish</code> — скомпилированные файлы для запуска</li>
<li><code>_site</code> — папка с документацией</li>
<li><code>ZametkiApp</code> — папка с скриптами проекта</li>
<li><code>docfx.json</code> — файл с настройкой сборки документации</li>
<li><code>ZametkiApp.sln</code> — главное решение</li>
</ul>
