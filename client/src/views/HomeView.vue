<script setup>
import { useSignalR } from '@dreamonkey/vue-signalr'
import { onUnmounted, ref } from 'vue'

const gameHub = useSignalR()
const games = ref([])

const GameState = Object.freeze({ WaitingForPlayers: 1, Started: 2, Over: 3 })

gameHub.invoke('SubscribeToGamesStatutes')
  .then((res) => {
    games.value = res
  })
  .catch((error) => {
    console.error(error)
  })

gameHub.on('GameStateChange', (res) => {
  games.value.splice(games.value.findIndex(x => x.id === res.id), 1, res)
})

onUnmounted(() => {
  gameHub.invoke('UnsubscribeFromGamesStatutes')
})
</script>

<template>
  <main>
    <h1>Games</h1>
    <ul>
      <li v-for="game in games" :key="game.id">
        <router-link :to="`/game/${game.id}`">
          <span>{{ game.id }}</span>
          <span v-if="game.state === GameState.WaitingForPlayers">Waiting for players</span>
          <span v-else-if="game.state === GameState.Started">Started</span>
          <span v-else-if="game.state === GameState.Over">Over</span>
          <span v-if="game.winner">{{ game.winner.name }} won!</span>
        </router-link>
      </li>
    </ul>
  </main>
</template>
