<script setup>
import { inject, ref } from 'vue'
import { gameHubServiceSymbol } from '@/services/gameHubServiceSymbol.js'

const cells = ref(Array(9).fill(''))

const currentPlayer = ref('O')

const gameHubService = inject(gameHubServiceSymbol)
gameHubService.joinGame().then((res) => {
  console.log(res)
  console.log('Player joined game')
  currentPlayer.value = res.player.symbol.toUpperCase()
}).catch((error) => {
  console.error(error)
  setTimeout(() => {
    gameHubService.joinGame().then((res) => {
      console.log(res)
      console.log('Player joined game')
      currentPlayer.value = res.player.symbol.toUpperCase()
    }).catch((error) => {
      console.error(error)
    })
  }, 1000)
})

const handleCellClick = (index) => {
  if (cells.value[index]) {
    return
  }

  cells.value[index] = currentPlayer.value

  currentPlayer.value = currentPlayer.value === 'X' ? 'O' : 'X'
}
</script>

<template>
  <div class="board">
    <div
      v-for="(cell, index) in cells"
      :key="index"
      :class="['cell', { 'cell:empty': !cell }]"
      @click="handleCellClick(index)"
    >
      {{ cell }}
    </div>
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

</style>