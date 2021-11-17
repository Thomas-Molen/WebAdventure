import './GamePlayComponent.css'
import { React, useState } from 'react';
import { useLocation, useHistory } from 'react-router-dom';
import { CreateAuthEntityManagerRequest, cmd} from '../../helpers';
import { useRecoilValue } from 'recoil';
import { JWTState } from '../../state';
import { Icon } from '@iconify/react';
import ReactTooltip from 'react-tooltip';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

export function GamePlayComponent() {
    const globalJWTState = useRecoilValue(JWTState);
    const [Adventurer, setAdventurer] = useState({ id: null, experience: 0, health: 0, name: "Adventurer", damage: 0, positionX: 0, positionY: 0, dungeonId: null });
    const [selectedView, setSelectedView] = useState("stats");

    let URI = useLocation();
    let history = useHistory();

    const [connection, setConnection] = useState(new HubConnectionBuilder()
    .withUrl("https://localhost:5101/game", { accessTokenFactory: () => globalJWTState })
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect()
    .build());

    ConnectToHub();
    GetAdventurer();

    return (
        <>
            <div className="gameBackground">
                <div className="gameHeader offset-1 col">
                    Welcome {Adventurer.name}
                </div>
                <div className="container-fluid">
                    <div className="row">
                        <div className="col-8">
                            <div className="offset-1 col">
                                <textarea className="gameConsole" readOnly />
                            </div>
                        </div>
                        <div className="col-4 d-flex">
                            <div className="offset-1 col-10">
                                {/* stats UI */}
                                <div className="characterStats mb-5">
                                    <div className="row-fluid statsOptionsBar">
                                        <Icon icon="ant-design:user-outlined" color="#585858" width="30" className="statsoption selectedStatOption stats" data-tip="Stats" onClick={(e) => SetStatsWindow("stats", e)} />
                                        <Icon icon="mdi:treasure-chest" color="#585858" width="30" className="statsoption inventory" data-tip="Inventory" onClick={(e) => SetStatsWindow("inventory", e)} />
                                        <Icon icon="mdi:sword-cross" color="#585858" width="30" className="statsoption enemy" data-tip="Enemy details" onClick={(e) => SetStatsWindow("enemy", e)} />
                                        <ReactTooltip />
                                    </div>
                                    <hr className="m-0" />
                                    <div className="statsInformationWindow">

                                        {selectedView == "stats" &&
                                            <div className="row">
                                                <div className="col-4">
                                                    <ReactTooltip />
                                                    <div className="d-flex align-items-center">
                                                        <div data-tip="Level">
                                                            <Icon icon="mdi:chevron-double-up" width="50" color="white" />{Adventurer.experience / 100}
                                                        </div>
                                                    </div>
                                                    <div className="d-flex align-items-center">
                                                        <div data-tip="Health">
                                                            <Icon icon="akar-icons:heart" width="35" color="white" className="ms-2 me-2" />{Adventurer.health}
                                                        </div>
                                                    </div>
                                                    <div className="d-flex align-items-center">
                                                        <div data-tip="Attack power from equipped weapon">
                                                            <Icon icon="mdi:sword" rotate={1} width="40" color="white" className="ms-1 me-1" />{Adventurer.damage}
                                                        </div>
                                                    </div>

                                                </div>
                                                <div className="col">
                                                    <div className="d-flex align-items-center">
                                                        <div data-tip="Successfully cleared rooms">
                                                            <Icon icon="ic:baseline-meeting-room" width="50" color="white" className="me-1" />{0}
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        }
                                        {selectedView == "inventory" &&
                                            <div className="row-fluid">
                                                <ReactTooltip />
                                                INVENTORY
                                            </div>
                                        }
                                        {selectedView == "enemy" &&
                                            <div className="row-fluid">
                                                <ReactTooltip />
                                                ENEMY
                                            </div>
                                        }
                                    </div>
                                </div>
                                {/* minimap */}
                                <div className="MiniMap characterStats" data-tip="Minimap">

                                </div>

                            </div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-8">
                            <div className="offset-1 col">
                                <textarea className="gameInput" rows="1" onChange={(e) => GameInputOnChange(e)} onKeyPress={(e) => CheckForEnterKey(e)}>

                                </textarea>
                                <Icon icon="akar-icons:send" width="28" className="sendCommandIcon float-end EmptyConsoleInputIcon" onClick={(e) => ClickSendButton(e)} />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    )

    //startup
    function GetAdventurer() {
        const adventurerId = parseInt(URI.search.replace("?user=", ""));
        if (isNaN(adventurerId)) {
            history.replace("/");
        }
        if (Adventurer == undefined || Adventurer.id == null) {
            window.scrollTo(0, 0);
            CreateAuthEntityManagerRequest('GET', 'Adventurer/get/' + adventurerId, globalJWTState)
                .then(data => {
                    if (data == undefined || data == null) {
                        history.replace("/");
                    }
                    setAdventurer(data);
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    }

    async function ConnectToHub() {
        if (connection.state == "Disconnected" && Adventurer.id != null)
        {
            try {
                await connection.start();
                cmd.Clear();
                //invoke connection commands
                //general background commands
                connection.on("RenewJWT", async(delay) => {
                    await new Promise(r => setTimeout(r, delay));
                    connection.invoke("UpdateJWT", globalJWTState);
                });

                //game commands
                connection.on("ReceiveMessage", (message) => {
                    cmd.DisplayMessage(message);
                });

                await connection.invoke("JoinGame", {adventurerId: Adventurer.id, dungeonId: Adventurer.dungeonId, JWTToken: globalJWTState});
            }
            catch (e) {
                console.log("Error: " + e);
            }
        }
    }

    //input styling
    function GameInputOnChange(input) {
        const element = input.target.parentElement.childNodes[1];
        if (input.target.value == "") {
            RemoveClasses(element);
            element.classList.add("EmptyConsoleInputIcon");
        }
        else if (element.classList.contains("EmptyConsoleInputIcon")) {
            RemoveClasses(element);
            element.classList.add("ConsoleInputIcon");
        }
    }

    function RemoveClasses(object) {
        object.classList.remove("ConsoleInputIcon");
        object.classList.remove("EmptyConsoleInputIcon");
    }

    function CheckForEnterKey(input) {
        if (input.key == "Enter") {
            if (input.preventDefault) input.preventDefault();
            const element = input.target.parentElement.childNodes[1];

            RemoveClasses(element);
            element.classList.add("EmptyConsoleInputIcon");

            SendCommand(input.target.value);
            input.target.value = "";
        }
    }

    function ClickSendButton(button) {
        let svg = button.target;
        while (svg.tagName !== "svg") {
            svg = svg.parentElement;
        }
        SendCommand(svg.parentElement.childNodes[0].value);
        svg.parentElement.childNodes[0].value = "";
        RemoveClasses(svg);
        svg.classList.add("EmptyConsoleInputIcon");
    }

    //stats styling
    function SetStatsWindow(option, button) {
        let svg = button.target;
        while (svg.tagName !== "svg") {
            svg = svg.parentElement;
        }
        if (svg.classList.contains("selectedStatOption")) {
            return;
        }
        document.getElementsByClassName("selectedStatOption")[0].classList.remove("selectedStatOption");
        svg.classList.add("selectedStatOption");
        setSelectedView(option);
    }

    //game logic
    function SendCommand(command) {
        // AddToConsole(command);
        console.log(connection.state);
    }
}