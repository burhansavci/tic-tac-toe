<script setup>
import { RouterView } from 'vue-router'
import { gameHubServiceSymbol } from '@/services/gameHubServiceSymbol.js'
import { inject, onMounted } from 'vue'

const gameHubService = inject(gameHubServiceSymbol)

onMounted(() => {
  gameHubService.startGameHub().then(() => {
    console.log('GameHub started')
  }).catch((error) => {
    console.error(error)
  })
})
</script>

<template>
  <main>
    <RouterView />
  </main>
</template>

<style scoped>
main {
  display: grid;
  place-items: center;
  height: 100vh;
  background-color: var(--color-background);
}

</style>
