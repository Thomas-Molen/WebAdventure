import { atom } from 'recoil';

const adventurerState = atom({
    key: "adventurerState",
    default: { id: null, experience: 0, health: 0, name: "Adventurer", damage: 0, positionX: 0, positionY: 0 },
});

export { adventurerState }