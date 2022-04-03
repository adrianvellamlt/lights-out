window.onload = () =>
{
    fetch("/api/gamesettings")
        .then(response => response.json())
        .then(data => {
            if (!data.length) return;

            const list = document.querySelector("#gameSelection ul");

            const firstId = data[0].id;

            for (const gameSetting of data)
            {
                const text = `${gameSetting.noOfRows} x ${gameSetting.noOfColumns} - Time Limit: ${gameSetting.gameMaxDuration}`;

                const li = document.createElement("li");

                const a = document.createElement("a");

                a.classList.add("dropdown-item");

                if (gameSetting.id === firstId) a.classList.add("active");

                a.href = "#";

                a.setAttribute("gameId", gameSetting.id);

                a.appendChild(document.createTextNode(text));

                li.appendChild(a);

                list.appendChild(li);
            }

            for (const aTag of document.querySelectorAll("#gameSelection a"))
            {
                aTag.onclick = (e) =>
                {
                    const thisGameId = e.currentTarget.getAttribute("gameId");

                    for(const a of document.querySelectorAll("#gameSelection a"))
                    {
                        if (a.getAttribute("gameId") !== thisGameId)
                        {
                            a.classList.remove("active");
                        }
                    }

                    e.currentTarget.classList.add("active");
                };
            }
        });
};

const startGame = (btn) =>
{
    btn.hidden = true;

    document.getElementById("surrender").hidden = false;
    document.getElementById("gameSelection").hidden = true;

    const gameId = document.querySelector("#gameSelection .active").getAttribute("gameId");

    const username = document.getElementById("username").value;

    fetch(
            `/api/game/play/${gameId}`,
            {
                method: 'POST',
                body: `{ "username": "${username}" }`,
                headers: {
                    'Content-Type': 'application/json'
                },
            }
        )
        .then(response => {
            if (response.status === 404)
            {
                game.prepend(
                    "Game Over! You took too long to complete the puzzle", 
                    document.createElement("br"),
                    document.createElement("br")
                );

                document.getElementById("start").hidden = true;
                document.getElementById("surrender").hidden = false;
                document.getElementById("gameSelection").hidden = true;

                return;
            }

            const gameStateId = response.headers.get("x-gamestateid");

            const game = document.getElementsByTagName("game")[0];

            game.id = gameStateId;

            return response.text();
        })
        .then(data => {

            const game = document.getElementsByTagName("game")[0];

            window.gameStartTime = new Date();
            
            game.innerHTML = data;
            
            game.prepend(
                `Moves: 0`, 
                document.createElement("br"),
                `Started: ${window.gameStartTime.toLocaleString()}`,
                document.createElement("br"),
                document.createElement("br")
            );

            const gameWrapper = game.querySelector(".wrapper");

            const rows = parseInt(gameWrapper.getAttribute("rows"));

            const columns = parseInt(gameWrapper.getAttribute("columns"));

            const rowSize = 100 / rows;

            const columnsSize = 100 / columns;

            gameWrapper.style.cssText = `grid-template-columns: repeat(${columns}, ${columnsSize}%);grid-template-rows: repeat(${rows}, ${rowSize}%);`;

            for (const cell of document.querySelectorAll("game .wrapper .cell"))
            {
                cell.setAttribute("onclick","toggleCell(this);");
            }
        });
};

const surrender = (btn) =>
{
    btn.hidden = true;

    document.getElementById("start").hidden = false;
    document.getElementById("surrender").hidden = true;
    document.getElementById("gameSelection").hidden = false;

    const game = document.getElementsByTagName("game")[0];

    fetch(`/api/game/surrender/${game.id}`, { method: 'POST' })
        .then(_ => game.innerHTML = "Game Over! The puzzle beat you!");
};

const toggleCell = (cell) =>
{
    const row = parseInt(cell.id.substring(1, 2));

    const column = parseInt(cell.id.substring(3, 4));

    const game = document.getElementsByTagName("game")[0];

    let remainingSeconds = undefined;
    let moveCount = undefined;
    let isSolved = false;

    fetch(
            `/api/game/toggle/${game.id}`,
            {
                method: 'POST',
                body: `{ "rowNumber": ${row}, "columnNumber": ${column} }`,
                headers: {
                    'Content-Type': 'application/json'
                },
            }
        )
        .then(response => {
            if (response.status === 404)
            {
                game.prepend(
                    "Game Over! You took too long to complete the puzzle", 
                    document.createElement("br"),
                    document.createElement("br")
                );
                
                document.getElementById("start").hidden = false;
                document.getElementById("surrender").hidden = true;
                document.getElementById("gameSelection").hidden = false;

                return;
            }

            remainingSeconds = parseInt(response.headers.get("x-remainingtime"));

            moveCount = parseInt(response.headers.get("x-movecount"));

            isSolved = parseInt(response.headers.get("x-issolved")) === 1;
            
            return response.text();
        })
        .then(data => {
            const gameWrapper = game.querySelector(".wrapper");

            const styling = gameWrapper.style.cssText;
            
            game.innerHTML = data;

            if (remainingSeconds === 0)
            {
                game.prepend(
                    "Game Over! You took too long to complete the puzzle", 
                    document.createElement("br"),
                    document.createElement("br")
                );

                document.getElementById("start").hidden = false;
                document.getElementById("surrender").hidden = true;
                document.getElementById("gameSelection").hidden = false;
            }
            else if (isSolved)
            {
                game.prepend(
                    "Puzzle Solved 🎉", 
                    document.createElement("br"),
                    document.createElement("br")
                );

                document.getElementById("start").hidden = false;
                document.getElementById("surrender").hidden = true;
                document.getElementById("gameSelection").hidden = false;
            }
            else
            {
                game.prepend(
                    `Moves: ${moveCount}`, 
                    document.createElement("br"),
                    `Started: ${window.gameStartTime.toLocaleString()}`,
                    document.createElement("br"),
                    document.createElement("br")
                );
            }

            document.querySelector("game .wrapper").style.cssText = styling;
            
            if (!isSolved && remainingSeconds > 0)
            {
                for (const cell of document.querySelectorAll("game .wrapper .cell"))
                {
                    cell.setAttribute("onclick","toggleCell(this);");
                }
            }
        });
};

const getHighScores = () =>
{
    const highScoreElement = document.querySelector("#highScoreModal .modal-body");

    fetch("/api/game/highscores")
        .then(response => response.json())
        .then(data => 
        {
            let rows = ``;

            for (const score of data)
            {
                rows +=
`<tr>
    <th scope="row">${score.rank}</th>
    <td>${score.username}</td>
    <td>${score.complexityLevel}</td>
    <td>${score.timeTakenSeconds} seconds</td>
    <td>${score.remainingLights}</td>
    <td>${score.noOfMoves}</td>
</tr>`
            }

            const html = 
`<table class="table">
    <thead>
        <tr>
            <th scope="col">#</th>
            <th scope="col">Username</th>
            <th scope="col">Complexity</th>
            <th scope="col">Time Taken</th>
            <th scope="col">Remaining Lights</th>
            <th scope="col">Move count</th>
        </tr>
    </thead>
    <tbody>
        ${rows}
    </tbody>
</table>`;
            highScoreElement.innerHTML = html;
        });
};