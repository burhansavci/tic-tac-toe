<script setup>
import { ref } from 'vue'
import { useSignalR } from '@dreamonkey/vue-signalr'

const cells = ref(Array(9).fill(null))

console.log(cells.value)

const player = ref('')
const nextPlayer = ref('')

const gameHub = useSignalR()
const GameState = Object.freeze({ WaitingForPlayers: 1, Started: 2, Over: 3 })
const currentGameState = ref(GameState.WaitingForPlayers)


gameHub
  .invoke('JoinGame')
  .then((res) => {
    console.log(res)
    player.value = res.player
    currentGameState.value = res.state
    console.log('Player joined game')
  })
  .catch((error) => {
    console.error(error)
  })

gameHub.on('GameStateChange', (res) => {
  console.log(res)
  cells.value = res.board.moves
  currentGameState.value = res.state
  nextPlayer.value = res.nextPlayer

  if (currentGameState.value === GameState.Over) {
    res.winner ? alert(`Player ${res.winner.symbol} won!`) : alert('It\'s a draw!')
    console.log('Game over')
  }

  if (currentGameState.value === GameState.Started) {
    console.log('Game started')
  }

  if (currentGameState.value === GameState.WaitingForPlayers) {
    console.log('Waiting for players')
  }
})

const handleCellClick = (index) => {
  if (cells.value[index]?.symbol) {
    alert('Cell already taken!')
    return
  }

  if (currentGameState.value !== GameState.Started) {
    alert('Game is not started yet!')
    return
  }

  if (player.value.name !== nextPlayer.value.name) {
    alert('It\'s not your turn!')
    return
  }

  gameHub
    .invoke('PlayTurn', index)
    .then(() => {
      cells.value[index] = { symbol: player.value.symbol, position: index }
    })
    .catch((error) => {
      console.error(error)
    })
}

const resetGame = () => {
  gameHub
    .invoke('ResetGame')
    .then(() => {
      cells.value = Array(9).fill(null)
    })
    .catch((error) => {
      console.error(error)
    })
}
</script>

<template>
  <div class="board">
    <div
      v-for="(cell, index) in cells"
      :key="index"
      :class="['cell', { 'cell:empty': !cell, 'cell:disabled': currentGameState !== GameState.Started}]"
      @click="handleCellClick(index)"
    >
      {{ cell?.symbol }}
    </div>
    <button @click="resetGame">Reset Game</button>
    <button @click="gameHub.invoke('LeaveGame')">Leave Game</button>
  </div>
</template>

<style scoped>
.board {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1px;
  width: 300px;
  margin: 0 auto;
  border: 2px solid var(--color-border);
}

.cell {
  display: flex;
  place-items: center;
  justify-content: center;
  background-color: var(--color-background);
  color: var(--color-text);
  font-size: 2rem;
  font-weight: bold;
  height: 100px;
  border: 1px solid var(--color-border);
}

.cell:nth-child(3n) {
  border-right: 2px solid var(--color-border);
}

.cell:nth-child(n + 7) {
  border-bottom: 2px solid var(--color-border);
}

.cell:hover {
  background-color: var(--color-background-hover);
}

.cell:active {
  background-color: var(--color-background-active);
}

.cell:empty {
  background-color: var(--color-background-empty);
  cursor: pointer;
}

.cell:empty:hover {
  background-color: var(--color-background-empty-hover);
}

.cell:empty:active {
  background-color: var(--color-background-empty-active);
}

.cell:disabled {
  background-color: var(--color-background-disabled);
  cursor: not-allowed;
}
</style>
