<script setup>
import { ref } from 'vue'
import { useSignalR } from '@dreamonkey/vue-signalr'

const cells = ref(Array(9).fill(null))

console.log(cells.value)

const currentPlayer = ref('O')

const gameHub = useSignalR()
const gameStarted = ref(false)
const GameState = Object.freeze({ WaitingForPlayers: 1, Started: 2, Over: 3 })

gameHub
  .invoke('JoinGame')
  .then((res) => {
    console.log(res)
    if (res.state === GameState.Started) {
      gameStarted.value = true
      console.log('You cannot join a game that has already started')
    } else {
      console.log('Player joined game')
      currentPlayer.value = res.player.symbol
    }
  })
  .catch((error) => {
    console.error(error)
  })

gameHub.on('GameStarted', () => {
  gameStarted.value = true
  console.log('Game started')
})

const handleCellClick = (index) => {
  if (cells.value[index]?.symbol || !gameStarted.value) {
    return
  }

  gameHub
    .invoke('PlayTurn', index)
    .then(() => {
      cells.value[index] = { symbol: currentPlayer.value, position: index }
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

gameHub.on('GameStateChange', (res) => {
  console.log(res)
  cells.value = res.board.moves
  if (res.state === GameState.Over) {
    gameStarted.value = false
    console.log('Game over')
    if (res.winner) {
      alert(`Player ${res.winner.symbol} wins!`)
    } else {
      alert("It's a draw!")
    }
  }

  if (res.state === GameState.Started) {
    gameStarted.value = true
    console.log('Game started')
  }

  if (res.state === GameState.WaitingForPlayers) {
    gameStarted.value = true
    console.log('Waiting for players')
  }
})
</script>

<template>
  <div class="board">
    <div
      v-for="(cell, index) in cells"
      :key="index"
      :class="['cell', { 'cell:empty': !cell, 'cell:disabled': !gameStarted.value }]"
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
