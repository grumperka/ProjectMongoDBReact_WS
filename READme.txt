Aby uruchomić projekt należy wykonać następujące kroki:

0. Utorzyć na dysku C katalog data/db.

1. Uruchomić plik mongod.exe w folderze 

C:\Program Files\MongoDB\Server\3.6\bin

2. Należy otworzyć folder Projekt_WS w wierszu polecenia.

3. Należy zainicjować bazę danych w MongoDB

- w wierszu poleceń należy wpisać poniższą komendę

	node InitDatabase.js
	
(jeżeli wystąpił błąd, należy wykonać instrukcję
npm install mongodb
)

4. Należy uruchomić projekt z folderu ProjectMongoDBReact w środowisku Visual Studio

5. By przetestować aplikację, należy się zarejestrować za pomocą dowolnego adresu e-mail i hasła.
Stworzone konto będzie kontem klienta hotelu.

6. By zalogować się jako pracownik, należy zarejestrować się mailem z bazy danych, z kolekcji "pracownicy", np:

e-mail: 
kowalski.jan@gmail.com

hasło: 
kowalski.Jan1
