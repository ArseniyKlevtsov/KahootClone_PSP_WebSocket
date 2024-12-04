const socket = new WebSocket('ws://127.0.0.1:7890/kahoot');
let mode = "";
let myId = "";
let myName = "";
let roomName = "";

// creator
let questionCount = 0;
let respondentsCount = 0;
let currentQuestionIndex = 0;

//player
let currentQuestionId = "";

socket.onopen = function () {
    showStartForm();
};

socket.onmessage = function (event) {
    const message = JSON.parse(event.data);
    console.log('Message from server: ', message);
    handleMessage(message);
};

function sendMessage(messageSrt) {
    const jsonString = JSON.stringify(messageSrt);
    socket.send(jsonString);
}

function handleMessage(message) {

    switch (message.Command) {
        // all modes
        case "PlayerJoined":
            PlayerJoinedHandler(message); break;
        case "NameChanged":
            NameChangedHandler(message); break;
        // creator mode
        case "RoomCreated":
            RoomCreatedHandler(message); break;
        case "QuestionCreated":
            QuestionCreatedHandler(message); break;
        case "RoomOpenned":
            RoomOpennedHandler(message); break;
        case "GameStartedInRoom":
            GameStartedInRoomHandler(message); break;
        // creator/player mode
        case "ConectedToRoom":
            ConnectToRoomHandler(message); break;
        case "NextQuestion":
            NextQuestionHandler(message); break;
        case "AnswerSaved":
            AnswerSavedHandler(message); break;
        case "QuestionStats":
            QuestionStatsHandler(message); break;
        case "EndGameStats":
            EndGameStatsHandler(message); break;
        default:
            break;
    }

}

// Handlers
function PlayerJoinedHandler(message) {
    myId = message.Data.Id;
}

function NameChangedHandler(message) {
    myName = message.Data;
    showRoomForm();
}

function RoomCreatedHandler(message) {
    mode = "creator";
    roomName = message.Data.Name;
    showRoom(message.Data.Name);
}

function QuestionCreatedHandler(message) {

    const { Id, Question, CorrectAnswerIndex, Answers } = message.Data;

    // Создаем HTML для нового вопроса
    const questionHtml = `
        <div class="question-item border p-2 mb-2">
            <h5>${Question}</h5>
            <ul class="no-bullets">
                ${Answers.map((answer, index) => `
                    <li ${index === CorrectAnswerIndex ? 'style="font-weight: bold;"' : ''}>
                        ${index + 1}. ${answer}
                    </li>
                `).join('')}
            </ul>
        </div>
    `;
    questionCount++;

    const questionCountBox = document.getElementById('questionCountBox');
    const questionNumber = document.getElementById('questionNumber');

    questionNumber.innerText = `Текущий номер вопроса: ${currentQuestionIndex}/${questionCount}`;
    questionCountBox.innerText = `Вопросы(${questionCount})`;

    // Добавляем новый вопрос в questionsBlock
    const questionsBlock = document.getElementById('questionsBlock');
    questionsBlock.insertAdjacentHTML('beforeend', questionHtml);
}

function RoomOpennedHandler(message) {
    const statusBox = document.getElementById('statusBox');
    statusBox.innerText = 'Статус: ожидание игроков';

    const createQuestionButton = document.getElementById('createQuestionButton');
    const openButton = document.getElementById('openButton');
    const startButton = document.getElementById('startButton');

    createQuestionButton.disabled = true;
    openButton.disabled = true;
    startButton.disabled = false;
}

function GameStartedInRoomHandler(message) {
    const statusBox = document.getElementById('statusBox');
    statusBox.innerText = 'Статус: идёт игра';

    if (mode === "creator") {
        const startButton = document.getElementById('startButton');
        const nextQuestionButton = document.getElementById('nextQuestionButton');

        startButton.disabled = true;
        nextQuestionButton.disabled = false;
    }
}

function NextQuestionHandler(message) {
    if (mode === "creator") {
        currentQuestionIndex++;
        respondentsCount = 0;

        const questionNumber = document.getElementById('questionNumber');
        const respondentsCountDiv = document.getElementById('respondentsCount');
        const nextQuestionButton = document.getElementById('nextQuestionButton');
        const statsButton = document.getElementById('statsButton');

        questionNumber.innerText = `Текущий номер вопроса: ${currentQuestionIndex}/${questionCount}`;
        respondentsCountDiv.innerText = "Ответивших: "+ respondentsCount;
        nextQuestionButton.disabled = true;
        statsButton.disabled = false;
    }
    if (mode === "player") {
        currentQuestionId = message.Data.Id;
        showQuestion(message, "actionBoxId")
    }
}

function AnswerSavedHandler(message) {
    if (mode === "creator") {
        const respondentsCountDiv = document.getElementById('respondentsCount');
        respondentsCount++;
        respondentsCountDiv.innerText = respondentsCount;
    }
    if (mode === "player") {
        const container = document.getElementById("actionBoxId");
        
        // Форматирование времени
        const answerTime = new Date(message.Data.AnswerTime);
        const formattedTime = answerTime.toLocaleTimeString('ru-RU', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    
        // Создаем красивый вывод
        container.innerHTML = `
            <div class="alert alert-success" role="alert">
                Ответ сохранён: <strong>№${message.Data.AnswerIndex + 1}</strong> 
                в <strong>${formattedTime}</strong>
            </div>
        `;
    }
}

function QuestionStatsHandler(message) {
    if (mode === "creator") {
        const statsButton = document.getElementById('statsButton');
        statsButton.disabled = true;
        if (currentQuestionIndex === questionCount) {
            const endButton = document.getElementById('endButton');
            endButton.disabled = false;
        }
        else {
            const nextQuestionButton = document.getElementById('nextQuestionButton');
            nextQuestionButton.disabled = false;
        }
        ShowQuestionStats(message, 'questionStatsBlock');
    }
    if (mode === "player") {
        ShowQuestionStats(message, 'actionBoxId');
    }
}

function ConnectToRoomHandler(message) {
    if (message.Data.ConectedPlayerName === myName) {
        mode = "player";
        roomName = message.Data.RoomName;
        showPlayerRoom(message);
    }
    const playersList = document.getElementById('playersList');
    const playersCountBox = document.getElementById('playersCountBox');

    const data = message.Data;

    playersList.innerHTML = ''; // Очищаем предыдущий список пользователей
    playersCountBox.innerHTML = "Игроки (" + data.UserNames.length + ")";

    // Фиксированные цвета для пользователей
    const colors = [
        '#FFB3BA', '#FFDFBA', '#FFFFBA',
        '#BAFFC9', '#BAE1FF', '#FFBAF1',
        '#FFC3A0', '#FF677D', '#D4A5A5',
        '#392F5A'
    ];

    // Генерация цветов для пользователей
    const generateColor = (index) => colors[index % colors.length];

    // Создаем блоки для пользователей
    data.UserNames.forEach((userName, index) => {
        const userBlock = document.createElement('div');
        userBlock.innerText = userName;
        userBlock.style.backgroundColor = generateColor(index);
        userBlock.style.borderRadius = '15px'; // Скругленные углы
        userBlock.style.padding = '10px 15px'; // Отступы
        userBlock.style.margin = '5px'; // Отступ между блоками
        userBlock.style.display = 'inline-block'; // Выравнивание по центру
        userBlock.style.textAlign = 'center'; // Центрирование текста
        userBlock.style.fontWeight = 'bold'; // Жирный текст

        playersList.appendChild(userBlock);
    });
}

function EndGameStatsHandler(message) {
    if (mode === "creator") {
        const endButton = document.getElementById('endButton');
        endButton.disabled = true;
        ShowEndGameStats(message, "questionStatsBlock");
    }
    if (mode === "player") {
        ShowEndGameStats(message, 'actionBoxId');
    }
}

// Show Funcs
function showStartForm() {
    const container = document.getElementById('body');

    // Удаляем всё содержимое контейнера
    container.innerHTML = '';

    // Создаем форму
    const formHTML = `
        <div class="form-wrapper">
            <form class="enter-form">
                <h2> Kahoot!</h2>
                <div class="form-group">
                    <input type="text" class="form-control" id="nameInput" placeholder="Ваше имя" required>
                </div>
                <button type="submit" class="btn btn-purple">Проложить</button>
            </form>
        </div>
    `;

    // Вставляем форму в контейнер
    container.innerHTML = formHTML;

    // Обработчик события для формы
    container.querySelector('form').addEventListener('submit', function (event) {
        event.preventDefault();
        const nameInputValue = document.getElementById('nameInput').value;
        let request = {
            Command: "ChangePlayerName",
            Data: {
                NewPlayerName: nameInputValue
            }
        };
        sendMessage(request);
    });
}

function showRoomForm() {
    const container = document.getElementById('body');
    container.innerHTML = `
    <div class="form-wrapper">
        <form class="text-center enter-form">
            <h2> Kahoot!</h2>
            <div class="form-group room-input">
                <input type="text" class="form-control" id="roomInput" placeholder="Введите название комнаты" required>
            </div>
                <div class="d-flex justify-content-between mt-3">
                    <button type="button" class="btn btn-purple room-form-btn" id="createButton">Создать</button>
                    <button type="button" class="btn btn-purple room-form-btn" id="joinButton">Войти</button>
                </div>
        </form>
    </div>
    `;

    // Обработчик для кнопки "Создать"
    document.getElementById('createButton').addEventListener('click', function () {
        const roomInputValue = document.getElementById('roomInput').value;

        let request = {
            Command: "CreateRoom",
            Data: {
                Name: roomInputValue
            }
        };

        sendMessage(request);
    });

    // Обработчик для кнопки "Войти"
    document.getElementById('joinButton').addEventListener('click', function () {
        const roomInputValue = document.getElementById('roomInput').value;
        let request = {
            Command: "ConnectToRoom",
            Data: {
                RoomName: roomInputValue
            }
        };

        sendMessage(request);
    });
}

function showRoom(RoomName) {
    const container = document.getElementById('body');

    container.innerHTML = `
        <div class="navbar">
            Kahoot
        </div>
        <div class="main-container" id="main-container">
            <div class="d-flex flex-column align-items-center" ">
                <h2 class="text-center mt-3">Комната: ${RoomName}</h2>
                <h5 class="text-center mt-3" id="statusBox">Статус: Создание комнаты</h5>
                <h4 class="text-center mt-3" id="playersCountBox">Игроки(0)</h4>
                <div id="playersList" class="border w-100" style="min-height: 50px;"></div>
                
                <form class="w-100 mt-3" id="questionForm">
                    <input type="text" class="form-control" id="questionInput" placeholder="Введите вопрос" required>
                    
                    <div class="mt-3">
                        <h5>Ответы:</h5>
                        <div class="form-group">
                            <div class="input-group mb-2">
                                <input type="text" class="form-control" placeholder="Ответ 1" required>
                                <div class="input-group-append">
                                    <div class="input-group-text">
                                        <input type="radio" name="correctAnswer" value="0">
                                    </div>
                                </div>
                            </div>
                            <div class="input-group mb-2">
                                <input type="text" class="form-control" placeholder="Ответ 2" required>
                                <div class="input-group-append">
                                    <div class="input-group-text">
                                        <input type="radio" name="correctAnswer" value="1">
                                    </div>
                                </div>
                            </div>
                            <div class="input-group mb-2">
                                <input type="text" class="form-control" placeholder="Ответ 3" required>
                                <div class="input-group-append">
                                    <div class="input-group-text">
                                        <input type="radio" name="correctAnswer" value="2">
                                    </div>
                                </div>
                            </div>
                            <div class="input-group mb-2">
                                <input type="text" class="form-control" placeholder="Ответ 4" required>
                                <div class="input-group-append">
                                    <div class="input-group-text">
                                        <input type="radio" name="correctAnswer" value="3">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <button type="button" class="btn btn-purple w-100" id="createQuestionButton">Создать вопрос</button>
                </form>

                <h4 class="text-center mt-3" id="questionCountBox">Вопросы(0)</h4>
                <div id="questionsBlock" class="border w-100 mt-3" style="min-height: 50px;"></div>

                <div class="d-flex justify-content-between w-100 mt-3">
                    <button type="button" class="btn btn-purple" id="openButton">Открыть</button>
                    <button type="button" class="btn btn-purple" id="startButton" disabled>Старт</button>
                    <button type="button" class="btn btn-purple" id="nextQuestionButton" disabled>След. вопрос</button>
                    <button type="button" class="btn btn-purple" id="statsButton" disabled>Статис. вопроса</button>
                    <button type="button" class="btn btn-purple" id="endButton" disabled>Конец</button>
                </div>

                <div class="d-flex justify-content-center mt-3">
                    <div class="mx-3" id="questionNumber">Текущий номер вопроса: 0/0</div>
                    <div class="mx-3" id="respondentsCount">Ответивших: 0</div>
                </div>

                <div class="mt-3 w-100 text-center">
                    <h5>Статистика</h5>
                    <div id="questionStatsBlock"></div>
                </div>
            </div>
        </div>
    `;

    document.getElementById('createQuestionButton').addEventListener('click', function () {
        const questionInputValue = document.getElementById('questionInput').value;
        const answerInputs = document.querySelectorAll('.form-group .form-control');
        const answers = Array.from(answerInputs).map(input => input.value);

        const correctAnswer = document.querySelector('input[name="correctAnswer"]:checked');
        const correctAnswerIndex = correctAnswer ? parseInt(correctAnswer.value) : 0; // Преобразуем в индекс

        if (!questionInputValue || answers.some(answer => answer.trim() === '') || correctAnswerIndex === null) {
            alert('Пожалуйста, заполните все поля и выберите правильный ответ.');
            return;
        }

        let request = {
            Command: "AddQuestionToRoom",
            Data: {
                RoomName: roomName,
                CreatorId: myId,
                Question: questionInputValue,
                CorrectAnswerIndex: correctAnswerIndex,
                Answers: answers
            }
        };

        sendMessage(request);
    });
    document.getElementById('openButton').addEventListener('click', function () {
        let request = {
            Command: "OpenRoom",
            Data: {
                RoomName: roomName,
                CreatorId: myId
            }
        };

        sendMessage(request);
    });
    document.getElementById('startButton').addEventListener('click', function () {
        let request = {
            Command: "StartGameInRoom",
            Data: {
                RoomName: roomName,
                CreatorId: myId
            }
        };

        sendMessage(request);
    });
    document.getElementById('nextQuestionButton').addEventListener('click', function () {
        let request = {
            Command: "NextQuestionInRoom",
            Data: {
                RoomName: roomName,
                CreatorId: myId
            }
        };

        sendMessage(request);
    });
    document.getElementById('statsButton').addEventListener('click', function () {
        let request = {
            Command: "ShowQuestionStats",
            Data: {
                RoomName: roomName,
                CreatorId: myId
            }
        };

        sendMessage(request);
    });
    document.getElementById('endButton').addEventListener('click', function () {
        let request = {
            Command: "EndGameInRoom",
            Data: {
                RoomName: roomName,
                CreatorId: myId
            }
        };

        sendMessage(request);
    });
}

function showPlayerRoom(message) {
    const container = document.getElementById('body');
    container.innerHTML = `
    <div class="navbar">
        Kahoot
    </div>
    <div class="main-container" id="main-container">
        <div class="d-flex flex-column align-items-center" ">
            <h2 class="text-center mt-3">Комната: ${message.Data.RoomName}</h2>
            <h6 class="text-center mt-3">Автор: ${message.Data.CreatorName}</h6>
            <h6 class="text-center mt-3">Вы: ${message.Data.ConectedPlayerName}</h6>
            <h5 class="text-center mt-3" id="statusBox">Статус: Ожидание игроков</h5>
            <h4 class="text-center mt-3" id="playersCountBox">Игроки(0)</h4>
            <div id="playersList" class="border w-100" style="min-height: 50px;"></div>  
            <div id="actionBoxId" class="border w-100" style="min-height: 50px;"></div> 
        </div>
    </div>
`;
}

function ShowQuestionStats(message, blokId) {
    // Находим контейнер по id
    const container = document.getElementById(blokId);
    // Очищаем контейнер
    container.innerHTML = '';
    container.style.background = "#800566";
    // Извлекаем данные из сообщения
    const data = message.Data;

    // Создаем элемент для правильного ответа
    const correctAnswerElement = document.createElement('h5');
    correctAnswerElement.innerText = `Правильный ответ: ${data.CorrectAnswer}`;
    correctAnswerElement.style.margin = "0";
    correctAnswerElement.className = 'text-center mt-3 text-white'; // Стиль Bootstrap
    correctAnswerElement.style.background = "#800566";
    container.appendChild(correctAnswerElement);

    // Создаем таблицу для результатов
    const scoreTable = document.createElement('table');
    scoreTable.className = 'table table-striped mt-3'; // Используем классы Bootstrap

    // Создаем заголовок таблицы
    const thead = document.createElement('thead');
    thead.innerHTML = `
        <tr>
            <th class ="text-white">Игрок</th>
            <th class ="text-white">Общий балл</th>
            <th class ="text-white">Очки за вопрос</th>
            <th class ="text-white">Ответ</th>
        </tr>
    `;
    scoreTable.appendChild(thead);

    // Создаем тело таблицы
    const tbody = document.createElement('tbody');
    // Перебираем таблицу результатов игроков
    data.PlayerScoreBoard.forEach((playerStats, index) => {
        const row = document.createElement('tr');
        row.style.background = "linear-gradient(to right, #ffffff, #f0f0f0)";

        // Устанавливаем цвет фона в зависимости от места
        if (index === 0) {
            row.style.background = 'linear-gradient(to right, #FFD700, #FFA500)'; // Золотой градиент для первого места
        } else if (index === 1) {
            row.style.background = 'linear-gradient(to right, #C0C0C0, #A9A9A9)'; // Серебряный градиент для второго места
        } else if (index === 2) {
            row.style.background = 'linear-gradient(to right, #cd7f32, #8B4513)'; // Бронзовый градиент для третьего места
        }

        // Проверяем имя игрока на совпадение с myName
        if (playerStats.PlayerName === myName) {
            row.style.border = '4px solid #05801f'; // Ярко-зеленая рамка для текущего игрока
            row.style.fontWeight = 'bold'; // Выделяем жирным шрифтом
            row.style.color = "#800565";
            row.style.fontSize = "20px";
        }

        // Заполняем ячейки строки
        row.innerHTML = `
            <td>${index + 1}.${playerStats.PlayerName}</td>
            <td>${playerStats.TotalScore} очков</td>
            <td>${playerStats.QuestionScore} очков</td>
            <td class="${playerStats.IsCorrectAnswer ? 'text-success' : 'text-danger'}">
                ${playerStats.PlayerAnswerIndex === -1 ? '-' : playerStats.PlayerAnswerIndex + 1}
            </td>
        `;
        tbody.appendChild(row);
    });

    // Добавляем тело таблицы в таблицу
    scoreTable.appendChild(tbody);
    // Добавляем таблицу в контейнер
    container.appendChild(scoreTable);
}

function ShowEndGameStats(message, blokId) {
    // Находим контейнер по id
    const container = document.getElementById(blokId);
    // Очищаем контейнер
    container.innerHTML = '';
    container.style.background = "#800566";

    // Извлекаем данные из сообщения
    const data = message.Data;

    // Определяем победителя
    const winner = data.PlayerScoreBoard.length > 0 ? data.PlayerScoreBoard[0].PlayerName : 'Нет победителей';

    // Создаем элемент для победителя
    const winnerElement = document.createElement('h5');
    winnerElement.innerText = `Победитель: ${winner}`;
    winnerElement.style.margin = "0";
    winnerElement.className = 'text-center mt-3 text-white';
    container.appendChild(winnerElement);

    // Создаем таблицу для результатов
    const scoreTable = document.createElement('table');
    scoreTable.className = 'table table-striped mt-3';

    // Создаем заголовок таблицы
    const thead = document.createElement('thead');
    thead.innerHTML = `
        <tr>
            <th class="text-white">Игрок</th>
            <th class="text-white">Общий балл</th>
        </tr>
    `;
    scoreTable.appendChild(thead);

    // Создаем тело таблицы
    const tbody = document.createElement('tbody');
    // Перебираем таблицу результатов игроков
    data.PlayerScoreBoard.forEach((playerStats, index) => {
        const row = document.createElement('tr');
        row.style.background = "linear-gradient(to right, #ffffff, #f0f0f0)";

        // Устанавливаем цвет фона в зависимости от места
        if (index === 0) {
            row.style.background = 'linear-gradient(to right, #FFD700, #FFA500)'; // Золотой градиент для первого места
        } else if (index === 1) {
            row.style.background = 'linear-gradient(to right, #C0C0C0, #A9A9A9)'; // Серебряный градиент для второго места
        } else if (index === 2) {
            row.style.background = 'linear-gradient(to right, #cd7f32, #8B4513)'; // Бронзовый градиент для третьего места
        }

        // Проверяем имя игрока на совпадение с myName
        if (playerStats.PlayerName === myName) {
            row.style.border = '4px solid #05801f'; // Ярко-зеленая рамка для текущего игрока
            row.style.fontWeight = 'bold'; // Выделяем жирным шрифтом
            row.style.color = "#800565";
            row.style.fontSize = "20px";
        }

        // Заполняем ячейки строки
        row.innerHTML = `
            <td>${index + 1}. ${playerStats.PlayerName}</td>
            <td>${playerStats.TotalScore} очков</td>
        `;
        tbody.appendChild(row);
    });

    // Добавляем тело таблицы в таблицу
    scoreTable.appendChild(tbody);
    // Добавляем таблицу в контейнер
    container.appendChild(scoreTable);
}

function showQuestion(message, blokId) {
    // Находим контейнер по id
    const container = document.getElementById(blokId);
    // Очищаем контейнер
    container.innerHTML = '';

    // Извлекаем данные из сообщения
    const data = message.Data;

    // Создаем элемент для вопроса
    const questionElement = document.createElement('h3');
    questionElement.innerText = data.Question;
    questionElement.className = 'text-center mt-3';
    container.appendChild(questionElement);

    // Создаем кнопки для ответов
    data.Answers.forEach((answer, index) => {
        const button = document.createElement('button');
        button.innerText = (index + 1) + ". " + answer;
        button.className = `btn btn-block mt-2`;
        button.style.textAlign = 'left';
        button.style.fontStyle = 'bold';

        // Устанавливаем разные цвета для кнопок
        switch (index) {
            case 0:
                button.style.backgroundColor = '#ff9999'; // Light Red
                break;
            case 1:
                button.style.backgroundColor = '#9999ff'; // Light Blue
                break;
            case 2:
                button.style.backgroundColor = '#99ff99'; // Light Green
                break;
            case 3:
                button.style.backgroundColor = '#ffff99'; // Light Yellow
                break;
        }

        // Заглушка для обработчика события
        button.onclick = function () {
            console.log(`Выбран ответ: ${answer}`);

            let request = {
                Command: "Answer",
                Data: {
                    RoomName: roomName,
                    AnswerIndex: index,
                    QuestionId: currentQuestionId
                }
            };
            container.innerHTML = 'Ответ отправлен';
            sendMessage(request);
        };

        container.appendChild(button);
    });
}

//showStartForm()
//showRoomForm();

const testMessage = {
    Command: "ShowQuestionStats",
    Data: {
        CorrectAnswer: "Ответ A",
        PlayerScoreBoard: [
            { TotalScore: 300, QuestionScore: 100, PlayerAnswerIndex: 1, IsCorrectAnswer: true, PlayerName: "Игрок1" },
            { TotalScore: 250, QuestionScore: 80, PlayerAnswerIndex: 2, IsCorrectAnswer: false, PlayerName: "Игрок2" },
            { TotalScore: 200, QuestionScore: 70, PlayerAnswerIndex: 3, IsCorrectAnswer: false, PlayerName: "Игрок3" },
            { TotalScore: 200, QuestionScore: 70, PlayerAnswerIndex: 3, IsCorrectAnswer: false, PlayerName: "Игрок3" },
            { TotalScore: 200, QuestionScore: 70, PlayerAnswerIndex: 3, IsCorrectAnswer: false, PlayerName: "Игрок3" },
            { TotalScore: 150, QuestionScore: 60, PlayerAnswerIndex: 4, IsCorrectAnswer: false, PlayerName: "Игрок4" }
        ]
    }
};

const testMessage2 = {
    Command: "ConnectToRoom",
    Data: {
        UserNames: ["Игрок1", "Игрок2", "Игрок3", "Игрок4", "Игрок5", "Игрок6", "Игрок6", "Игрок6", "Игрок6", "Игрок6", "Игрок6", "Игрок6"],
        CreatorName: "Создатель1",
        RoomName: "Комната 1"
    }
};

showRoom("leka")
ShowQuestionStats(testMessage, "questionStatsBlock")
ConnectToRoomHandler(testMessage2);
